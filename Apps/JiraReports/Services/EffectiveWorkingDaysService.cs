using JiraReports.Services.Interfaces;
using JiraReports.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    [SingleInstance(typeof(IEffectiveWorkingDaysService))]
    public class EffectiveWorkingDaysService : IEffectiveWorkingDaysService
    {
        private IPersonnelService personnelService;
        private ISprintService sprintService;

        public EffectiveWorkingDaysService(IPersonnelService personnelService, ISprintService sprintService)
        {
            this.personnelService = personnelService;
            this.sprintService = sprintService;
        }

        public decimal GetWorkingDays(string person, string project, string sprint, DateTime startDate, DateTime endDate, 
            bool includePersonalAvailability = true)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            int workingDays = 0;

            SprintParameters sprintParams = this.sprintService.GetSprintParameters(project, sprint);
            DateTime targetReleaseDate = this.sprintService.GetTargetSprintReleaseDate(project, sprint, startDate);
            DateTime targetSprintPlanningDate = this.sprintService.GetTargetSprintPlanningDate(project, sprint, targetReleaseDate);

            for (DateTime day = startDate; day <= endDate; day = day.AddDays(1))
            {
                // Don't count sprint planning days
                if (day == startDate && day.DayOfWeek == sprintParams.SprintPlanningDay)
                    continue;

                // Don't count release days
                if (day == targetReleaseDate)
                    continue;

                //Don't count sprint planning day
                if (day == targetSprintPlanningDate)
                    continue;

                // Don't count weekends
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (includePersonalAvailability)
                {
                    // Don't count PTO / holidays
                    if (personnelService.IsPersonOff(person, day))
                        continue;
                }

                workingDays++;
            }

            if (includePersonalAvailability)
            {
                decimal allocation = this.personnelService.GetPersonAllocation(person, project, sprint);
                return workingDays * allocation;
            }

            return workingDays;
        }

        public decimal GetProjectedHours(string person, string project, string sprint, DateTime startDate, DateTime endDate)
        {
            return GetWorkingDays(person, project, sprint, startDate, endDate) * 6;
        }

        public decimal GetPersonSprintAvailability(string person, string project, string sprint, DateTime startDate, DateTime endDate)
        {
            decimal workingDaysTotal = this.GetWorkingDays(person, project, sprint, startDate, endDate, includePersonalAvailability: false);
            decimal workingDaysWithAvailability = this.GetWorkingDays(person, project, sprint, startDate, endDate, includePersonalAvailability: true);

            return workingDaysTotal > 0 ? (workingDaysWithAvailability / workingDaysTotal) : 0m;
        }
    }


}
