using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JiraReports.Common.JiraConstants;

namespace JiraReports.Services
{
    [SingleInstance(typeof(IPersonnelService))]
    public class PersonnelService : IPersonnelService
    {
        private IHolidayService holidayService;

        public PersonnelService(IHolidayService holidayService)
        {
            this.holidayService = holidayService;
        }

        public PersonRole GetRole(string person, string sprint)
        {
            if (!allocationList.TryGetValue(person, out Employee employee))
                return PersonRole.NA;

            ProjectSprintAllocation allocation = employee.Allocation.FirstOrDefault(a => a.Sprint == sprint);
            if (allocation == null)
                return PersonRole.NA;

            return allocation.Role;
        }

        public bool IsPersonOff(string person, DateTime date)
        {
            if (!allocationList.TryGetValue(person, out Employee employee))
                return false;

            if (this.holidayService.IsDateHoliday(employee.EmployeeOffice, date))
                return true;

            // Will be filled with CSA endoint (eventually)
            return employee.VacationDays.Any(d => d == date.Date);
        }

        public decimal GetPersonAllocation(string person, string project, string sprint)
        {
            List<ProjectSprintAllocation> allocations = GetAllocation(person, project, sprint);

            if (allocations.Count == 0) return 0M;

            return allocations.Sum(a => a.Allocation);
        }

        public List<ProjectSprintAllocation> GetAllPersonAllocations(string person, string sprint)
        {
            var result = new List<ProjectSprintAllocation>();
            List<ProjectSprintAllocation> allAllocations = allocationList.Where(a => a.Key == person)?
                .FirstOrDefault().Value.Allocation.ToList();

            foreach (var project in allAllocations.Select(a => a.Project).Distinct())
            {
                var currentProjectAllocations = this.GetAllocation(person, project, sprint);

                result.AddRange(currentProjectAllocations);
            }

            return result;
        }

        public IList<PersonRole> GetRoles(string person, string project, string sprint)
        {
            List<ProjectSprintAllocation> allocations = GetAllocation(person, project, sprint);

            return allocations.Select(a => a.Role).Distinct().ToList();
        }

        public PersonRole GetFirstRole(string person, string project, string sprint)
        {
            List<ProjectSprintAllocation> allocations = GetAllocation(person, project, sprint);

            if (allocations.Count == 0) return PersonRole.NA;

            return allocations.Select(a => a.Role).Distinct().FirstOrDefault();
        }

        public List<(string Name, PersonRole Role)> GetProjectContributors(string project, string sprint)
        {
            var result = new List<(string Name, PersonRole Role)>();

            foreach (string name in EngineerNames.GetAllEngNames())
            {
                List<ProjectSprintAllocation> allocation = GetAllocation(name, project, sprint);

                if (allocation.Count > 0 && allocation.First().Allocation > 0)
                {
                    result.Add((Name: name, Role: PersonRole.Eng));
                }
            }

            foreach (string name in QANames.GetAllQANames())
            {
                List<ProjectSprintAllocation> allocation = GetAllocation(name, project, sprint);

                if (allocation.Count > 0 && allocation.First().Allocation > 0)
                {
                    result.Add((Name: name, Role: PersonRole.QA));
                }
            }

            return result;
        }

        private List<ProjectSprintAllocation> GetAllocation(string person, string project, string sprint)
        {
            var result = new List<ProjectSprintAllocation>();
            //see if the employee is configured at all
            if (allocationList.TryGetValue(person, out Employee employee))
            {
                //find all allocation for this employee and project
                List<ProjectSprintAllocation> projectAllocation = employee.Allocation
                    .Where(a => string.Compare(a.Project, project, true) == 0).ToList();

                //find sprint allocation for the target sprint
                result = projectAllocation.Where(a => a.Sprint == sprint).ToList();

                //no allocation was found for this sprint but there is allocation for other sprints
                if (result.Count == 0 && projectAllocation.Count > 0)
                {
                    ProjectSprintAllocation latestAllocation = projectAllocation.Where(p => int.Parse(p.Sprint) < int.Parse(sprint))
                        .OrderByDescending(p => p.Sprint).FirstOrDefault();

                    if (latestAllocation != null)
                    {
                        result.Add(new ProjectSprintAllocation()
                        {
                            Sprint = sprint,
                            Role = latestAllocation.Role,
                            Allocation = latestAllocation.Allocation,
                            Project = latestAllocation.Project
                        });
                    }
                }
            }

            return result;
        }

