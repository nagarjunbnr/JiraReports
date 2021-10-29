using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.NoEngNotes
{
    public class NoNotesByEngineerDataItem
    {
        [DisplayName("Engineer")]
        public string Engineer { get; set; }

        [DisplayName("Count")]
        public int Count { get; set; }
    }
}
