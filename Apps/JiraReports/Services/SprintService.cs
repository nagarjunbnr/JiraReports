using JiraReports.Common;
using JiraReports.Services.Interfaces;
using JiraReports.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    [SingleInstance(typeof(ISprintService))]
    public class SprintService : ISprintService
    {

        //specific sprint overrides
        private readonly Dictionary<string, List<SprintParameters>> sprintParams
            = new Dictionary<string, List<SprintParameters>>()
            {
                //XRMCore
                //{
                //    JiraConstants.SprintNames.XRMCore,

                //    new List<SprintParameters>()
                //    {
                //        new SprintParameters()
                //        {
                //            SprintNumber = "314",
                //            SprintPlanningDay = DayOfWeek.Thursday
                //        }
                //    }
                //}
            };


        //some teams work using different parameters
        private readonly Dictionary<string, SprintParameters> projectDefaults
            = new Dictionary<string, SprintParameters>()
        {
            {
                JiraConstants.SprintNames.Mobile,

                new SprintParameters()
                {
                    SprintPlanningDay = DayOfWeek.Thursday,
                    SprintReleaseDay = DayOfWeek.Friday,
                    SprintRetrospectiveDay = DayOfWeek.Thursday,
                    IsPlanningBeforeRelease = true
                }
            },
            {
                JiraConstants.SprintNames.XRMCore,

                new SprintParameters()
                {
                    SprintPlanningDay = DayOfWeek.Thursday,
                    SprintReleaseDay = DayOfWeek.Wednesday,
                    SprintRetrospectiveDay = DayOfWeek.Thursday
                }
            }
        };


        public SprintParameters GetSprintParameters(string projectKey, string sprintNumber)
        {
            this.sprintParams.TryGetValue(projectKey, out var allSprintParams);

            SprintParameters result = allSprintParams?.FirstOrDefault(s => s.SprintNumber == sprintNumber);

            if (result == null)
            {
                //no specific configuration found, fallback to the default, if it exists

                projectDefaults.TryGetValue(projectKey, out var defaultParams);

                //fallback to default parameters
                result = new SprintParameters()
                {
                    SprintNumber = sprintNumber,
                    SprintPlanningDay = defaultParams?.SprintPlanningDay ?? DayOfWeek.Wednesday,
                    SprintReleaseDay = defaultParams?.SprintReleaseDay ?? DayOfWeek.Wednesday,
                    SprintRetrospectiveDay = defaultParams?.SprintRetrospectiveDay ?? DayOfWeek.Wednesday,
                    IsPlanningBeforeRelease = defaultParams?.IsPlanningBeforeRelease ?? false
                };
            }

            return result;
        }

        public DateTime GetTargetSprintReleaseDate(string projectKey, string sprintNumber, DateTime startDate)
        {
            DateTime currentDate = startDate;
            SprintParameters sprintParams = this.GetSprintParameters(projectKey, sprintNumber);

            int releaseDaysOfWeekCount = 0;

            do
            {
                currentDate = currentDate.AddDays(1);

                if (currentDate.DayOfWeek == sprintParams.SprintReleaseDay)
                {
                    releaseDaysOfWeekCount++;
                }
            }
            while (releaseDaysOfWeekCount < 2);

            //the second occurance of the release day of week after the start of the sprint
            //should be the release day
            return currentDate;
        }

        public DateTime GetTargetSprintPlanningDate(string projectKey, string sprintNumber, DateTime sprintReleaseDate)
        {
            SprintParameters sprintParams = this.GetSprintParameters(projectKey, sprintNumber);
            DateTime currentDate = sprintReleaseDate;

            //find the first occurance of the sprint planning day of week after the release day
            //can be same date
            while (currentDate.DayOfWeek != sprintParams.SprintPlanningDay)
            {
                currentDate = currentDate.AddDays(!sprintParams.IsPlanningBeforeRelease ? 1 : -1);
            }

            return currentDate;

        }
    }
        
    
}
