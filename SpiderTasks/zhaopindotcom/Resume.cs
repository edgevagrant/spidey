using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.Storage
{

    public class Resume
    {
        public string ResumeID { get; set; }
        public string Name { get; set; }
        public short Age { get; set; }
        public bool IsMarried { get; set; }
        public DegreeType Degree { get; set; }
        public string Hukou { get; set; }
        public string LiveIn { get; set; }
        public List<Education> Educations { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<ContactItem> Contacts { get; set; }
        public ExpectedPosition ExpectedJob { get; set; }
    }

    public class ContactItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ExpectedPosition
    {
        public List<string> Location{get;set;}
        public string SalaryScope { get; set; }
        public string Note { get; set; }
        public List<string> JobTitles { get; set; }
        public List<string> Industries{get;set;}
    }
    public class Experience
    {
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        TimeSpan Period { get; set; }
        public string Note { get; set; }
    }
    public class Education
    {
        TimeSpan Period { get; set; }
        string School { get; set; }
        DegreeType Degree { get; set; }
        string Major { get; set; }
        string Minor { get; set; }
        List<string> Courses { get; set; }
    }


}
