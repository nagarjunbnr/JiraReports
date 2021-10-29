using JiraReports.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class AgileSprint
    {
        public string ID { get; set; }
        public string Self { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public int? OriginBoardID { get; set; }

        public DateTime? FinishDate
        {
            get
            {
                return this.EndDate;
                //return this.CompleteDate > this.EndDate ? this.CompleteDate : this.EndDate;
            }
        }

        public string SprintProject
        {
            get
            {
                return this.ParseSprintName().project;
            }
        }

        public (bool matched, string project, int sprint) ParseSprintName()
        {
            Regex regex = new Regex(@"(?<project>\w+( \w+)?) Sprint (?<sprint>\d+)");

            Match match = regex.Match(this.Name);
            string project = null;
            int sprint = 0;

            if (match.Success)
            {
                project = match.Groups["project"].Value;
                sprint = int.Parse(match.Groups["sprint"].Value);
            }

            return (match.Success, project, sprint);
        }

    }
}
