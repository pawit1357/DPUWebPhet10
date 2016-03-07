using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using DPUWebPhet10.Models.Common;
using DPUWebPhet10.Utility;
using System.Globalization;
using System.Web.Security;
using System.Transactions;
using System.Data.Entity.Validation;

namespace DPUWebPhet10.Controllers
{

    public class ApplicationController : Controller
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ApplicationController));
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        private IRepository repository = new DPUPhet10Repository();
        public string errorMessage = string.Empty;

        #region APPLICANT FORM (REGISTER)
        // GET: /SchoolRegister/
        public ActionResult Index()
        {
            //Thread.Sleep(5000);
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
             * SCHOOL TYPE
             */
            List<RadioButtonModel> list = new List<RadioButtonModel>();

            list.Add(new RadioButtonModel() { ID = 1, Name = Resources.Application.Application.SCHOOL_TYPE_01 });//สพฐ
            list.Add(new RadioButtonModel() { ID = 2, Name = Resources.Application.Application.SCHOOL_TYPE_02 });//เอกชน
            list.Add(new RadioButtonModel() { ID = 3, Name = Resources.Application.Application.SCHOOL_TYPE_03 });//กทม
            list.Add(new RadioButtonModel() { ID = 4, Name = Resources.Application.Application.SCHOOL_TYPE_04 });//อุดมศึกษา
            list.Add(new RadioButtonModel() { ID = 5, Name = Resources.Application.Application.SCHOOL_TYPE_OTHER });//อื่น ๆ 
            SelectList schoolTypes = new SelectList(list, "ID", "Name");

            String _selectLabel = "";
            CultureInfo ci = (CultureInfo)this.Session["PhetCulture"];
            if (ci != null)
            {
                _selectLabel = (ci.Name.ToUpper().Equals("TH")) ? "-- เลือก --" : "-- select --";
            }
            var model = new ApplicationModel()
            {

                selectLabel = _selectLabel,
                provinceLists = provinceLists,
                rSchoolTypes = schoolTypes
            };



            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index([Bind(Exclude = "SCHOOL_ID")]ApplicationModel model)
        {
            //Thread.Sleep(5000);
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
                model.rSchoolTypes = schoolTypes;
                return View(model);
            }

            //using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            //{
     
                //using (TransactionScope tran = new TransactionScope())
                //{
                    try
                    {
                        //Check school duplicate
                        var isExist = db.TB_APPLICATION_SCHOOL.Any(k => k.SCHOOL_EMAIL == model.school.SCHOOL_EMAIL);
                        if (!isExist)
                        {
                            /*
                             SCHOOL MODEL
                             */
                            CultureInfo ci = (CultureInfo)this.Session["PhetCulture"];
                            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();


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

                            model.school.SCHOOL_EMAIL = (model.school.SCHOOL_EMAIL == null) ? "" : model.school.SCHOOL_EMAIL;
                            model.school.SCHOOL_CULTURE = ci.Name;
                            model.school.SCHOOL_ROUND = Convert.ToInt32(application.PROJECT_ROUND);




                            model.school.SCHOOL_TYPE = (model.idSelectedSchoolType == null) ? "" : model.idSelectedSchoolType;
                            model.school.SCHOOL_APPROVED_STATUS = 1;//Y=Approved,N=DisApproved

                            /* generate password*/
                            String generatedPassword = GeneratePassword.Generate(8);
                            model.school.SCHOOL_PASSWORD = generatedPassword;// MD5.md5(generatedPassword);
                            db.TB_APPLICATION_SCHOOL.Add(model.school);
                            db.SaveChanges();
                            /*
                             STAFF
                             */
                            if (model.Staffs != null)
                            {
                                foreach (TB_APPLICATION_STAFF staff in model.Staffs)
                                {
                                    ////VALIDATE NULL VALUE
                                    staff.STAFF_SCHOOL_ID = model.school.SCHOOL_ID;
                                    staff.STAFF_POSITION = (staff.STAFF_POSITION == null) ? 0 : staff.STAFF_POSITION;
                                    staff.STAFF_NAME = (staff.STAFF_NAME == null) ? "" : staff.STAFF_NAME;
                                    staff.STAFF_SURNAME = (staff.STAFF_SURNAME == null) ? "" : staff.STAFF_SURNAME;
                                    staff.STAFF_PHONE = (staff.STAFF_PHONE == null) ? "" : staff.STAFF_PHONE;
                                    staff.STAFF_TITLE_ID = (staff.STAFF_TITLE_ID == null) ? 0 : staff.STAFF_TITLE_ID;
                                    staff.STAFF_FOR_LEVEL = (staff.STAFF_FOR_LEVEL == null) ? 0 : staff.STAFF_FOR_LEVEL;

                                    if (!String.IsNullOrEmpty(staff.STAFF_NAME) && !String.IsNullOrEmpty(staff.STAFF_SURNAME))
                                    {
                                        staff.STAFF_SCHOOL_ID = model.school.SCHOOL_ID;
                                        db.TB_APPLICATION_STAFF.Add(staff);
                                    }

                                }
                            }
                            /*
                             STUDENT
                             */
                            if (model.Students != null)
                            {
                                foreach (TB_APPLICATION_STUDENT student in model.Students)
                                {
                                    //VALIDATE NULL VALUE
                                    student.STD_TITLE_ID = (student.STD_TITLE_ID == null) ? 0 : student.STD_TITLE_ID;
                                    student.STD_NAME = (student.STD_NAME == null) ? "" : student.STD_NAME;
                                    student.STD_SURNAME = (student.STD_SURNAME == null) ? "" : student.STD_SURNAME;
                                    student.STD_LEVEL_ID = (student.STD_LEVEL_ID == null) ? 0 : student.STD_LEVEL_ID;
                                    student.STD_SCHOOL_ID =  model.school.SCHOOL_ID;
                                    student.STD_PICTURE_PATH = (student.STD_PICTURE_PATH == null) ? "" : student.STD_PICTURE_PATH;
                                    student.STD_PHONE = (student.STD_PHONE == null) ? "" : student.STD_PHONE;
                                    student.STD_EMAIL = (student.STD_EMAIL == null) ? "" : student.STD_EMAIL;
                                    student.STD_ID_CARD = (student.STD_ID_CARD == null) ? "" : student.STD_ID_CARD;
                                    student.STD_PASSPORT_ID = (student.STD_PASSPORT_ID == null) ? "" : student.STD_PASSPORT_ID;
                                    student.STD_GRADE = (student.STD_GRADE == null) ? 0 : student.STD_GRADE;
                                    student.STD_PHONE_PROVIDER = (student.STD_PHONE_PROVIDER == null) ? "" : student.STD_PHONE_PROVIDER;
                                    student.STD_APPROVED_STATUS = 1;
                                    student.STD_IS_CONCERN = "0";
                                    student.STD_NATION = student.STD_NATION;
                                    student.STD_NATION_OTHER = student.STD_NATION_OTHER;

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

                                    if (!String.IsNullOrEmpty(student.STD_NAME) && !String.IsNullOrEmpty(student.STD_SURNAME))
                                    {
                                        db.TB_APPLICATION_STUDENT.Add(student);
                                    }



                                }
                            }

                            db.SaveChanges();
                            //tran.Complete();

                            /* send email school account to user*/
                            if (!String.IsNullOrEmpty(model.school.SCHOOL_EMAIL))
                            {
                                if (Email.IsValidEmail(model.school.SCHOOL_EMAIL))
                                {
                                    Email.SendEmail(model.school, generatedPassword, ci.Name);
                                }
                                else
                                {
                                    logger.Debug(model.school.SCHOOL_ID + "," + model.school.SCHOOL_NAME + "invalid email format.");
                                }
                            }
                            else
                            {
                                logger.Debug(model.school.SCHOOL_ID + "," + model.school.SCHOOL_NAME + "no have email.");
                            }
                        }
                        else
                        {
                            return View("Exception");
                        }
                    }
                    catch (DbEntityValidationException dbEx)
                    {

                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                            }
                        }
                        throw new Exception(errorMessage, dbEx);
                    }
                //}
            //}
            ViewBag.UserName = model.school.SCHOOL_EMAIL;
            ViewBag.Password = model.school.SCHOOL_PASSWORD;
            ViewBag.PageContent = Resources.Application.Application.REGISTER_ITEM016;
            return View("Complete");
        }
        #endregion

        #region EDIT APPLICANT FORM

        public ActionResult SetEditLanguage(int id = 0)
        {

            if (Session["Phet10School"] == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("../");
            }
            TB_APPLICATION_SCHOOL tb_application_school = (TB_APPLICATION_SCHOOL)Session["Phet10School"];
            /*
             * SET INITIAL APPLICATION LANG
             */
            Session["PhetCulture"] = new CultureInfo((tb_application_school.SCHOOL_CULTURE == null) ? "th" : tb_application_school.SCHOOL_CULTURE);

            return RedirectToAction("Edit");
        }
        // GET: /SchoolRegister/
        public ActionResult Edit(int id = 0)
        {
            //Thread.Sleep(5000);
            /*
             * SCHOOL SESSION
             */
            if (Session["Phet10School"] == null)
            {
                return HttpNotFound();
            }



            TB_APPLICATION_SCHOOL tbSchoolSession = (TB_APPLICATION_SCHOOL)Session["Phet10School"];

            id = Convert.ToInt32(tbSchoolSession.SCHOOL_ID);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit([Bind(Exclude = "school_SCHOOL_EMAIL,school_SCHOOL_NAME")]ApplicationModel model)
        {
            //Thread.Sleep(5000);
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


            //using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            //{

            //    using (TransactionScope tran = new TransactionScope())
            //    {
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
                                    db.TB_APPLICATION_STAFF.Add(staff);
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
                                    db.TB_APPLICATION_STUDENT.Add(student);
                                }
                            }
                        }
                        db.SaveChanges();
                        //tran.Complete();

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.InnerException.Message);
                    }
            //    }
            //}



            return View("EditComplete");

        }
        #endregion

        #region PROVINCE CASCADE
        public ActionResult AsyncAmphurs(string SCHOOL_ADDR_PROVINCE)
        {
            DelayResponse();
            var amphurs = repository.GetAmphur(SCHOOL_ADDR_PROVINCE).ToList().Select(a => new SelectListItem()
            {
                Text = a.AMPHUR_NAME,
                Value = a.AMPHUR_ID.ToString(),
            });

            return Json(amphurs);
        }
        public ActionResult AsyncDistricts(string SCHOOL_ADDR_AMPHUR)
        {
            DelayResponse();
            var districts = repository.GetDistrict(SCHOOL_ADDR_AMPHUR).ToList().Select(a => new SelectListItem()
            {
                Text = a.DISTRICT_NAME,
                Value = a.DISTRICT_ID.ToString(),
            });
            return Json(districts);
        }
        private static void DelayResponse()
        {
            System.Threading.Thread.Sleep(100);
        }
        #endregion

        #region CUSTOM EVENT
        public PartialViewResult AddStaff(String STAFF_TITLE_ID, String STAFF_NAME, String STAFF_SURNAME, String STAFF_PHONE)
        {

            return PartialView("_AddStaffPartial",
                new TB_APPLICATION_STAFF
                {
                    STAFF_TITLE_ID = Convert.ToInt16(STAFF_TITLE_ID),
                    STAFF_NAME = STAFF_NAME,
                    STAFF_SURNAME = STAFF_SURNAME,
                    STAFF_PHONE = STAFF_PHONE
                }
                );
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
        public PartialViewResult AddStudent(String STD_LEVEL_ID, String STD_GRADE, String STD_TITLE_ID, String STD_NAME, String STD_SURNAME, String STD_PHONE, String STD_EMAIL, String STD_BIRTH_DAY, String STD_NATION, String STD_NATION_OTHER)
        {
            return PartialView("_AddStudentPartial",
                new TB_APPLICATION_STUDENT
                {
                    STD_LEVEL_ID = Convert.ToInt16(STD_LEVEL_ID),
                    STD_GRADE = Convert.ToInt16(STD_GRADE),
                    STD_TITLE_ID = Convert.ToInt16(STD_TITLE_ID),
                    STD_NAME = STD_NAME,
                    STD_SURNAME = STD_SURNAME,
                    STD_PHONE = STD_PHONE,
                    STD_EMAIL = STD_EMAIL,
                    STD_BIRTH_DAY = STD_BIRTH_DAY,
                    STD_NATION = Convert.ToInt16(STD_NATION),
                    STD_NATION_OTHER = STD_NATION_OTHER
                }
                );
        }
        public PartialViewResult EditStudent(String STD_LEVEL_ID, String STD_GRADE, String STD_TITLE_ID, String STD_NAME, String STD_SURNAME, String STD_PHONE, String STD_EMAIL, String STD_BIRTH_DAY, String STD_NATION, String STD_NATION_OTHER)
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
                    STD_BIRTH_DAY = STD_BIRTH_DAY,
                    STD_NATION = Convert.ToInt16(STD_NATION),
                    STD_NATION_OTHER = STD_NATION_OTHER
                }
                );
        }
        public ActionResult Autocomplete(string term)
        {
            String[] items = null;
            if (ViewData["phetSchoolAutoComplete"] == null)
            {
                List<TB_M_SCHOOL_9> schoolLists = db.TB_M_SCHOOL_9.ToList<TB_M_SCHOOL_9>();
                String[] array = new String[schoolLists.Count];
                int index = 0;
                foreach (TB_M_SCHOOL_9 school in schoolLists)
                {
                    array[index] = school.SCHOOL_NAME;
                    index++;
                }

                ViewData["phetSchoolAutoComplete"] = array;
                items = array;
            }
            else
            {

                items = (String[])ViewData["phetSchoolAutoComplete"];

            }

            var filteredItems = items.Where(
                item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0
                );
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult CheckEmailAvailability(String email)
        {

            var isExist = Email.IsValidEmail(email);
            return Json(isExist);
        }
        [HttpPost]
        public JsonResult checkEmailExist(ApplicationModel model)
        {

            var isExist = !db.TB_APPLICATION_SCHOOL.Any(k => k.SCHOOL_EMAIL == model.school.SCHOOL_EMAIL);
            return Json(isExist);
        }
        [HttpPost]
        public JsonResult checkExistSchoolName(ApplicationModel model)
        {

            var isExist = !db.TB_APPLICATION_SCHOOL.Any(k => k.SCHOOL_NAME == model.school.SCHOOL_NAME);
            return Json(isExist);
        }

        [HttpPost]
        public JsonResult deleteStaff(String id)
        {
            int deleteId = Convert.ToInt16(id);
            TB_APPLICATION_STAFF staff = db.TB_APPLICATION_STAFF.Where(s => s.STAFF_ID == deleteId).FirstOrDefault();
            db.TB_APPLICATION_STAFF.Remove(staff);

            int result = db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult deleteStudent(String id)
        {
            int deleteId = Convert.ToInt16(id);
            TB_APPLICATION_STUDENT student = db.TB_APPLICATION_STUDENT.Where(s => s.STD_ID == deleteId).FirstOrDefault();
            db.TB_APPLICATION_STUDENT.Remove(student);
            int result = db.SaveChanges();

            return Json(true);
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


