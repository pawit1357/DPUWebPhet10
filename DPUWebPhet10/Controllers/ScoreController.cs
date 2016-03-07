using System;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using DPUWebPhet10.Utils;
using DPUWebPhet10.Utility;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Transactions;

namespace DPUWebPhet10.Controllers
{
    public class ScoreController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();
        private IFileStore _fileStore = new DiskFileStore();
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ScoreController));


        public ActionResult Round1()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var model = new ScoreRound1Model { };
            return View("Round1", model);
        }

        [HttpPost]
        public ActionResult doUploadRound1()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            String resultMsg = "โหลดข้อมูลเข้าระบบเรียบร้อยแล้ว";
            if (Request.Files["FileUpload1"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName);
                if (extension.ToUpper().Equals(".CSV"))
                {
                    string path1 = Path.Combine(Server.MapPath("~/FileUpload/include"), Request.Files["FileUpload1"].FileName);


                    if (System.IO.File.Exists(path1))
                        System.IO.File.Delete(path1);

                    Request.Files["FileUpload1"].SaveAs(path1);

                    Import_To_Grid(path1, extension, "No");
                }
                else
                {
                    resultMsg = string.Format("{0} มีรูปแบบไฟล์ไม่ถูกต้อง", Request.Files["FileUpload1"].FileName);
                }
            }

            ViewBag.ResultMsg = resultMsg;
            return View("Round1");
        }

        public ActionResult ProcessRound1()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            return View("ProcessRound1");
        }

        [HttpPost]
        public ActionResult doProcessRound1(ScoreRound1Model model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            int _levelId = Convert.ToInt16(model.studentLevel);




            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        where s.STD_LEVEL_ID == _levelId && s.STD_NATION != 2
                        orderby s.STD_LEVEL_ID ascending, r.ROUND_SCORE descending
                        select new
                        {
                            StdCode = ss.STUDENT_CODE,
                            Score = r.ROUND_SCORE,
                            StudentLevel = _levelId,
                            s.STD_NATION,
                            r.ROUND_SCORE
                        };

            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                int index = 1;
                foreach (var item in items)
                {
                    TB_SCORE_ROUND_1 sr = db.TB_SCORE_ROUND_1.Where(s => s.STD_CODE == item.StdCode).FirstOrDefault();
                    if (sr != null)
                    {
                        if (item.StudentLevel == 5)
                        {
                            //check condition
                            if (index >= 1 && index <= 10)
                            {
                                sr.PRIZE_ID = 1;//เข้ารอบเพชรยอดมงกุฏ
                            }
                            else if (index > 10 && index <= 20)
                            {
                                sr.PRIZE_ID = 2;//รางวัลชอมเชย
                            }
                            else
                            {
                                sr.PRIZE_ID = 4;
                            }

                        }
                        else
                        {
                            if (index >= 1 && index <= 10)
                            {
                                sr.PRIZE_ID = 1;//เข้ารอบเพชรยอดมงกุฏ
                            }
                            else if (index > 10 && index <= 50)
                            {
                                sr.PRIZE_ID = 2;//รางวัลชอมเชย
                            }
                            else if (index > 50 && index <= 100)
                            {
                                sr.PRIZE_ID = 3;//รางวัลผ่านเกณฑ์
                            }
                            else
                            {
                                sr.PRIZE_ID = 4;
                            }
                        }

                        index++;
                        if (index % 200 == 0)
                        {
                            db.SaveChanges();
                        }
                    }
                }

                db.SaveChanges();

            }
            ViewBag.ResultMsg = "ปรับคะแนน 100 อันดับ ระดับชั้นที่ " + model.studentLevel + " รอบเจียรไนเพชรเรียบร้อยแล้ว";
            return View("ProcessRound1");
        }

        public ActionResult ProcessRound2()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            return View("ProcessRound2");
        }
        [HttpPost]
        public ActionResult doProcessRound2(ScoreRound1Model model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            int _levelId = Convert.ToInt16(model.studentLevel);


            var round2Result = from stdSeat in db.TB_STUDENT_SEAT
                               join std in db.TB_APPLICATION_STUDENT on stdSeat.STUDENT_ID equals std.STD_ID
                               join r1 in db.TB_SCORE_ROUND_1 on stdSeat.STUDENT_CODE equals r1.STD_CODE
                               join r2 in db.TB_SCORE_ROUND_2 on stdSeat.STUDENT_CODE equals r2.STD_CODE
                               where std.STD_LEVEL_ID == _levelId && std.STD_NATION != 2
                               orderby (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                        r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                        r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                        r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                        r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                        r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34

                               ) descending
                               select new
                               {
                                   studentCode = std.TB_STUDENT_SEAT.STUDENT_CODE,
                                   studentFullName = std.TB_M_TITLE.TITLE_NAME_TH + "" + std.STD_NAME + "  " + std.STD_SURNAME,
                                   round1Score = r1.ROUND_SCORE,
                                   round2Score = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                               r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                               r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                               r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                               r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                               r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34)

                               };
            if (round2Result != null)
            {
                int index = 1;
                foreach (var item in round2Result)
                {

                    TB_SCORE_ROUND_2 round2 = db.TB_SCORE_ROUND_2.Where(s => s.STD_CODE == item.studentCode).FirstOrDefault();
                    if (round2 != null)
                    {


                        if (index == 1)
                        {
                            round2.PRIZE_ID = 5;//เหรียญทอง
                        }
                        else if (index == 2)
                        {
                            round2.PRIZE_ID = 6;//เหรียญเงิน
                        }
                        else if (index == 3)
                        {
                            round2.PRIZE_ID = 7;//เหรียญทองแดง
                        }
                        else if (index <= 10)
                        {
                            round2.PRIZE_ID = 8;//ชมเชย
                        }
                        else
                        {
                            round2.PRIZE_ID = 4;
                        }
                        round2.ROUND_1_SCORE = item.round1Score;
                        index++;
                    }
                }
                db.SaveChanges();

            }
            ViewBag.ResultMsg = "ประมวลผลคะแนน ระดับชั้นที่ " + model.studentLevel + " รอบเพชรยอดมงกุฏเรียบร้อยแล้ว";
            return View("ProcessRound2");
        }

        public ActionResult Round2_1(ScoreRound21Model model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            switch (model.actionName)
            {
                case "ค้นหา":

                    if (!CommonUtils.isNumber(model.studentCode))
                    {
                        ViewBag.ResultErrorMsg = "เลขผู้สมัครต้องเป็นตัวเลขเท่านั้น";
                        model.studentCode = "";
                        model.studentName = null;
                        model.score11 = "";
                        model.score12 = "";
                        model.score13 = "";
                        model.score14 = "";
                        model.score15 = "";

                        model.score21 = "";
                        model.score22 = "";
                        model.score23 = "";
                        model.score24 = "";
                        model.score25 = "";

                        model.score31 = "";
                        model.score32 = "";
                        model.score33 = "";
                        model.score34 = "";
                        model.score35 = "";
                        ModelState.Clear();
                    }
                    else
                    {
                        int studentCodeSearch = Convert.ToInt32(model.studentCode);
                        TB_STUDENT_SEAT resultStudent = db.TB_STUDENT_SEAT.Where(s => s.STUDENT_CODE == studentCodeSearch).FirstOrDefault();
                        if (resultStudent != null)
                        {
                            if (resultStudent.TB_APPLICATION_STUDENT != null)
                            {

                                model.student = resultStudent;
                                model.studentName = resultStudent.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + resultStudent.TB_APPLICATION_STUDENT.STD_NAME + "  " + resultStudent.TB_APPLICATION_STUDENT.STD_SURNAME + "  (โรงเรียน" + resultStudent.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME + ")";
                                TB_SCORE_ROUND_2 score21 = db.TB_SCORE_ROUND_2.Where(k => k.STD_CODE == resultStudent.STUDENT_CODE).FirstOrDefault();
                                if (score21 != null)
                                {
                                    ModelState.Clear();
                                    model.score11 = Convert.ToString(score21.SCORE_1_11);
                                    model.score12 = Convert.ToString(score21.SCORE_1_12);
                                    model.score13 = Convert.ToString(score21.SCORE_1_13);
                                    model.score14 = Convert.ToString(score21.SCORE_1_14);
                                    model.score15 = Convert.ToString(score21.SCORE_1_15);

                                    model.score21 = Convert.ToString(score21.SCORE_1_21);
                                    model.score22 = Convert.ToString(score21.SCORE_1_22);
                                    model.score23 = Convert.ToString(score21.SCORE_1_23);
                                    model.score24 = Convert.ToString(score21.SCORE_1_24);
                                    model.score25 = Convert.ToString(score21.SCORE_1_25);

                                    model.score31 = Convert.ToString(score21.SCORE_1_31);
                                    model.score32 = Convert.ToString(score21.SCORE_1_32);
                                    model.score33 = Convert.ToString(score21.SCORE_1_33);
                                    model.score34 = Convert.ToString(score21.SCORE_1_34);
                                    model.score35 = Convert.ToString(score21.SCORE_1_35);
                                }
                                else
                                {
                                    ModelState.Clear();
                                    model.score11 = "";
                                    model.score12 = "";
                                    model.score13 = "";
                                    model.score14 = "";
                                    model.score15 = "";

                                    model.score21 = "";
                                    model.score22 = "";
                                    model.score23 = "";
                                    model.score24 = "";
                                    model.score25 = "";

                                    model.score31 = "";
                                    model.score32 = "";
                                    model.score33 = "";
                                    model.score34 = "";
                                    model.score35 = "";
                                }
                            }
                            else
                            {
                                ViewBag.ResultErrorMsg = "ไม่พบข้อมูลนักเรียน-ไม่มีข้อมูลโรงเรียน";
                            }
                        }
                        else
                        {
                            ViewBag.ResultErrorMsg = "ไม่พบข้อมูลนักเรียน";
                        }
                    }
                    break;
                case "บันทึก":

                    if (String.IsNullOrEmpty(model.score11) ||
                        String.IsNullOrEmpty(model.score12) ||
                        String.IsNullOrEmpty(model.score13) ||
                        String.IsNullOrEmpty(model.score14) ||
                        String.IsNullOrEmpty(model.score15) ||

                        String.IsNullOrEmpty(model.score21) ||
                        String.IsNullOrEmpty(model.score22) ||
                        String.IsNullOrEmpty(model.score23) ||
                        String.IsNullOrEmpty(model.score24) ||
                        String.IsNullOrEmpty(model.score25) ||

                        String.IsNullOrEmpty(model.score31) ||
                        String.IsNullOrEmpty(model.score32) ||
                        String.IsNullOrEmpty(model.score33) ||
                        String.IsNullOrEmpty(model.score34) ||
                        String.IsNullOrEmpty(model.score35)
                        )
                    {
                        ViewBag.ResultErrorMsg = "ยังไม่ได้ป้อนคะแนน";
                    }
                    else if (validateScore21(model).Length > 0)
                    {
                        ViewBag.ResultErrorMsg = validateScore21(model);
                    }
                    else if (
                            !CommonUtils.isDouble(model.score11)
                            || !CommonUtils.isDouble(model.score12)
                            || !CommonUtils.isDouble(model.score13)
                            || !CommonUtils.isDouble(model.score14)
                            || !CommonUtils.isDouble(model.score15)

                            || !CommonUtils.isDouble(model.score21)
                            || !CommonUtils.isDouble(model.score22)
                            || !CommonUtils.isDouble(model.score23)
                            || !CommonUtils.isDouble(model.score24)
                            || !CommonUtils.isDouble(model.score25)

                            || !CommonUtils.isDouble(model.score31)
                            || !CommonUtils.isDouble(model.score32)
                            || !CommonUtils.isDouble(model.score33)
                            || !CommonUtils.isDouble(model.score34)
                            || !CommonUtils.isDouble(model.score35)
                            )
                    {
                        ViewBag.ResultErrorMsg = "ตรวจสอบคะแนนที่บันทึกต้องเป็นตัวเลขหรือทศนิยมเท่านั้น";
                    }
                    else
                    {
                        int stuentCodeSave = Convert.ToInt32(model.studentCode);
                        TB_SCORE_ROUND_2 score21 = db.TB_SCORE_ROUND_2.Where(k => k.STD_CODE == stuentCodeSave).FirstOrDefault();
                        if (score21 == null)
                        {
                            TB_SCORE_ROUND_2 round2 = new TB_SCORE_ROUND_2();
                            round2.STD_CODE = stuentCodeSave;
                            round2.SCORE_1_11 = model.score11.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score11);
                            round2.SCORE_1_12 = model.score12.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score12);
                            round2.SCORE_1_13 = model.score13.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score13);
                            round2.SCORE_1_14 = model.score14.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score14);
                            round2.SCORE_1_15 = model.score15.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score15);

                            round2.SCORE_1_21 = model.score21.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score21);
                            round2.SCORE_1_22 = model.score22.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score22);
                            round2.SCORE_1_23 = model.score23.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score23);
                            round2.SCORE_1_24 = model.score24.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score24);
                            round2.SCORE_1_25 = model.score25.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score25);

                            round2.SCORE_1_31 = model.score21.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score21);
                            round2.SCORE_1_32 = model.score22.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score22);
                            round2.SCORE_1_33 = model.score23.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score23);
                            round2.SCORE_1_34 = model.score24.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score24);
                            round2.SCORE_1_35 = model.score25.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score25);
                            db.TB_SCORE_ROUND_2.Add(round2);
                        }
                        else
                        {
                            score21.STD_CODE = stuentCodeSave;
                            score21.SCORE_1_11 = model.score11.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score11);
                            score21.SCORE_1_12 = model.score12.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score12);
                            score21.SCORE_1_13 = model.score13.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score13);
                            score21.SCORE_1_14 = model.score14.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score14);
                            score21.SCORE_1_15 = model.score15.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score15);

                            score21.SCORE_1_21 = model.score21.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score21);
                            score21.SCORE_1_22 = model.score22.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score22);
                            score21.SCORE_1_23 = model.score23.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score23);
                            score21.SCORE_1_24 = model.score24.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score24);
                            score21.SCORE_1_25 = model.score25.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score25);

                            score21.SCORE_1_31 = model.score31.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score31);
                            score21.SCORE_1_32 = model.score32.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score32);
                            score21.SCORE_1_33 = model.score33.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score33);
                            score21.SCORE_1_34 = model.score34.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score34);
                            score21.SCORE_1_35 = model.score35.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score35);


                            //db.ObjectStateManager.ChangeObjectState(score21, System.Data.EntityState.Modified);
                        }
                        db.SaveChanges();

                        model.studentCode = "";
                        model.studentName = null;
                        model.score11 = "";
                        model.score12 = "";
                        model.score13 = "";
                        model.score14 = "";
                        model.score15 = "";

                        model.score21 = "";
                        model.score22 = "";
                        model.score23 = "";
                        model.score24 = "";
                        model.score25 = "";

                        model.score31 = "";
                        model.score32 = "";
                        model.score33 = "";
                        model.score34 = "";
                        model.score35 = "";
                        ModelState.Clear();
                        ViewBag.ResultMsg = "บันทึกข้อมูลเรียบร้อยแล้ว";
                    }
                    break;
                default:
                    break;
            }

            return View("Round2_1", model);
        }


        public ActionResult Round2_2(ScoreRound22Model model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            switch (model.actionName)
            {
                case "ค้นหา":

                    if (!CommonUtils.isNumber(model.studentCode))
                    {
                        ViewBag.ResultErrorMsg = "เลขผู้สมัครต้องเป็นตัวเลขเท่านั้น";
                        model.studentCode = "";
                        model.studentName = null;
                        model.score11 = "";
                        model.score12 = "";
                        model.score13 = "";
                        model.score14 = "";

                        model.score21 = "";
                        model.score22 = "";
                        model.score23 = "";
                        model.score24 = "";

                        model.score31 = "";
                        model.score32 = "";
                        model.score33 = "";
                        model.score34 = "";
                        ModelState.Clear();
                    }
                    else
                    {
                        int studentCodeSearch = Convert.ToInt32(model.studentCode);
                        TB_STUDENT_SEAT resultStudent = db.TB_STUDENT_SEAT.Where(s => s.STUDENT_CODE == studentCodeSearch).FirstOrDefault();
                        if (resultStudent != null)
                        {
                            if (resultStudent.TB_APPLICATION_STUDENT != null)
                            {

                                model.student = resultStudent;
                                model.studentName = resultStudent.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + resultStudent.TB_APPLICATION_STUDENT.STD_NAME + "  " + resultStudent.TB_APPLICATION_STUDENT.STD_SURNAME + "  (โรงเรียน" + resultStudent.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME + ")";
                                TB_SCORE_ROUND_2 score22 = db.TB_SCORE_ROUND_2.Where(k => k.STD_CODE == resultStudent.STUDENT_CODE).FirstOrDefault();
                                if (score22 != null)
                                {
                                    ModelState.Clear();
                                    model.score11 = Convert.ToString(score22.SCORE_2_11);
                                    model.score12 = Convert.ToString(score22.SCORE_2_12);
                                    model.score13 = Convert.ToString(score22.SCORE_2_13);
                                    model.score14 = Convert.ToString(score22.SCORE_2_14);

                                    model.score21 = Convert.ToString(score22.SCORE_2_21);
                                    model.score22 = Convert.ToString(score22.SCORE_2_22);
                                    model.score23 = Convert.ToString(score22.SCORE_2_23);
                                    model.score24 = Convert.ToString(score22.SCORE_2_24);

                                    model.score31 = Convert.ToString(score22.SCORE_2_31);
                                    model.score32 = Convert.ToString(score22.SCORE_2_32);
                                    model.score33 = Convert.ToString(score22.SCORE_2_33);
                                    model.score34 = Convert.ToString(score22.SCORE_2_34);
                                }
                                else
                                {
                                    ModelState.Clear();
                                    model.score11 = "";
                                    model.score12 = "";
                                    model.score13 = "";
                                    model.score14 = "";

                                    model.score21 = "";
                                    model.score22 = "";
                                    model.score23 = "";
                                    model.score24 = "";

                                    model.score31 = "";
                                    model.score32 = "";
                                    model.score33 = "";
                                    model.score34 = "";
                                }
                            }
                            else
                            {
                                ViewBag.ResultErrorMsg = "ไม่พบข้อมูลนักเรียน-ไม่มีข้อมูลโรงเรียน";
                            }
                        }
                        else
                        {
                            ViewBag.ResultErrorMsg = "ไม่พบข้อมูลนักเรียน";
                        }
                    }
                    break;
                case "บันทึก":

                    if (String.IsNullOrEmpty(model.score11) ||
                        String.IsNullOrEmpty(model.score12) ||
                        String.IsNullOrEmpty(model.score13) ||
                        String.IsNullOrEmpty(model.score14) ||

                        String.IsNullOrEmpty(model.score21) ||
                        String.IsNullOrEmpty(model.score22) ||
                        String.IsNullOrEmpty(model.score23) ||
                        String.IsNullOrEmpty(model.score24) ||

                        String.IsNullOrEmpty(model.score31) ||
                        String.IsNullOrEmpty(model.score32) ||
                        String.IsNullOrEmpty(model.score33) ||
                        String.IsNullOrEmpty(model.score34)

                        )
                    {
                        ViewBag.ResultErrorMsg = "ยังไม่ได้ป้อนคะแนน";
                    }
                    else if (!CommonUtils.isDouble(model.score11)
                            || !CommonUtils.isDouble(model.score12)
                            || !CommonUtils.isDouble(model.score13)
                            || !CommonUtils.isDouble(model.score14)

                            || !CommonUtils.isDouble(model.score21)
                            || !CommonUtils.isDouble(model.score22)
                            || !CommonUtils.isDouble(model.score23)
                            || !CommonUtils.isDouble(model.score24)

                            || !CommonUtils.isDouble(model.score31)
                            || !CommonUtils.isDouble(model.score32)
                            || !CommonUtils.isDouble(model.score33)
                            || !CommonUtils.isDouble(model.score34)
                            )
                    {
                        ViewBag.ResultErrorMsg = "ตรวจสอบคะแนนที่บันทึกต้องเป็นตัวเลขหรือทศนิยมเท่านั้น";
                    }
                    else if (validateScore22(model).Length > 0)
                    {
                        ViewBag.ResultErrorMsg = validateScore22(model);
                    }
                    else
                    {
                        int stuentCodeSave = Convert.ToInt32(model.studentCode);
                        TB_SCORE_ROUND_2 score22 = db.TB_SCORE_ROUND_2.Where(k => k.STD_CODE == stuentCodeSave).FirstOrDefault();
                        if (score22 == null)
                        {
                            TB_SCORE_ROUND_2 round2 = new TB_SCORE_ROUND_2();
                            round2.STD_CODE = stuentCodeSave;
                            round2.SCORE_2_11 = Convert.ToDecimal(model.score11);
                            round2.SCORE_2_12 = Convert.ToDecimal(model.score12);
                            round2.SCORE_2_13 = Convert.ToDecimal(model.score13);
                            round2.SCORE_2_14 = Convert.ToDecimal(model.score14);

                            round2.SCORE_2_21 = Convert.ToDecimal(model.score21);
                            round2.SCORE_2_22 = Convert.ToDecimal(model.score22);
                            round2.SCORE_2_23 = Convert.ToDecimal(model.score23);
                            round2.SCORE_2_24 = Convert.ToDecimal(model.score24);

                            round2.SCORE_2_31 = Convert.ToDecimal(model.score31);
                            round2.SCORE_2_32 = Convert.ToDecimal(model.score32);
                            round2.SCORE_2_33 = Convert.ToDecimal(model.score33);
                            round2.SCORE_2_34 = Convert.ToDecimal(model.score34);
                            db.TB_SCORE_ROUND_2.Add(round2);
                        }
                        else
                        {
                            score22.STD_CODE = stuentCodeSave;
                            score22.SCORE_2_11 = model.score11.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score11);
                            score22.SCORE_2_12 = model.score12.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score12);
                            score22.SCORE_2_13 = model.score13.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score13);
                            score22.SCORE_2_14 = model.score14.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score14);

                            score22.SCORE_2_21 = model.score21.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score21);
                            score22.SCORE_2_22 = model.score22.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score22);
                            score22.SCORE_2_23 = model.score23.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score23);
                            score22.SCORE_2_24 = model.score24.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score24);

                            score22.SCORE_2_31 = model.score31.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score31);
                            score22.SCORE_2_32 = model.score32.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score32);
                            score22.SCORE_2_33 = model.score33.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score33);
                            score22.SCORE_2_34 = model.score34.Equals("") ? Convert.ToDecimal(0) : Convert.ToDecimal(model.score34);

                            //db.ObjectStateManager.ChangeObjectState(score22, System.Data.EntityState.Modified);
                        }
                        db.SaveChanges();

                        model.studentCode = "";
                        model.studentName = null;
                        model.score11 = "";
                        model.score12 = "";
                        model.score13 = "";
                        model.score14 = "";

                        model.score21 = "";
                        model.score22 = "";
                        model.score23 = "";
                        model.score24 = "";

                        model.score31 = "";
                        model.score32 = "";
                        model.score33 = "";
                        model.score34 = "";
                        ModelState.Clear();
                        ViewBag.ResultMsg = "บันทึกข้อมูลเรียบร้อยแล้ว";
                    }
                    break;
                default:
                    break;
            }


            return View("Round2_2", model);
        }


        public ActionResult deleteRound1Data()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                foreach (TB_SCORE_ROUND_1 r1 in context.TB_SCORE_ROUND_1.ToList())
                {
                    context.TB_SCORE_ROUND_1.Remove(r1);
                }
                foreach (TB_SCORE_ROUND_1_ERROR r1_error in context.TB_SCORE_ROUND_1_ERROR.ToList())
                {
                    context.TB_SCORE_ROUND_1_ERROR.Remove(r1_error);
                }
                //context.TB_SCORE_ROUND_1.ToList().ForEach(context.TB_SCORE_ROUND_1.DeleteObject);
                //context.TB_SCORE_ROUND_1_ERROR.ToList().ForEach(context.TB_SCORE_ROUND_1_ERROR.DeleteObject);

                context.SaveChanges();
            }
            ViewBag.ResultMsg = "ล้างข้อมูลรอบเจียรไนเพชรเรียบร้อยแล้ว";
            return View("Round1");
        }
        public ActionResult deleteRound2Data()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                foreach (TB_SCORE_ROUND_2 r1 in context.TB_SCORE_ROUND_2.ToList())
                {
                    context.TB_SCORE_ROUND_2.Remove(r1);
                }
                //context.TB_SCORE_ROUND_2.ToList().ForEach(context.TB_SCORE_ROUND_2.DeleteObject);
                context.SaveChanges();
            }
            ViewBag.ResultMsg = "ล้างข้อมูลรอบเจียรไนเพชรเรียบร้อยแล้ว";
            return View("Round1");
        }

        public ActionResult clearRound21Score(int id)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                Int32 stdCode = Convert.ToInt32(id);
                TB_SCORE_ROUND_2 score21 = context.TB_SCORE_ROUND_2.Where(s21 => s21.STD_CODE == stdCode).FirstOrDefault();
                if (score21 != null)
                {
                    score21.SCORE_1_11 = 0;
                    score21.SCORE_1_12 = 0;
                    score21.SCORE_1_13 = 0;
                    score21.SCORE_1_14 = 0;
                    score21.SCORE_1_15 = 0;

                    score21.SCORE_1_21 = 0;
                    score21.SCORE_1_22 = 0;
                    score21.SCORE_1_23 = 0;
                    score21.SCORE_1_24 = 0;
                    score21.SCORE_1_25 = 0;

                    score21.SCORE_1_31 = 0;
                    score21.SCORE_1_32 = 0;
                    score21.SCORE_1_33 = 0;
                    score21.SCORE_1_34 = 0;
                    score21.SCORE_1_35 = 0;
                    context.SaveChanges();
                    ViewBag.ResultMsg = "ล้างข้อมูลเรียบร้อยแล้ว";
                }
                else
                {
                    ViewBag.ResultErrorMsg = "ยังไม่มีการบันทึกข้อมูล ไม่สามารถลบได้";
                }
            }
            return View("Round2_1", new ScoreRound21Model());
        }
        public ActionResult clearRound22Score(int id)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                Int32 stdCode = Convert.ToInt32(id);
                TB_SCORE_ROUND_2 score22 = context.TB_SCORE_ROUND_2.Where(s21 => s21.STD_CODE == stdCode).FirstOrDefault();
                if (score22 != null)
                {
                    score22.SCORE_2_11 = 0;
                    score22.SCORE_2_12 = 0;
                    score22.SCORE_2_13 = 0;
                    score22.SCORE_2_14 = 0;

                    score22.SCORE_2_21 = 0;
                    score22.SCORE_2_22 = 0;
                    score22.SCORE_2_23 = 0;
                    score22.SCORE_2_24 = 0;

                    score22.SCORE_2_31 = 0;
                    score22.SCORE_2_32 = 0;
                    score22.SCORE_2_33 = 0;
                    score22.SCORE_2_34 = 0;

                    context.SaveChanges();
                    ViewBag.ResultMsg = "ล้างข้อมูลเรียบร้อยแล้ว";
                }
                else
                {
                    ViewBag.ResultErrorMsg = "ยังไม่มีการบันทึกข้อมูล ไม่สามารถลบได้";
                }
            }
            return View("Round2_2", new ScoreRound22Model());
        }
        #region "FUNCTION"
        private void Import_To_Grid(string FilePath, string Extension, string isHDR)
        {
            logger.Debug(String.Format("####### --- Begin Start Load File at {0} --- ####",DateTime.Now));
            int rejected = 0;
            List<TB_SCORE_ROUND_1> stdList = new List<TB_SCORE_ROUND_1>();
            List<TB_EXAM_ABSENT> stdAppsentList = new List<TB_EXAM_ABSENT>();
            List<TB_CONCERN> stdConcerntList = new List<TB_CONCERN>();

            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                using (CsvFileReader reader = new CsvFileReader(FilePath))
                {
                    CsvRow row = new CsvRow();
                    while (reader.ReadRow(row))
                    {
                        try
                        {
                            int stdCode = ((row[0] == null) ? 0 : (row[0].Equals("")) ? 0 : Convert.ToInt32(row[0]));
                            if (!stdList.Any(x => x.STD_CODE == stdCode))
                            {
                                TB_SCORE_ROUND_1 round1 = new TB_SCORE_ROUND_1();
                                round1.STD_CODE = stdCode;
                                round1.ROUND_SCORE = ((row[1] == null) ? 0 : (row[1].Equals("")) ? 0 : Convert.ToInt32(row[1]));
                                round1.PRIZE_ID = 0;
                                stdList.Add(round1);
                                //บันทึกขาดสอบ
                                if (row[2].ToString().Equals("F") && round1.ROUND_SCORE == 0)
                                {
                                    if (!stdAppsentList.Any(x => x.STD_CODE == stdCode))
                                    {
                                        TB_EXAM_ABSENT tb_exam_absent = new TB_EXAM_ABSENT();
                                        tb_exam_absent.STD_CODE = stdCode;
                                        stdAppsentList.Add(tb_exam_absent);
                                    }
                                }
                                //สงสัยว่าผิดระดับชั้น 
                                if (row[3].ToString().Equals("T"))
                                {
                                    if (!stdConcerntList.Any(x => x.STD_CODE == stdCode))
                                    {
                                        TB_CONCERN concern = new TB_CONCERN();
                                        concern.STD_CODE = stdCode;
                                        stdConcerntList.Add(concern);
                                    }
                                }
                            }
                            else
                            {
                                rejected++;
                                logger.Debug("Reject>>duplicate student code:" + stdCode);
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                }
                //Begin Insert
                foreach (TB_EXAM_ABSENT _val in stdAppsentList)
                {
                    context.TB_EXAM_ABSENT.Add(_val);
                }
                context.SaveChanges();
                foreach (TB_CONCERN _val in stdConcerntList)
                {
                    context.TB_CONCERN.Add(_val);
                }
                context.SaveChanges();
                int index = 1;
                foreach (TB_SCORE_ROUND_1 _val in stdList)
                {
                    context.TB_SCORE_ROUND_1.Add(_val);
                    index++;
                    if (index % 200 == 0)
                    {
                        context.SaveChanges();
                    }
                }
                context.SaveChanges();

            }
            logger.Debug("######## SUMMARY ########");
            logger.Debug(String.Format("Total {0} (s),Reject {1} (s)>>>> Remain {2} (s)", stdList.Count, rejected,stdList.Count-rejected));
            logger.Debug(String.Format("####### --- Finish Load File at {0} --- ####", DateTime.Now));

        }
        #endregion


        private String validateScore21(ScoreRound21Model model)
        {
            StringBuilder sb = new StringBuilder();
            /* การนำเข้าสู่เนื้อหา (5)	 เนื้อหาสาระ (25)	 สรุป (5)	 ความคิดสร้างสรรค์ (5)	 ความถูกต้องของตัวอักษร (10) */

            //กรรมการคนที่ 1
            if (Convert.ToDouble(model.score11) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 1 มีการป้อน " + model.score11 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score12) > 25)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 2 มีการป้อน " + model.score12 + " ซึ่งมากกว่าคะแนนที่กำหนด (25 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score13) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 3 มีการป้อน " + model.score13 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score14) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 4 มีการป้อน " + model.score14 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score15) > 10)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 5 มีการป้อน " + model.score15 + " ซึ่งมากกว่าคะแนนที่กำหนด (10 คะแนน)\n");
            }
            //กรรมการคนที่ 2
            if (Convert.ToDouble(model.score21) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 2 คอลัมภ์ที่ 1 มีการป้อน " + model.score21 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score22) > 25)
            {
                sb.Append("คะแนนกรรมการคนที่ 2 คอลัมภ์ที่ 2 มีการป้อน " + model.score22 + " ซึ่งมากกว่าคะแนนที่กำหนด (25 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score23) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 2 คอลัมภ์ที่ 3 มีการป้อน " + model.score23 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score24) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 2 คอลัมภ์ที่ 4 มีการป้อน " + model.score24 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score25) > 10)
            {
                sb.Append("คะแนนกรรมการคนที่ 2 คอลัมภ์ที่ 5 มีการป้อน " + model.score25 + " ซึ่งมากกว่าคะแนนที่กำหนด (10 คะแนน)\n");
            }
            //กรรมการคนที่ 3
            if (Convert.ToDouble(model.score31) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 3 คอลัมภ์ที่ 1 มีการป้อน " + model.score31 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score32) > 25)
            {
                sb.Append("คะแนนกรรมการคนที่ 3 คอลัมภ์ที่ 2 มีการป้อน " + model.score32 + " ซึ่งมากกว่าคะแนนที่กำหนด (25 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score33) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 3 คอลัมภ์ที่ 3 มีการป้อน " + model.score33 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score34) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 3 คอลัมภ์ที่ 4 มีการป้อน " + model.score34 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score35) > 10)
            {
                sb.Append("คะแนนกรรมการคนที่ 3 คอลัมภ์ที่ 5 มีการป้อน " + model.score35 + " ซึ่งมากกว่าคะแนนที่กำหนด (10 คะแนน)\n");
            }
            return sb.ToString();
        }

        private String validateScore22(ScoreRound22Model model)
        {
            StringBuilder sb = new StringBuilder();
            /*
             * เนื้อหาสาระ(20)	 การออกเสียงอักขระ(20)	 บุคลิกภาพ(5)	 เวลา(5)
             * 
             */

            if (Convert.ToDouble(model.score11) > 20)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 1 มีการป้อน " + model.score11 + " ซึ่งมากกว่าคะแนนที่กำหนด (20 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score12) > 20)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 2 มีการป้อน " + model.score12 + " ซึ่งมากกว่าคะแนนที่กำหนด (20 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score13) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 3 มีการป้อน " + model.score13 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score14) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 4 มีการป้อน " + model.score14 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }

            if (Convert.ToDouble(model.score21) > 20)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 1 มีการป้อน " + model.score21 + " ซึ่งมากกว่าคะแนนที่กำหนด (20 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score22) > 20)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 2 มีการป้อน " + model.score22 + " ซึ่งมากกว่าคะแนนที่กำหนด (20 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score23) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 3 มีการป้อน " + model.score23 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score24) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 4 มีการป้อน " + model.score24 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }

            if (Convert.ToDouble(model.score31) > 20)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 1 มีการป้อน " + model.score31 + " ซึ่งมากกว่าคะแนนที่กำหนด (20 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score32) > 20)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 2 มีการป้อน " + model.score32 + " ซึ่งมากกว่าคะแนนที่กำหนด (20 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score33) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 3 มีการป้อน " + model.score33 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            if (Convert.ToDouble(model.score34) > 5)
            {
                sb.Append("คะแนนกรรมการคนที่ 1 คอลัมภ์ที่ 4 มีการป้อน " + model.score34 + " ซึ่งมากกว่าคะแนนที่กำหนด (5 คะแนน)\n");
            }
            return sb.ToString();
        }

    }
}
