using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public interface IJiraAgileService
    {
        List<AgileBoard> GetAllBoards();
        List<AgileBoard> GetDeliveryBoards();
        List<AgileSprint> GetSprintsForBoard(string boardId);
        List<JiraIssue> GetIssuesForSprint(string sprintId, params string[] fields);
        List<int> GetBoardSprintNumbers(IEnumerable<string> boardIds);
        List<int> GetBoardSprintNumbers(string boardId);
    }
}
