using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using PagedList;
using CrystalDecisions.CrystalReports.Engine;
using System.Text;
//using ExcelTools = Ms.Office;
//using Excel = Microsoft.Office.Interop.Excel;

namespace DPUWebPhet10.Controllers
{
    public class ExaminationSeatingPlanController : Controller
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ExaminationSeatingPlanController));
        private ChinaPhet10Entities db = new ChinaPhet10Entities();


        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<TB_M_LEVEL> tbMLevel = db.TB_M_LEVEL.Where(l => l.LEVEL_ID > 0).ToList();
            ViewBag.StudentLevel = new SelectList(tbMLevel, "LEVEL_ID", "LEVEL_NAME_TH");
            var model = new ProcessStudentModel { };
            return View(model);
        }
        public ActionResult Special()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            var model = new ProcessStudentModel { };
            return View(model);
        }

        [HttpPost]
        public ActionResult Special(ProcessStudentModel model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            ViewBag.ResultMsg = "";

            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                List<TB_APPLICATION_STUDENT> students = null;


                var _result = from a in db.TB_APPLICATION_STUDENT
                              orderby a.STD_LEVEL_ID, a.STD_ID
                              select a;
                var sitPlaned = from b in db.TB_STUDENT_SEAT select b.STUDENT_ID;

                var stdCodes = from b in db.TB_STUDENT_SEAT orderby b.STUDENT_CODE ascending select b;
                
                int lastLevel1 = 0;
                int lastLevel2 = 0;
                int lastLevel3 = 0;
                int lastLevel4 = 0;
                int lastLevel5 = 0;
                lastLevel1 = getLastStudentCode(stdCodes.ToList(), 1);
                lastLevel2 = getLastStudentCode(stdCodes.ToList(), 2);
                lastLevel3 = getLastStudentCode(stdCodes.ToList(), 3);
                lastLevel4 = getLastStudentCode(stdCodes.ToList(), 4);
                lastLevel5 = getLastStudentCode(stdCodes.ToList(), 5);

                //นำข้อมูลรายชื่อนักเรียนที่ยังไม่ได้จัดมาเก็บไว้ใน tmp
                var tmp = from x in _result where !sitPlaned.Contains(x.STD_ID) orderby x.STD_LEVEL_ID, x.STD_ID select x;

                students = tmp.ToList();
                if (students.Count > 0)
                {
                    var varRoom = from a in db.TB_ROOM where a.ROOM_FOR_LEVEL == 6 orderby a.ROOM_FOR_LEVEL, a.ROOM_ID select a;
                    List<TB_ROOM> rooms = varRoom.ToList();
                    if (rooms != null)
                    {


                        int studentIndex = 0;
                        String[] rowPrefix = { "A", "B", "C", "D", "E", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                        foreach (TB_ROOM room in rooms)
                        {

                            int sitIndex = 1;
                            String[] rows = room.ROOM_ROW.ToUpper().Split('X');
                            if (rows.Length == 2)
                            {
                                int col = Convert.ToInt16(rows[0]);//c
                                int row = Convert.ToInt16(rows[1]);//r

                                // 1. วนแต่ละแถวเพื่อเรียงลำดับเลขที่นั่งสอบ
                                for (int c = 0; c < col; c++)
                                {
                                    int level = 0;
                                    switch (c + 1)
                                    {
                                        case 1:
                                            level = Convert.ToInt16(room.ROW_1);
                                            break;
                                        case 2:
                                            level = Convert.ToInt16(room.ROW_2);
                                            break;
                                        case 3:
                                            level = Convert.ToInt16(room.ROW_3);
                                            break;
                                        case 4:
                                            level = Convert.ToInt16(room.ROW_4);
                                            break;
                                        case 5:
                                            level = Convert.ToInt16(room.ROW_5);
                                            break;
                                        case 6:
                                            level = Convert.ToInt16(room.ROW_6);
                                            break;
                                        case 7:
                                            level = Convert.ToInt16(room.ROW_7);
                                            break;
                                        case 8:
                                            level = Convert.ToInt16(room.ROW_8);
                                            break;
                                        case 9:
                                            level = Convert.ToInt16(room.ROW_9);
                                            break;
                                        case 10:
                                            level = Convert.ToInt16(room.ROW_10);
                                            break;
                                        case 11:
                                            level = Convert.ToInt16(room.ROW_11);
                                            break;
                                        case 12:
                                            level = Convert.ToInt16(room.ROW_12);
                                            break;
                                    }

                                    for (int r = 0; r < row; r++)
                                    {
                                        if (studentIndex < students.Count)
                                        {
                                            if (level == students[studentIndex].STD_LEVEL_ID)
                                            {
                                                //get last index
                                                logger.Debug(c + 1 + "," + r + "=" + students[studentIndex].STD_ID + "," + room.ROOM_NUMBER + "\n");

                                                TB_STUDENT_SEAT ss = new TB_STUDENT_SEAT();
                                                ss.STUDENT_ID = students[studentIndex].STD_ID;
                                                switch (level)
                                                {
                                                    case 1: ss.STUDENT_CODE = Convert.ToInt32(String.Format("{0}{1}", students[studentIndex].STD_LEVEL_ID,(lastLevel1+ sitIndex).ToString("0000")));
                                                        break;
                                                    case 2: ss.STUDENT_CODE = Convert.ToInt32(String.Format("{0}{1}", students[studentIndex].STD_LEVEL_ID, (lastLevel2+ sitIndex).ToString("0000")));
                                                        break;
                                                    case 3: ss.STUDENT_CODE = Convert.ToInt32(String.Format("{0}{1}", students[studentIndex].STD_LEVEL_ID, (lastLevel3 + sitIndex).ToString("0000")));
                                                        break;
                                                    case 4: ss.STUDENT_CODE = Convert.ToInt32(String.Format("{0}{1}", students[studentIndex].STD_LEVEL_ID, (lastLevel4 + sitIndex).ToString("0000")));
                                                        break;
                                                    case 5: ss.STUDENT_CODE = Convert.ToInt32(String.Format("{0}{1}", students[studentIndex].STD_LEVEL_ID, (lastLevel5 + sitIndex).ToString("0000")));
                                                        break;
                                                }
                                                
                                                ss.ROOM_ID = room.ROOM_ID;
                                                ss.SIT_NUMBER_PREFIX = rowPrefix[c];
                                                ss.SIT_NUMBER = (r + 1);
                                                context.TB_STUDENT_SEAT.Add(ss);

                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        sitIndex++;
                                        studentIndex++;
                                    }
                                }
                            }
                            else
                            {
                                logger.Debug(room.ROOM_NUMBER + ":ข้อมูล COLXROW ใน DB ของห้องไม่ถุกต้อง");
                            }
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        logger.Debug("ไม่พบข้อมุลห้องว่าง");
                    }
                }
                else
                {
                    ViewBag.ResultMsg = "ไม่พบข้อมูลนักเรียนที่ยังไม่ได้ที่นั่งสอบ";
                    return View(model);
                }
            }
            ViewBag.ResultMsg = "ประมวลผลที่นั่งสอบเรียบร้อยแล้ว";
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(ProcessStudentModel model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            StringBuilder noRoomStd = new StringBuilder();
            ViewBag.ResultMsg = "";
            //int countOfCommittee = 0;
            int countOfSuccess = 0;
            //int countOfFail = 0;
            using (ChinaPhet10Entities context = new ChinaPhet10Entities())
            {
                List<TB_APPLICATION_STUDENT> students = null;

                int studentLevel = Convert.ToInt16(model.StudentLevel);
                if (!String.IsNullOrWhiteSpace(model.StudentLevel))
                {
                    var varStudent = from a in db.TB_APPLICATION_STUDENT where a.STD_LEVEL_ID == studentLevel && a.STD_APPROVED_STATUS == 3 orderby a.STD_LEVEL_ID, a.STD_ID select a;
                    students = varStudent.ToList();
                }

                if (students != null)
                {

                    /*
                    * DELETE TB_STUDENT_SEAT BY LEVEL
                    */
                    foreach (TB_APPLICATION_STUDENT std in students)
                    {
                        if (std.TB_STUDENT_SEAT != null)
                        {
                            TB_STUDENT_SEAT studentSeat = context.TB_STUDENT_SEAT.Where(s => s.STUDENT_CODE == std.TB_STUDENT_SEAT.STUDENT_CODE).FirstOrDefault();
                            context.TB_STUDENT_SEAT.Remove(studentSeat);
                        }
                    }


                    var varRoom = from a in db.TB_ROOM where a.ROOM_FOR_LEVEL == studentLevel orderby a.ROOM_FOR_LEVEL, a.ROOM_ID select a;
                    List<TB_ROOM> rooms = varRoom.ToList();
                    if (rooms != null)
                    {
                        int index = 0;
                        int studentIndex = 1;
                        int examSitIndex = 1;
                        int tmpLevel = Convert.ToInt16(rooms[0].ROOM_FOR_LEVEL);
                        String[] rowPrefix = { "A", "B", "C", "D", "E", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                        foreach (TB_ROOM room in rooms)
                        {

                            if (tmpLevel != room.ROOM_FOR_LEVEL)
                            {
                                studentIndex = 1;
                                examSitIndex = 1;
                            }

                            String[] rows = room.ROOM_ROW.ToUpper().Split('X');
                            if (rows.Length == 2)
                            {
                                int width = Convert.ToInt16(rows[0]);
                                int height = Convert.ToInt16(rows[1]);

                                // 1. วนแต่ละแถวเพื่อเรียงลำดับเลขที่นั่งสอบ
                                for (int w = 0; w < width; w++)
                                {
                                    for (int h = 0; h < height; h++)
                                    {
                                        if (index < students.Count)
                                        {

                                            TB_STUDENT_SEAT ss = new TB_STUDENT_SEAT();
                                            ss.STUDENT_ID = students[index].STD_ID;
                                            ss.STUDENT_CODE = Convert.ToInt32(String.Format("{0}{1}", students[index].STD_LEVEL_ID, studentIndex.ToString("0000")));
                                            ss.ROOM_ID = room.ROOM_ID;
                                            ss.SIT_NUMBER_PREFIX = rowPrefix[w];
                                            ss.SIT_NUMBER = (h + 1);//Convert.ToInt32(String.Format("{0}{1}", room.ROOM_FOR_LEVEL, examSitIndex.ToString("0000")));
                                            context.TB_STUDENT_SEAT.Add(ss);

                                            index++;
                                            studentIndex++;
                                            examSitIndex++;

                                            countOfSuccess++;

                                        }
                                        else
                                        {
                                      
                                            Console.WriteLine("จำนวนคนไม่พอกับจำนวนห้อง !");
                                        }

                                    }
                                }
                            }
                            else
                            {
                                //generate row error.
                                Console.WriteLine();
                            }

                        }
                    }
                    context.SaveChanges();
                }
            }
            List<TB_M_LEVEL> tbMLevel = db.TB_M_LEVEL.Where(l => l.LEVEL_ID > 0).ToList();
            ViewBag.StudentLevel = new SelectList(tbMLevel, "LEVEL_ID", "LEVEL_NAME_TH", model.StudentLevel);
            ViewBag.ResultMsg = "ประมวลผลที่นั่งสอบเรียบร้อยแล้ว";//+ ("จำนวนนักเรียนทั้งหมด " + students. + " คน <br> สำเร็จ " + countOfSuccess + " รายการ <br> ไม่สำเร็จ " + countOfFail + " รายการ)";
            return View(model);
        }


        public ActionResult ReportExportToPaper(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Round1Report01Model> reportLists = GetListForExport(_model);

            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 25);
            }

            return View("ReportExportToPaper", _model);
        }


        public ActionResult ReportExportToExcel(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            ReportClass rptH = new ReportClass();

            List<Round1Report01Model> reportLists = GetListForExport(model);
            /*Init value*/
            rptH.FileName = Server.MapPath("~/Reports/RptForExamPaper.rpt");
            rptH.SetDataSource(reportLists);


            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            return File(stream, "application/xls", "ReportData.xls");

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public List<Round1Report01Model> GetListForExport(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = new List<Round1Report01Model>();

            int index = 1;
            var items = from ss in db.TB_STUDENT_SEAT
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        where s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, s.TB_STUDENT_SEAT.STUDENT_CODE ascending
                        select new
                        {
                            studentCode = ss.STUDENT_CODE,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            studentLevel = s.TB_M_LEVEL.LEVEL_NAME_TH,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            sitPerfix = ss.SIT_NUMBER_PREFIX,
                            sitNumber = ss.SIT_NUMBER
                        };
            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "";
                rr.studentFullName = item.studentFullName;
                rr.schoolName = item.schoolName;
                rr.studentLevel = item.studentLevel;
                rr.roomNo = item.roomNo + "";
                rr.remark = item.sitPerfix + "" + Convert.ToInt16(item.sitNumber).ToString("00");
                rr.phone = "สอบแข่งขันภาษาจีน";
                reportLists.Add(rr);
                index++;
            }
            return reportLists;
        }

        private int getLastStudentCode(List<TB_STUDENT_SEAT> stdCodes,int level) {
            int result = 0;
            foreach (TB_STUDENT_SEAT code in stdCodes)
            {
                if (Convert.ToInt32(code.STUDENT_CODE)/10000 == level)
                {
                    result = Convert.ToInt32(code.STUDENT_CODE) % 10000;
                }
            }

            return result;
        }

    }
}