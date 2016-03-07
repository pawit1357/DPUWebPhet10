using System.Web.Mvc;

namespace DPUWebPhet10.Controllers
{
    public class SumController : Controller
    {
        //
        // GET: /Summary/

        public ActionResult Index()
        {
            return RedirectToAction("../Report/DailyReport");
        }

    }
}
