using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TextPlagiarismWebApp.Models;
using TextPlagiarismWebApp.Models.ViewModels;

namespace TextPlagiarismWebApp.Controllers
{
    public class AnalyzeController : Controller
    {
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Analyze(string id, string path)
        {
            DocumentsLayer dl = DocumentsLayer.getInstance();
            var sentences = dl.getMatchedSentencesByDocumentId(id, path);
            ViewBag.Id = id;

            // Calculate the original content vs Plagiarism content
            var plagiarizedContent = Math.Truncate(((double)sentences.Count/dl.getDocumentSentencesCount()) * 100);
            ViewBag.PlagiarizedContent = string.Format("{0:N2}%",plagiarizedContent);


            return View(sentences);
        }
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult ManageDocument(string id)
        {
            DocumentsLayer dl = DocumentsLayer.getInstance();
            var manageDocumentViewModel = new ManageDocumentViewModel();
            manageDocumentViewModel.sentence = dl.getSentenceBySentenceId(id);
            manageDocumentViewModel.documentPath = manageDocumentViewModel.sentence["document_path"].AsString.ToString().Replace(".docx", ".html");
            ViewBag.Sentence = manageDocumentViewModel.sentence["sentence"].AsString.ToString();
            return View(manageDocumentViewModel);
            
        }
    }
}