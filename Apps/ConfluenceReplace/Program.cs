using Loop.Confluence.IoC;
using Loop.Confluence.Services;
using Loop.Confluence.Services.Model;
using Loop.Confluence.Utilities;
using Newtonsoft.Json;
using Optify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfluenceReplace
{
    public class Program
    {
		private static void Main(string[] args)
        {
			try
			{
				ReplaceOptions options = Options.Parse<ReplaceOptions>(Environment.CommandLine);

				Console.WriteLine("");
				Console.WriteLine("Using the following options:");
				Console.WriteLine(JsonConvert.SerializeObject(options, Formatting.Indented));
				Console.WriteLine("Is this okay? [Y/n]");
				string response = Console.ReadLine().Trim();

				switch(response.ToLower())
				{
					case "y":
					case "yes":
						break;
					default:
						return;
				}

				ConfluenceConfig config = new ConfluenceConfig() 
				{ 
					APIBaseUrl = options.ConfluenceBaseURL, 
					Authentication = new BasicAuthentication()
					{
						Username = options.Username,
						Password = options.Password
					}
				};

				if (options.TargetOneSpace)
				{
					IConfluenceSpaceService spaceSvc = ServiceLocator.Instance.GetObject<IConfluenceSpaceService>();
					IEnumerable<ConfluenceSpace> spaces = spaceSvc.GetSpaces(config);
					ConfluenceSpace space
						= spaces.FirstOrDefault(s => s.Name == options.SpaceToTarget)
						?? spaces.FirstOrDefault(s => s.Key == options.SpaceToTarget)
						?? spaces.FirstOrDefault(s => String.Compare(s.Name, options.SpaceToTarget, true) == 0)
						?? spaces.FirstOrDefault(s => String.Compare(s.Key, options.SpaceToTarget, true) == 0);

					if(space == null)
					{
						throw new Exception($"Can't find space '{options.SpaceToTarget}'");
					}

					ModifyPagesInSpace(config, space, options.ReplaceWhat, options.WithWhat);
				}

				if(options.TargetAllSpaces)
				{
					ModifyPagesInAllSpaces(config, options.ReplaceWhat, options.WithWhat);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void ModifyPagesInAllSpaces(IConfluenceConfig config, string replaceWhat, string withWhat)
		{
			Console.WriteLine("Finding spaces...");

			IConfluenceSpaceService spaceSvc = ServiceLocator.Instance.GetObject<IConfluenceSpaceService>();
			foreach(ConfluenceSpace space in spaceSvc.GetSpaces(config))
			{
				ModifyPagesInSpace(config, space, replaceWhat, withWhat);
			}
		}


		private static void ModifyPagesInSpace(IConfluenceConfig config, ConfluenceSpace space, string replaceWhat, string withWhat)
		{
			Console.WriteLine($"Modifying space '{space.Name}'");

			IConfluenceSpaceService spaceSvc = ServiceLocator.Instance.GetObject<IConfluenceSpaceService>();
			IConfluenceContentService contentSvc = ServiceLocator.Instance.GetObject<IConfluenceContentService>();

			IEnumerable<ConfluencePage> pages = spaceSvc.GetPagesForSpace(config, space.Key);
			foreach (ConfluencePage page in pages)
			{
				Thread.Sleep(100);
				ConfluencePage pageContent = contentSvc.GetPage(config, page.ID);
				if (pageContent.Content == null)
					continue;

				if (!pageContent.Content.Contains(replaceWhat))
					continue;

				Console.WriteLine($"    => Modifying page '{pageContent.Title}'");
				string newContent = pageContent.Content.Replace(replaceWhat, withWhat);
				contentSvc.SavePage(config, pageContent.ID, pageContent.Title, newContent, pageContent.Version.Number + 1);
				Console.WriteLine("       Done...");
			}

			Console.WriteLine("Done...");
		}
	}
}
