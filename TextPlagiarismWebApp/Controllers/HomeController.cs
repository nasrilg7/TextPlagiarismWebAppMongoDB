using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TextPlagiarismWebApp.Models;
using TextPlagiarismWebApp.Models.ViewModels;

namespace TextPlagiarismWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult getDocuments()
        {
            ////if you wanna show documents and sentences you have to create a ViewModel
            //var documentsLayer = DocumentsLayer.getInstance();
            //var documents = documentsLayer.getDocuments();
            ////var sentences = documentsLayer.getSentences();
            //DocumentsViewModel vm = new DocumentsViewModel();
            //vm.Documents = documents;
            ////vm.Sentences = sentences;

            //return View("getDocuments", vm);
            return View();
        }
    }
}