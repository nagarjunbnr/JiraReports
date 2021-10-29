using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class ItemList<ItemType>
    {
        public int Size { get; set; }
        public ItemType[] Items { get; set; }
    }
}
