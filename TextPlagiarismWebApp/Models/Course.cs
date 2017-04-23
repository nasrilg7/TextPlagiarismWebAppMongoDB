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

    }
}