using Microsoft.AspNet.Identity;
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
using Microsoft.AspNet.Identity;
using TextPlagiarismWebApp.Models.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TextPlagiarismWebApp.Controllers
{
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Assignments
        [HttpGet]
        [Route("AssignmentRoute")]
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult Index(string id)
        {
            var assignments = db.Assignments
                .Where(a => a.Course.Id == id)
                .ToList();
            var course = db.Courses.Find(id);
            foreach(var ass in assignments)
            {
                var submissions = db.Submissions
                    .Where(a => a.Assignment.Id == ass.Id).ToList().Count();
            }

            AssignmentUserViewModel assignmentUserViewModel = new AssignmentUserViewModel();
            assignmentUserViewModel.Assignments = assignments;
            assignmentUserViewModel.Course = course;

            return View(assignmentUserViewModel);
        }

        // GET: Assignments/Details/5
        [Filter.Filter(Roles = "Student")]
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
        [Filter.Filter(Roles = "Teacher")]
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
        [Filter.Filter(Roles = "Teacher")]
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
            GuidString = GuidString.Replace(@"\", "");
            GuidString = GuidString.Replace(@"/", "");
            return GuidString;
        }

        // GET: Assignments/Edit/5
        [Filter.Filter(Roles = "Teacher")]
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
        [Filter.Filter(Roles = "Teacher")]
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
        [Filter.Filter(Roles = "Teacher")]
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
        [Filter.Filter(Roles = "Teacher")]
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
        [Filter.Filter(Roles = "Teacher")]
        public ActionResult ViewSubmission(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index", "Submissions", new { id = id });
        }

        [Filter.Filter(Roles = "Student")]
        public ActionResult StudentIndexToEnroll(string id)
        {
            //view of all courses
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id); //return the course
            return View(course);
        }
        [Filter.Filter(Roles = "Student")]
        public ActionResult StudentIndexEnrolled(string id)
        {

            Course course = db.Courses.Find(id); //return the course
            var studentsEnrolled = course.ApplicationUsers.ToList();

            if ( studentsEnrolled == null || !studentsEnrolled.Any(s => s.Email == User.Identity.Name))
            {
                return RedirectToAction("StudentIndexToEnroll", "Assignments", new { id = id });
            }

            var assignments = db.Assignments
                .Where(a => a.Course.Id == id)
                .ToList();
            foreach (var ass in assignments)
            {
                var submissions = db.Submissions
                    .Where(a => a.Assignment.Id == ass.Id).ToList().Count();
            }

            AssignmentUserViewModel assignmentUserViewModel = new AssignmentUserViewModel();
            assignmentUserViewModel.Assignments = assignments;
            assignmentUserViewModel.Course = course;

            return View(assignmentUserViewModel);
        }
        [Filter.Filter(Roles = "Student")]
        public ActionResult EnrollStudent(string id)
        {
            using (Models.ApplicationDbContext applicationDbContext = new Models.ApplicationDbContext())
            {
                Course course = applicationDbContext.Courses.Find(id); //return the course

                UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(applicationDbContext);

                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

                var user = userManager.FindById(User.Identity.GetUserId());

                course.ApplicationUsers.Add(user);

                applicationDbContext.SaveChanges();

                return RedirectToAction("StudentIndexEnrolled", "Assignments", new { id = id });
            }
        }
    }
}
