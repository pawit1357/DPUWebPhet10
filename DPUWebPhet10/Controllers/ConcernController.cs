using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using PagedList;
using System.IO;
using System.Web.UI;
namespace DPUWebPhet10.Controllers
{
    public class ConcernController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /ExamAbsent/



        public ActionResult Index(TB_APPLICATION_STUDENT _model)
        {
            var varListOfConcern = from sch in db.TB_APPLICATION_SCHOOL
                                   join std in db.TB_APPLICATION_STUDENT on sch.SCHOOL_ID equals std.STD_SCHOOL_ID
                                   join staff in db.TB_APPLICATION_STAFF on sch.SCHOOL_ID equals staff.STAFF_SCHOOL_ID
                                   join lv in db.TB_M_LEVEL on std.STD_LEVEL_ID equals lv.LEVEL_ID
                                   where std.STD_IS_CONCERN == "1" && std.STD_BIRTH_DAY != null
                                   select new ConcernView()
                                   {
                                       SCHOOL_NAME = sch.SCHOOL_NAME,
                                       STD_NAME = std.STD_NAME,
                                       STD_SURNAME = std.STD_SURNAME,
                                       STAFF_NAME = staff.STAFF_NAME,
                                       STAFF_PHONE = staff.STAFF_PHONE,
                                       LEVEL_NAME = lv.LEVEL_NAME_TH
                                   };


            List<ConcernView> reportLists = varListOfConcern.ToList();

            int seq = 1;
            foreach (ConcernView std in reportLists)
            {
                std.seq = seq;
                seq++;
            }
            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 20);
            }
            return View(_model);
        }

        public void ExportClientsListToExcel()
        {
            var grid = new System.Web.UI.WebControls.GridView();


          var result=  from sch in db.TB_APPLICATION_SCHOOL
                                   join std in db.TB_APPLICATION_STUDENT on sch.SCHOOL_ID equals std.STD_SCHOOL_ID
                                   join staff in db.TB_APPLICATION_STAFF on sch.SCHOOL_ID equals staff.STAFF_SCHOOL_ID
                                   join lv in db.TB_M_LEVEL on std.STD_LEVEL_ID equals lv.LEVEL_ID
                                   where std.STD_IS_CONCERN == "1" && std.STD_BIRTH_DAY != null
                                   select new ConcernView()
                                   {
                                       SCHOOL_NAME = sch.SCHOOL_NAME,
                                       STD_NAME = std.STD_NAME,
                                       STD_SURNAME = std.STD_SURNAME,
                                       STAFF_NAME = staff.STAFF_NAME,
                                       STAFF_PHONE = staff.STAFF_PHONE,
                                       LEVEL_NAME = lv.LEVEL_NAME_TH
                                   };
          grid.DataSource = result.ToList(); /*from d in dbContext.diners
                              where d.user_diners.All(m => m.user_id == userID) && d.active == true */
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Exported_Student_concern.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }
        //public void ExportClientsListToCSV()
        //{

        //    StringWriter sw = new StringWriter();

        //    sw.WriteLine("\"First Name\",\"Last Name\",\"Email\"");

        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment;filename=Exported_Users.csv");
        //    Response.ContentType = "text/csv";

        //    foreach (var line in ClientsList)
        //    {
        //        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
        //                                   line.FirstName,
        //                                   line.LastName,
        //                                   line.Dob,
        //                                   line.Email));
        //    }

        //    Response.Write(sw.ToString());

        //    Response.End();

        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}