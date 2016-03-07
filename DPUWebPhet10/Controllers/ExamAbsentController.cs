using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using PagedList;
namespace DPUWebPhet10.Controllers
{
    public class ExamAbsentController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /ExamAbsent/

        public ActionResult Index(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<TB_APPLICATION_STUDENT> studentLists = db.TB_APPLICATION_STUDENT.Where(s => s.STD_SCHOOL_ID == _model.id).OrderBy(s=>s.STD_ID).ToList();

            if (studentLists != null)
            {
                if (studentLists.Count > 0)
                {
                    if (_model.action.Equals("บันทึก"))
                    {
                        //ล้างข้อมูลเก่าออก
                        foreach (TB_APPLICATION_STUDENT std in studentLists)
                        {
                            TB_EXAM_ABSENT studentSeat = db.TB_EXAM_ABSENT.Where(s => s.STD_CODE == std.TB_STUDENT_SEAT.STUDENT_CODE).FirstOrDefault();
                            if (studentSeat != null)
                            {
                                db.TB_EXAM_ABSENT.Remove(studentSeat);
                            }
                        }
                        //บันทึกเด็กที่ขาดใหม่
                        foreach (String id in _model.SelectedStudentIDs)
                        {
                            int absentId = Convert.ToInt32(id);
                            TB_EXAM_ABSENT tb_exam_absent = new TB_EXAM_ABSENT();
                            tb_exam_absent.STD_CODE = absentId;
                            db.TB_EXAM_ABSENT.Add(tb_exam_absent);
                        }
                        db.SaveChanges();
                    }

                    List<Round1Report01Model> lists = new List<Round1Report01Model>();
                    int seq = 1;
                    foreach (TB_APPLICATION_STUDENT std in studentLists)
                    {
                        if (std.TB_STUDENT_SEAT == null)
                        {
                            ViewBag.ResultMsg = "ยังไม่ได้จัดผังสอบ ไม่สามารถทำรายการได้!";
                            return RedirectToAction("../Management/SchoolList");

                            //return View("Management/SchoolList");
                        }
                        Round1Report01Model rpt = new Round1Report01Model();
                        rpt.seq = seq;
                        rpt.studentCode = std.TB_STUDENT_SEAT.STUDENT_CODE + "";
                        rpt.studentFullName = std.TB_M_TITLE.TITLE_NAME_TH + std.STD_NAME + "  " + std.STD_SURNAME;
                        rpt.schoolName = std.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        rpt.remark = "";
                        seq++;
                        lists.Add(rpt);
                    }

                    List<String> absentStudents = new List<string>();
                    foreach (TB_EXAM_ABSENT exam in db.TB_EXAM_ABSENT.ToList())
                    {
                        absentStudents.Add(exam.STD_CODE + "");
                    }
                    if (lists != null)
                    {
                        var pageIndex = _model.Page ?? 1;
                        _model.reports = lists.ToPagedList(pageIndex, 50);
                        _model.SelectedStudentIDs = absentStudents;
                    }
                }
            }

            return View(_model);
        }
        public ActionResult Index1(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<TB_APPLICATION_STUDENT> studentLists = db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == _model.studentLevel).OrderBy(s=>s.STD_ID).ToList();
            List<TB_M_LEVEL> tbMLevel = db.TB_M_LEVEL.Where(l => l.LEVEL_ID > 0).ToList();
            if (studentLists != null)
            {
                if (studentLists.Count > 0)
                {
                    if (_model.action.Equals("บันทึก"))
                    {
                        //ล้างข้อมูลเก่าออก
                        foreach (TB_APPLICATION_STUDENT std in studentLists)
                        {
                            TB_EXAM_ABSENT studentSeat = db.TB_EXAM_ABSENT.Where(s => s.STD_CODE == std.TB_STUDENT_SEAT.STUDENT_CODE).FirstOrDefault();
                            if (studentSeat != null)
                            {
                                db.TB_EXAM_ABSENT.Remove(studentSeat);
                            }
                        }
                        //บันทึกเด็กที่ขาดใหม่
                        foreach (String id in _model.SelectedStudentIDs)
                        {
                            int absentId = Convert.ToInt32(id);
                            TB_EXAM_ABSENT tb_exam_absent = new TB_EXAM_ABSENT();
                            tb_exam_absent.STD_CODE = absentId;
                            db.TB_EXAM_ABSENT.Add(tb_exam_absent);
                        }
                        db.SaveChanges();
                    }

                    List<Round1Report01Model> lists = new List<Round1Report01Model>();
                    int seq = 1;
                    foreach (TB_APPLICATION_STUDENT std in studentLists)
                    {
                        if (std.TB_STUDENT_SEAT == null)
                        {
                            ViewBag.ResultMsg = "ยังไม่ได้จัดผังสอบ ไม่สามารถทำรายการได้!";
                            
                            ViewBag.StudentLevel = new SelectList(tbMLevel, "LEVEL_ID", "LEVEL_NAME_TH", _model.studentLevel);
                            return View(_model);
                        }

                           
                        Round1Report01Model rpt = new Round1Report01Model();
                        rpt.seq = seq;
                        rpt.studentCode = std.TB_STUDENT_SEAT.STUDENT_CODE + "";
                        rpt.studentFullName = std.TB_M_TITLE.TITLE_NAME_TH + std.STD_NAME + "  " + std.STD_SURNAME;
                        rpt.schoolName = std.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        rpt.remark = "";
                        seq++;
                        lists.Add(rpt);
                    }

                    List<String> absentStudents = new List<string>();
                    foreach (TB_EXAM_ABSENT exam in db.TB_EXAM_ABSENT.ToList())
                    {
                        absentStudents.Add(exam.STD_CODE + "");
                    }
                    if (lists != null)
                    {
                        var pageIndex = _model.Page ?? 1;
                        _model.reports = lists.ToPagedList(pageIndex, 50);
                        _model.SelectedStudentIDs = absentStudents;
                    }
                }
            }
            ViewBag.StudentLevel = new SelectList(tbMLevel, "LEVEL_ID", "LEVEL_NAME_TH", _model.studentLevel);
            return View(_model);
        }
        public ActionResult AbsentList(ReportModelCriteria _model)
        {
            List<TB_M_LEVEL> tbMLevel = db.TB_M_LEVEL.Where(l => l.LEVEL_ID > 0).ToList();
            ViewBag.StudentLevel = new SelectList(tbMLevel, "LEVEL_ID", "LEVEL_NAME_TH");
            int total = (_model.studentLevel == 0) ? db.TB_APPLICATION_STUDENT.ToList().Count : db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == _model.studentLevel).ToList().Count;

            int absentTotal = 0;
            if (_model.studentLevel == 0)
            {
                absentTotal = db.TB_EXAM_ABSENT.ToList().Count;
            }
            else {
                foreach (TB_EXAM_ABSENT stdAbsent in db.TB_EXAM_ABSENT.ToList())
                {
                    List<TB_STUDENT_SEAT> students = db.TB_STUDENT_SEAT.Where(ss => ss.TB_APPLICATION_STUDENT.STD_LEVEL_ID == _model.studentLevel && ss.STUDENT_CODE == stdAbsent.STD_CODE).ToList();
                    absentTotal += students.Count;
                }
            }
            
            List<Report19Model> reportLists = new List<Report19Model>();
            Report19Model rpt = new Report19Model();
            rpt.seq = 1;
            rpt.description = "ทั้งหมด";
            rpt.count = total;
            reportLists.Add(rpt);
            rpt = new Report19Model();
            rpt.seq = 2;
            rpt.description = "ไม่มีสิทธิ์สอบ";
            rpt.count = 0;
            reportLists.Add(rpt);
            rpt = new Report19Model();
            rpt.seq = 3;
            rpt.description = "เข้าสอบ";
            rpt.count = total-absentTotal;
            reportLists.Add(rpt);
            rpt = new Report19Model();
            rpt.seq = 4;
            rpt.description = "ไม่เข้าสอบ";
            rpt.count = absentTotal;
            reportLists.Add(rpt);
            if (reportLists != null)
            { 
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 50);
            }
            return View(_model);
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}