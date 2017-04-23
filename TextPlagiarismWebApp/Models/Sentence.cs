using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;

namespace TextPlagiarismWebApp.Models
{
    public class Sentence
    {
        public ObjectId Id { get; set; }

        public string sentence { get; set; }
        public string path { get; set; }
        public ObjectId DID { get; set; }
        public Sentence()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}