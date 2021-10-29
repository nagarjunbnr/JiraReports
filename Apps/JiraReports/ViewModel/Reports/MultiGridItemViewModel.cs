using JiraReports.Common;
using JiraReports.Services;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections;
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
    public class MultiGridItemViewModel : ViewModel
    {
        private IEnumerable modelData;
        private DataTable table;
        protected Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();

        public ViewModelCommand SaveExcelCommand { get; set; }
        public string Name { get; set; }

        public DataTable Data
        {
            get => this.table;
            set
            {
                this.table = value;
                RaisePropertyChangedEvent("Data");
            }
        }

        public MultiGridItemViewModel()
        {
            this.SaveExcelCommand = new ViewModelCommand(SaveExcel, CanExecuteSave);
        }

        private bool CanExecuteSave()
        {
            return (this.table != null);
        }

        public void SetModel(IEnumerable modelData)
        {
            this.columns.Clear();
            this.table = null;
            this.modelData = modelData;

            GenerateData();
        }

        private void SaveExcel()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel File (*.xlsx)|*.xlsx";
            save.FileName = this.Name;

            if (save.ShowDialog() == true)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    using (Stream outputStream = File.OpenWrite(save.FileName))
                    {
                        using (ExcelPackage outputPackage = new ExcelPackage(outputStream))
                        {
                            ExcelWorksheet worksheet = outputPackage.Workbook.Worksheets.Add("Time Sheet");

                            ExcelTypeWriter typeWriter = new ExcelTypeWriter(modelData.ToObjectEnumerable().First().GetType());
                            int rowNumber = 1;

                            int cellNumber = 1;
                            foreach (string columnName in columns.Keys)
                            {
                                worksheet.Cells[rowNumber, cellNumber].Value = columnName;
                                cellNumber++;
                            }

                            rowNumber++;
                            foreach (object val in modelData)
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


        protected DataTable GenerateData()
        {
            if (this.table != null) return this.table;

            GenerateColumns();

            this.table = new DataTable();

            if (modelData == null) return this.table;

            foreach ((string name, PropertyInfo property) in this.columns)
            {
                //bool isNullable = false;
                Type baseType = property.PropertyType;
                Type nullableType = Nullable.GetUnderlyingType(baseType);
                if (nullableType != null)
                {
                    baseType = nullableType;
                    //isNullable = true;
                }

                DataColumn column = this.table.Columns.Add(name, baseType);
                //column.AllowDBNull = isNullable;
            }

            foreach (object model in modelData)
            {
                DataRow row = this.table.Rows.Add();
                foreach ((string name, PropertyInfo property) in this.columns)
                {
                    object value = property.GetValue(model);
                    row[name] = (value == null) ? DBNull.Value : value;
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
            if (columns.Count != 0) return;

            Type modelType = modelData.ToObjectEnumerable().FirstOrDefault()?.GetType();

            if(modelType == null)
            {
                modelType = GetModelType(modelData.GetType());
            }

            PropertyInfo[] properties = modelType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                DisplayNameAttribute displayName = property.GetCustomAttribute<DisplayNameAttribute>(true);
                columns.Add(displayName?.DisplayName ?? property.Name, property);
            }
        }

        private Type GetModelType(Type type)
        {
            if(!type.IsConstructedGenericType)
                return null;

            return type.GetGenericArguments().First();
        }
    }
}
