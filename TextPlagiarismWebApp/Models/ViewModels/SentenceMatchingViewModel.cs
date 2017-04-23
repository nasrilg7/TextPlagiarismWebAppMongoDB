using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPlagiarismWebApp.Models.ViewModels
{
    public class SentenceMatchingViewModel
    {
        public string Id { get; set; }
        public string sentence { get; set; }
        public string documentId { get; set; }
        public string documentPath { get; set; }
        public string matchedSentenceId { get; set; }
        public string matchedSentence { get; set; }

    }
}