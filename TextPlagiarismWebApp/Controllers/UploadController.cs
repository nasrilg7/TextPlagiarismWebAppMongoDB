using Microsoft.Office.Interop.Word;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TextPlagiarismWebApp.Models;

namespace TextPlagiarismWebApp.Controllers
{
    public class UploadController : Controller
    {
        [Filter.Filter(Roles = "Student")]
        public ActionResult UploadDocument(string id)
        {
            ViewBag.SubmissionID = id;
            return View();
        }

        [HttpPost]
        [Filter.Filter(Roles = "Student")]
        public ActionResult Upload(string id)
        {
            bool isSavedSuccessfully = true;
            string fName = string.Empty;
            var path = string.Empty;
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                //Save file content goes here
                fName = file.FileName;
                if (file != null && file.ContentLength > 0)
                {

                    var originalDirectory = new DirectoryInfo(string.Format("{0}Documents", Server.MapPath(@"\")));

                    string pathString = Path.Combine(originalDirectory.ToString(), "documentpath");

                    var fileName1 = Path.GetFileName(file.FileName);


                    bool isExists = Directory.Exists(pathString);

                    if (!isExists)
                        Directory.CreateDirectory(pathString);

                    path = string.Format("{0}\\{1}", pathString, file.FileName);
                    try
                    {
                        file.SaveAs(path);
                    }
                    catch (Exception e)
                    {

                        throw new HttpException(404, "An error occured trying this action. Document are used by another process?");
                    }

                }

            }

            if (isSavedSuccessfully)
            {
                DocumentsLayer dl = DocumentsLayer.getInstance();
                Models.Document document = new Models.Document();
                //var sentencesText = readSentences(path, document.Id);
                ConvertToHtml(path, WdSaveFormat.wdFormatHTML);
                var documentText = readDocument(path);

                document.document = documentText;
                document.path = path.Replace(".docx", ".html");
                dl.insertDocument(document);
                //dl.setSentences(sentencesText);
                ViewBag.Document = documentText;
                ViewBag.Name = fName;

                var db = new ApplicationDbContext();
                var submission = db.Submissions.Find(id);
                submission.DocumentName = fName;
                submission.DocumentURL = document.path;
                submission.StudentName = User.Identity.Name;
                submission.TimeSubmitted = DateTime.Now.ToString("dd/MM/yyyy");
                submission.DocumentMongoDBID = document.Id.ToString();
                var assingment = db.Assignments.Find(submission.Assignment.Id);
                assingment.Submissions.Add(submission);
                db.SaveChanges();
                return RedirectToAction("ShowDocumentStudent", new { name = fName, path = document.path});

            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        [Filter.Filter(Roles = "Student")]
        public ActionResult ShowDocumentStudent(string name, string path)
        {
            ViewBag.Name = name;
            ViewBag.Path = path;
            return View();
        }

        [Filter.Filter(Roles = "Teacher")]
        public ActionResult ShowDocumentTeacher(string name,string sname, string id, string path)
        {
            ViewBag.Name = name;
            ViewBag.Path = path;
            ViewBag.SName = sname;
            ViewBag.DocumentMongoDBID = id;
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetHtmlPage(string path)
        {
            return new FilePathResult(path, "text/html");
        }

        private void ConvertToHtml(string path, WdSaveFormat format)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo wordFile = new FileInfo(path);

            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            try
            {
                word.Visible = false;
                word.ScreenUpdating = false;

                Object filename = (Object)wordFile.FullName;
                Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(ref filename, ref oMissing,
                                                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                                   ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                try
                {
                    doc.Activate();
                    object outputFileName = wordFile.FullName.Replace(".docx", ".html");
                    object fileFormat = format;
                    doc.SaveAs(ref outputFileName,
                               ref fileFormat, ref oMissing, ref oMissing,
                               ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                               ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                               ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                }
                finally
                {
                    object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                    ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
                    doc = null;
                }


            }
            finally
            {
                ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
                word = null;
            }
        }

        private static string readDocument(string filePath)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = @"" + filePath;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            var documentText = "";
            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                documentText += docs.Paragraphs[i + 1].Range.Text.ToString();
            }
            docs.Close();
            word.Quit();
            return documentText;


        }
        private static List<Sentence> readSentences(string filePath, ObjectId did)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = @"" + filePath;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            List<Sentence> sentencesList = new List<Sentence>();
            for (int i = 0; i < docs.Sentences.Count; i++)
            {
                Sentence sen = new Sentence();
                sen.sentence = docs.Sentences[i + 1].Text.ToString();
                sen.path = filePath;
                sen.DID = did;
                sentencesList.Add(sen);
            }
            docs.Close();
            word.Quit();
            return sentencesList;


        }
    }
}