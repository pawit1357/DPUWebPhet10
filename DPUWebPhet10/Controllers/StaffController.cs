using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class StaffController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /Staff/

        public ActionResult Index(int id = 0)
        {

            Session["Phet10ManagementSchoolId"] = id;
            var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == id select a;
            if (tb_application_staff == null)
            {
                return HttpNotFound();
            }
            List<TB_APPLICATION_STAFF> staffs = tb_application_staff.ToList();

            if (staffs != null & staffs.Count > 0)
            {
                foreach (TB_APPLICATION_STAFF staff in staffs)
                {
                    staff.STAFF_NAME = staff.TB_M_TITLE.TITLE_NAME_TH + "" + staff.STAFF_NAME + "  " + staff.STAFF_SURNAME;
                }
            }

            return View(tb_application_staff.ToList());
        }

        //
        // GET: /Staff/Create

        public ActionResult Create()
        {
            ViewBag.STAFF_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH");
            ViewBag.STAFF_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH");
            return View();
        }

        //
        // POST: /Staff/Create

        [HttpPost]
        public ActionResult Create(TB_APPLICATION_STAFF tb_application_staff)
        {
            if (ModelState.IsValid)
            {
                int schoolId = (int)Session["Phet10ManagementSchoolId"];
                tb_application_staff.STAFF_SCHOOL_ID = schoolId;
                db.TB_APPLICATION_STAFF.Add(tb_application_staff);
                db.SaveChanges();
                return RedirectToAction("Index/" + schoolId);
            }

            ViewBag.STAFF_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_application_staff.STAFF_TITLE_ID);
            ViewBag.STAFF_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_application_staff.STAFF_FOR_LEVEL);
            return View(tb_application_staff);
        }

        //
        // GET: /Staff/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TB_APPLICATION_STAFF tb_application_staff = db.TB_APPLICATION_STAFF.Single(t => t.STAFF_ID == id);
            if (tb_application_staff == null)
            {
                return HttpNotFound();
            }


            ViewBag.STAFF_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_application_staff.STAFF_TITLE_ID);
            ViewBag.STAFF_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_application_staff.STAFF_FOR_LEVEL);
            return View(tb_application_staff);
        }

        //
        // POST: /Staff/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_APPLICATION_STAFF tb_application_staff)
        {
            if (ModelState.IsValid)
            {
                int schoolId = (int)Session["Phet10ManagementSchoolId"];

                var _update = db.TB_APPLICATION_STAFF.FirstOrDefault(f => f.STAFF_ID == tb_application_staff.STAFF_ID);
                if (_update != null)
                {
                    _update.STAFF_SCHOOL_ID = schoolId;
                    _update.STAFF_POSITION = tb_application_staff.STAFF_POSITION;
                    _update.STAFF_NAME = tb_application_staff.STAFF_NAME;
                    _update.STAFF_SURNAME = tb_application_staff.STAFF_SURNAME;
                    _update.STAFF_PHONE = tb_application_staff.STAFF_PHONE;
                    _update.STAFF_TITLE_ID = tb_application_staff.STAFF_TITLE_ID;
                    _update.STAFF_FOR_LEVEL = tb_application_staff.STAFF_FOR_LEVEL;
                    _update.STAFF_NATION = tb_application_staff.STAFF_NATION;
                }

                db.SaveChanges();
                return RedirectToAction("Index/" + schoolId);
            }
            ViewBag.STAFF_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_application_staff.STAFF_TITLE_ID);
            ViewBag.STAFF_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_application_staff.STAFF_FOR_LEVEL);
            return View(tb_application_staff);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /* CUSTOM EVENT*/
        public JsonResult DeleteStaff(int ID)
        {
            TB_APPLICATION_STAFF tb_application_staff = db.TB_APPLICATION_STAFF.Single(t => t.STAFF_ID == ID);
            db.TB_APPLICATION_STAFF.Remove(tb_application_staff);
            db.SaveChanges();
            // delete the record from ID and return true else false
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}