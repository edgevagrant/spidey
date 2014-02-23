using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.Storage
{
    public enum JobType
    { 
        Fulltime,
        Contract,
        Parttime,
        Other
    }

    public class JobDesc
    {
        public List<string> Location { get; set; }
        public string Title { get; set; }
        public List<string> Duties { get; set; }
        public List<string> Requirements { get; set; }
        public DegreeType Degree { get; set; }
        public List<string> Industries { get; set; }
        public string SalaryScope { get; set; }

        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        public List<JobType> WorkTypes { get; set; }
        public string Note { get; set; }
    }
}
