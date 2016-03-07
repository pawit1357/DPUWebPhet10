using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class UserController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /User/

        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var tb_user = db.TB_USER.Include("TB_M_STATUS").Include("TB_ROLE");
            return View(tb_user.Where(u => u.STATUS == 6).ToList());// && u.ROLE_ID > 0).ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_USER tb_user = db.TB_USER.Single(t => t.ID == id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            ViewBag.STATUS = new SelectList(db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 2), "STATUS_ID", "STATUS_NAME_TH");
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME");
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(TB_USER tb_user)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                tb_user.LASTED_LOGIN = DateTime.Now;
                tb_user.CREATE_DATE = DateTime.Now;
                tb_user.CREATE_BY = 0;

                db.TB_USER.Add(tb_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.STATUS = new SelectList(db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 2), "STATUS_ID", "STATUS_NAME_TH", tb_user.STATUS);
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME", tb_user.ROLE_ID);
            return View(tb_user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_USER tb_user = db.TB_USER.Single(t => t.ID == id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            ViewBag.STATUS = new SelectList(db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 2), "STATUS_ID", "STATUS_NAME_TH", tb_user.STATUS);
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME", tb_user.ROLE_ID);
            return View(tb_user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_USER tb_user)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                var _update = db.TB_USER.FirstOrDefault(f => f.ID == tb_user.ID);
                if (_update != null)
                {
                    _update.ROLE_ID = tb_user.ROLE_ID;
                    _update.USERNAME = tb_user.USERNAME;
                    _update.PASSWORD = tb_user.PASSWORD;
                    _update.STATUS = tb_user.STATUS;
                    _update.CREATE_BY = tb_user.CREATE_BY;
                    _update.EMAIL = tb_user.EMAIL;
                    db.SaveChanges();
                }

                //db.TB_USER.Attach(tb_user);
                //db.ObjectStateManager.ChangeObjectState(tb_user, EntityState.Modified);
                //db.SaveChanges();


                return RedirectToAction("Index");
            }
            ViewBag.STATUS = new SelectList(db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 2), "STATUS_ID", "STATUS_NAME_TH", tb_user.STATUS);
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME", tb_user.ROLE_ID);
            return View(tb_user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_USER tb_user = db.TB_USER.Single(t => t.ID == id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_USER tb_user = db.TB_USER.Single(t => t.ID == id);
            db.TB_USER.Remove(tb_user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}