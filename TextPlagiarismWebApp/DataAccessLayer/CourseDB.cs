using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TextPlagiarismWebApp.Models;

namespace TextPlagiarismWebApp.DataAccessLayer
{
    public class CourseDB :DbContext
    {
        public CourseDB() : base("name = DefaultConnection")
        {

        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Submission> Submissions { get; set; }

    }
}