using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using DPUWebPhet10.Models.Common;
using System.Web.Security;

namespace DPUWebPhet10.Controllers
{
    public class SchoolController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();
        IRepository repository = new DPUPhet10Repository();

        private IFileStore _fileStore = new DiskFileStore();
        //
        // GET: /School/
        public ActionResult Index()
        {
            if (Session["Phet10School"] == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("../");
            }
            TB_APPLICATION_SCHOOL school = (TB_APPLICATION_SCHOOL)Session["Phet10School"];

            var tb_application_school = from a in db.TB_APPLICATION_SCHOOL where a.SCHOOL_ID == school.SCHOOL_ID select a;

            //a.TB_M_STATUS.STATUS_NAME
            ViewBag.PageContent = "ตรวจสอบผลการสมัคร";

            return View(tb_application_school.ToList());
        }

        //
        // GET: /School/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session["Phet10School"] != null)
            {
                TB_APPLICATION_SCHOOL school = (TB_APPLICATION_SCHOOL)Session["Phet10School"];
                id = Convert.ToInt32(school.SCHOOL_ID);
            }
            TB_APPLICATION_SCHOOL tb_application_school = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == id);
            if (tb_application_school == null)
            {
                return HttpNotFound();
            }
            ViewBag.SCHOOL_ADDR_PROVINCE = new SelectList(db.TB_M_PROVINCE, "PROVINCE_ID", "PROVINCE_NAME", tb_application_school.SCHOOL_ADDR_PROVINCE);
            ViewBag.SCHOOL_ADDR_TOMBON = new SelectList(db.TB_M_DISTRICT, "DISTRICT_ID", "DISTRICT_NAME", tb_application_school.SCHOOL_ADDR_TOMBON);
            ViewBag.SCHOOL_ADDR_AMPHUR = new SelectList(db.TB_M_AMPHUR, "AMPHUR_ID", "AMPHUR_NAME", tb_application_school.SCHOOL_ADDR_AMPHUR);

            /*
             * SCHOOL TYPE
             */
            List<RadioButtonModel> list = new List<RadioButtonModel>();
            list.Add(new RadioButtonModel() { ID = 1, Name = Resources.Application.Application.SCHOOL_TYPE_01 });//สพฐ
            list.Add(new RadioButtonModel() { ID = 2, Name = Resources.Application.Application.SCHOOL_TYPE_02 });//เอกชน
            list.Add(new RadioButtonModel() { ID = 3, Name = Resources.Application.Application.SCHOOL_TYPE_03 });//กทม
            list.Add(new RadioButtonModel() { ID = 4, Name = Resources.Application.Application.SCHOOL_TYPE_04 });//อุดมศึกษา
            list.Add(new RadioButtonModel() { ID = 5, Name = Resources.Application.Application.SCHOOL_TYPE_OTHER });//อื่น ๆ 

            SelectList schoolTypes = new SelectList(list, "ID", "Name", tb_application_school.SCHOOL_TYPE);

            ViewBag.SCHOOL_TYPE = schoolTypes;
            ViewBag.SCHOOL_PROVINCE = new SelectList(db.TB_M_PROVINCE, "PROVINCE_ID", "PROVINCE_NAME", tb_application_school.SCHOOL_PROVINCE);

            ViewBag.PrintComfirmDoc = false;
            return View(tb_application_school);
        }

        //
        // POST: /School/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_APPLICATION_SCHOOL tb_application_school)
        {
            if (ModelState.IsValid)
            {
                var _updateSchool = db.TB_APPLICATION_SCHOOL.FirstOrDefault(f => f.SCHOOL_ID == tb_application_school.SCHOOL_ID);
                if (_updateSchool != null)
                {

                    _updateSchool.SCHOOL_ADDR_PROVINCE = tb_application_school.SCHOOL_ADDR_PROVINCE;
                    _updateSchool.SCHOOL_ADDR_AMPHUR = tb_application_school.SCHOOL_ADDR_AMPHUR;
                    _updateSchool.SCHOOL_ADDR_TOMBON = tb_application_school.SCHOOL_ADDR_TOMBON;
                    _updateSchool.SCHOOL_ZONE_EDU = tb_application_school.SCHOOL_ZONE_EDU;
                    _updateSchool.SCHOOL_ZONE = tb_application_school.SCHOOL_ZONE;
                    _updateSchool.SCHOOL_TYPE_OTHER = tb_application_school.SCHOOL_TYPE_OTHER;
                    _updateSchool.SCHOOL_ADDR = tb_application_school.SCHOOL_ADDR;
                    _updateSchool.SCHOOL_ADDR_SOI = tb_application_school.SCHOOL_ADDR_SOI;
                    _updateSchool.SCHOOL_ADDR_ROAD = tb_application_school.SCHOOL_ADDR_ROAD;
                    _updateSchool.SCHOOL_ADDR_ZIPCODE = tb_application_school.SCHOOL_ADDR_ZIPCODE;
                    _updateSchool.SCHOOL_ADDR_PHONE = tb_application_school.SCHOOL_ADDR_PHONE;
                    _updateSchool.SCHOOL_ADDR_FAX = tb_application_school.SCHOOL_ADDR_FAX;
                    _updateSchool.SCHOOL_NAME = tb_application_school.SCHOOL_NAME;
                    _updateSchool.SCHOOL_PROVINCE = tb_application_school.SCHOOL_PROVINCE;
                    _updateSchool.SCHOOL_PASSWORD = tb_application_school.SCHOOL_PASSWORD;
                    _updateSchool.SCHOOL_DOC_PATH = tb_application_school.SCHOOL_DOC_PATH;
                    _updateSchool.SCHOOL_REGISTER_DATE = tb_application_school.SCHOOL_REGISTER_DATE;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_application_school);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}