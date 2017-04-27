using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TextPlagiarismWebApp.DataAccessLayer;
using TextPlagiarismWebApp.Models;

namespace TextPlagiarismWebApp.Controllers
{
    public class SubmissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Submissions
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Index(string id)
        {
            var submissions = db.Submissions
                .Where(s => s.Assignment.Id == id && !string.IsNullOrEmpty(s.DocumentName))
                .Distinct()
                .ToList();
            return View(submissions);
        }

        // GET: Submissions/Details/5
        [Filter.Filter(Roles = "Student")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Submission submission = db.Submissions.Find(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            
            return View(submission);
        }

        // GET: Submissions/Create
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Submissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Create([Bind(Include = "Id,TimeSubmitted,FeedBack,StudentName,DocumentURL,DocumentName")] Submission submission)
        {
            if (ModelState.IsValid)
            {
                db.Submissions.Add(submission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(submission);
        }

        // GET: Submissions/Edit/5
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Submission submission = db.Submissions.Find(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            return View(submission);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "Id,TimeSubmitted,FeedBack,StudentName,DocumentURL,DocumentName")] Submission submission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(submission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(submission);
        }

        // GET: Submissions/Delete/5
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Submission submission = db.Submissions.Find(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            return View(submission);
        }

        // POST: Submissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult DeleteConfirmed(string id)
        {
            Submission submission = db.Submissions.Find(id);
            db.Submissions.Remove(submission);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [Filter.Filter(Roles = "Student")]
        public ActionResult UploadSubmission(string id)
        {
            Submission submission = new Submission();
            submission.Id = generateID();
            submission.Assignment = db.Assignments.Find(id);
            db.Submissions.Add(submission);
            db.SaveChanges();
            return RedirectToAction("UploadDocument", "Upload", new { id = submission.Id });
        }

        // POST: Submissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        public ActionResult DisplayDocument(string id)
        {
            var submission = db.Submissions.Find(id);

            return RedirectToAction("ShowDocumentTeacher", "Upload", new { name = submission.DocumentName, sname = submission.StudentName, id = submission.DocumentMongoDBID, path = submission.DocumentURL});
        }


        private string generateID()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            GuidString = GuidString.Replace(@"\", "");
            GuidString = GuidString.Replace(@"/", "");
            return GuidString;
        }
    }
}
