using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JiraReports.Services.PersonnelService;

namespace JiraReports.Services
{
    public interface IPersonnelService
    {
        PersonRole GetRole(string person, string sprint);
        bool IsPersonOff(string person, DateTime date);
        decimal GetPersonAllocation(string person, string project, string sprint);
        List<ProjectSprintAllocation> GetAllPersonAllocations(string person, string sprint);
        IList<PersonRole> GetRoles(string person, string project, string sprint);
        PersonRole GetFirstRole(string person, string project, string sprint);
        List<(string Name, PersonRole Role)> GetProjectContributors(string project, string sprint);
    }
}
