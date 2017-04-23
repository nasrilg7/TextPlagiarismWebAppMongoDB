using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPlagiarismWebApp.Models.ViewModels
{
    public class DocumentsViewModel
    {
        public List<Document> Documents { get; set; }
        public List<Sentence> Sentences { get; set; }


    }
}