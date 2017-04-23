using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPlagiarismWebApp.Models
{
    public class Course
    {
        public Course()
        {
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Hours { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
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

        public DateTime TimeCreated { get; set; }

        public ICollection<Submission> Submissions { get; set; }

        public virtual Course Course { get; set; }
    }

    public class Submission
    {
        [Key]
        public string Id { get; set; }

        public string DocumentURL { get; set; }

        public virtual Assignment Assignment { get; set; }
    }
}