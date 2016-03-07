using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using System.Globalization;
using DPUWebPhet10.Models.Common;
using System.Data.Objects;
using DPUWebPhet10.Utils;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using PagedList;
using System.Transactions;
namespace DPUWebPhet10.Controllers
{
    public class ManagementController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            return View();
        }


        //[HttpPost]
        public ActionResult SchoolList(ManagementModel model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<TB_APPLICATION_SCHOOL> tmpSchoolList = new List<TB_APPLICATION_SCHOOL>();
            List<TB_APPLICATION_SCHOOL> schoolList = new List<TB_APPLICATION_SCHOOL>();


            if (model.startDate == null && model.endDate == null)
            {
                String _startDate = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                String _endDate = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));

                DateTime dt = DateTime.Now;
                var tb_application_school = from a in db.TB_APPLICATION_SCHOOL
                                            where EntityFunctions.TruncateTime(a.SCHOOL_REGISTER_DATE) == dt.Date
                                            select a;
                model.startDate = _startDate;
                model.endDate = _endDate;
                schoolList = tb_application_school.ToList();
            }
            else
            {
                int startDate = Convert.ToInt32(CommonUtils.toDate(model.startDate).ToString("yyyyMMdd", new CultureInfo("th-TH")));
                int endDate = Convert.ToInt32(CommonUtils.toDate(model.endDate).ToString("yyyyMMdd", new CultureInfo("th-TH")));

                if (!String.IsNullOrWhiteSpace(model.schoolName))
                {
                    var school = from a in db.TB_APPLICATION_SCHOOL where a.SCHOOL_NAME.Contains(model.schoolName) select a;
                    if (!String.IsNullOrWhiteSpace(model.approveStatus))
                    {
                        int approvedStatus = Convert.ToInt16(model.approveStatus);
                        tmpSchoolList = school.Where(t => t.SCHOOL_APPROVED_STATUS == approvedStatus).ToList();
                    }
                    else
                    {
                        tmpSchoolList = school.ToList();
                    }
                }
                else
                {
                    var school = from a in db.TB_APPLICATION_SCHOOL select a;
                    if (!String.IsNullOrWhiteSpace(model.approveStatus))
                    {
                        int approvedStatus = Convert.ToInt16(model.approveStatus);
                        tmpSchoolList = school.Where(t => t.SCHOOL_APPROVED_STATUS == approvedStatus).ToList();
                    }
                    else
                    {
                        tmpSchoolList = school.ToList();
                    }
                }

                StringBuilder x = new StringBuilder();
                foreach (TB_APPLICATION_SCHOOL school in tmpSchoolList)
                {
                    int curDate = Convert.ToInt32(school.SCHOOL_REGISTER_DATE.Value.ToString("yyyyMMdd", new CultureInfo("en-US")));
                    if (curDate >= startDate && curDate <= endDate)
                    {
                        x.Append("|" + curDate + "," + startDate + "," + endDate + "|");
                        schoolList.Add(school);
                    }
                }
            }
            List<TB_M_STATUS> tbMStatus = db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 1).ToList();
            ViewBag.approveStatus = new SelectList(tbMStatus, "STATUS_ID", "STATUS_NAME_TH", model.approveStatus);
            if (schoolList != null)
            {
                var pageIndex = model.Page ?? 1;
                model.schools = schoolList.ToPagedList(pageIndex, 15);
            }
            return View(model);
        }

        public ActionResult EditSchool(int id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            TB_APPLICATION_SCHOOL tb_application_school = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == id);

            /*
             * TITLE PROVINCE
             */
            IList<TB_M_PROVINCE> mProvinceLists = db.TB_M_PROVINCE.ToList<TB_M_PROVINCE>();
            IEnumerable<SelectListItem> provinceLists =
            from s in mProvinceLists
            select new SelectListItem
            {
                Text = s.PROVINCE_NAME,
                Value = s.PROVINCE_ID.ToString()
            };

            /*
             * TITLE AMPHUR
             */
            IList<TB_M_AMPHUR> mAmphur = db.TB_M_AMPHUR.ToList<TB_M_AMPHUR>();
            IEnumerable<SelectListItem> amPhurList =
            from s in mAmphur
            select new SelectListItem
            {
                Text = s.AMPHUR_NAME,
                Value = s.AMPHUR_ID.ToString()
            };

            /*
             * TITLE TUMBON
             */
            IList<TB_M_DISTRICT> mTumbon = db.TB_M_DISTRICT.ToList<TB_M_DISTRICT>();
            IEnumerable<SelectListItem> tumbonLists =
            from s in mTumbon
            select new SelectListItem
            {
                Text = s.DISTRICT_NAME,
                Value = s.DISTRICT_ID.ToString()
            };

            /*
             * SCHOOL TYPE
             */
            List<RadioButtonModel> list = new List<RadioButtonModel>();

            list.Add(new RadioButtonModel() { ID = 1, Name = Resources.Application.Application.SCHOOL_TYPE_01 });//สพฐ
            list.Add(new RadioButtonModel() { ID = 2, Name = Resources.Application.Application.SCHOOL_TYPE_02 });//เอกชน
            list.Add(new RadioButtonModel() { ID = 3, Name = Resources.Application.Application.SCHOOL_TYPE_03 });//กทม
            list.Add(new RadioButtonModel() { ID = 4, Name = Resources.Application.Application.SCHOOL_TYPE_04 });//กทม
            list.Add(new RadioButtonModel() { ID = 5, Name = Resources.Application.Application.SCHOOL_TYPE_OTHER });//กทม
            SelectList schoolTypes = new SelectList(list, "ID", "Name");

            /*
             * STAFF
             */
            var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == id select a;
            /*
             * STUDENT
             */
            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_SCHOOL_ID == id select a;

            var model = new ApplicationModel()
            {
                idSelectedSchoolType = tb_application_school.SCHOOL_TYPE + "",
                SCHOOL_ADDR_PROVINCE = tb_application_school.SCHOOL_ADDR_PROVINCE + "",
                SCHOOL_ADDR_AMPHUR = tb_application_school.SCHOOL_ADDR_AMPHUR + "",
                SCHOOL_ADDR_TOMBON = tb_application_school.SCHOOL_ADDR_TOMBON + "",
                provinceLists = provinceLists,
                tumbonLists = tumbonLists,
                amphurLists = amPhurList,
                rSchoolTypes = schoolTypes,
                school = tb_application_school,
                Staffs = tb_application_staff.ToList(),
                Students = tb_application_student.ToList()
            };

            return View(model);
        }

        public ActionResult DetailSchool(int id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            TB_APPLICATION_SCHOOL tb_application_school = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == id);

            /*
             * TITLE PROVINCE
             */
            IList<TB_M_PROVINCE> mProvinceLists = db.TB_M_PROVINCE.ToList<TB_M_PROVINCE>();
            IEnumerable<SelectListItem> provinceLists =
            from s in mProvinceLists
            select new SelectListItem
            {
                Text = s.PROVINCE_NAME,
                Value = s.PROVINCE_ID.ToString()
            };

            /*
             * TITLE AMPHUR
             */
            IList<TB_M_AMPHUR> mAmphur = db.TB_M_AMPHUR.ToList<TB_M_AMPHUR>();
            IEnumerable<SelectListItem> amPhurList =
            from s in mAmphur
            select new SelectListItem
            {
                Text = s.AMPHUR_NAME,
                Value = s.AMPHUR_ID.ToString()
            };

            /*
             * TITLE TUMBON
             */
            IList<TB_M_DISTRICT> mTumbon = db.TB_M_DISTRICT.ToList<TB_M_DISTRICT>();
            IEnumerable<SelectListItem> tumbonLists =
            from s in mTumbon
            select new SelectListItem
            {
                Text = s.DISTRICT_NAME,
                Value = s.DISTRICT_ID.ToString()
            };

            /*
             * SCHOOL TYPE
             */
            List<RadioButtonModel> list = new List<RadioButtonModel>();

            list.Add(new RadioButtonModel() { ID = 1, Name = Resources.Application.Application.SCHOOL_TYPE_01 });//สพฐ
            list.Add(new RadioButtonModel() { ID = 2, Name = Resources.Application.Application.SCHOOL_TYPE_02 });//เอกชน
            list.Add(new RadioButtonModel() { ID = 3, Name = Resources.Application.Application.SCHOOL_TYPE_03 });//กทม
            list.Add(new RadioButtonModel() { ID = 4, Name = Resources.Application.Application.SCHOOL_TYPE_04 });//กทม
            list.Add(new RadioButtonModel() { ID = 5, Name = Resources.Application.Application.SCHOOL_TYPE_OTHER });//กทม
            SelectList schoolTypes = new SelectList(list, "ID", "Name");

            /*
             * STAFF
             */
            var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == id select a;
            /*
             * STUDENT
             */
            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_SCHOOL_ID == id select a;

            var model = new ApplicationModel()
            {
                idSelectedSchoolType = tb_application_school.SCHOOL_TYPE + "",
                SCHOOL_ADDR_PROVINCE = tb_application_school.SCHOOL_ADDR_PROVINCE + "",
                SCHOOL_ADDR_AMPHUR = tb_application_school.SCHOOL_ADDR_AMPHUR + "",
                SCHOOL_ADDR_TOMBON = tb_application_school.SCHOOL_ADDR_TOMBON + "",
                provinceLists = provinceLists,
                tumbonLists = tumbonLists,
                amphurLists = amPhurList,
                rSchoolTypes = schoolTypes,
                school = tb_application_school,
                Staffs = tb_application_staff.ToList(),
                Students = tb_application_student.ToList()
            };
            ViewBag.SCHOOL_APPROVED_STATUS = new SelectList(db.TB_M_STATUS, "STATUS_ID", "STATUS_NAME_TH", tb_application_school.SCHOOL_APPROVED_STATUS);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSchool([Bind(Exclude = "school_SCHOOL_EMAIL,school_SCHOOL_NAME")]ApplicationModel model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (!ModelState.IsValid)
            {
                /*
                 * TITLE PROVINCE
                 */
                IList<TB_M_PROVINCE> mProvinceLists = db.TB_M_PROVINCE.ToList<TB_M_PROVINCE>();
                IEnumerable<SelectListItem> provinceLists =
                from s in mProvinceLists
                select new SelectListItem
                {
                    Text = s.PROVINCE_NAME,
                    Value = s.PROVINCE_ID.ToString()
                };
                /*
                 * TITLE AMPHUR
                 */
                IList<TB_M_AMPHUR> mAmphur = db.TB_M_AMPHUR.ToList<TB_M_AMPHUR>();
                IEnumerable<SelectListItem> amPhurList =
                from s in mAmphur
                select new SelectListItem
                {
                    Text = s.AMPHUR_NAME,
                    Value = s.AMPHUR_ID.ToString()
                };

                /*
                 * TITLE TUMBON
                 */
                IList<TB_M_DISTRICT> mTumbon = db.TB_M_DISTRICT.ToList<TB_M_DISTRICT>();
                IEnumerable<SelectListItem> tumbonLists =
                from s in mTumbon
                select new SelectListItem
                {
                    Text = s.DISTRICT_NAME,
                    Value = s.DISTRICT_ID.ToString()
                };


                /*
                 * SCHOOL TYPE
                 */
                List<RadioButtonModel> list = new List<RadioButtonModel>();

                list.Add(new RadioButtonModel() { ID = 1, Name = Resources.Application.Application.SCHOOL_TYPE_01 });//สพฐ
                list.Add(new RadioButtonModel() { ID = 2, Name = Resources.Application.Application.SCHOOL_TYPE_02 });//เอกชน
                list.Add(new RadioButtonModel() { ID = 3, Name = Resources.Application.Application.SCHOOL_TYPE_03 });//กทม
                list.Add(new RadioButtonModel() { ID = 4, Name = Resources.Application.Application.SCHOOL_TYPE_04 });//อุดมศึกษา
                list.Add(new RadioButtonModel() { ID = 5, Name = Resources.Application.Application.SCHOOL_TYPE_OTHER });//อื่น ๆ 

                SelectList schoolTypes = new SelectList(list, "ID", "Name");
                model.provinceLists = provinceLists;
                model.amphurLists = amPhurList;
                model.tumbonLists = tumbonLists;
                model.rSchoolTypes = schoolTypes;
                return View(model);
            }


            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {

                        /*
                         SCHOOL MODEL
                         */
                        //VALIDATE NULL VALUE
                        model.school.SCHOOL_ADDR_PROVINCE = (model.SCHOOL_ADDR_PROVINCE == null) ? -1 : Convert.ToInt32(model.SCHOOL_ADDR_PROVINCE);
                        model.school.SCHOOL_ADDR_AMPHUR = (model.SCHOOL_ADDR_AMPHUR == null) ? -1 : Convert.ToInt32(model.SCHOOL_ADDR_AMPHUR);
                        model.school.SCHOOL_ADDR_TOMBON = (model.SCHOOL_ADDR_TOMBON == null) ? -1 : Convert.ToInt32(model.SCHOOL_ADDR_TOMBON);
                        model.school.SCHOOL_ZONE_EDU = (model.school.SCHOOL_ZONE_EDU == null) ? "" : model.school.SCHOOL_ZONE_EDU;
                        model.school.SCHOOL_ZONE = (model.school.SCHOOL_ZONE == null) ? "" : model.school.SCHOOL_ZONE;
                        model.school.SCHOOL_TYPE_OTHER = (model.school.SCHOOL_TYPE_OTHER == null) ? "" : model.school.SCHOOL_TYPE_OTHER;
                        model.school.SCHOOL_ADDR = (model.school.SCHOOL_ADDR == null) ? "" : model.school.SCHOOL_ADDR;
                        model.school.SCHOOL_ADDR_SOI = (model.school.SCHOOL_ADDR_SOI == null) ? "" : model.school.SCHOOL_ADDR_SOI;
                        model.school.SCHOOL_ADDR_ROAD = (model.school.SCHOOL_ADDR_ROAD == null) ? "" : model.school.SCHOOL_ADDR_ROAD;
                        model.school.SCHOOL_ADDR_ZIPCODE = (model.school.SCHOOL_ADDR_ZIPCODE == null) ? "" : model.school.SCHOOL_ADDR_ZIPCODE;
                        model.school.SCHOOL_ADDR_PHONE = (model.school.SCHOOL_ADDR_PHONE == null) ? "" : model.school.SCHOOL_ADDR_PHONE;
                        model.school.SCHOOL_ADDR_FAX = (model.school.SCHOOL_ADDR_FAX == null) ? "" : model.school.SCHOOL_ADDR_FAX;
                        model.school.SCHOOL_NAME = (model.school.SCHOOL_NAME == null) ? "" : model.school.SCHOOL_NAME;
                        model.school.SCHOOL_PROVINCE = (model.school.SCHOOL_PROVINCE == null) ? -1 : model.school.SCHOOL_PROVINCE;
                        model.school.SCHOOL_PASSWORD = (model.school.SCHOOL_PASSWORD == null) ? "" : model.school.SCHOOL_PASSWORD;
                        model.school.SCHOOL_DOC_PATH = (model.school.SCHOOL_DOC_PATH == null) ? "" : model.school.SCHOOL_DOC_PATH;
                        model.school.SCHOOL_REGISTER_DATE = (model.school.SCHOOL_REGISTER_DATE == null) ? DateTime.Now : model.school.SCHOOL_REGISTER_DATE;
                        model.school.SCHOOL_TYPE = model.idSelectedSchoolType;

                        var _updateSchool = db.TB_APPLICATION_SCHOOL.FirstOrDefault(f => f.SCHOOL_ID == model.school.SCHOOL_ID);
                        if (_updateSchool != null)
                        {

                            _updateSchool.SCHOOL_ADDR_PROVINCE = model.school.SCHOOL_ADDR_PROVINCE;
                            _updateSchool.SCHOOL_ADDR_AMPHUR = model.school.SCHOOL_ADDR_AMPHUR;
                            _updateSchool.SCHOOL_ADDR_TOMBON = model.school.SCHOOL_ADDR_TOMBON;
                            _updateSchool.SCHOOL_ZONE_EDU = model.school.SCHOOL_ZONE_EDU;
                            _updateSchool.SCHOOL_ZONE = model.school.SCHOOL_ZONE;
                            _updateSchool.SCHOOL_TYPE_OTHER = model.school.SCHOOL_TYPE_OTHER;
                            _updateSchool.SCHOOL_ADDR = model.school.SCHOOL_ADDR;
                            _updateSchool.SCHOOL_ADDR_SOI = model.school.SCHOOL_ADDR_SOI;
                            _updateSchool.SCHOOL_ADDR_ROAD = model.school.SCHOOL_ADDR_ROAD;
                            _updateSchool.SCHOOL_ADDR_ZIPCODE = model.school.SCHOOL_ADDR_ZIPCODE;
                            _updateSchool.SCHOOL_ADDR_PHONE = model.school.SCHOOL_ADDR_PHONE;
                            _updateSchool.SCHOOL_ADDR_FAX = model.school.SCHOOL_ADDR_FAX;
                            _updateSchool.SCHOOL_NAME = model.school.SCHOOL_NAME;
                            _updateSchool.SCHOOL_PROVINCE = model.school.SCHOOL_PROVINCE;
                            _updateSchool.SCHOOL_PASSWORD = model.school.SCHOOL_PASSWORD;
                            _updateSchool.SCHOOL_DOC_PATH = model.school.SCHOOL_DOC_PATH;
                            _updateSchool.SCHOOL_REGISTER_DATE = model.school.SCHOOL_REGISTER_DATE;
                            _updateSchool.SCHOOL_TYPE = model.idSelectedSchoolType;

                        }

                        /*
                         STAFF
                         */
                        if (model.Staffs != null)
                        {

                            /*
                             * INSERT STAFF
                             */
                            foreach (TB_APPLICATION_STAFF staff in model.Staffs)
                            {
                                staff.STAFF_SCHOOL_ID = model.school.SCHOOL_ID;
                                staff.STAFF_FOR_LEVEL = 0;
                                if (staff.STAFF_ID > 0)
                                {
                                    var _update = db.TB_APPLICATION_STAFF.FirstOrDefault(f => f.STAFF_ID == staff.STAFF_ID);
                                    if (_update != null)
                                    {
                                        _update.STAFF_SCHOOL_ID = staff.STAFF_SCHOOL_ID;
                                        _update.STAFF_POSITION = staff.STAFF_POSITION;
                                        _update.STAFF_NAME = staff.STAFF_NAME;
                                        _update.STAFF_SURNAME = staff.STAFF_SURNAME;
                                        _update.STAFF_PHONE = staff.STAFF_PHONE;
                                        _update.STAFF_TITLE_ID = staff.STAFF_TITLE_ID;
                                        _update.STAFF_FOR_LEVEL = staff.STAFF_FOR_LEVEL;
                                        _update.STAFF_NATION = staff.STAFF_NATION;
                                    }
                                }
                                else
                                {

                                    //insert
                                    context.TB_APPLICATION_STAFF.Add(staff);
                                }

                            }
                        }
                        /*
                         STUDENT
                         */
                        if (model.Students != null)
                        {
                            /*
                             * INSERT STAFF
                             */
                            foreach (TB_APPLICATION_STUDENT student in model.Students)
                            {
                                student.STD_SCHOOL_ID = model.school.SCHOOL_ID;
                                student.STD_IS_CONCERN = "0";
                                #region "CONCERN STUDENT."
                                bool isValidLevel = false;
                                int currentYear = DateTime.Now.Year;
                                if (!String.IsNullOrEmpty(student.STD_BIRTH_DAY))
                                {
                                    currentYear = currentYear - Convert.ToInt16(student.STD_BIRTH_DAY.Split('-')[0]);
                                }

                                if (currentYear <= 9)
                                {
                                    if (student.STD_LEVEL_ID == 1)
                                    {
                                        isValidLevel = true; ;
                                    }
                                }
                                else if (currentYear >= 10 && currentYear <= 12)
                                {
                                    if (student.STD_LEVEL_ID == 2) isValidLevel = true;
                                }
                                else if (currentYear >= 13 && currentYear <= 15)
                                {
                                    if (student.STD_LEVEL_ID == 3) isValidLevel = true;
                                }
                                else if (currentYear >= 16 && currentYear <= 18)
                                {
                                    if (student.STD_LEVEL_ID == 4) isValidLevel = true;
                                }
                                else
                                {
                                    if (student.STD_LEVEL_ID == 5) isValidLevel = true;
                                }
                                if (isValidLevel == false)
                                {
                                    student.STD_IS_CONCERN = "1";
                                }
                                #endregion
                                if (student.STD_ID > 0)
                                {
                                    //update
                                    var _update = db.TB_APPLICATION_STUDENT.FirstOrDefault(f => f.STD_ID == student.STD_ID);
                                    if (_update != null)
                                    {
                                        _update.STD_TITLE_ID = student.STD_TITLE_ID;
                                        _update.STD_NAME = student.STD_NAME;
                                        _update.STD_SURNAME = student.STD_SURNAME;
                                        _update.STD_LEVEL_ID = student.STD_LEVEL_ID;
                                        _update.STD_SCHOOL_ID = student.STD_SCHOOL_ID;
                                        _update.STD_PICTURE_PATH = student.STD_PICTURE_PATH;
                                        _update.STD_PHONE = student.STD_PHONE;
                                        _update.STD_EMAIL = student.STD_EMAIL;
                                        _update.STD_ID_CARD = student.STD_ID_CARD;
                                        _update.STD_PASSPORT_ID = student.STD_PASSPORT_ID;
                                        _update.STD_GRADE = student.STD_GRADE;
                                        _update.STD_PHONE_PROVIDER = student.STD_PHONE_PROVIDER;
                                        _update.STD_APPROVED_STATUS = student.STD_APPROVED_STATUS;
                                        _update.STD_BIRTH_DAY = student.STD_BIRTH_DAY;
                                        _update.STD_IS_CONCERN = student.STD_IS_CONCERN;

                                        _update.STD_NATION = student.STD_NATION;
                                        _update.STD_NATION_OTHER = student.STD_NATION_OTHER;
                                    }
                                }
                                else
                                {

                                    //insert
                                    student.STD_APPROVED_STATUS = 1;
                                    context.TB_APPLICATION_STUDENT.Add(student);
                                }
                            }
                        }
                        context.SaveChanges();
                        tran.Complete();

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.InnerException.Message);
                    }
                }
            }



            return View("EditComplete");

        }

        public ActionResult Approve(int id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION_SCHOOL tb_application_school = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == id);
            if (tb_application_school == null)
            {
                return HttpNotFound();
            }
            List<TB_M_STATUS> tbMStatus = db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 1).ToList();
            ViewBag.SCHOOL_APPROVED_STATUS = new SelectList(tbMStatus, "STATUS_ID", "STATUS_NAME_TH", tb_application_school.SCHOOL_APPROVED_STATUS);
            return View(tb_application_school);
        }

        [HttpPost]
        public ActionResult Approve(TB_APPLICATION_SCHOOL tb_application_school)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            if (tb_application_school.SCHOOL_APPROVED_STATUS == 3)
            {
                List<TB_APPLICATION_STUDENT> studentLists = db.TB_APPLICATION_STUDENT.Where(s => s.STD_SCHOOL_ID == tb_application_school.SCHOOL_ID).ToList();
                if (studentLists != null)
                {
                    foreach (TB_APPLICATION_STUDENT student in studentLists)
                    {
                        var _update = db.TB_APPLICATION_STUDENT.FirstOrDefault(f => f.STD_ID == student.STD_ID);
                        if (_update != null)
                        {
                            _update.STD_APPROVED_STATUS = 3;

                        }
                    }
                }
            }
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
                _updateSchool.SCHOOL_APPROVED_STATUS = tb_application_school.SCHOOL_APPROVED_STATUS;
                _updateSchool.SCHOOL_EMAIL = tb_application_school.SCHOOL_EMAIL;
                _updateSchool.SCHOOL_CULTURE = tb_application_school.SCHOOL_CULTURE;
                _updateSchool.SCHOOL_ROUND = tb_application_school.SCHOOL_ROUND;

            }
            //db.TB_APPLICATION_SCHOOL.Attach(tb_application_school);
            //db.ObjectStateManager.ChangeObjectState(tb_application_school, EntityState.Modified);

            db.SaveChanges();

            return RedirectToAction("SchoolList/" + tb_application_school.SCHOOL_ID);

        }

        public ActionResult ApproveStudentList(int id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<TB_APPLICATION_STUDENT> _students = db.TB_APPLICATION_STUDENT.Where(s => s.STD_SCHOOL_ID == id).ToList();

            var model = new ApplicationModel { Students = _students };
            return View(model);
        }

        public ActionResult ApproveStudent(int id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION_STUDENT student = db.TB_APPLICATION_STUDENT.Single(t => t.STD_ID == id);
            if (student == null)
            {
                return HttpNotFound();
            }
            List<TB_M_STATUS> tbMStatus = db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 1).ToList();
            ViewBag.STD_APPROVED_STATUS = new SelectList(tbMStatus, "STATUS_ID", "STATUS_NAME_TH", student.STD_APPROVED_STATUS);

            return View(student);
        }

        [HttpPost]
        public ActionResult ApproveStudent(TB_APPLICATION_STUDENT student)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            if (ModelState.IsValid)
            {
                var _update = db.TB_APPLICATION_STUDENT.FirstOrDefault(f => f.STD_ID == student.STD_ID);
                if (_update != null)
                {
                    _update.STD_TITLE_ID = student.STD_TITLE_ID;
                    _update.STD_NAME = student.STD_NAME;
                    _update.STD_SURNAME = student.STD_SURNAME;
                    _update.STD_LEVEL_ID = student.STD_LEVEL_ID;
                    _update.STD_SCHOOL_ID = student.STD_SCHOOL_ID;
                    _update.STD_PICTURE_PATH = student.STD_PICTURE_PATH;
                    _update.STD_PHONE = student.STD_PHONE;
                    _update.STD_EMAIL = student.STD_EMAIL;
                    _update.STD_ID_CARD = student.STD_ID_CARD;
                    _update.STD_PASSPORT_ID = student.STD_PASSPORT_ID;
                    _update.STD_GRADE = student.STD_GRADE;
                    _update.STD_PHONE_PROVIDER = student.STD_PHONE_PROVIDER;
                    _update.STD_APPROVED_STATUS = student.STD_APPROVED_STATUS;
                    _update.STD_BIRTH_DAY = student.STD_BIRTH_DAY;
                    _update.STD_IS_CONCERN = student.STD_IS_CONCERN;
                    _update.STD_NATION = student.STD_NATION;
                    _update.STD_NATION_OTHER = student.STD_NATION_OTHER;
                }
                db.SaveChanges();
                return RedirectToAction("ApproveStudentList/" + student.STD_SCHOOL_ID);
            }
            return View(student);
        }

        public ActionResult ConfirmDocumentList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var model = new ConfirmDocumentModel { };
            return View(model);
        }
        [HttpPost]
        public ActionResult ConfirmDocumentList(ConfirmDocumentModel model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            List<TB_APPLICATION_STUDENT> students = null;
            if (!CommonUtils.isNumber(model.schoolName))
            {
                var student = from s in db.TB_APPLICATION_STUDENT where s.STD_NAME.Contains(model.schoolName) select s;
                students = student.ToList();
                foreach (TB_APPLICATION_STUDENT std in students)
                {
                    if (std.TB_STUDENT_SEAT != null)
                    {
                        std.STD_EMAIL = std.TB_STUDENT_SEAT.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(std.TB_STUDENT_SEAT.SIT_NUMBER).ToString("00");
                    }
                }
                model.students = students;
            }
            else
            {
                int stdNum = Convert.ToInt32(model.schoolName);
                var student = from s in db.TB_APPLICATION_STUDENT where s.TB_STUDENT_SEAT.STUDENT_CODE == stdNum select s;
                students = student.ToList();
                foreach (TB_APPLICATION_STUDENT std in students)
                {
                    std.STD_EMAIL = std.TB_STUDENT_SEAT.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(std.TB_STUDENT_SEAT.SIT_NUMBER).ToString("00");
                }
                model.students = students;
            }
            return View(model);
        }


        public ActionResult SearchStudent()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var model = new ConfirmDocumentModel { };
            return View(model);
        }
        [HttpPost]
        public ActionResult SearchStudent(ConfirmDocumentModel model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            List<TB_APPLICATION_STUDENT> students = null;
            if (CommonUtils.isNumber(model.schoolName))
            {
                int stdNum = Convert.ToInt32(model.schoolName);
                var student = from s in db.TB_APPLICATION_STUDENT where s.TB_STUDENT_SEAT.STUDENT_CODE == stdNum select s;
                students = student.ToList();
                foreach (TB_APPLICATION_STUDENT std in students)
                {
                    std.STD_EMAIL = std.TB_STUDENT_SEAT.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(std.TB_STUDENT_SEAT.SIT_NUMBER).ToString("00");
                }
                model.students = students;


                if (model.action.Equals("adjust"))
                {
                    //int stdNum1 = Convert.ToInt32(model.schoolName);
                    //Move Old code from tb_student_seat
                    TB_APPLICATION_STUDENT tmpStd = db.TB_APPLICATION_STUDENT.Single(t => t.TB_STUDENT_SEAT.STUDENT_CODE == stdNum);
                    if (tmpStd != null)
                    {
                        tmpStd.STD_LEVEL_ID = model.SelectedLevelIDs[0];
                        tmpStd.STD_TITLE_ID = model.STD_TITLE_IDs[0];
                        tmpStd.STD_NAME = model.names[0];
                        tmpStd.STD_SURNAME = model.surnames[0];

                        //tmpStd.
                        //db.TB_APPLICATION_STUDENT.Remove(tmpStd);
                    }
                    //TB_STUDENT_SEAT tmpSeat = db.TB_STUDENT_SEAT.Single(t => t.STUDENT_CODE == stdNum);
                    //db.TB_STUDENT_SEAT.Remove(tmpSeat);
                    //Commit
                    db.SaveChanges();
                    Console.WriteLine();
                }
            }
            return View(model);
        }

        public ActionResult PrintConfirmDocument(int id = 0)
        {

            CultureInfo ci = (CultureInfo)this.Session["PhetCulture"];
            /*
             * DECLARE VARIABLE
             */
            CommonUtils util = new CommonUtils();


            List<Report01Model> lists = new List<Report01Model>();

            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_ID == id select a;


            if (tb_application_student != null)
            {

                List<TB_APPLICATION_STUDENT> students = tb_application_student.ToList();
                int schId = Convert.ToInt16(students[0].STD_SCHOOL_ID);
                TB_APPLICATION_SCHOOL school = db.TB_APPLICATION_SCHOOL.Where(s => s.SCHOOL_ID == schId).FirstOrDefault();
                foreach (TB_APPLICATION_STUDENT student in students)
                {

                    /*
                     * ที่นั่งสอบ
                     */
                    TB_STUDENT_SEAT studentSeat = db.TB_STUDENT_SEAT.SingleOrDefault(t => t.STUDENT_ID == student.STD_ID);
                    Report01Model report = new Report01Model();
                    report.map = CommonUtils.getByteImage(Convert.ToInt16(student.STD_LEVEL_ID));
                    if (studentSeat != null)
                    {
                        report.p_std_id = studentSeat.STUDENT_CODE + "";
                        report.p_exam_room = studentSeat.TB_ROOM.ROOM_NUMBER + "";
                        report.p_exam_seat = studentSeat.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(studentSeat.SIT_NUMBER).ToString("00");
                        report.p_exam_building = studentSeat.TB_ROOM.ROOM_BUILD + "";
                        report.p_exam_floor = studentSeat.TB_ROOM.ROOM_FLOOR + "";

                    }
                    else
                    {
                        report.p_std_id = "-";
                        report.p_exam_room = "-";
                        report.p_exam_seat = "-";
                        report.p_exam_building = "-";
                        report.p_exam_floor = "-";
                    }
                    /*
                     * บอร์ดที่ประกาศผลสอบ
                     */
                    TB_EXAM_ANOUNCE examAnounce = db.TB_EXAM_ANOUNCE.Where(t => t.ANOUNCE_FOR_LEVEL == student.STD_LEVEL_ID).FirstOrDefault();
                    if (examAnounce != null)
                    {
                        report.p_exam_announce_building = examAnounce.ANOUNCE_BUILDING + "";
                        report.p_exam_announce_board = examAnounce.ANOUNCE_BOARD;
                    }
                    else
                    {
                        report.p_exam_announce_building = "-";
                        report.p_exam_announce_board = "-";
                    }

                    report.p_exam_date = "9 สิงหาคม 2558";
                    report.p_exam_time = "09.00-10.00 น.";

                    report.p_student_name = util.getStudentFullName(student);
                    report.p_school_name = school.SCHOOL_NAME;
                    report.p_school_type = getSchoolType(school.SCHOOL_TYPE);
                    report.p_school_province = school.TB_M_PROVINCE.PROVINCE_NAME;
                    report.p_student_level = student.TB_M_LEVEL.LEVEL_NAME_TH.Split(' ')[0];// +" " + student.STD_GRADE;
                    report.p_student_grade = student.STD_GRADE + "";

                    String phoneLable = (ci.Name.ToUpper().Equals("TH")) ? "เบอร์ติดต่อ  " : "Phone Number  ";

                    var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == school.SCHOOL_ID select a;
                    //APPEND STAFF
                    if (tb_application_staff != null)
                    {
                        List<TB_APPLICATION_STAFF> staffs = tb_application_staff.ToList();

                        if (staffs.Count >= 1)
                        {
                            report.p_staff01_name = util.getStaffFullName(staffs[0]);
                            report.p_staff01_phone = phoneLable + staffs[0].STAFF_PHONE;
                        }
                        if (staffs.Count >= 2)
                        {
                            report.p_staff02_name = util.getStaffFullName(staffs[1]);
                            report.p_staff02_phone = phoneLable + staffs[1].STAFF_PHONE;
                        }
                        if (staffs.Count >= 3)
                        {
                            report.p_staff03_name = util.getStaffFullName(staffs[2]);
                            report.p_staff03_phone = phoneLable + staffs[2].STAFF_PHONE;
                        }
                        if (staffs.Count >= 4)
                        {
                            report.p_staff04_name = util.getStaffFullName(staffs[3]);
                            report.p_staff04_phone = phoneLable + staffs[3].STAFF_PHONE;
                        }
                        if (staffs.Count >= 5)
                        {
                            report.p_staff05_name = util.getStaffFullName(staffs[4]);
                            report.p_staff05_phone = phoneLable + staffs[4].STAFF_PHONE;
                        }
                    }
                    lists.Add(report);
                }
            }


            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Reports/Rpt01_th.rpt");
            rptH.Load();
            rptH.SetDataSource(lists);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        /* CUSTOM EVENT*/
        public ActionResult DeleteSchool(ManagementModel model)
        {
            try
            {

                foreach (TB_APPLICATION_STUDENT dStd in db.TB_APPLICATION_STUDENT.Where(s => s.STD_SCHOOL_ID == model.id).ToList())
                {
                    db.TB_APPLICATION_STUDENT.Remove(dStd);
                }
                foreach (TB_APPLICATION_STAFF dStaff in db.TB_APPLICATION_STAFF.Where(s => s.STAFF_SCHOOL_ID == model.id).ToList())
                {
                    db.TB_APPLICATION_STAFF.Remove(dStaff);
                }


                TB_APPLICATION_SCHOOL tb_application_stchool = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == model.id);
                db.TB_APPLICATION_SCHOOL.Remove(tb_application_stchool);


                db.SaveChanges();
                // delete the record from ID and return true else false
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.ResultMsg = "ไม่สามารถลบข้อมูลได้";
                //return View();
                //return Json(ex.InnerException.Message, JsonRequestBehavior.AllowGet);
            }



            List<TB_M_STATUS> tbMStatus = db.TB_M_STATUS.Where(s => s.STATUS_GROUP == 1).ToList();
            ViewBag.approveStatus = new SelectList(tbMStatus, "STATUS_ID", "STATUS_NAME_TH", model.approveStatus);


            int startDate = Convert.ToInt32(CommonUtils.toDate(model.startDate).ToString("yyyyMMdd", new CultureInfo("th-TH")));
            int endDate = Convert.ToInt32(CommonUtils.toDate(model.endDate).ToString("yyyyMMdd", new CultureInfo("th-TH")));

            var varSchool = from a in db.TB_APPLICATION_SCHOOL select a;
            List<TB_APPLICATION_SCHOOL> tmpSchoolList = varSchool.ToList();


            List<TB_APPLICATION_SCHOOL> schoolList = new List<TB_APPLICATION_SCHOOL>();
            StringBuilder x = new StringBuilder();
            foreach (TB_APPLICATION_SCHOOL school in tmpSchoolList)
            {
                int curDate = Convert.ToInt32(school.SCHOOL_REGISTER_DATE.Value.ToString("yyyyMMdd", new CultureInfo("en-US")));
                if (curDate >= startDate && curDate <= endDate)
                {
                    x.Append("|" + curDate + "," + startDate + "," + endDate + "|");
                    schoolList.Add(school);
                }
            }
            if (schoolList != null)
            {
                var pageIndex = model.Page ?? 1;
                model.schools = schoolList.ToPagedList(pageIndex, 15);
            }

            return View("SchoolList", model);
        }

        private String getSchoolType(String type)
        {
            String result = "";
            switch (Convert.ToInt16(type))
            {
                case 1:
                    result = Resources.Application.Application.SCHOOL_TYPE_01;
                    break;
                case 2:
                    result = Resources.Application.Application.SCHOOL_TYPE_02;
                    break;
                case 3:
                    result = Resources.Application.Application.SCHOOL_TYPE_03;
                    break;
                case 4:
                    result = Resources.Application.Application.SCHOOL_TYPE_04;
                    break;
                case 5:
                    result = Resources.Application.Application.SCHOOL_TYPE_OTHER;
                    break;
                default: break;
            }
            return result;
        }

        public PartialViewResult editStaff(String STAFF_TITLE_ID, String STAFF_NAME, String STAFF_SURNAME, String STAFF_PHONE)
        {

            return PartialView("_AddStaffPartial_Edit",
                new TB_APPLICATION_STAFF
                {
                    STAFF_TITLE_ID = Convert.ToInt16(STAFF_TITLE_ID),
                    STAFF_NAME = STAFF_NAME,
                    STAFF_SURNAME = STAFF_SURNAME,
                    STAFF_PHONE = STAFF_PHONE
                }
                );
        }

        public PartialViewResult EditStudent(String STD_LEVEL_ID, String STD_GRADE, String STD_TITLE_ID, String STD_NAME, String STD_SURNAME, String STD_PHONE, String STD_EMAIL, String STD_BIRTH_DAY)
        {
            return PartialView("_AddStudentPartial_Edit",
                new TB_APPLICATION_STUDENT
                {
                    STD_LEVEL_ID = Convert.ToInt16(STD_LEVEL_ID),
                    STD_GRADE = Convert.ToInt16(STD_GRADE),
                    STD_TITLE_ID = Convert.ToInt16(STD_TITLE_ID),
                    STD_NAME = STD_NAME,
                    STD_SURNAME = STD_SURNAME,
                    STD_PHONE = STD_PHONE,
                    STD_EMAIL = STD_EMAIL,
                    STD_BIRTH_DAY = STD_BIRTH_DAY
                }
                );
        }



        public ActionResult ManagementAnnounce()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var model = db.TB_APPLICATION.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult ManagementAnnounce(TB_APPLICATION _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            TB_APPLICATION editApp = db.TB_APPLICATION.Single(t => t.PROJECT_ID == _model.PROJECT_ID);
            if (editApp != null)
            {
                editApp.PROJECT_IS_ANNOUNCE_ROUND1 = _model.PROJECT_IS_ANNOUNCE_ROUND1;
                editApp.PROJECT_IS_ANNOUNCE_ROUND2 = _model.PROJECT_IS_ANNOUNCE_ROUND2;
                db.SaveChanges();
            }

            return View(_model);
        }


        public ActionResult AddStudentAdhoc()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var model = new ApplicationAdhocModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddStudentAdhoc(ApplicationAdhocModel _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            TB_ROOM room = db.TB_ROOM.Where(x => x.ROOM_NUMBER == _model.ROOM_NUMBER).FirstOrDefault();
            if (room != null)
            {
                #region "SCHOOL"
                TB_APPLICATION_SCHOOL school = new TB_APPLICATION_SCHOOL();
                school.SCHOOL_ADDR_PROVINCE = -1;
                school.SCHOOL_NAME = _model.SCHOOL_NAME;
                school.SCHOOL_REGISTER_DATE = DateTime.Now;
                school.SCHOOL_APPROVED_STATUS = 3;
                school.SCHOOL_CULTURE = "th";
                school.SCHOOL_ROUND = 12;
                db.TB_APPLICATION_SCHOOL.Add(school);
                db.SaveChanges();
                #endregion
                #region "SCHOOL"
                TB_APPLICATION_STUDENT student = new TB_APPLICATION_STUDENT();
                student.STD_TITLE_ID = _model.STD_TITLE_ID;
                student.STD_NAME = _model.STD_NAME;
                student.STD_SURNAME = _model.STD_SURNAME;
                student.STD_LEVEL_ID = _model.STD_LEVEL_ID;
                student.STD_SCHOOL_ID = school.SCHOOL_ID;
                student.STD_APPROVED_STATUS = 3;
                student.STD_IS_CONCERN = "0";
                student.STD_NATION = _model.STD_NATION;
                db.TB_APPLICATION_STUDENT.Add(student);
                db.SaveChanges();
                #endregion
                #region "SEAT"
                TB_STUDENT_SEAT seat = new TB_STUDENT_SEAT();
                seat.STUDENT_ID = student.STD_ID;
                seat.STUDENT_CODE = Convert.ToDecimal(_model.STUDENT_CODE);
                seat.SIT_NUMBER = _model.SIT_NUMBER;
                seat.SIT_NUMBER_PREFIX = _model.SIT_NUMBER_PREFIX;
                seat.ROOM_ID = room.ROOM_ID;
                db.TB_STUDENT_SEAT.Add(seat);
                #endregion
                db.SaveChanges();
                ViewBag.ResultMsg = "บันทึกเรียบร้อยแล้ว";
            }
            else
            {
                ViewBag.ResultMsg = "ไม่พบห้อง";

            }
            return RedirectToAction("../Management/AddStudentAdhoc");

        }
    }
}
