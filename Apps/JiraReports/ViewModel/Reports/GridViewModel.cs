using JiraReports.Common;
using JiraReports.Services;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.Reports
{
    [InstancePerRequest]
    public abstract class GridViewModel : ReportDisplayModel
    {
        protected Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();

        public DataTable Data
        {
            get => GenerateData();
            set
            {

            }
        }

        protected abstract DataTable GenerateData();
    }

    public abstract class GridViewModel<ModelType> : GridViewModel
    {
        private IEnumerable<ModelType> modelData;
        private DataTable table;

        public ViewModelCommand SaveExcelCommand { get; set; }

        public GridViewModel()
        {
            this.SaveExcelCommand = new ViewModelCommand(SaveExcel, CanExecuteSave);
        }

        private bool CanExecuteSave()
        {
            return this.table != null;
        }

        private void SaveExcel()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel File (*.xlsx)|*.xlsx";

            if(save.ShowDialog() == true)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    using (Stream outputStream = File.OpenWrite(save.FileName))
                    {
                        using (ExcelPackage outputPackage = new ExcelPackage(outputStream))
                        {
                            ExcelWorksheet worksheet = outputPackage.Workbook.Worksheets.Add("Time Sheet");

                            ExcelTypeWriter typeWriter = new ExcelTypeWriter(typeof(ModelType));
                            int rowNumber = 1;

                            int cellNumber = 1;
                            foreach (string columnName in columns.Keys)
                            {
                                worksheet.Cells[rowNumber, cellNumber].Value = columnName;
                                cellNumber++;
                            }

                            rowNumber++;
                            foreach (ModelType val in modelData)
                            {
                                typeWriter.WriteRow(worksheet, rowNumber, val);
                                rowNumber++;
                            }

                            outputPackage.Save();
                        }
                    }
                }));

                thread.Start();
            }
        }

        public void SetModel(IEnumerable<ModelType> modelData)
        {
            this.columns.Clear();
            this.table = null;
            this.modelData = modelData;

            GenerateData();
        }

        protected override DataTable GenerateData()
        {
            if(this.table != null) return this.table;

            GenerateColumns();

            this.table = new DataTable();

            if(modelData == null) return this.table;

            foreach((string name, PropertyInfo property) in this.columns)
            {
                this.table.Columns.Add(name, property.PropertyType);
            }

            foreach(ModelType model in modelData)
            {
                DataRow row = this.table.Rows.Add();
                foreach ((string name, PropertyInfo property) in this.columns)
                {
                    row[name] = property.GetValue(model);
                }
            }

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                RaisePropertyChangedEvent("Data");
                this.SaveExcelCommand.OnCanExcuteChanged();
            });
            return table;
        }

        private void GenerateColumns()
        {
            if(columns.Count != 0) return;

            Type modelType = typeof(ModelType);
            PropertyInfo[] properties = modelType.GetProperties();

            foreach(PropertyInfo property in properties)
            {
                DisplayNameAttribute displayName = property.GetCustomAttribute<DisplayNameAttribute>(true);
                columns.Add(displayName?.DisplayName ?? property.Name, property);
            }
        }
    }



}
