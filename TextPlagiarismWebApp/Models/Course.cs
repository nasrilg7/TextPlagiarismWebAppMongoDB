using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextPlagiarismWebApp.Models
{
    public class Course
    {
        public Course()
        {
            Assignments = new HashSet<Assignment>();
            EnrolledStudentsEmails = new HashSet<string>();
        }

        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Hours { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<string> EnrolledStudentsEmails { get; set; }
        public string UserName{ get; set; }

    }

    public class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string TimeCreated { get; set; }

        public string TimeDue { get; set; }
        public int Weight { get; set; }

        public ICollection<Submission> Submissions { get; set; }

        public virtual Course Course { get; set; }
    }

    public class Submission
    {
        [Key]
        public string Id { get; set; }
        public string TimeSubmitted { get; set; }
        public string FeedBack { get; set; }

        public string StudentName { get; set; }

        public string DocumentURL { get; set; }
        public string DocumentName { get; set; }

        public virtual Assignment Assignment { get; set; }
    }
}