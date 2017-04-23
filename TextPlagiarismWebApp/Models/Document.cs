using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TextPlagiarismWebApp.DataAccessLayer;
using MongoDB.Bson;

namespace TextPlagiarismWebApp.Models
{
    public class Document
    {
        public ObjectId Id { get; set; }

        public string document { get; set; }
        public string path { get; set; }

        public Document()
        {
            Id = ObjectId.GenerateNewId();
        }


    }
}