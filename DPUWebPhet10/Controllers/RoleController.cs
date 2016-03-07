using System.Data;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class RoleController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /Role/

        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            return View(db.TB_ROLE.ToList());
        }

        //
        // GET: /Role/Details/5

        public ActionResult Details(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE tb_role = db.TB_ROLE.Single(t => t.ID == id);
            if (tb_role == null)
            {
                return HttpNotFound();
            }
            return View(tb_role);
        }

        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        public ActionResult Create(TB_ROLE tb_role)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                db.TB_ROLE.Add(tb_role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_role);
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE tb_role = db.TB_ROLE.Single(t => t.ID == id);
            if (tb_role == null)
            {
                return HttpNotFound();
            }
            return View(tb_role);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_ROLE tb_role)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                var _update = db.TB_ROLE.FirstOrDefault(f => f.ID == tb_role.ID);
                if (_update != null)
                {
                    _update.NAME = tb_role.NAME;
                    _update.STATUS = tb_role.STATUS;
                    _update.DESCRIPTON = tb_role.DESCRIPTON;

                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_role);
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE tb_role = db.TB_ROLE.Single(t => t.ID == id);
            if (tb_role == null)
            {
                return HttpNotFound();
            }
            return View(tb_role);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_ROLE tb_role = db.TB_ROLE.Single(t => t.ID == id);
            db.TB_ROLE.Remove(tb_role);
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