using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports
{
    public interface IGraphReportResponse<XDataType, YDataType>
    {
        IGraphAxis<XDataType> XAxis { get; set; }
        IGraphAxis<YDataType> YAxis { get; set; }
    }
}
