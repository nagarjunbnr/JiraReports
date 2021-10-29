using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports
{
    public interface IGraphAxis<AxisDataType>
    {
        string Label { get; set; }
        AxisDataType Min { get; set; }
        AxisDataType Max { get; set; }
    }
}
