using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports
{
    public interface IGraphDataPoint<XDataType, YDataType>
    {
        XDataType X { get; set; }
        YDataType Y { get; set; }
    }
}
