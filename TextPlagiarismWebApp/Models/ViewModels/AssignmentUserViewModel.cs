using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPlagiarismWebApp.Models.ViewModels
{
    public class AssignmentUserViewModel
    {
        public List<Assignment> Assignments { get; set; }
        public Course Course { get; set; }
    }
}