using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPlagiarismWebApp.Models.ViewModels
{
    public class ManageDocumentViewModel
    {
        public BsonDocument sentence { get; set; }
        //public List<BsonDocument> document { get; set; }
        public string documentPath { get; set; }
    }
}