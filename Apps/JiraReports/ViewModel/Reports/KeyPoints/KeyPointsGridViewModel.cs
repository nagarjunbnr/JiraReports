using JiraReports.Common;
using JiraReports.Reports.Common.EffectiveValue;
using JiraReports.Reports.KeyPoints.Productivity;
using JiraReports.Reports.KeyPoints.ViewModel;
using JiraReports.Reports.TimeTracking;
using JiraReports.Services;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.Reports.KeyPoints
{
    [InstancePerRequest]
    public class KeyPointsGridViewModel : MultiGridViewModel
    {
        private List<KeyPointsSprintTotals> kpiSprintTotals = new List<KeyPointsSprintTotals>();
        private List<TeamSprintStatsView> teamSprintStats = new List<TeamSprintStatsView>();

        public ViewModelCommand ExportKPIExcelCommand { get; set; }
        public ViewModelCommand ExportKPIPPTCommand { get; set; }

        public List<KeyPointsSprintTotals> KPISprintTotals
        {
            get => this.kpiSprintTotals;
            set
            {
                this.kpiSprintTotals = value;

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.ExportKPIExcelCommand.OnCanExcuteChanged();
                });
            }
        }

        public List<TeamSprintStatsView> TeamSprintStats
        {
            get => this.teamSprintStats;
            set
            {
                this.teamSprintStats = value;

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.ExportKPIExcelCommand.OnCanExcuteChanged();
                });
            }
        }


        public KeyPointsGridViewModel(IServiceLocator serviceLocator) : base(serviceLocator)
        {
            this.ExportKPIExcelCommand = new ViewModelCommand(ExportKPIExcel, CanExecuteExportKPIExcel);
        }

        public new void ClearModels()
        {
            this.kpiSprintTotals.Clear();
            base.ClearModels();
        }

        private bool CanExecuteExportKPIExcel()
        {
            return this.kpiSprintTotals.Count > 0;
        }

        private void ExportKPIExcel()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel File (*.xlsx)|*.xlsx";
            save.FileName = "KPI Report.xlsx";

            if (save.ShowDialog() == true)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    using (Stream outputStream = File.OpenWrite(save.FileName))
                    {
                        using (ExcelPackage outputPackage = new ExcelPackage())
                        {
                            outputPackage.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("JiraReports.Reports.KeyPoints.Files.KPITemplate.xlsx"));
                            ExcelWorksheet dataWorksheet = outputPackage.Workbook.Worksheets["KPI - Data"];
                            ExcelWorksheet sprintValueDataWorksheet = outputPackage.Workbook.Worksheets["Sprint Value - Data"];

                            //var productivityByProjectSprint = this.productivityData
                            //    .GroupBy(t => new { t.Sprint, t.Project })
                            //    .Select(t => new ProductivityTeamTotals()
                            //    {
                            //        Project = t.Key.Project,
                            //        Sprint = t.Key.Sprint,
                            //        ProjectedHours = Math.Round(t.Sum(p => p.Projected), 2),
                            //        LoggedHours = Math.Round(t.Sum(h => h.Projected > 0 ? h.Hours : 0M), 2)
                            //    })
                            //    .Select(t => new
                            //    {
                            //        t.Project,
                            //        t.Sprint,
                            //        ProductivityData = t,
                            //        PredictabilityData = this.predictabilityData
                            //            .Where(p => p.Sprint == t.Sprint && String.Compare(p.Project, t.Project, true) == 0)
                            //            .SingleOrDefault()
                            //    })
                            //    .OrderBy(t => t.Project)
                            //    .ThenBy(t => t.Sprint)
                            //    .ToList();

                            //var chartData = productivityByProjectSprint
                            //        .GroupBy(t => t.Sprint)
                            //        .Select(t => new
                            //        {
                            //            Project = "All",
                            //            Sprint = t.Key,
                            //            ProductivityPct = t.Sum(p => p.ProductivityData.LoggedHours) / t.Sum(p => p.ProductivityData.ProjectedHours),
                            //            PredictabilityPct = Math.Round(1 - ((t.Sum(p => p.PredictabilityData.HoursPulled) / (decimal)t.Sum(p => p.PredictabilityData.HoursPlanned))), 2)
                            //        })
                            //    .Union(productivityByProjectSprint.Select(t => new
                            //        {
                            //            Project = t.Project,
                            //            Sprint = t.Sprint,
                            //            ProductivityPct = t.ProductivityData.Productivity / 100M,
                            //            PredictabilityPct = t.PredictabilityData.PredictabilityScorePct
                            //        }))
                            //    .OrderBy(t => t.Project)
                            //    .ThenBy(t => t.Sprint)
                            //    .ToList();

                            int teamColumnIndex = 1;
                            int sprintColumnIndex = 2;
                            int predictabilityColumnIndex = 3;
                            int productivityColumnIndex = 4;
                            int sprintValueColumnIndex = 5;
                            int slidingWindowSize = 6;

                            var chartDataGrouped = KPISprintTotals
                                .GroupBy(d => d.Team);

                            int rowNumber = 2;
                            foreach (var projectChartData in chartDataGrouped)
                            {
                                var projectSprintChartData = projectChartData.OrderByDescending(d => d.Sprint).Take(slidingWindowSize).OrderBy(d => d.Sprint).ToList();

                                if (projectSprintChartData.Count < slidingWindowSize) //not enough data to populate the full sliding window
                                {
                                    var firstSprint = projectSprintChartData.FirstOrDefault();

                                    //copy the first sprint in the window and fill in the missing sprints using that data
                                    if (firstSprint != null)
                                    {
                                        int missingSprintCount = slidingWindowSize - projectSprintChartData.Count;

                                        for (int i = 1; i <= missingSprintCount; i++)
                                        {
                                            projectSprintChartData.Insert(0, new KeyPointsSprintTotals()
                                            {
                                                Team = firstSprint.Team,
                                                Sprint = firstSprint.Sprint - i,
                                                Productivity = firstSprint.Productivity,
                                                Predictability = firstSprint.Predictability
                                            });
                                        }
                                    }
                                }

                                foreach(var item in projectSprintChartData)
                                {
                                    dataWorksheet.Cells[rowNumber, teamColumnIndex].Value = item.Team;
                                    dataWorksheet.Cells[rowNumber, sprintColumnIndex].Value = item.Sprint;
                                    dataWorksheet.Cells[rowNumber, predictabilityColumnIndex].Value = item.PredictabilityPct > 1 ? 1M : item.PredictabilityPct;
                                    dataWorksheet.Cells[rowNumber, productivityColumnIndex].Value = item.ProductivityPct > 1 ? 1M : item.ProductivityPct;
                                    dataWorksheet.Cells[rowNumber, sprintValueColumnIndex].Value = item.SprintValue;
                                    rowNumber++;
                                }

                                int svDataRownumber = 2;
                                int svTeamColumnIndex = 1;
                                int svSprintColumnIndex = 2;
                                int svTotalLoggedHoursColumnIndex = 3;
                                int svValueLoggedHoursColumnIndex = 4;
                                int svNonValueLoggedHoursColumnIndex = 5;
                                int svFailureRateColumnIndex = 6;
                                int svPredictabilityColumnIdex = 7;
                                int svTotalResourcesColumnIndex = 8;
                                int svSprintValueColumnIndex = 9;

                                foreach (var s in this.TeamSprintStats)
                                {
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svTeamColumnIndex].Value = s.Team;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svSprintColumnIndex].Value = s.Sprint;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svTotalLoggedHoursColumnIndex].Value = s.TotalLoggedHours;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svValueLoggedHoursColumnIndex].Value = s.ValueAddedLoggedHours;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svNonValueLoggedHoursColumnIndex].Value = s.NonValueLoggedHours;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svFailureRateColumnIndex].Value = s.FailureRateHours;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svPredictabilityColumnIdex].Value = s.Predictability;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svTotalResourcesColumnIndex].Value = s.TotalResources;
                                    sprintValueDataWorksheet.Cells[svDataRownumber, svSprintValueColumnIndex].Value = s.SprintValue;

                                    svDataRownumber++;
                                }
                            }

                            dataWorksheet.Calculate();
                            outputPackage.SaveAs(outputStream);
                        }
                    }
                }));

                thread.Start();
            }
        }
    }
}