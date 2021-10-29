using JiraReports.Services;
using JiraReports.Services.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JiraReports.ViewModel.Reports.KeyPoints
{
    [InstancePerRequest]
    public class KeyPointsOptionsViewModel : ReportOptionsModel
    {

        private IJiraAgileService agileService;

        public List<SelectableBoard> SelectableBoards
        {
            get; set;
        }

        public List<SelectableSprintNumber> SelectableSprintNumbers
        {
            get; set;
        }

        public SelectableBoard SelectedBoard
        {
            get; set;
        }

        public SelectableSprintNumber SelectedSprintNumber
        {
            get; set;
        }

        public KeyPointsOptionsViewModel(IJiraAgileService agileService) : base()
        {
            this.agileService = agileService;

            InitializeTeamSelector();
            InitializeSprintSelector();
        }

        private void InitializeTeamSelector()
        {
            var defaultBoard = new SelectableBoard()
            {
                Name = "All",
                ID = null
            };

            SelectableBoards = agileService.GetDeliveryBoards().Select(b => new SelectableBoard()
            {
                ID = b.ID,
                Name = b.Name
            }).ToList();

            this.SelectedBoard = defaultBoard;
            SelectableBoards.Insert(0, defaultBoard);
        }

        private void InitializeSprintSelector()
        {
            SelectableSprintNumbers = agileService.GetBoardSprintNumbers(this.SelectableBoards
                .Where(d => !string.IsNullOrEmpty(d.ID)).Select(b => b.ID)).OrderByDescending(n => n)
                .Select(v => new SelectableSprintNumber() {
                    Name = v.ToString(),
                    Value = v
                })
                .ToList();

            SelectableSprintNumbers.Insert(0, new SelectableSprintNumber()
            {
                Name = "Any",
                Value = null
            });

            this.SelectedSprintNumber = this.SelectableSprintNumbers.First();
        }

        public class SelectableBoard
        {

            public string Name { get; set; }

            public string ID { get; set; }

        }

        public class SelectableSprintNumber
        {
            public string Name { get; set; }

            public int? Value { get; set; }
        }

    }
}
