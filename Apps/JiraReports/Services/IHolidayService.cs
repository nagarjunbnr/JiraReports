using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    public interface IHolidayService
    {
        bool IsDateHoliday(Office office, DateTime date);
    }
}
