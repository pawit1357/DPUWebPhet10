using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using System.Web.Security;
using PagedList;
using DPUWebPhet10.Utility;
namespace DPUWebPhet10.Controllers
{

    public class AccountController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();
        //
        // GET: /Login/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Logout()
        {
            Session["Phet10School"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("../Home");
        }

        [HttpPost]
        public JsonResult CheckLogin(string uname, string pwd)
        {
            bool result = true;



            ///* CHECK USER ACCOUNT */
            var _school = from s in db.TB_APPLICATION_SCHOOL where s.SCHOOL_EMAIL == uname && s.SCHOOL_PASSWORD == pwd select s;

            if (_school == null)
            {
                result = false;
            }
            else
            {
                TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();
                TB_APPLICATION_SCHOOL school = _school.FirstOrDefault();
                if (school != null)
                {
                    result = true;
                    Session["Phet10School"] = (TB_APPLICATION_SCHOOL)school;
                    Session["TB_APPLICATION"] = application;

                    FormsAuthentication.SetAuthCookie(school.SCHOOL_NAME + "  (" + uname + ")", true);
                }
                else
                {
                    result = false;
                }
            }

            return Json(new { Success = result, responseText = "" });

        }


        public ActionResult CheckPassword()
        {
            var model = new CheckPasswordModel
            {

            };
            return View(model);
        }
        [HttpPost]
        public ActionResult CheckPassword(CheckPasswordModel _model)
        {

            List<TB_APPLICATION_SCHOOL> schoolList = db.TB_APPLICATION_SCHOOL.Where(r => r.SCHOOL_NAME.Contains(_model.school)).ToList();
            if (schoolList != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.schools = schoolList.ToPagedList(pageIndex, 15);
                ViewBag.PageContent = "ตรวจสอบรายชื่อผู้สมัคร";
            }

            return View(_model);
        }


        public ActionResult ManagementLogin()
        {
            return View("ManagementLogin");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ManagementLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (ChinaPhet10Entities context = new ChinaPhet10Entities())
                {
                    var user = context.TB_USER.FirstOrDefault(x => x.USERNAME == model.UserName && x.PASSWORD == model.Password);
                    if (user == null)
                    {
                        ModelState.AddModelError("LogOnError", "The user name or password provided is incorrect.");
                    }
                    else
                    {
                        //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                           && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            //update lasted login time
                            //user.LASTED_LOGIN = DateTime.Now;
                            //context.ObjectStateManager.ChangeObjectState(user, System.Data.EntityState.Modified);
                            var _update = db.TB_USER.FirstOrDefault(f => f.ID == user.ID);
                            if (_update != null)
                            {
                                _update.ROLE_ID = user.ROLE_ID;
                                _update.USERNAME = user.USERNAME;
                                _update.PASSWORD = user.PASSWORD;
                                _update.STATUS = user.STATUS;
                                _update.CREATE_BY = user.CREATE_BY;
                                _update.EMAIL = user.EMAIL;
                            }


                            context.SaveChanges();


                            //Redirect to default page
                            Session["USER"] = user;
                            Session["ROLE"] = user.TB_ROLE.NAME;
                            String[] rolPermission = new String[user.TB_ROLE.TB_ROLE_PERMISSION.Count];
                            int index = 0;
                            foreach (TB_ROLE_PERMISSION item in user.TB_ROLE.TB_ROLE_PERMISSION.ToList())
                            {
                                rolPermission[index] = item.PERMISSION_CODE;
                                index++;
                            }
                            Session["ROLE_PERMISSION"] = rolPermission;
                            return RedirectToAction("Index", "Management");
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public ActionResult ManagementLogout()
        {
            Session["USER"] = null;
            Session["ROLE"] = null;
            Session["ROLE_PERMISSION"] = null;
            Session["PhetCulture"] = null;
            return RedirectToAction("../Account/ManagementLogin");
        }
    }
}
