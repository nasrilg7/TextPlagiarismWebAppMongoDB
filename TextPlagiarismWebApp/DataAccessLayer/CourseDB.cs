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
        public DbSet<Course> Courses { get; set; }

    }
}