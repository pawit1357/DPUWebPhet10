using System;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using System.Web.Security;
using System.Linq;
namespace DPUWebPhet10.Controllers
{
    public class UploadController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();
        private IFileStore _fileStore = new DiskFileStore();
        //
        // GET: /Upload/
        public ActionResult Index()
        {

            if (Session["Phet10School"] == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("../");
            }

            TB_APPLICATION_SCHOOL tb_application_school = (TB_APPLICATION_SCHOOL)Session["Phet10School"];

            ViewBag.PageContent = "ส่งเอกสารยืนยันการสมัคร";
            var model = new UploadModel
            {
                school = tb_application_school
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(UploadModel model)
        {
            //if (ModelState.IsValid)
            //{
            try
            {

                if (Request.Files["photo"] != null) // If uploaded synchronously
                {
                    model.photo_guid = _fileStore.SaveUploadedFile(Request.Files["photo"]);
                    model.photo_filename = Request.Files["photo"].FileName;
                }

                TB_SCHOOL_DOCUMENT schoolDocument = new TB_SCHOOL_DOCUMENT();
                schoolDocument.SCHOOL_ID = model.school.SCHOOL_ID;
                schoolDocument.DOCUMENT_PATH = model.photo_filename;
                db.TB_SCHOOL_DOCUMENT.Add(schoolDocument);

                var _update = db.TB_APPLICATION_SCHOOL.FirstOrDefault(f => f.SCHOOL_ID == model.school.SCHOOL_ID);
                if (_update != null)
                {
                    _update.SCHOOL_DOC_PATH = model.photo_filename;
                    _update.SCHOOL_APPROVED_STATUS = 2;
                }


                db.SaveChanges();
                return RedirectToAction("../School/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "ยังไม่ได้เลือกไฟล์");
            }
            //}
            ViewBag.PageContent = "ส่งเอกสารยืนยันการสมัคร";
            return View("Index", model);
        }

        public Guid AsyncUpload()
        {
            return _fileStore.SaveUploadedFile(Request.Files[0]);
        }

    }
}