        private readonly Dictionary<string, Employee> allocationList
            = new Dictionary<string, Employee>(StringComparer.OrdinalIgnoreCase)
        {
                   // LSEC
            {   EngineerNames.FrancisSpor, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("05/22/2020"),
                        DateTime.Parse("07/17/2020"),
                    }
                }
            },
            {   EngineerNames.RajBojja, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("05/22/2020"),
                        DateTime.Parse("07/17/2020"),
                    }
                }
            },
            {   EngineerNames.LewisCook, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "318", Role = PersonRole.Eng, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/22/2019"),
                        DateTime.Parse("10/23/2019"),
                        DateTime.Parse("10/24/2019"),
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("10/28/2019"),
                        DateTime.Parse("10/29/2019"),
                        DateTime.Parse("10/30/2019"),
                        DateTime.Parse("10/31/2019"),
                        DateTime.Parse("11/1/2019"),
                        DateTime.Parse("11/4/2019"),
                        DateTime.Parse("11/5/2019"),
                    }
                }
            },
            {   QANames.KevinMurphy, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "315", Role = PersonRole.QA, Allocation = 0.66M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "315", Role = PersonRole.QA, Allocation = 0.33M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "317", Role = PersonRole.QA, Allocation = 0.66M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "317", Role = PersonRole.QA, Allocation = 0.33M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.QA, Allocation = 0.75M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.QA, Allocation = 0.25M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "322", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "322", Role = PersonRole.QA, Allocation = 0.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("10/28/2019"),
                        DateTime.Parse("10/29/2019"),
                    }
                }
            },
             {   QANames.AlexCulp, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.LoopSecurity, Sprint = "315", Role = PersonRole.QA, Allocation = 0.66M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "315", Role = PersonRole.QA, Allocation = 0.33M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "317", Role = PersonRole.QA, Allocation = 0.66M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "317", Role = PersonRole.QA, Allocation = 0.33M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.QA, Allocation = 0.75M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.QA, Allocation = 0.25M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "322", Role = PersonRole.QA, Allocation = 1.0M },

                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("10/28/2019"),
                        DateTime.Parse("10/29/2019"),
                    }
                }
            },

            // Triggers
            {   EngineerNames.FloreminMesic, new Employee()
                {
                    EmployeeOffice = Office.Contractor,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/19/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                    }
                }
            },
            {   EngineerNames.PaulPatterson, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("02/12/2020"),
                        DateTime.Parse("03/02/2020"),
                        DateTime.Parse("03/03/2020"),
                        DateTime.Parse("03/04/2020"),
                    }
                }
            },
            {   EngineerNames.ChrisLamb, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 0.2M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "318", Role = PersonRole.Eng, Allocation = 0.8M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                    }
                }
            },
            {   QANames.BrianZimmerman, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.QA, Allocation = 0.75M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "309", Role = PersonRole.QA, Allocation = 0.25M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "312", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.QA, Allocation = 0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "315", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.QA, Allocation = 0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "317", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/29/2019"),
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("07/06/2020"),
                    }
                }
            },
             {   QANames.KrishnaveniKondeti, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.QA, Allocation = 0.75M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "309", Role = PersonRole.QA, Allocation = 0.25M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "312", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.QA, Allocation = 0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "315", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.QA, Allocation = 0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "317", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "360", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "360", Role = PersonRole.QA, Allocation = 0.5M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/29/2019"),
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("07/06/2020"),
                    }
                }
            },
             {   QANames.RadhaBotcha, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.QA, Allocation = 0.75M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "309", Role = PersonRole.QA, Allocation = 0.25M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "312", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.QA, Allocation = 0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "315", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.QA, Allocation = 0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "317", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "360", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "360", Role = PersonRole.QA, Allocation = 0.5M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/29/2019"),
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("07/06/2020"),
                    }
                }
            },
            {   QANames.JamesGerken, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.QA, Allocation = 0.25M },
                        new ProjectSprintAllocation() { Project = ProjectNames.QAA, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("01/23/2020"),
                        DateTime.Parse("01/24/2020"),
                        DateTime.Parse("02/17/2020"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("02/19/2020"),
                        DateTime.Parse("02/20/2020"),
                        DateTime.Parse("02/21/2020"),
                    }
                }
            },

             {   QANames.HarikaAppalabhatla, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Triggers, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "317", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("01/23/2020"),
                        DateTime.Parse("01/24/2020"),
                        DateTime.Parse("02/17/2020"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("02/19/2020"),
                        DateTime.Parse("02/20/2020"),
                        DateTime.Parse("02/21/2020"),
                    }
                }
            },

            // Campaigns
            {   EngineerNames.ChristofJans, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("08/8/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/25/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/01/2020"),
                        DateTime.Parse("03/19/2020"),
                        DateTime.Parse("03/20/2020"),
                    }
                }
            },
            {   EngineerNames.NicholasPalmer, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "319", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/21/2019"),
                        DateTime.Parse("10/22/2019"),
                        DateTime.Parse("10/23/2019"),
                        DateTime.Parse("10/24/2019"),
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("04/22/2020"),
                        DateTime.Parse("04/23/2020"),
                        DateTime.Parse("04/24/2020"),
                        DateTime.Parse("06/22/2020"),
                        DateTime.Parse("06/23/2020"),
                        DateTime.Parse("06/24/2020"),
                        DateTime.Parse("06/25/2020"),
                        DateTime.Parse("06/26/2020"),
                    }
                }
            },
            {   EngineerNames.AshClifton, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318",Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("08/30/2019"),
                        DateTime.Parse("09/18/2019"),
                        DateTime.Parse("11/05/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("03/02/2020"),
                        DateTime.Parse("03/03/2020"),
                        DateTime.Parse("03/04/2020"),
                        DateTime.Parse("06/01/2020"),
                        DateTime.Parse("07/02/2020"),
                        DateTime.Parse("07/06/2020"),
                    }
                }
            },
            {   QANames.SandraVanDeWeerd, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/20/2019"),
                        DateTime.Parse("9/6/2019"),
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/15/2019"),
                        DateTime.Parse("10/16/2019"),
                        DateTime.Parse("10/17/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("03/19/2020"),
                        DateTime.Parse("03/20/2020"),
                        DateTime.Parse("03/23/2020"),
                    }
                }
            },
            {   QANames.BrandonVest, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "318", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("11/08/2019"),
                        DateTime.Parse("11/11/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("03/05/2020"),
                        DateTime.Parse("03/06/2020"),
                    }
                }
            },
            {   QANames.SarahPaul, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Campaigns, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("10/28/2019"),
                        DateTime.Parse("11/08/2019"),
                        DateTime.Parse("11/11/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                    }
                }
            },
            {    QANames.ScottBrezniak, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("10/28/2019"),
                        DateTime.Parse("11/08/2019"),
                        DateTime.Parse("11/11/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                    }
                }
            },


            // Integrations
            {   EngineerNames.ManojVijayan, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/6/2019"),
                        DateTime.Parse("11/18/2019"),
                        DateTime.Parse("11/19/2019"),
                        DateTime.Parse("11/20/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/25/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("03/09/2020"),
                        DateTime.Parse("06/29/2020"),
                        DateTime.Parse("06/30/2020"),
                        DateTime.Parse("07/01/2020"),
                        DateTime.Parse("07/02/2020"),
                    }
                }
            },
            {   EngineerNames.MannyPena, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "328", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "353", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "353", Role = PersonRole.Eng, Allocation = 0.5M }
                    },
                    VacationDays = new List<DateTime>()
                    {

                    }
                }
            },
            {   EngineerNames.SampathKandula, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {

                    }
                }
            },
            {   EngineerNames.SamBenedict, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "353", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "353", Role = PersonRole.Eng, Allocation = 0.5M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("11/25/2019"),
                        DateTime.Parse("11/26/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("11/28/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/02/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/25/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("06/05/2020"),
                    }
                }
            },
            {   QANames.BrianHuynh, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "318", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "318", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "329", Role = PersonRole.QA, Allocation = 1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "329", Role = PersonRole.QA, Allocation = 0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/29/2019"),
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("9/2/2019"),
                        DateTime.Parse("9/3/2019"),
                        DateTime.Parse("9/26/2019"),
                        DateTime.Parse("9/27/2019"),
                        DateTime.Parse("11/08/2019"),
                        DateTime.Parse("11/26/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("04/16/2020"),
                        DateTime.Parse("04/17/2020"),
                        DateTime.Parse("04/20/2020"),
                    }
                }
            },
            {   QANames.AaronMarshall, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "353", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "353", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/27/2019"),
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("11/14/2019"),
                        DateTime.Parse("11/15/2019"),
                        DateTime.Parse("11/18/2019"),
                        DateTime.Parse("11/19/2019"),
                        DateTime.Parse("12/19/2019"),
                        DateTime.Parse("12/20/2019"),
                        DateTime.Parse("12/26/2019"),
                    }
                }
            },
            {   QANames.RobertMosqueda, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Integrations, Sprint = "328", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "335", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "353", Role = PersonRole.QA, Allocation = 0.5M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("07/06/2020"),
                        DateTime.Parse("07/07/2020"),
                    }
                }
            },
            // Reporting
            {   EngineerNames.LoganWebb, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/15/2019"),
                        DateTime.Parse("8/16/2019"),
                        DateTime.Parse("9/20/2019"),
                        DateTime.Parse("10/11/2019"),
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/15/2019"),
                        DateTime.Parse("10/16/2019"),
                        DateTime.Parse("10/17/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("02/05/2020"),
                        DateTime.Parse("02/06/2020"),
                        DateTime.Parse("02/07/2020"),
                        DateTime.Parse("05/22/2020"),
                    }
                }
            },
            {   EngineerNames.MauriceReeves, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "329", Role = PersonRole.Eng, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/5/2019"),
                        DateTime.Parse("8/6/2019"),
                        DateTime.Parse("8/7/2019"),
                        DateTime.Parse("8/8/2019"),
                        DateTime.Parse("8/9/2019"),
                    }
                }
            },
            {   EngineerNames.SamLombardo, new Employee()
                {
                    EmployeeOffice = Office.Contractor,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("02/06/2020"),
                        DateTime.Parse("02/07/2020"),
                        DateTime.Parse("04/13/2020"),
                        DateTime.Parse("05/18/2020"),
                        DateTime.Parse("11/06/2020"),
                        DateTime.Parse("12/06/2020"),
                    }
                }
            },
            {   QANames.GeraldPeer, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "324", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "324", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "326", Role = PersonRole.QA, Allocation = 0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "326", Role = PersonRole.QA, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("01/09/2020"),
                        DateTime.Parse("01/10/2020"),
                        DateTime.Parse("01/13/2020"),
                        DateTime.Parse("01/14/2020"),
                        DateTime.Parse("01/15/2020"),
                        DateTime.Parse("01/16/2020"),
                        DateTime.Parse("01/17/2020"),
                        DateTime.Parse("01/20/2020"),
                        DateTime.Parse("01/21/2020"),
                        DateTime.Parse("01/22/2020"),
                    }
                }
            },
            {   QANames.FrankRivera, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "309", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "309", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "312", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        //new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "315", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "317", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "317", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "319", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Architecture, Sprint = "319", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/21/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                    }
                }
            },

                     // DPS
            {   EngineerNames.StephanieVaul, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.DPS, Sprint = "352", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/26/2019"),

                    }
                }
            },
            {   EngineerNames.MatthewMcEntee, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.DPS, Sprint = "352", Role = PersonRole.Eng, Allocation = 1M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                        DateTime.Parse("8/9/2019"),
                    }
                }
            },
            {   EngineerNames.MukeshGujju, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {

                        new ProjectSprintAllocation() { Project = ProjectNames.DPS, Sprint = "352", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                        DateTime.Parse("11/06/2020"),
                        DateTime.Parse("12/06/2020"),
                    }
                }
            },
            {   EngineerNames.SrinivasaraoArevarapu, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.DPS, Sprint = "352", Role = PersonRole.Eng, Allocation = 1M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                        DateTime.Parse("01/22/2020"),
                    }
                }
            },
             {   EngineerNames.NagarjunaBheemreddy, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.DPS, Sprint = "352", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TimeHighway, Sprint = "363", Role = PersonRole.Eng, Allocation = 0.5M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("01/22/2020"),
                    }
                }
            },
            // COE
            {   EngineerNames.SivaJogireddy, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/26/2019"),

                    }
                }
            },
            {   EngineerNames.SunilBolla, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/26/2019"),

                    }
                }
            },
            {   EngineerNames.ToddHoeffert, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/26/2019"),

                    }
                }
            },
            {   EngineerNames.SaiMaheshVegunta, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                        DateTime.Parse("8/9/2019"),
                    }
                }
            },
            {   EngineerNames.SivaPavaniIndukari, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {

                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                        DateTime.Parse("11/06/2020"),
                        DateTime.Parse("12/06/2020"),
                    }
                }
            },
            {   EngineerNames.ShivaKota, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                        DateTime.Parse("01/22/2020"),
                    }
                }
            },
             {   EngineerNames.RajeshParimilla, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("01/22/2020"),
                    }
                }
            },
             {   EngineerNames.RyanCraig, new Employee()
                {
                    EmployeeOffice = Office.Calabasas,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.COE, Sprint = "357", Role = PersonRole.Eng, Allocation = 1M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("01/22/2020"),
                    }
                }
            },
            // Mobile
            {   EngineerNames.StefanProgovac, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "323", Role = PersonRole.Eng, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/30/2019"),
                        DateTime.Parse("10/01/2019"),
                        DateTime.Parse("10/02/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/25/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("01/05/2020"),
                    }
                }
            },
            {   EngineerNames.GaryAlfred, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "318", Role = PersonRole.Eng, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "329", Role = PersonRole.Eng, Allocation = 0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "330", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("01/06/2020"),
                        DateTime.Parse("01/07/2020"),
                        DateTime.Parse("01/08/2020"),
                        DateTime.Parse("01/09/2020"),
                        DateTime.Parse("01/10/2020"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("02/04/2020"),
                        DateTime.Parse("02/05/2020"),
                    }
                }
            },

            {   EngineerNames.KenTran, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "329", Role = PersonRole.Eng, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("02/10/2020"),
                    }
                }
            },
            {   EngineerNames.LucasMedeiros, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.Eng, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "316", Role = PersonRole.Eng, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/21/2019"),
                        DateTime.Parse("10/22/2019"),
                        DateTime.Parse("10/23/2019"),
                        DateTime.Parse("10/24/2019"),
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("10/28/2019"),
                        DateTime.Parse("10/29/2019"),
                        DateTime.Parse("10/30/2019"),
                        DateTime.Parse("10/31/2019"),
                        DateTime.Parse("11/01/2019"),
                        DateTime.Parse("11/04/2019"),
                        DateTime.Parse("11/05/2019"),
                        DateTime.Parse("11/06/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("02/04/2020"),
                        DateTime.Parse("02/05/2020"),
                        DateTime.Parse("02/06/2020"),
                        DateTime.Parse("02/07/2020"),
                        DateTime.Parse("02/10/2020"),
                        DateTime.Parse("02/11/2020"),
                        DateTime.Parse("02/12/2020"),
                        DateTime.Parse("02/13/2020"),
                        DateTime.Parse("02/14/2020"),
                        DateTime.Parse("02/17/2020"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("02/19/2020"),
                        DateTime.Parse("02/20/2020"),
                        DateTime.Parse("02/21/2020"),
                        DateTime.Parse("03/20/2020"),
                        DateTime.Parse("04/20/2020"),
                        DateTime.Parse("04/21/2020"),
                    }
                }
            },
                    {   EngineerNames.DestinStrohaber, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "346", Role = PersonRole.Eng, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/13/2019"),
                        DateTime.Parse("12/16/2019"),
                        DateTime.Parse("12/17/2019"),
                        DateTime.Parse("12/18/2019"),
                        DateTime.Parse("12/19/2019"),
                        DateTime.Parse("12/20/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("05/21/2020"),
                        DateTime.Parse("05/22/2020"),
                        DateTime.Parse("07/06/2020"),
                        DateTime.Parse("07/07/2020"),
                        DateTime.Parse("07/08/2020"),
                        DateTime.Parse("07/09/2020"),
                        DateTime.Parse("07/10/2020"),
                    }
                }
            },
            {   EngineerNames.SeanLower, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/23/2019"),
                        DateTime.Parse("8/26/2019"),
                        DateTime.Parse("8/27/2019"),
                        DateTime.Parse("8/28/2019"),
                        DateTime.Parse("8/29/2019"),
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("9/2/2019"),
                        DateTime.Parse("9/3/2019"),
                        DateTime.Parse("07/09/2020"),
                        DateTime.Parse("07/10/2020"),
                        DateTime.Parse("07/13/2020"),
                    }
                }
            },
            {   EngineerNames.MatthewFrederick, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "329", Role = PersonRole.Eng, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/29/2019"),
                        DateTime.Parse("9/9/2019"),
                        DateTime.Parse("9/10/2019"),
                        DateTime.Parse("9/11/2019"),
                        DateTime.Parse("9/12/2019"),
                        DateTime.Parse("9/13/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                    }
                }
            },

            {   QANames.TreyLago, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/17/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("01/06/2020"),
                    }
                }
            },

            {   QANames.RameshPendayala, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Mobile, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/17/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("01/06/2020"),
                    }
                }
            },

            // Quote
            {   EngineerNames.RyanBowers, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/23/2019"),
                        DateTime.Parse("9/30/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/02/2019"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("03/11/2020"),
                        DateTime.Parse("03/25/2020"),
                        DateTime.Parse("03/26/2020"),
                        DateTime.Parse("03/27/2020"),
                    }
                }
            },
            {   EngineerNames.JamezPicard, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/26/2019"),
                        DateTime.Parse("11/22/2019"),
                        DateTime.Parse("11/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("03/23/2020"),
                        DateTime.Parse("03/25/2020"),

                    }
                }
            },
            {   EngineerNames.VenkatGaddameedi, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/26/2019"),
                        DateTime.Parse("11/22/2019"),
                        DateTime.Parse("11/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("03/23/2020"),
                        DateTime.Parse("03/25/2020"),

                    }
                }
            },
            {   EngineerNames.VenkatGundelli, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Reporting, Sprint = "324", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("02/24/2020"),
                        DateTime.Parse("02/25/2020"),
                        DateTime.Parse("02/26/2020"),
                        DateTime.Parse("02/27/2020"),
                        DateTime.Parse("02/28/2020"),
                        DateTime.Parse("05/14/2020"),
                        DateTime.Parse("05/15/2020"),
                    }
                }
            },
            {   QANames.JordanPearson, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "330", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/16/2019"),
                        DateTime.Parse("9/17/2019"),
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("10/21/2019"),
                        DateTime.Parse("03/09/2020"),
                        DateTime.Parse("03/10/2020"),
                    }
                }
            },
            {   QANames.LakshmiPampana, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "340", Role = PersonRole.QA, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/16/2019"),
                        DateTime.Parse("9/17/2019"),
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("10/21/2019"),
                        DateTime.Parse("03/09/2020"),
                        DateTime.Parse("03/10/2020"),
                    }
                }
            },
            {   QANames.AlexKudryashov, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "309", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "309", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "310", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "311", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "312", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "312", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "313", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "314", Role = PersonRole.QA, Allocation = 0.1M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "314", Role = PersonRole.QA, Allocation =   0.9M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "315", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "315", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "316", Role = PersonRole.QA, Allocation =   1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "316", Role = PersonRole.QA, Allocation =   0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "317", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "317", Role = PersonRole.QA, Allocation =   0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "326", Role = PersonRole.QA, Allocation =   0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "326", Role = PersonRole.QA, Allocation =   0.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("12/06/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("02/21/2020"),
                        DateTime.Parse("02/24/2020"),
                    }
                }
            },

            // SLT
            {   EngineerNames.MaureenOrozco, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("7/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("02/03/2020"),
                        DateTime.Parse("02/17/2020"),
                        DateTime.Parse("04/10/2020"),
                        DateTime.Parse("06/29/2020"),
                        DateTime.Parse("06/30/2020"),
                        DateTime.Parse("07/01/2020"),
                        DateTime.Parse("07/02/2020"),
                    }
                }
            },
            {   EngineerNames.EduardoBrasil, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/2/2019"),
                        DateTime.Parse("9/3/2019"),
                        DateTime.Parse("9/4/2019"),
                        DateTime.Parse("9/5/2019"),
                        DateTime.Parse("9/6/2019"),
                        DateTime.Parse("9/9/2019"),
                        DateTime.Parse("9/10/2019"),
                        DateTime.Parse("9/11/2019"),
                        DateTime.Parse("9/12/2019"),
                        DateTime.Parse("9/13/2019"),
                        DateTime.Parse("9/16/2019"),
                        DateTime.Parse("10/08/2019"),
                        DateTime.Parse("10/17/2019"),
                        DateTime.Parse("11/05/2019"),
                        DateTime.Parse("11/11/2019"),
                        DateTime.Parse("11/22/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/25/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("01/01/2020"),
                        DateTime.Parse("01/02/2020"),
                        DateTime.Parse("01/03/2020"),
                        DateTime.Parse("01/06/2020"),
                        DateTime.Parse("02/17/2020"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("02/24/2020"),
                        DateTime.Parse("02/25/2020"),
                        DateTime.Parse("02/26/2020"),
                        DateTime.Parse("04/10/2020"),
                        DateTime.Parse("04/21/2020"),
                        DateTime.Parse("05/19/2020"),
                        DateTime.Parse("05/20/2020"),
                        DateTime.Parse("05/21/2020"),
                        DateTime.Parse("05/22/2020"),
                        DateTime.Parse("05/25/2020"),
                        DateTime.Parse("05/26/2020"),
                        DateTime.Parse("05/27/2020"),
                        DateTime.Parse("05/28/2020"),
                        DateTime.Parse("05/29/2020"),
                        DateTime.Parse("06/01/2020"),
                        DateTime.Parse("06/02/2020"),
                        DateTime.Parse("06/02/2020"),
                    }
                }
            },
            {   QANames.MobinaQuadir, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "329", Role = PersonRole.QA, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/16/2019"),
                        DateTime.Parse("8/30/2019"),
                        DateTime.Parse("11/08/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/24/2019"),
                        DateTime.Parse("12/25/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("12/30/2019"),
                        DateTime.Parse("12/31/2019"),
                        DateTime.Parse("02/17/2020"),
                    }
                }
            },
            {   QANames.ZvjezdanVeselinovic, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "315", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "318", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote,Sprint = "353", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "318", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.QAA, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/13/2019"),
                        DateTime.Parse("10/16/2019"),
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/02/2019"),
                        DateTime.Parse("12/03/2019"),
                        DateTime.Parse("12/04/2019"),
                        DateTime.Parse("01/07/2020"),
                        DateTime.Parse("01/10/2020"),
                        DateTime.Parse("01/20/2020"),
                        DateTime.Parse("01/21/2020"),
                        DateTime.Parse("02/17/2020"),
                        DateTime.Parse("02/18/2020"),
                        DateTime.Parse("02/21/2020"),
                        DateTime.Parse("12/06/2020"),
                    }
                }
            },
            {   QANames.MonicaGullapalli, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.SLT, Sprint = "315", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "318", Role = PersonRole.QA, Allocation = 1.0M },

                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("01/10/2020")
                    }
                }
            },

            {   EngineerNames.ChrisKnight, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Loop5, Sprint = "318", Role = PersonRole.Eng, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "323", Role = PersonRole.Eng, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/7/2019"),
                        DateTime.Parse("10/17/2019"),
                        DateTime.Parse("10/18/2019"),
                        DateTime.Parse("11/26/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("11/28/2019"),
                        DateTime.Parse("11/29/2019"),
                    }
                }
            },
            

            // TIV
            {   EngineerNames.MichaelAguilar, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/15/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("01/30/2020")
                    }
                }
            },
            {   EngineerNames.HarshvardhanSindhu, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "366", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/15/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("12/26/2019"),
                        DateTime.Parse("12/27/2019"),
                        DateTime.Parse("01/30/2020")
                    }
                }
            },
            {   EngineerNames.SravaniAtluri, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.AffinitivCore, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.StebanDomingues, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("02/24/2020"),
                        DateTime.Parse("02/25/2020"),
                        DateTime.Parse("04/15/2020"),
                        DateTime.Parse("05/18/2020"),
                        DateTime.Parse("05/19/2020"),
                        DateTime.Parse("05/20/2020"),
                        DateTime.Parse("05/21/2020"),
                        DateTime.Parse("05/22/2020"),
                        DateTime.Parse("05/25/2020"),
                        DateTime.Parse("05/26/2020"),
                        DateTime.Parse("05/27/2020"),
                        DateTime.Parse("05/28/2020"),
                        DateTime.Parse("05/29/2020"),
                    }
                }
            },
            {   EngineerNames.KelvynRisso, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "315", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.Quote, Sprint = "357", Role = PersonRole.Eng, Allocation = 0.5M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("9/02/2019"),
                        DateTime.Parse("02/24/2020"),
                        DateTime.Parse("02/26/2020"),
                        DateTime.Parse("02/27/2020"),
                        DateTime.Parse("02/28/2020"),
                        DateTime.Parse("03/02/2020"),
                        DateTime.Parse("03/03/2020"),
                        DateTime.Parse("03/04/2020"),
                        DateTime.Parse("03/05/2020"),
                        DateTime.Parse("03/06/2020"),
                        DateTime.Parse("03/09/2020"),
                        DateTime.Parse("03/10/2020"),
                        DateTime.Parse("03/11/2020"),
                        DateTime.Parse("03/12/2020"),
                        DateTime.Parse("03/13/2020"),
                        DateTime.Parse("03/16/2020"),
                        DateTime.Parse("04/21/2020"),
                    }
                }
            },
            {   QANames.TJNoor, new Employee()
                {
                    EmployeeOffice = Office.Clearwater,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "316", Role = PersonRole.QA, Allocation = 0.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "317", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.TIV, Sprint = "318", Role = PersonRole.QA, Allocation = 0.0M },
                         new ProjectSprintAllocation() { Project = ProjectNames.Essentials, Sprint = "349", Role = PersonRole.QA, Allocation = 0.25M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                    }
                }
            },

            //XRM Core
            {   EngineerNames.RegisBrand, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "316", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "317", Role = PersonRole.Eng, Allocation = 0.9375M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                         DateTime.Parse("01/13/2020"),
                         DateTime.Parse("01/14/2020"),
                         DateTime.Parse("01/15/2020"),
                         DateTime.Parse("01/16/2020"),
                         DateTime.Parse("01/17/2020"),
                         DateTime.Parse("01/20/2020"),
                         DateTime.Parse("01/21/2020"),
                         DateTime.Parse("01/22/2020"),
                         DateTime.Parse("01/23/2020"),
                         DateTime.Parse("01/24/2020"),
                         DateTime.Parse("01/27/2020"),
                         DateTime.Parse("01/28/2020"),
                         DateTime.Parse("01/29/2020"),
                         DateTime.Parse("01/30/2020"),
                         DateTime.Parse("01/31/2020"),
                         DateTime.Parse("02/24/2020"),
                         DateTime.Parse("02/25/2020"),
                         DateTime.Parse("02/26/2020"),
                    }
                }
            },
            {   EngineerNames.PrabasiniHota, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                         DateTime.Parse("11/25/2019"),
                         DateTime.Parse("12/03/2019"),
                         DateTime.Parse("12/09/2019"),
                         DateTime.Parse("12/10/2019"),
                         DateTime.Parse("12/11/2019"),
                         DateTime.Parse("12/12/2019"),
                         DateTime.Parse("12/13/2019"),
                         DateTime.Parse("12/16/2019"),
                         DateTime.Parse("12/17/2019"),
                         DateTime.Parse("12/18/2019"),
                         DateTime.Parse("12/19/2019"),
                         DateTime.Parse("12/20/2019"),
                    }
                }
            },
            {   EngineerNames.RakeshKasala, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "325", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "326", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "327", Role = PersonRole.Eng, Allocation = 0.75M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.OleksiyLevenets, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "325", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "326", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.QuoiChung, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "316", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "317", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "319", Role = PersonRole.Eng, Allocation = 0.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/28/2019"),
                    }
                }
            },
            {   EngineerNames.ThiagusFerreira, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "316", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "317", Role = PersonRole.Eng, Allocation = 0.9375M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "318", Role = PersonRole.Eng, Allocation = 0.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("8/30/2019")
                    }
                }
            },
            {   QANames.SuparnaShaligram, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.QA, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "315", Role = PersonRole.QA, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/25/2019"),
                        DateTime.Parse("11/26/2019"),
                        DateTime.Parse("11/27/2019"),
                        DateTime.Parse("12/19/2019"),
                        DateTime.Parse("12/20/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("01/02/2020"),
                    }
                }
            },
            {   QANames.VishnuPriya, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "315", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/16/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("03/09/2020"),
                        DateTime.Parse("03/10/2020"),
                        DateTime.Parse("03/11/2020"),
                    }
                }
            },
            {   QANames.AnushaYeddula, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "309", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "310", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "311", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "312", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "313", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "314", Role = PersonRole.QA, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "360", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                        DateTime.Parse("10/14/2019"),
                        DateTime.Parse("10/16/2019"),
                        DateTime.Parse("11/29/2019"),
                        DateTime.Parse("12/23/2019"),
                        DateTime.Parse("03/09/2020"),
                        DateTime.Parse("03/10/2020"),
                        DateTime.Parse("03/11/2020"),
                    }
                }
            },
            {   QANames.VenkatRaghavan, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMCore, Sprint = "325", Role = PersonRole.QA, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },

            // XRM IntegrationB
            {   EngineerNames.MikeDonnelly, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "309", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "310", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "311", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "312", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "313", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "314", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "315", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "316", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "317", Role = PersonRole.Eng, Allocation = 0.5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "319", Role = PersonRole.Eng, Allocation = .75M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "325", Role = PersonRole.Eng, Allocation = .5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "326", Role = PersonRole.Eng, Allocation = .5M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "327", Role = PersonRole.Eng, Allocation = .75M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                         DateTime.Parse("10/21/2019"),
                         DateTime.Parse("11/29/2019"),
                         DateTime.Parse("12/02/2019"),
                         DateTime.Parse("12/02/2019"),
                         DateTime.Parse("12/23/2019"),
                         DateTime.Parse("12/26/2019"),
                         DateTime.Parse("12/27/2019"),
                    }
                }
            },
            {   EngineerNames.RafaelMelo, new Employee()
                {
                    EmployeeOffice = Office.Brazil,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "315", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "316", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "317", Role = PersonRole.Eng, Allocation = 0.9375M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "318", Role = PersonRole.Eng, Allocation = 1.0M },                    },
                    VacationDays = new List<DateTime>()
                    {
                         DateTime.Parse("12/30/2019"),
                         DateTime.Parse("12/31/2019"),
                         DateTime.Parse("01/02/2020"),
                         DateTime.Parse("01/03/2020"),
                         DateTime.Parse("01/06/2020"),
                         DateTime.Parse("01/07/2020"),
                         DateTime.Parse("01/08/2020"),
                         DateTime.Parse("02/24/2020"),
                         DateTime.Parse("02/25/2020"),
                         DateTime.Parse("02/26/2020"),
                    }
                }
            },
            {   EngineerNames.JaredCohn, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                         DateTime.Parse("12/30/2019"),
                         DateTime.Parse("12/31/2019"),
                         DateTime.Parse("01/02/2020"),
                         DateTime.Parse("01/03/2020"),
                         DateTime.Parse("01/06/2020"),
                         DateTime.Parse("01/07/2020"),
                         DateTime.Parse("01/08/2020"),
                         DateTime.Parse("02/24/2020"),
                         DateTime.Parse("02/25/2020"),
                         DateTime.Parse("02/26/2020"),
                    }
                }
            },
            {   EngineerNames.JohnHaynes, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "357", Role = PersonRole.Eng, Allocation = 1.0M }
                    },
                    VacationDays = new List<DateTime>()
                    {
                         DateTime.Parse("12/30/2019"),
                         DateTime.Parse("12/31/2019"),
                         DateTime.Parse("01/02/2020"),
                         DateTime.Parse("01/03/2020"),
                         DateTime.Parse("01/06/2020"),
                         DateTime.Parse("01/07/2020"),
                         DateTime.Parse("01/08/2020"),
                         DateTime.Parse("02/24/2020"),
                         DateTime.Parse("02/25/2020"),
                         DateTime.Parse("02/26/2020"),
                    }
                }
            },
            {   EngineerNames.MikeVenkata, new Employee()
                {
                    EmployeeOffice = Office.Houston,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "309", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "310", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "311", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "312", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "313", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "314", Role = PersonRole.Eng, Allocation = 1.0M },
                        new ProjectSprintAllocation() { Project = ProjectNames.XRMInt, Sprint = "315", Role = PersonRole.Eng, Allocation = 0M },
                    },
                    VacationDays = new List<DateTime>()
                    {

                    }
                }
            },
            {   EngineerNames.NagarajNeelapala, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.NarendraKolukula, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.ShanthiSandeep, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.SambaSarnala, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.PujithaPunukollu, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.SrinivasuluThottempudi, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.PavanKuruba, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.VinithaAlla, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.ADMT, Sprint = "362", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.AlexRybak, new Employee()
                {
                    EmployeeOffice = Office.Ukraine,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TimeHighway, Sprint = "363", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.YuriyMuhin, new Employee()
                {
                    EmployeeOffice = Office.Ukraine,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TimeHighway, Sprint = "363", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            },
            {   EngineerNames.SandeepMididoddi, new Employee()
                {
                    EmployeeOffice = Office.India,
                    Allocation = new List<ProjectSprintAllocation>()
                    {
                        new ProjectSprintAllocation() { Project = ProjectNames.TimeHighway, Sprint = "363", Role = PersonRole.Eng, Allocation = 1.0M },
                    },
                    VacationDays = new List<DateTime>()
                    {
                    }
                }
            }
        };

        private class Employee
        {
            public List<ProjectSprintAllocation> Allocation { get; set; }
            public List<DateTime> VacationDays { get; set; }
            public Office EmployeeOffice { get; set; }
        }

        public class ProjectSprintAllocation
        {
            public string Project { get; set; }
            public string Sprint { get; set; }
            public decimal Allocation { get; set; }
            public PersonRole Role { get; set; }
        }

        private class ProjectNames
        {
            public static string Triggers = ProjectKeys.Triggers;
            public static string Campaigns = ProjectKeys.Campaigns;
            public static string Reporting = ProjectKeys.Reporting;
            public static string Integrations = ProjectKeys.Integration;
            public static string LoopSecurity = ProjectKeys.LoopSecurity;
            public static string Architecture = ProjectKeys.Architecture;
            public static string Loop5 = ProjectKeys.Loop5;
            public static string Quote = ProjectKeys.Quote;
            public static string SLT = ProjectKeys.ServiceLaneTechnology;
            public static string Mobile = ProjectKeys.Mobile;
            public static string TIV = ProjectKeys.TradeInValet;
            public static string XRMCore = ProjectKeys.XRMCore;
            public static string XRMInt = ProjectKeys.XRMIntegration;
            public static string Essentials = ProjectKeys.Essentials;
            public static string AffinitivCore = ProjectKeys.AffinitivCore;
            public static string DPS = ProjectKeys.DPSLegacy;
            public static string COE = ProjectKeys.COEReporting;
            public static string QAA = ProjectKeys.QAAutomation;
            public static string ADMT = ProjectKeys.ADMTEngineering;
            public static string TimeHighway = ProjectKeys.TimeHighway;
        }

        private class EngineerNames
        {
            public const string FrancisSpor = "Francis Spor";
            public const string LewisCook = "Lewis Cook";
            public const string FloreminMesic = "Floremin Mesic";
            public const string PaulPatterson = "Paul Patterson";
            public const string ChrisLamb = "Chris Lamb";
            public const string ChristofJans = "Christof Jans";
            public const string NicholasPalmer = "Nicholas Palmer";
            public const string AshClifton = "Ash Clifton";
            public const string ManojVijayan = "Manoj Vijayan";
            public const string MannyPena = "Manny Pena";
            public const string SamBenedict = "Sam Benedict";
            public const string LoganWebb = "Logan Webb";
            public const string MauriceReeves = "Maurice Reeves";
            public const string SamLombardo = "Sam Lombardo";
            public const string StefanProgovac = "Stefan Progovac";
            public const string KenTran = "Ken Tran";
            public const string SeanLower = "Sean Lower";
            public const string MatthewFrederick = "Matthew Frederick";
            public const string RyanBowers = "Ryan Bowers";
            public const string JamezPicard = "Jamez Picard";
            public const string VenkatGaddameedi = "Venkat Reddy Gaddameedi";
            public const string ChrisKnight = "Chris Knight";
            public const string GaryAlfred = "Gary Alfred";
            public const string MaureenOrozco = "Maureen Orozco";
            public const string LucasMedeiros = "Lucas Medeiros";
            public const string EduardoBrasil = "Eduardo Brasil";
            public const string MichaelAguilar = "Michael Aguilar";
            public const string StebanDomingues = "Steban Domingues";
            public const string KelvynRisso = "Kelvyn Risso";
            public const string RegisBrand = "Regis Brand";
            public const string PrabasiniHota = "Prabasini Hota";
            public const string QuoiChung = "Quoi Chung";
            public const string MikeDonnelly = "Mike Donnelly";
            public const string RafaelMelo = "Rafael Melo";
            public const string MikeVenkata = "Mike Venkata";
            public const string ThiagusFerreira = "Thiagus Ferreira";
            public const string VenkatGundelli = "Venkat Gundelli";
            public const string SravaniAtluri = "Sravani Atluri";

            public const string RakeshKasala = "Rakesh Kasala";
            public const string DestinStrohaber = "Destin Strohaber";
            public const string StephanieVaul = "Stephanie Vaul";
            public const string MatthewMcEntee = "Matthew McEntee";
            public const string MukeshGujju = "Mukesh Gujju";
            public const string NagarjunaBheemreddy = "Nagarjuna Bheemreddy";
            public const string SrinivasaraoArevarapu = "Srinivasarao Arevarapu";

            public const string SivaJogireddy = "Siva Jogireddy";
            public const string SaiMaheshVegunta = "Sai Mahesh Vegunta";
            public const string SivaPavaniIndukari = "Siva Pavani Indukuri";
            public const string ShivaKota = "Shiva Kota";
            public const string RajeshParimilla = "Rajesh Parimilla";
            public const string RyanCraig = "Ryan Craig";
            public const string SunilBolla = "Sunil Bolla";
            public const string ToddHoeffert = "Todd Hoeffert";
            public const string RajBojja = "Raj Bojja";

            public const string SampathKandula = "Sampath Kandula";
            public const string JaredCohn = "Jared Cohn";
            public const string OleksiyLevenets = "Oleksiy Levenets";
            public const string JohnHaynes = "John Haynes";
            //public const string ManojVijayan = "Manoj Vijayan"

            //ADMT Engineering
            public const string NagarajNeelapala = "Nagaraj Neelapala";
            public const string NarendraKolukula = "Narendra Kolukula";
            public const string SambaSarnala = "Samba Sarnala";
            public const string ShanthiSandeep = "Shanthi Sandeep";
            public const string PujithaPunukollu = "Pujitha Punukollu";
            public const string SrinivasuluThottempudi = "Srinivasulu Thottempudi";
            public const string PavanKuruba = "Pavan Kuruba";
            public const string VinithaAlla = "Vinitha Alla";

            public const string AlexRybak = "Alex Rybak";
            public const string YuriyMuhin = "Yuriy Muhin";
            public const string SandeepMididoddi = "Sandeep Modidoddi";

            public const string HarshvardhanSindhu = "Harshvardhan Sindhu";

            public static List<string> GetAllEngNames()
            {
                IEnumerable<string> GetNames()
                {
                    yield return FrancisSpor;
                    yield return LewisCook;
                    yield return FloreminMesic;
                    yield return PaulPatterson;
                    yield return ChrisLamb;
                    yield return ChristofJans;
                    yield return NicholasPalmer;
                    yield return AshClifton;
                    yield return ManojVijayan;
                    yield return MannyPena;
                    yield return SamBenedict;
                    yield return LoganWebb;
                    yield return MauriceReeves;
                    yield return SamLombardo;
                    yield return StefanProgovac;
                    yield return KenTran;
                    yield return SeanLower;
                    yield return MatthewFrederick;
                    yield return RyanBowers;
                    yield return JamezPicard;
                    yield return VenkatGaddameedi;
                    yield return ChrisKnight;
                    yield return GaryAlfred;
                    yield return MaureenOrozco;
                    yield return LucasMedeiros;
                    yield return EduardoBrasil;
                    yield return MichaelAguilar;
                    yield return StebanDomingues;
                    yield return KelvynRisso;
                    yield return RegisBrand;
                    yield return PrabasiniHota;
                    yield return QuoiChung;
                    yield return MikeDonnelly;
                    yield return RafaelMelo;
                    yield return MikeVenkata;
                    yield return ThiagusFerreira;
                    yield return VenkatGundelli;
                    yield return SravaniAtluri;
                    yield return RakeshKasala;
                    yield return DestinStrohaber;
                    yield return StephanieVaul;
                    yield return MukeshGujju;
                    yield return SrinivasaraoArevarapu;
                    yield return MatthewMcEntee;
                    yield return NagarjunaBheemreddy;
                    yield return JaredCohn;
                    yield return OleksiyLevenets;
                    yield return JohnHaynes;
                    yield return ShivaKota;
                    yield return SivaJogireddy;
                    yield return SivaPavaniIndukari;
                    yield return SaiMaheshVegunta;
                    yield return RajeshParimilla;
                    yield return RyanCraig;
                    yield return SunilBolla;
                    yield return ToddHoeffert;
                    yield return RajBojja;
                    yield return SambaSarnala;
                    yield return ShanthiSandeep;
                    yield return NagarajNeelapala;
                    yield return NarendraKolukula;
                    yield return PujithaPunukollu;
                    yield return SrinivasuluThottempudi;
                    yield return PavanKuruba;
                    yield return VinithaAlla;
                    yield return AlexRybak;
                    yield return YuriyMuhin;
                    yield return SandeepMididoddi;
                    yield return HarshvardhanSindhu;
                    yield break;
                    // yield return ManojVijayan;
                }

                return GetNames().ToList();
            }
        }

        private class QANames
        {
            public const string AaronMarshall = "Aaron Marshall";
            public const string AlexCulp = "Alex Culp";
            public const string AnushaYeddula = "Anusha Yeddula";
            public const string BrandonVest = "Brandon Vest";
            public const string BrianHuynh = "Brian Huynh";
            public const string BrianZimmerman = "Brian Zimmerman";
            public const string EvgeniyaParfilova = "Evgeniya Parfilova";
            public const string FrankRivera = "Frank Rivera";
            public const string JamesGerken = "James Gerken";
            public const string JordanPearson = "Jordan Pearson";
            public const string KrishnaveniKondeti = "Krishnaveni Kondeti";
            public const string LakshmiPampana = "Lakshmi Pampana";
            public const string MansoorMian = "Mansoor Mian";
            public const string MonicaGullapalli = "Monica Gullapalli";
            public const string NickKesting = "Nick Kesting";
            public const string RadhaBotcha = "Radha Botcha";
            public const string RameshPendayala = "Ramesh Pendyala";
            public const string SandraVanDeWeerd = "Sandra Van De Weerd";
            public const string SarahPaul = "Sarah Paul";
            public const string ScottBrezniak = "Scott Brezniak";
            public const string TreyLago = "Trey Lago";
            public const string VenkatRaghavan = "Venkat Tircoveluri";
            public const string VishnuPriya = "Vishnu Priya";
            public const string ZvjezdanVeselinovic = "Zvjezdan Veselinovic";

            public const string JaredCohn = "Jared Cohn";

            public const string AlexKudryashov = "Alex Kudryashov";
            public const string GeraldPeer = "Gerald Peer";
            public const string HarikaAppalabhatla = "Harika Appalabhatla";
            public const string KevinMurphy = "Kevin Murphy";
            public const string MobinaQuadir = "Mobina Quadir";
            public const string RobertMosqueda = "Robert Mosqueda";
            public const string SuparnaShaligram = "Suparna Shaligram";
            public const string SarahMcCallister = "Sarah McCallister";
            public const string TJNoor = "TJ Noor";
            //public const string DestinStrohaber = "Destin Strohaber";

            public static List<string> GetAllQANames()
            {
                IEnumerable<string> GetNames()
                {
                    yield return GeraldPeer;
                    yield return FrankRivera;
                    yield return BrianHuynh;
                    yield return AaronMarshall;
                    yield return JamesGerken;
                    yield return BrianZimmerman;
                    yield return SandraVanDeWeerd;
                    yield return BrandonVest;
                    yield return SarahMcCallister;
                    yield return SarahPaul;
                    yield return ScottBrezniak;
                    yield return KevinMurphy;
                    // yield return DestinStrohaber;
                    yield return TreyLago;
                    yield return JordanPearson;
                    yield return AlexKudryashov;
                    yield return MobinaQuadir;
                    yield return MansoorMian;
                    yield return ZvjezdanVeselinovic;
                    yield return TJNoor;
                    yield return JaredCohn;
                    yield return SuparnaShaligram;
                    yield return VishnuPriya;
                    yield return VenkatRaghavan;
                    yield return RobertMosqueda;
                    yield return RameshPendayala;
                    yield return AlexCulp;
                    yield return HarikaAppalabhatla;
                    yield return KrishnaveniKondeti;
                    yield return LakshmiPampana;
                    yield return MonicaGullapalli;
                    yield return RadhaBotcha;
                    yield return AnushaYeddula;
                    yield return EvgeniyaParfilova;
                    yield return NickKesting;

                    yield break;
                }

                return GetNames().ToList();
            }
        }
    }
}
