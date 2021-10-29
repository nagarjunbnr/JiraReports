using JiraReports.Services;
using JiraReports.Services.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static JiraReports.ViewModel.Reports.KeyPoints.KeyPointsOptionsViewModel;

namespace JiraReports.ViewModel.Reports.SprintHealth
{

    [InstancePerRequest]
    public class SprintHealthOptionsViewModel : ReportOptionsModel
    {

        private IJiraAgileService agileService;

        private List<SelectableBoard> _selectableBoards = null;
        private List<SelectableSprintNumber> _selectableSprintNumbers = null;
        private SelectableBoard _selectedBoard = null;
        private SelectableSprintNumber _selectedSprintNumber = null;

        public List<SelectableBoard> SelectableBoards
        {
            get => this._selectableBoards;
            set
            {
                this._selectableBoards = value;
                RaisePropertyChangedEvent("SelectableBoards");
            }
        }

        public List<SelectableSprintNumber> SelectableSprintNumbers
        {
            get => this._selectableSprintNumbers;
            set
            {
                this._selectableSprintNumbers = value;
                RaisePropertyChangedEvent("SelectableSprintNumber");
            }
        }

        public SelectableBoard SelectedBoard
        {
            get => this._selectedBoard;
            set
            {
                this._selectedBoard = value;
                this.PopulateSelectableSprintNumbers();
                RaisePropertyChangedEvent("SelectedBoard");
            }
        }

        public SelectableSprintNumber SelectedSprintNumber
        {
            get => this._selectedSprintNumber;
            set
            {
                this._selectedSprintNumber = value;
                RaisePropertyChangedEvent("SelectedSprintNumber");
            }
        }

        public SprintHealthOptionsViewModel(IJiraAgileService agileService)
        {
            this.agileService = agileService;

            this.PopulateSelectableBoards();
        }

        public void PopulateSelectableBoards()
        {
            SelectableBoards = agileService.GetDeliveryBoards().Select(s => new SelectableBoard()
            {
                ID = s.ID,
                Name = s.Name
            }).ToList();

            SelectedBoard = SelectableBoards.FirstOrDefault();
        }

        public void PopulateSelectableSprintNumbers()
        {
            List<int> sprintNumbers 
                = agileService.GetBoardSprintNumbers(new string[] { this.SelectedBoard.ID });

            this.SelectableSprintNumbers = sprintNumbers.Select(n => new SelectableSprintNumber()
            {
                Name = n.ToString(),
                Value = n
            }).OrderByDescending(s => s.Value).ToList();

            this.SelectedSprintNumber = this.SelectableSprintNumbers.FirstOrDefault();
            RaisePropertyChangedEvent("SelectableSprintNumbers");

        }

    }
}
