using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.NoEngNotes
{
    public class AllEngNotesForQADataItem
    {
        [DisplayName("Sprint")]
        public string Sprint { get; set; }

        [DisplayName("Issue")]
        public string Issue { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Eng Notes")]
        public string EngNotes { get; set; }
    }
}
