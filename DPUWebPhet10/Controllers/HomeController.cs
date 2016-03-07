using System;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;
using DPUWebPhet10.Models;
using DPUWebPhet10.Utils;
using System.Web.Security;
namespace DPUWebPhet10.Controllers
{
    public class HomeController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();
        public ActionResult Index()
        {
            if (Session["Phet10School"] == null)
            {
                Session["Phet10School"] = null;
                FormsAuthentication.SignOut();
            }

            if (Session["TB_APPLICATION"] == null )
            {
                Boolean isInRegisterPeriod = false;
                Boolean isAnnounceDate = false;

                TB_APPLICATION app = db.TB_APPLICATION.FirstOrDefault();
                if (app != null)
                {
                    int startAppDate = Convert.ToInt32(Convert.ToDateTime(app.PROJECT_REGISTER_START).ToString("yyyyMMdd"));
                    int endAppDate = Convert.ToInt32(Convert.ToDateTime(app.PROJECT_REGISTER_END).ToString("yyyyMMdd"));
                    int announceDate = Convert.ToInt32(Convert.ToDateTime(app.PROJECT_ANNOUNCE_DATE).ToString("yyyyMMdd"));
                    int curDate = CommonUtils.getCurrentDateInt();


                    if (curDate >= startAppDate && curDate <= endAppDate)
                    {
                        isInRegisterPeriod = true;
                    }
                    if (curDate >= announceDate)
                    {
                        isAnnounceDate = true;
                    }
                }

                /* 
                 * ตรวจสอบสิ้นสุดระยะเวลารับสมัคร
                 */
                Session["isInRegisterPeriod"] = isInRegisterPeriod;
                /* 
                 * ตรวจสอบสามารถกดเมนู "พิมพ์เอกสารยืนยัน"
                 * ตรวจสอบสามารถกดเมนู "ประกาศเลขที่นั่งสอบ
                 * ตรวจสอบสามารถกดเมนู "ประกาศห้องสอบ
                 * ตรวจสอบสามารถกดเมนู "ประกาศผลการแข่งขัน
                 */
                Session["isAnnounceDate"] = isAnnounceDate;

                Session["PROJECT_IS_ANNOUNCE_ROUND1"] = (app.PROJECT_IS_ANNOUNCE_ROUND1==1)? "1":"0";
                Session["PROJECT_IS_ANNOUNCE_ROUND2"] = (app.PROJECT_IS_ANNOUNCE_ROUND2==1)? "1":"0";

            }

            ViewBag.PageContent = "คำนิยม";

            return View("Index");
        }

        public ActionResult Schedule()
        {
            ViewBag.PageContent = "กำหนดการแข่งขัน";
            return View("Schedule");
        }
        
        public ActionResult Contact()
        {
            ViewBag.PageContent = "ติดต่อ";
            return View();
        }
        public ActionResult Map()
        {
            ViewBag.PageContent = "แผนผัง อาคารสอบ";
            return View();
        }
        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            Session["PhetCulture"] = new CultureInfo(lang);
            return Redirect(returnUrl);
        }
    }
}
