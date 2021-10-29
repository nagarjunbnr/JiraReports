using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.NoEngNotes
{
    public class NoNotesBySprintDataItem
    {
        [DisplayName("Sprint")]
        public string Sprint { get; set; }

        [DisplayName("Count")]
        public int Count { get; set; }
    }
}
