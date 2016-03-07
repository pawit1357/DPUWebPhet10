using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class StudentController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /Student/

        public ActionResult Index(int id = 0)
        {

            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_SCHOOL_ID == id select a;
            if (tb_application_student != null)
            {
                HttpNotFound();
            }

            //TB_APPLICATION_SCHOOL school = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == id);
            Session["Phet10ManagementSchoolId"] = id;
            List<TB_APPLICATION_STUDENT> students = tb_application_student.ToList();
            foreach (TB_APPLICATION_STUDENT student in students)
            {
                student.STD_NAME = student.TB_M_TITLE.TITLE_NAME_TH + "" + student.STD_NAME + "  " + student.STD_SURNAME;

            }
            return View(tb_application_student.ToList());
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {

            ViewBag.STD_LEVEL_ID = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", 1);
            ViewBag.STD_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", 0);
            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        public ActionResult Create(TB_APPLICATION_STUDENT tb_application_student)
        {
            if (ModelState.IsValid)
            {
                int schoolId = (int)Session["Phet10ManagementSchoolId"];
                tb_application_student.STD_SCHOOL_ID = schoolId;
                db.TB_APPLICATION_STUDENT.Add(tb_application_student);
                db.SaveChanges();
                return RedirectToAction("Index/" + schoolId);
            }

            ViewBag.STD_LEVEL_ID = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_application_student.STD_LEVEL_ID);
            ViewBag.STD_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_application_student.STD_TITLE_ID);

            return View(tb_application_student);
        }

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TB_APPLICATION_STUDENT tb_application_student = db.TB_APPLICATION_STUDENT.Single(t => t.STD_ID == id);
            if (tb_application_student == null)
            {
                return HttpNotFound();
            }
            ViewBag.STD_LEVEL_ID = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_application_student.STD_LEVEL_ID);
            ViewBag.STD_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_application_student.STD_TITLE_ID);
            ViewBag.STD_APPROVED_STATUS = new SelectList(db.TB_M_STATUS, "STATUS_ID", "STATUS_NAME_TH", tb_application_student.STD_APPROVED_STATUS);
            return View(tb_application_student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_APPLICATION_STUDENT tb_application_student)
        {
            if (ModelState.IsValid)
            {
                int schoolId = (int)Session["Phet10ManagementSchoolId"];

                var _update = db.TB_APPLICATION_STUDENT.FirstOrDefault(f => f.STD_ID == tb_application_student.STD_ID);
                if (_update != null)
                {
                    _update.STD_SCHOOL_ID = schoolId;
                    _update.STD_TITLE_ID = tb_application_student.STD_TITLE_ID;
                    _update.STD_NAME = tb_application_student.STD_NAME;
                    _update.STD_SURNAME = tb_application_student.STD_SURNAME;
                    _update.STD_LEVEL_ID = tb_application_student.STD_LEVEL_ID;
                    _update.STD_SCHOOL_ID = tb_application_student.STD_SCHOOL_ID;
                    _update.STD_PICTURE_PATH = tb_application_student.STD_PICTURE_PATH;
                    _update.STD_PHONE = tb_application_student.STD_PHONE;
                    _update.STD_EMAIL = tb_application_student.STD_EMAIL;
                    _update.STD_ID_CARD = tb_application_student.STD_ID_CARD;
                    _update.STD_PASSPORT_ID = tb_application_student.STD_PASSPORT_ID;
                    _update.STD_GRADE = tb_application_student.STD_GRADE;
                    _update.STD_PHONE_PROVIDER = tb_application_student.STD_PHONE_PROVIDER;
                    _update.STD_APPROVED_STATUS = tb_application_student.STD_APPROVED_STATUS;
                    _update.STD_BIRTH_DAY = tb_application_student.STD_BIRTH_DAY;
                    _update.STD_IS_CONCERN = tb_application_student.STD_IS_CONCERN;
                }

                db.SaveChanges();
                return RedirectToAction("Index/" + schoolId);
            }
            ViewBag.STD_LEVEL_ID = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_application_student.STD_LEVEL_ID);
            ViewBag.STD_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_application_student.STD_TITLE_ID);
            return View(tb_application_student);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /* CUSTOM EVENT*/
        public JsonResult DeleteStudent(int ID)
        {

            TB_APPLICATION_STUDENT tb_application_student = db.TB_APPLICATION_STUDENT.Single(t => t.STD_ID == ID);
            if (tb_application_student != null)
            {
                db.TB_APPLICATION_STUDENT.Remove(tb_application_student);
                db.SaveChanges();
            }

            // delete the record from ID and return true else false
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}