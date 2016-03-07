using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using System.Configuration;
using System.Data.OleDb;
//using ExcelTools = Ms.Office;
//using Excel = Microsoft.Office.Interop.Excel;

namespace DPUWebPhet10.Controllers
{
    public class CommitteeController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /Committee/

        public ActionResult Index()
        {
            Console.WriteLine();
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var tb_commitee = db.TB_COMMITEE.Include("TB_M_TITLE");

            List<TB_COMMITEE> commitees = tb_commitee.ToList();

            foreach (TB_COMMITEE commitee in commitees)
            {
                commitee.COMMITEE_NAME = commitee.COMMITEE_NAME + "  " + commitee.COMMITEE_SURNAME;
            }
            return View(tb_commitee.ToList());
        }

        //
        // GET: /Committee/Create

        public ActionResult Create()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            ViewBag.COMMITEE_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH");
            ViewBag.Menu01 = "";
            ViewBag.Menu02 = "active";
            ViewBag.Menu03 = "";
            ViewBag.Menu04 = "";
            ViewBag.Menu05 = "";
            return View();
        }

        //
        // POST: /Committee/Create

        [HttpPost]
        public ActionResult Create(TB_COMMITEE tb_commitee)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                db.TB_COMMITEE.Add(tb_commitee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.COMMITEE_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_commitee.COMMITEE_TITLE_ID);
            return View(tb_commitee);
        }

        //
        // GET: /Committee/Edit/5

        public ActionResult Edit(decimal id = 0)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_COMMITEE tb_commitee = db.TB_COMMITEE.Single(t => t.COMMITEE_ID == id);
            if (tb_commitee == null)
            {
                return HttpNotFound();
            }
            ViewBag.COMMITEE_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_commitee.COMMITEE_TITLE_ID);
            return View(tb_commitee);
        }

        //
        // POST: /Committee/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_COMMITEE tb_commitee)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (ModelState.IsValid)
            {
                var _update = db.TB_COMMITEE.FirstOrDefault(f => f.COMMITEE_ID == tb_commitee.COMMITEE_ID);
                if (_update != null)
                {
                    _update.COMMITEE_ID = tb_commitee.COMMITEE_ID;
                    _update.COMMITEE_TITLE_ID = tb_commitee.COMMITEE_TITLE_ID;
                    _update.COMMITEE_NAME = tb_commitee.COMMITEE_NAME;
                    _update.COMMITEE_SURNAME = tb_commitee.COMMITEE_SURNAME;
                    _update.COMMITEE_STATUS = tb_commitee.COMMITEE_STATUS;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.COMMITEE_TITLE_ID = new SelectList(db.TB_M_TITLE, "TITLE_ID", "TITLE_NAME_TH", tb_commitee.COMMITEE_TITLE_ID);
            return View(tb_commitee);
        }

        //
        // GET: /Committee/Delete/5

        public JsonResult Delete(int ID)
        {

            try
            {
                TB_COMMITEE tb_commitee = db.TB_COMMITEE.Single(t => t.COMMITEE_ID == ID);
                db.TB_COMMITEE.Remove(tb_commitee);
                db.SaveChanges();
                // delete the record from ID and return true else false
            }
            catch (Exception ex)
            {
                return Json(ex.InnerException.Message, JsonRequestBehavior.AllowGet);
            }
            //return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { msg = "success" } };
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Importexcel()
        {


            if (Request.Files["FileUpload1"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName);
                string path1 = string.Format("{0}/{1}", Server.MapPath("~/FileUpload/include"), Request.Files["FileUpload1"].FileName);
                if (System.IO.File.Exists(path1))
                    System.IO.File.Delete(path1);

                Request.Files["FileUpload1"].SaveAs(path1);

                Import_To_Grid(path1, extension, "No");


            }

            return RedirectToAction("Index");
        }

        public ActionResult ProcessCommittee()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            return View();
        }
        public ActionResult doProcessCommittee()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            ViewBag.ResultMsg = "";
            int countOfCommittee = 0;
            int countOfSuccess = 0;
            int countOfFail = 0;
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                List<TB_COMMITEE> commitees = db.TB_COMMITEE.OrderBy(c => c.COMMITEE_ID).Include("TB_M_TITLE").ToList();
                if (commitees != null)
                {
                    /*
                    * DELETE TB_COMMITEE_ROOM
                    */
                    foreach (TB_COMMITEE_ROOM dcmtr in context.TB_COMMITEE_ROOM.ToList())
                    {
                        context.TB_COMMITEE_ROOM.Remove(dcmtr);
                    }
                    //context.TB_COMMITEE_ROOM.ToList().ForEach(context.TB_COMMITEE_ROOM.DeleteObject);

                    countOfCommittee = context.TB_COMMITEE_ROOM.Count();

                    var varRoom = from a in db.TB_ROOM orderby a.ROOM_FOR_LEVEL, a.ROOM_NUMBER select a;
                    List<TB_ROOM> rooms = varRoom.ToList();
                    if (rooms != null)
                    {
                        int index = 0;
                        foreach (TB_ROOM room in rooms)
                        {
                            for (int i = 0; i < Convert.ToInt16(room.ROOM_COMMITTEE_COUNT); i++)
                            {
                                if (index < commitees.Count)
                                {

                                    TB_COMMITEE_ROOM cr = new TB_COMMITEE_ROOM();
                                    cr.COMMITEE_ID = commitees[index].COMMITEE_ID;
                                    cr.COMMITEE_ROOM_ID = room.ROOM_ID;
                                    context.TB_COMMITEE_ROOM.Add(cr);
                                    countOfSuccess++;
                                    index++;
                                }
                                else
                                {
                                    //countOfFail++;
                                    //ViewBag.ResultMsg = "จำนวนคนไม่พอกับจำนวนห้อง !";
                                }
                            }

                        }
                    }
                    context.SaveChanges();
                }
            }

            ViewBag.ResultMsg = "ประมวลผลกรรมการคุมสอบเรียบร้อยแล้ว (จำนวนกรรมการทั้งหมด " + countOfCommittee + " คน <br> สำเร็จ " + countOfSuccess + " รายการ <br> ไม่สำเร็จ " + countOfFail + " รายการ)";
            return View("ProcessCommittee");
        }


      


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        private void Import_To_Grid(string FilePath, string Extension, string isHDR)
        {
            string conStr = "";
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;
            }
            conStr = String.Format(conStr, FilePath, isHDR);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            cmdExcel.Connection = connExcel;

            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            connExcel.Close();

            //Read Data from First Sheet
            connExcel.Open();
            cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
            oda.SelectCommand = cmdExcel;
            oda.Fill(dt);
            //String xx = dt.Rows[0][0].ToString();
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                //context.TB_COMMITEE.ToList().ForEach(context.TB_COMMITEE);

                foreach (TB_COMMITEE deleteCmt in context.TB_COMMITEE.ToList())
                {
                    context.TB_COMMITEE.Remove(deleteCmt);
                }

                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    TB_COMMITEE commitee = new TB_COMMITEE();
                    commitee.COMMITEE_TITLE_ID = 0;
                    commitee.COMMITEE_NAME = dt.Rows[i][1].ToString();
                    commitee.COMMITEE_SURNAME = dt.Rows[i][2].ToString();
                    commitee.COMMITEE_PHONE = dt.Rows[i][3].ToString();
                    commitee.COMMITEE_STATUS = dt.Rows[i][4].ToString();
                    context.TB_COMMITEE.Add(commitee);

                }
                context.SaveChanges();
            }
            connExcel.Close();

            //Bind Data to GridView
            //GridView1.Caption = Path.GetFileName(FilePath);
            //GridView1.DataSource = dt;
            //GridView1.DataBind();
        }
    }
}