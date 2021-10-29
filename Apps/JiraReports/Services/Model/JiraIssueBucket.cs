using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Model
{
    public class JiraIssueBucket
    {
        /// <summary>
        /// Start hour value for this bucket
        /// </summary>
        public decimal StartValue { get; set; }

        /// <summary>
        /// End hour value for this bucket
        /// </summary>
        public decimal EndValue { get; set; }

        /// <summary>
        /// Number of points this bucket is worth
        /// </summary>
        public decimal Points { get; set; }

        /// <summary>
        /// The bucket's index
        /// </summary>
        public int BucketNumber { get; set; }

        /// <summary>
        /// The number of issues in this bucket for the past 2 years
        /// </summary>
        public int IssueCount { get; set; }

        public void Parse(string line)
        {
            string[] items = line.Split(new string[] { @"	" }, StringSplitOptions.RemoveEmptyEntries);
            if(items.Length != 5) throw new ArgumentException("Cannot parse line for Bucket");

            this.StartValue = decimal.Parse(items[0]);
            this.EndValue = decimal.Parse(items[1]);
            this.Points = decimal.Parse(items[2]);
            this.BucketNumber = int.Parse(items[3]);
            this.IssueCount = int.Parse(items[4]);
        }
    }
}
