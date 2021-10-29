using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    public interface IEffectiveWorkingDaysService
    {
        decimal GetPersonSprintAvailability(string person, string project, string sprint, DateTime startDate, DateTime endDate);
        decimal GetWorkingDays(string person, string project, string sprint, DateTime startDate, DateTime endDate, bool includePersonalAvailability = true);
        decimal GetProjectedHours(string person, string project, string sprint, DateTime startDate, DateTime endDate);
    }
}
