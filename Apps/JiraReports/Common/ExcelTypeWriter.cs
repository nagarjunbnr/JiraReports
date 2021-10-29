using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Common
{
    public class ExcelTypeWriter
    {
        private Type objType;
        private List<ColumnMap> columnMaps = new List<ColumnMap>();

        public ExcelTypeWriter(Type objType)
        {
            this.objType = objType;

            int columnIndex = 1;
            Type dateTimeType = typeof(DateTime);
            foreach (PropertyInfo property in this.objType.GetProperties())
            {
                ColumnMap map = new ColumnMap()
                {
                    Property = property,
                    ColumnIndex = columnIndex
                };

                if (property.PropertyType == dateTimeType || Nullable.GetUnderlyingType(property.PropertyType) == dateTimeType)
                {
                    map.Formatter = FormatDate;
                }
                else
                {
                    map.Formatter = FormatObject;
                }

                columnMaps.Add(map);
                columnIndex++;
            }
        }

        public void WriteHeader(ExcelWorksheet worksheet)
        {
            foreach (ColumnMap map in columnMaps)
            {
                worksheet.Cells[1, map.ColumnIndex].Value = map.Property.Name;
            }
        }

        public void WriteRow(ExcelWorksheet worksheet, int rowNumber, object val)
        {
            foreach (ColumnMap map in columnMaps)
            {
                worksheet.Cells[rowNumber, map.ColumnIndex].Value = map.Formatter(map.Property.GetValue(val));
            }
        }

        private object FormatObject(object value)
        {
            return value;
        }

        private object FormatDate(object value)
        {
            if (value == null) return null;

            return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
        }

        private class ColumnMap
        {
            public PropertyInfo Property { get; set; }
            public int ColumnIndex { get; set; }
            public Func<object, object> Formatter { get; set; }
        }
    }
}
