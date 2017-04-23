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
using PagedList;
using TextPlagiarismWebApp.Filter;

namespace TextPlagiarismWebApp.Controllers

{
    
    public class CoursesController : Controller
    {
        private CourseDB db = new CourseDB();

        // GET: Courses
        public ActionResult Index(string searchTerm = null, int page =1)
        {
            var model =
                (from c in db.Courses
                 where searchTerm == null ||
                 c.Id.StartsWith(searchTerm) ||
                 c.Name.StartsWith(searchTerm)

                 select new
                 {
                     courseId = c.Id,
                     courseName = c.Name,
                     description = c.Description,
                     Hours = c.Hours
                 }).AsEnumerable().Select(c => new Course
                 {
                     Id = c.courseId,
                     Name = c.courseName,
                     Description = c.description,
                     Hours = c.Hours
                 }).ToPagedList(page, 10);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Coursec", model);
            }

            return View(model);
        }

        // GET: Courses/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        //[Filter.Filter(Roles = "admin, teacher")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Filter.Filter(Roles = "admin, teacher")]
        [ValidateAntiForgeryToken]
         public ActionResult Create([Bind(Include = "Id,Name,Description,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        //[Filter.Filter(Roles = "admin, owner")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Filter.Filter(Roles = "admin, owner")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        //[Filter.Filter(Roles = "admin, owner")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Filter.Filter(Roles = "admin, owner")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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

        public ActionResult ManageCourse (string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
    }
}
