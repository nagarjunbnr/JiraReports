using JiraReports.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Interfaces
{
    public interface ISprintService
    {
        SprintParameters GetSprintParameters(string projectSprintName, string sprintNumber);
        DateTime GetTargetSprintReleaseDate(string projectKey, string sprintNumber, DateTime startDate);
        DateTime GetTargetSprintPlanningDate(string projectKey, string sprintNumber, DateTime sprintReleaseDate);
    }
}
