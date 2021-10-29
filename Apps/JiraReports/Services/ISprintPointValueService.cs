using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    public interface ISprintPointValueService
    {
        decimal GetPointValueForHours(decimal hours);
        decimal GetPointValueForHours(double hours);
        decimal GetNormalizedTotalPointValue(decimal totalPointValue, decimal projectedHours);
    }
}
