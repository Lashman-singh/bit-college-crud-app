﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BITCollege_LS.Data;
using BITCollege_LS.Models;

namespace BITCollege_LS.Controllers
{
    public class RegularStatesController : Controller
    {
        private BITCollege_LSContext db = new BITCollege_LSContext();

        // GET: RegularStates
        public ActionResult Index()
        {
            /*return View(db.RegularStates.ToList());*/
            return View(RegularState.GetInstance());
        }

        // GET: RegularStates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegularState regularState = db.RegularStates.Find(id);
            if (regularState == null)
            {
                return HttpNotFound();
            }
            return View(regularState);
        }

        // GET: RegularStates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RegularStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GradePointStateId,LowerLimit,UpperLimit,TuitionRateFactor")] RegularState regularState)
        {
            if (ModelState.IsValid)
            {
                db.GradePointStates.Add(regularState);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(regularState);
        }

        // GET: RegularStates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegularState regularState = db.RegularStates.Find(id);
            if (regularState == null)
            {
                return HttpNotFound();
            }
            return View(regularState);
        }

        // POST: RegularStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GradePointStateId,LowerLimit,UpperLimit,TuitionRateFactor")] RegularState regularState)
        {
            if (ModelState.IsValid)
            {
                db.Entry(regularState).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(regularState);
        }

        // GET: RegularStates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegularState regularState = db.RegularStates.Find(id);
            if (regularState == null)
            {
                return HttpNotFound();
            }
            return View(regularState);
        }

        // POST: RegularStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RegularState regularState = db.RegularStates.Find(id);
            db.GradePointStates.Remove(regularState);
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
    }
}
