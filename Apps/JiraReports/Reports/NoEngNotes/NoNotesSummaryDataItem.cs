using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.NoEngNotes
{
    public class NoNotesSummaryDataItem
    {
        [DisplayName("Sprint")]
        public string Sprint { get; set; }

        [DisplayName("Issue")]
        public string Issue { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Engineer")]
        public string Engineer { get; set; }
    }
}
