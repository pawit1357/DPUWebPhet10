using System.Data;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class RolePermissionController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /RolePermission/

        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var tb_role_permission = db.TB_ROLE_PERMISSION.Include("TB_ROLE");
            return View(tb_role_permission.ToList());
        }

        //
        // GET: /RolePermission/Details/5

        public ActionResult Details(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE_PERMISSION tb_role_permission = db.TB_ROLE_PERMISSION.Single(t => t.ID == id);
            if (tb_role_permission == null)
            {
                return HttpNotFound();
            }
            return View(tb_role_permission);
        }

        //
        // GET: /RolePermission/Create

        public ActionResult Create()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME");
            return View();
        }

        //
        // POST: /RolePermission/Create

        [HttpPost]
        public ActionResult Create(TB_ROLE_PERMISSION tb_role_permission)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                db.TB_ROLE_PERMISSION.Add(tb_role_permission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME", tb_role_permission.ROLE_ID);
            return View(tb_role_permission);
        }

        //
        // GET: /RolePermission/Edit/5

        public ActionResult Edit(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE_PERMISSION tb_role_permission = db.TB_ROLE_PERMISSION.Single(t => t.ID == id);
            if (tb_role_permission == null)
            {
                return HttpNotFound();
            }
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME", tb_role_permission.ROLE_ID);
            return View(tb_role_permission);
        }

        //
        // POST: /RolePermission/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_ROLE_PERMISSION tb_role_permission)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                var _update = db.TB_ROLE_PERMISSION.FirstOrDefault(f => f.ID == tb_role_permission.ID);
                if (_update != null)
                {
                    _update.ROLE_ID = tb_role_permission.ROLE_ID;
                    _update.PERMISSION_CODE = tb_role_permission.PERMISSION_CODE;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ROLE_ID = new SelectList(db.TB_ROLE, "ID", "NAME", tb_role_permission.ROLE_ID);
            return View(tb_role_permission);
        }

        //
        // GET: /RolePermission/Delete/5

        public ActionResult Delete(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE_PERMISSION tb_role_permission = db.TB_ROLE_PERMISSION.Single(t => t.ID == id);
            if (tb_role_permission == null)
            {
                return HttpNotFound();
            }
            return View(tb_role_permission);
        }

        //
        // POST: /RolePermission/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE_PERMISSION tb_role_permission = db.TB_ROLE_PERMISSION.Single(t => t.ID == id);
            db.TB_ROLE_PERMISSION.Remove(tb_role_permission);
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