using System.Data;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class SchoolDocumentController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /SchoolDocument/

        public ActionResult Index()
        {
            if (Session["Phet10School"] == null)
            {
                //FormsAuthentication.SignOut();
                return RedirectToAction("../");
            }
            TB_APPLICATION_SCHOOL school = (TB_APPLICATION_SCHOOL)Session["Phet10School"];

            return View(db.TB_SCHOOL_DOCUMENT.Where(s=>s.SCHOOL_ID==school.SCHOOL_ID).ToList());
        }

        public ActionResult DocumentDetail(int id = 0)
        {

            return View(db.TB_SCHOOL_DOCUMENT.Where(s=>s.SCHOOL_ID==id).ToList());
        }

        
        //public ActionResult Index(int id =0)
        //{

        //    return View(db.TB_SCHOOL_DOCUMENT.Where(sd=>sd.SCHOOL_ID==id).ToList());
        //}
        //
        // GET: /SchoolDocument/Details/5

        //public ActionResult Details(decimal id = 0)
        //{
        //    TB_SCHOOL_DOCUMENT tb_school_document = db.TB_SCHOOL_DOCUMENT.Single(t => t.ID == id);
        //    if (tb_school_document == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tb_school_document);
        //}

        //
        // GET: /SchoolDocument/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SchoolDocument/Create

        [HttpPost]
        public ActionResult Create(TB_SCHOOL_DOCUMENT tb_school_document)
        {
            if (ModelState.IsValid)
            {
                db.TB_SCHOOL_DOCUMENT.Add(tb_school_document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_school_document);
        }

        //
        // GET: /SchoolDocument/Edit/5

        public ActionResult Edit(decimal id = 0)
        {
            TB_SCHOOL_DOCUMENT tb_school_document = db.TB_SCHOOL_DOCUMENT.Single(t => t.ID == id);
            if (tb_school_document == null)
            {
                return HttpNotFound();
            }
            return View(tb_school_document);
        }

        //
        // POST: /SchoolDocument/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_SCHOOL_DOCUMENT tb_school_document)
        {
            if (ModelState.IsValid)
            {
                var _update = db.TB_SCHOOL_DOCUMENT.FirstOrDefault(f => f.ID == tb_school_document.ID);
                if (_update != null)
                {
                    _update.DOCUMENT_PATH = tb_school_document.DOCUMENT_PATH;
                    _update.SCHOOL_ID = tb_school_document.ID;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_school_document);
        }

        //
        // GET: /SchoolDocument/Delete/5

        public ActionResult Delete(decimal id = 0)
        {
            TB_SCHOOL_DOCUMENT tb_school_document = db.TB_SCHOOL_DOCUMENT.Single(t => t.ID == id);
            if (tb_school_document == null)
            {
                return HttpNotFound();
            }
            return View(tb_school_document);
        }

        //
        // POST: /SchoolDocument/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            TB_SCHOOL_DOCUMENT tb_school_document = db.TB_SCHOOL_DOCUMENT.Single(t => t.ID == id);
            db.TB_SCHOOL_DOCUMENT.Remove(tb_school_document);
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