using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    [SingleInstance(typeof(IHolidayService))]
    public class HolidayService : IHolidayService
    {
        public bool IsDateHoliday(Office office, DateTime date)
        {
            return holidays.Any(h => h.HolidayDate == date.Date && h.Offices.Contains(office));
        }

        private List<OfficeHoliday> holidays = new List<OfficeHoliday>()
        {
            //2018/19
            new OfficeHoliday() { HolidayDate = new DateTime(2018, 12, 24), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2018, 12, 25), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 01, 01), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 05, 27), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 07, 04), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 09, 02), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 11, 28), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 12, 24), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 12, 25), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 09, 20), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 10, 12), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2019, 11, 15), Offices = new List<Office>() { Office.Brazil } },
            //2020
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 01, 01), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor, Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 04, 25), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 07, 03), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 09, 07), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 11, 26), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 11, 27), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 12, 24), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Contractor } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 12, 25), Offices = new List<Office>() { Office.Clearwater, Office.Houston, Office.Brazil, Office.Contractor, Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 01, 15), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 02, 21), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 03, 25), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 04, 02), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 04, 10), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 08, 03), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 08, 21), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 10, 02), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 10, 26), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 11, 13), Offices = new List<Office>() { Office.India } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 02, 25), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 02, 26), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 04, 10), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 04, 21), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 04, 10), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 05, 01), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 06, 11), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 09, 07), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 09, 20), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 10, 12), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 11, 02), Offices = new List<Office>() { Office.Brazil } },
            new OfficeHoliday() { HolidayDate = new DateTime(2020, 11, 15), Offices = new List<Office>() { Office.Brazil } },
        };
    }
}
