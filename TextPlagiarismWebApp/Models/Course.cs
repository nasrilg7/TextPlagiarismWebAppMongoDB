using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPlagiarismWebApp.Models
{
    public class Course
    {
        public string courseId { get; set; }
        public string courseName { get; set; }
        public string description { get; set; }
        public int Hours { get; set; }

        public List<Assignments> assignments { get; set; }

        public class Assignments
        {
            public string id { get; set; }
            public string name { get; set; }
            public DateTime timeCreated { get; set; }
            public List<Submission> Submissions { get; set; }

            public class Submission
            {
                public string userID { get; set; }
                public string documentURL { get; set; }

            }
        }
    }
}