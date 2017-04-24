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
using TextPlagiarismWebApp.Models.ViewModels;

namespace TextPlagiarismWebApp.Controllers
{
    public class AssignmentsController : Controller
    {
        private CourseDB db = new CourseDB();

        // GET: Assignments
        [HttpGet]
        [Route("AssignmentRoute")]
        public ActionResult Index(string id)
        {
            var assignments = db.Assignments
                .Where(a => a.Course.Id == id)
                .ToList();
            var course = db.Courses.Find(id);

            AssignmentUserViewModel assignmentUserViewModel = new AssignmentUserViewModel();
            assignmentUserViewModel.Assignments = assignments;
            assignmentUserViewModel.Course = course;

            return View(assignmentUserViewModel);
        }

        // GET: Assignments/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // GET: Assignments/Create
        public ActionResult Create(string id)
        {
            ViewBag.course_id = id;
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,TimeDue, Weight")] Assignment assignment, string id)
        {
            if (ModelState.IsValid)
            {
                assignment.Id = generateID();
                assignment.TimeCreated = DateTime.Now.ToString("dd/MM/yyyy");
                Course course = db.Courses.Find(id);
                assignment.Course = course;
                db.Assignments.Add(assignment);
                course.Assignments.Add(assignment);

                db.SaveChanges();
                return RedirectToAction("Index", new { id = assignment.Course.Id });
            }

            return View(assignment);
        }

        private string generateID()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }

        // GET: Assignments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,TimeCreated")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Assignment assignment = db.Assignments.Find(id);
            db.Assignments.Remove(assignment);
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
        public ActionResult ViewSubmission(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index", "Submissions", new { id = id });
        }
    }
}
