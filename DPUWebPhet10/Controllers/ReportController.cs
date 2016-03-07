using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPUWebPhet10.Models;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using DPUWebPhet10.Utils;
using System.Web.Security;
using PagedList;
using System.Data.Objects;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using System.Text;
using DotNet.Highcharts.Enums;
namespace DPUWebPhet10.Controllers
{
    public class ReportController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();
        private IRepository repository = new DPUPhet10Repository();

        //
        // GET: /Report/
        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            var model = new ReportModelCriteria
            {

            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            Boolean isHaveReportData = false;
            if (model.reportType > 0)
            {
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("~/Reports/Rpt" + model.reportType.ToString("00") + ".rpt");
                //rptH.Load();
                switch (model.reportType)
                {
                    case 4:
                        List<Report04Model> report04Lists = Report04(model.studentLevel, model.roomNo);
                        if (report04Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report04Lists);
                            rptH.SetParameterValue("P_ROUND", "เจียระไนเพชร");


                        }
                        break;
                    case 5:
                        List<Report05Model> report05Lists = Report05(model.studentLevel, model.roomNo);
                        if (report05Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report05Lists);
                        }
                        break;
                    case 8:
                        List<Report08Model> report08Lists = Report08();
                        if (report08Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report08Lists);
                        }
                        break;
                    case 9:
                        List<Report09Model> report09Lists = Report09();
                        if (report09Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report09Lists);
                        }
                        break;
                    case 10:// ใบเซ็นชื่อเข้าห้องสอบ
                        List<Report10Model> report10Lists = Report10(model.roomNo);
                        if (report10Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report10Lists);
                        }
                        break;
                    case 11:
                        List<Report11Model> report11Lists = Report11(model.roomNo);
                        if (report11Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report11Lists);
                        }
                        break;
                    case 12:
                        List<Report11Model> report12Lists = Report12(model.roomNo);
                        if (report12Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report12Lists);
                        }
                        break;
                    case 13:
                        List<Report13Model> report13Lists = Report13();
                        if (report13Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report13Lists);
                        }
                        break;
                    case 14:
                        List<Report14Model> report14Lists = Report14(model.studentLevel);
                        if (report14Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report14Lists);
                        }
                        break;
                    case 15:
                        List<Report15Model> report15Lists = Report15(model.studentLevel);
                        if (report15Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report15Lists);
                        }
                        break;
                    case 19:
                        List<Report19Model> report19Lists = Report19(model.startDate, model.endDate, model.periodIndex);
                        if (report19Lists.Count > 0)
                        {
                            isHaveReportData = true;

                            /*Init value*/
                            rptH.SetDataSource(report19Lists);
                            /**
                             * รายงานสรุปจำนวนรวมของผู้สมัครในแต่ละวัน 
                             * "วันที่สมัคร","ระดับชั้น", "สถานศึกษา", "จังหวัด", "ประเทศ" 
                             */
                            rptH.SetParameterValue("P_REPORT_TYPE", getReport19Type(model.periodIndex));

                            //if (report19Lists != null)
                            //{
                            //    var pageIndex = model.Page ?? 1;
                            //    model.reports = report19Lists.ToPagedList(pageIndex, 10);
                            //}
                        }
                        break;
                    case 20:
                        List<Report19Model> report20Lists = Report20();
                        if (report20Lists.Count > 0)
                        {
                            isHaveReportData = true;
                            /*Init value*/
                            rptH.SetDataSource(report20Lists);

                        }
                        break;
                }

                if (isHaveReportData)
                {
                    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf");
                }
            }
            if (!isHaveReportData)
            {
                ViewBag.ResultMsg = "ไม่พบข้อมูลสำหรับสร้างรายงาน";
            }
            ViewBag.isHaveReportData = isHaveReportData;
            return View(model);

        }

        #region "ROUND 1,2 REPORT"

        public ActionResult Round1Report01(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            if (_model.action.Equals("adjust"))
            {
                List<String> stdIds = _model.SelectedStudentIDs;
                List<String> prizes = _model.SelectedPrizeValueIDs;
                if (stdIds.Count > 0)
                {
                    for (int i = 0; i < stdIds.Count; i++)
                    {
                        int studentCode = Convert.ToInt32(stdIds[i].Replace("*", ""));
                        int prize = Convert.ToInt32(prizes[i]);
                        TB_SCORE_ROUND_1 sr = db.TB_SCORE_ROUND_1.Where(s => s.STD_CODE == studentCode).FirstOrDefault();
                        sr.PRIZE_ID = prize;
                    }
                    //commit
                    db.SaveChanges();
                }


                return RedirectToAction("../Report/Round1Report01");
            }
            else
            {
                if (_model.studentLevel > 0)
                {
                    List<Round1Report01Model> reportLists = null;
                    switch (_model.OrderType)
                    {
                        case 0: //"เลขประจำตัว"
                            reportLists = GetRound1Lists(_model);
                            break;
                        case 1: //"ชื่อสกุล"
                            reportLists = GetRound1ListsByName(_model);
                            break;
                        case 2: //"คะแนนสอบ" 
                            reportLists = GetRound1ListsByScore(_model);
                            break;
                        case 3: //"โรงเรียน"
                            reportLists = GetRound1ListsBySchool(_model);
                            break;
                    }
                    if (reportLists != null)
                    {
                        var pageIndex = _model.Page ?? 1;
                        _model.reports = reportLists.ToPagedList(pageIndex, 150);
                    }
                }
                return View("Round1Report01", _model);
            }
        }

        public ActionResult Round1Report02(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            List<Round1Report01Model> reportLists = GetRound1AllNation(_model);

            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 25);
            }

            return View("Round1Report02", _model);
        }
        public ActionResult Round1Report03(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Round1Report01Model> reportLists = GetRound1Lists(_model);


            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 110);
            }

            return View("Round1Report03", _model);

        }
        public ActionResult Round1Report04(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Round1Report01Model> reportLists = GetRound1Lists(_model);


            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 110);
            }

            return View("Round1Report04", _model);

        }
        public ActionResult Round1Report05(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Round1Report01Model> reportLists = GetRound1All(_model);
            if (_model.action.Equals("adjust"))
            {
                List<String> stdIds = _model.SelectedStudentIDs;
                List<String> prizes = _model.SelectedPrizeValueIDs;
                if (stdIds.Count > 0)
                {
                    for (int i = 0; i < stdIds.Count; i++)
                    {
                        int studentCode = Convert.ToInt32(stdIds[i].Replace("*", ""));
                        int prize = Convert.ToInt32(prizes[i]);
                        TB_SCORE_ROUND_1 sr = db.TB_SCORE_ROUND_1.Where(s => s.STD_CODE == studentCode).FirstOrDefault();
                        sr.PRIZE_ID = prize;
                    }
                    //commit
                    db.SaveChanges();
                }


                return RedirectToAction("../Report/Round1Report01");
            }
            else
            {

                if (reportLists != null)
                {
                    var pageIndex = _model.Page ?? 1;
                    _model.reports = reportLists.ToPagedList(pageIndex, 110);
                }

                return View("Round1Report05", _model);
            }
        }

        public ActionResult Round2Report01(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }


            if (_model.action.Equals("adjust"))
            {
                List<String> stdIds = _model.SelectedStudentIDs;
                List<String> prizes = _model.SelectedPrizeValueIDs;
                if (stdIds.Count > 0)
                {
                    for (int i = 0; i < stdIds.Count; i++)
                    {
                        int studentCode = Convert.ToInt32(stdIds[i].Replace("*", ""));
                        int prize = Convert.ToInt32(prizes[i]);
                        TB_SCORE_ROUND_2 sr = db.TB_SCORE_ROUND_2.Where(s => s.STD_CODE == studentCode).FirstOrDefault();
                        sr.PRIZE_ID = prize;
                    }
                    //commit
                    db.SaveChanges();
                }


                return RedirectToAction("../Report/Round2Report01");
            }
            else
            {
                if (_model.studentLevel > 0)
                {
                    List<Round1Report01Model> reportLists = null;
                    switch (_model.OrderType)
                    {
                        case 0: //"เลขประจำตัว"
                            reportLists = GetRound2Lists1(_model);
                            break;
                        case 1: //"ชื่อสกุล"
                            reportLists = GetRound2Lists1ByName(_model);
                            break;
                        case 2: //"คะแนนสอบ" 
                            reportLists = GetRound2Lists1ByScore(_model);
                            break;
                        case 3: //"โรงเรียน"
                            reportLists = GetRound2Lists1BySchool(_model);
                            break;
                    }

                    if (reportLists != null)
                    {
                        var pageIndex = _model.Page ?? 1;
                        _model.reports = reportLists.ToPagedList(pageIndex, 25);
                    }
                }
                return View("Round2Report01", _model);
            }
        }
        public ActionResult Round2Report02(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Round1Report01Model> reportLists = GetRound4Lists(_model);


            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 110);
            }

            return View("Round2Report02", _model);

        }
        public ActionResult Round2Report03(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (_model.studentLevel > 0)
            {
                List<Round1Report01Model> reportLists = GetRound2Lists1(_model);
                if (reportLists != null)
                {
                    var pageIndex = _model.Page ?? 1;
                    _model.reports = reportLists.ToPagedList(pageIndex, 25);
                }
            }
            return View("Round2Report03", _model);

        }
        public ActionResult Round2Report04(Round1ReportModelCriteria _model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            if (_model.studentLevel > 0)
            {
                List<Round1Report01Model> reportLists = GetRound2Lists1(_model);
                if (reportLists != null)
                {
                    var pageIndex = _model.Page ?? 1;
                    _model.reports = reportLists.ToPagedList(pageIndex, 25);
                }
            }
            return View("Round2Report04", _model);

        }
        public ActionResult Round1ReportExport(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();

            ReportClass rptH = new ReportClass();


            List<Round1Report01Model> reportLists = null;
            switch (model.OrderType)
            {
                case 0: //"เลขประจำตัว"
                    reportLists = GetRound1Lists(model);
                    break;
                case 1: //"ชื่อสกุล"
                    reportLists = GetRound1ListsByName(model);
                    break;
                case 2: //"คะแนนสอบ" 
                    reportLists = GetRound1ListsByScore_Print(model);
                    break;
                case 3: //"โรงเรียน"
                    reportLists = GetRound1ListsBySchool(model);
                    break;
            }
            /*Init value*/
            rptH.FileName = Server.MapPath("~/Reports/RptRound1_01.rpt");
            rptH.SetDataSource(reportLists);
            rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
            rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));

            if (model.exportFormat == 1)
            {
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {

                System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                return File(stream, "application/xls", "ReportData.xls");
            }


        }
        public ActionResult Round1Report05Export(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();

            ReportClass rptH = new ReportClass();


            List<Round1Report01Model> reportLists = GetRound1All(model);
            /*Init value*/
            rptH.FileName = Server.MapPath("~/Reports/RptRound1_01.rpt");
            rptH.SetDataSource(reportLists);
            rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
            rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));

            if (model.exportFormat == 1)
            {
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {

                System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                return File(stream, "application/xls", "ReportData.xls");
            }


        }
        public ActionResult Round1ReportExportSMS(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();

            ReportClass rptH = new ReportClass();


            List<Round1Report01Model> reportLists = GetRound1Lists(model);
            /*Init value*/
            rptH.FileName = Server.MapPath("~/Reports/RptRound1SMS.rpt");
            rptH.SetDataSource(reportLists);


            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            return File(stream, "application/xls", "ExportSMS.xls");
        }
        public ActionResult Round1Report2Export(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();

            ReportClass rptH = new ReportClass();




            if (model.exportFormat == 1)
            {
                List<Round1Report01Model> reportLists = GetRound1AllNation(model);
                /*Init value*/
                rptH.FileName = Server.MapPath("~/Reports/RptRound1_02.rpt");
                rptH.SetDataSource(reportLists);
                rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
                rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));
                //rptH.SetParameterValue("P_ROOM", model.roomNo);
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else if (model.exportFormat == 2)
            {
                //only chiness
                List<Round1Report01Model> reportLists = GetRound1ListsOnlyChiness(model);
                /*Init value*/
                rptH.FileName = Server.MapPath("~/Reports/RptRound1_01_NationCondition.rpt");
                rptH.SetDataSource(reportLists);
                rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
                rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));
                //rptH.SetParameterValue("P_ROOM", model.roomNo);
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }else
            {
                List<Round1Report01Model> reportLists = GetRound1AllNation(model);
                /*Init value*/
                rptH.FileName = Server.MapPath("~/Reports/RptRound1_02.rpt");
                rptH.SetDataSource(reportLists);
                rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
                rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));
                //rptH.SetParameterValue("P_ROOM", model.roomNo);
                System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                return File(stream, "application/xls", "ReportData.xls");
            }

        }
        public ActionResult Round2ReportExport(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            FileStreamResult fsr = null;
            ReportClass rptH = new ReportClass();
            if (model.studentLevel > 0)
            {
                TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();
                List<Round1Report01Model> reportLists = null;
                switch (model.OrderType)
                {
                    case 0: //"เลขประจำตัว"
                        reportLists = GetRound2Lists1(model);
                        break;
                    case 1: //"ชื่อสกุล"
                        reportLists = GetRound2Lists1ByName(model);
                        break;
                    case 2: //"คะแนนสอบ" 
                        reportLists = GetRound2Lists1ByScore(model);
                        break;
                    case 3: //"โรงเรียน"
                        reportLists = GetRound2Lists1BySchool(model);
                        break;
                }

                Stream stream = null;
                switch (model.exportFormat)
                {
                    case 1://PDF ประกาศผลรอบทุดท้าย
                        /*Init value*/
                        rptH.FileName = Server.MapPath("~/Reports/RptRound2.rpt");
                        rptH.SetDataSource(reportLists);
                        rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
                        rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));
                        stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        fsr = File(stream, "application/pdf");
                        break;
                    case 2://EXCEL ประกาศผลรอบทุดท้าย
                        rptH.FileName = Server.MapPath("~/Reports/RptRound2.rpt");
                        rptH.SetDataSource(reportLists);
                        rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
                        rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));
                        stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                        fsr = File(stream, "application/xls", "ReportData.xls");
                        break;
                    case 3://PDF ประกาศผลรอบทุดท้าย
                        /*Init value*/
                        rptH.FileName = Server.MapPath("~/Reports/RptRound2_1.rpt");
                        rptH.SetDataSource(reportLists);
                        rptH.SetParameterValue("P_STDLEVEL", getStudentLevel(model.studentLevel));
                        stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        fsr = File(stream, "application/pdf");
                        break;
                    case 4://Export PDF คะแนนสุนทรพจ
                        /*Init value*/
                        rptH.FileName = Server.MapPath("~/Reports/RptRound2_2.rpt");
                        rptH.SetDataSource(reportLists);
                        rptH.SetParameterValue("P_STDLEVEL", getStudentLevel(model.studentLevel));
                        stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        fsr = File(stream, "application/pdf");
                        break;
                }


                return fsr;

            }
            return null;


        }
        public ActionResult Round2Report02Export(Round1ReportModelCriteria model)
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();

            ReportClass rptH = new ReportClass();


            List<Round1Report01Model> reportLists = GetRound4Lists(model);
            /*Init value*/
            rptH.FileName = Server.MapPath("~/Reports/RptRound1_03.rpt");
            rptH.SetDataSource(reportLists);
            rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
            rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));

            if (model.exportFormat == 1)
            {
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {

                System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                return File(stream, "application/xls", "ReportData.xls");
            }


        }
        public ActionResult summary25Export()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }
            TB_APPLICATION application = db.TB_APPLICATION.FirstOrDefault();

            ReportClass rptH = new ReportClass();

            List<ReportSummary25Model> reportLists = new List<ReportSummary25Model>();

            ReportSummary25Model sum = new ReportSummary25Model();
            sum.levelName = "ประถมศึกษาตอนต้น";
            sum.schoolThai = getschoolByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 1).ToList(), "TH");
            sum.schoolOther = 0;
            sum.studentThai = db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 1).Count(); ;
            sum.studentOther = 0;
            sum.province = getschoolProvinceByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 1).ToList());
            reportLists.Add(sum);
            sum = new ReportSummary25Model();
            sum.levelName = "ประถมศึกษาตอนปลาย";
            sum.schoolThai = getschoolByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 2).ToList(), "TH");
            sum.schoolOther = 0;
            sum.studentThai = db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 2).Count(); ;
            sum.studentOther = 0;
            sum.province = getschoolProvinceByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 2).ToList());
            reportLists.Add(sum);
            sum = new ReportSummary25Model();
            sum.levelName = "มัธยมศึกษาตอนต้น";
            sum.schoolThai = getschoolByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 3).ToList(), "TH"); ;
            sum.schoolOther = 0;
            sum.studentThai = db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 3).Count(); ;
            sum.studentOther = 0;
            sum.province = getschoolProvinceByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 3).ToList());
            reportLists.Add(sum);
            sum = new ReportSummary25Model();
            sum.levelName = "มัธยมศึกษาตอนปลาย";
            sum.schoolThai = getschoolByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 4).ToList(), "TH"); ;
            sum.schoolOther = 0;
            sum.studentThai = db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 4).Count(); ;
            sum.studentOther = 0;
            sum.province = getschoolProvinceByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 4).ToList());
            reportLists.Add(sum);
            sum = new ReportSummary25Model();
            sum.levelName = "อุดมศึกษา";
            sum.schoolThai = getschoolByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 5).ToList(), "TH"); ;
            sum.schoolOther = 0;
            sum.studentThai = db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 5).Count(); ;
            sum.studentOther = 0;
            sum.province = getschoolProvinceByLevelId(db.TB_APPLICATION_STUDENT.Where(s => s.STD_LEVEL_ID == 5).ToList());
            reportLists.Add(sum);




            /*Init value*/
            rptH.FileName = Server.MapPath("~/Reports/AdhocReport_Page25.rpt");
            rptH.SetDataSource(reportLists);
            //rptH.SetParameterValue("P_ROUND", application.PROJECT_ROUND);
            //rptH.SetParameterValue("P_STD_LEVEL", getStudentLevel(model.studentLevel));


            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");


        }
        public List<Round1Report01Model> GetRound1AllNation(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;
            reportLists = new List<Round1Report01Model>();
            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        //where s.STD_NATION != 2
                        orderby s.STD_LEVEL_ID ascending, r.STD_CODE ascending
                        select new
                        {
                            s.STD_NATION,
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true
                        };
            if (model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == model.studentLevel);
            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentCode.Equals(model.searchText));
            }
            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            int index = 1;
            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "" + (item.conernFlag ? "*" : "");
                rr.studentFullName = item.studentFullName + "" + ((item.STD_NATION == 2) ? "*" : "");
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = (item.STD_NATION == 2)? "สัญชาติจีน":  item.remark;
                rr.phone = item.phone;
                rr.roomNo = item.roomNo + "";
                rr.roomId = Convert.ToInt16(item.room_id);
                reportLists.Add(rr);
                index++;
            }


            return reportLists;
        }
        public List<Round1Report01Model> GetRound1All(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;
            reportLists = new List<Round1Report01Model>();
            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        //where s.STD_NATION != 2

                        orderby s.STD_LEVEL_ID ascending, r.STD_CODE ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            namesurname=ss.TB_APPLICATION_STUDENT.STD_NAME+""+ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true
                        };
            if (model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == model.studentLevel);
            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                if (!CommonUtils.isNumber(model.searchText))
                {
                    items = items.Where(x => x.namesurname.Contains(model.searchText) || x.schoolName.Contains(model.searchText));
                }
                else
                {
                    int stdCode = Convert.ToInt32(model.searchText);
                    items = items.Where(x => x.studentCode == stdCode);
                }
            }

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            int index = 1;
            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "" + (item.conernFlag ? "*" : "");
                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.roomNo = item.roomNo + "";
                rr.roomId = Convert.ToInt16(item.room_id);
                reportLists.Add(rr);
                index++;
            }


            return reportLists;
        }

        #region "ORDER ROUND 1"
        public List<Round1Report01Model> GetRound1ListsOnlyChiness(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION == 2 && s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, r.STD_CODE ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true,
                            s.STD_IS_CONCERN
                        };

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            reportLists = new List<Round1Report01Model>();
            int index = 1;

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                //var conc = from cc in db.TB_CONCERN where cc.STD_CODE == item.studentCode select cc;
                //conc.FirstOrDefault();
                //rr.studentCode = item.studentCode + "" + (conc.FirstOrDefault() == null ? "" : "*");
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                //if (rr.prize_id > 0 && rr.prize_id < 4)
                //{
                    reportLists.Add(rr);
                    index++;
                //}


            }

            return reportLists;
        }

        public List<Round1Report01Model> GetRound1Lists(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2 && s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, r.STD_CODE ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true,
                            s.STD_IS_CONCERN
                        };

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            reportLists = new List<Round1Report01Model>();
            int index = 1;

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                //var conc = from cc in db.TB_CONCERN where cc.STD_CODE == item.studentCode select cc;
                //conc.FirstOrDefault();
                //rr.studentCode = item.studentCode + "" + (conc.FirstOrDefault() == null ? "" : "*");
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                if (rr.prize_id > 0 && rr.prize_id < 4)
                {
                    reportLists.Add(rr);
                    index++;
                }


            }

            return reportLists;
        }
        public List<Round1Report01Model> GetRound1ListsByName(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2 && s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, s.STD_NAME ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true,
                            s.STD_IS_CONCERN
                        };

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            reportLists = new List<Round1Report01Model>();
            int index = 1;

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                //var conc = from cc in db.TB_CONCERN where cc.STD_CODE == item.studentCode select cc;
                //conc.FirstOrDefault();
                //rr.studentCode = item.studentCode + "" + (conc.FirstOrDefault() == null ? "" : "*");
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                if (rr.prize_id > 0 && rr.prize_id < 4)
                {
                    reportLists.Add(rr);
                    index++;
                }


            }

            return reportLists;
        }
        public List<Round1Report01Model> GetRound1ListsBySchool(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2 && s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, s.TB_APPLICATION_SCHOOL.SCHOOL_NAME ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true,
                            s.STD_IS_CONCERN
                        };

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            reportLists = new List<Round1Report01Model>();
            int index = 1;

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                //var conc = from cc in db.TB_CONCERN where cc.STD_CODE == item.studentCode select cc;
                //conc.FirstOrDefault();
                //rr.studentCode = item.studentCode + "" + (conc.FirstOrDefault() == null ? "" : "*");
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                if (rr.prize_id > 0 && rr.prize_id < 4)
                {
                    reportLists.Add(rr);
                    index++;
                }


            }

            return reportLists;
        }
        public List<Round1Report01Model> GetRound1ListsByScore(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join p in db.TB_M_PRIZE on r.PRIZE_ID equals p.PRIZE_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2 && s.STD_LEVEL_ID ==model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, r.ROUND_SCORE descending, p.PRIZE_ID ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = p.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true,
                            s.STD_IS_CONCERN
                        };

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            reportLists = new List<Round1Report01Model>();
            int index = 1;

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                //if (rr.prize_id > 0 && rr.prize_id < 4)
                //{
                reportLists.Add(rr);
                //}
                index++;
            }

            return reportLists;
        }
        public List<Round1Report01Model> GetRound1ListsByScore_Print(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join p in db.TB_M_PRIZE on r.PRIZE_ID equals p.PRIZE_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2 && s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, r.ROUND_SCORE descending, p.PRIZE_ID ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = p.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true,
                            s.STD_IS_CONCERN
                        };

            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }
            reportLists = new List<Round1Report01Model>();
            int index = 1;

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                if (rr.prize_id > 0 && rr.prize_id < 4)
                {
                    reportLists.Add(rr);
                    index++;
                }
            }

            return reportLists;
        }
        #endregion

        public List<Round1Report01Model> GetRound4Lists(Round1ReportModelCriteria model)
        {
            List<Round1Report01Model> reportLists = null;

            var items = from ss in db.TB_STUDENT_SEAT
                        join r in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2 && s.STD_LEVEL_ID == model.studentLevel
                        orderby s.STD_LEVEL_ID ascending, r.STD_CODE ascending
                        select new
                        {
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score = r.ROUND_SCORE,
                            phone = s.STD_PHONE,
                            room_id = ss.ROOM_ID,
                            prize_id = r.PRIZE_ID,
                            remark = r.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true
                        };
            reportLists = new List<Round1Report01Model>();
            int index = 1;
            if (model.roomNo > 0)
            {
                items = items.Where(x => x.roomNo == model.roomNo);
            }



            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.roomNo = item.roomNo + "";
                rr.studentCode = item.studentCode + "" + ((item.conernFlag == false) ? "" : "*");
                rr.studentLevel = model.studentLevel + "";
                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round1score = Convert.ToInt16(item.score);
                rr.remark = item.remark;
                rr.phone = item.phone;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                if (rr.prize_id == 1)
                {
                    reportLists.Add(rr);
                    index++;
                }

                
            }


            return reportLists;
        }

        #region "ORDER ROUND 1"
        private List<Round1Report01Model> GetRound2Lists1(Round1ReportModelCriteria model)
        {

            List<Round1Report01Model> reportLists = null;
            int index = 1;



            reportLists = new List<Round1Report01Model>();
            var items = from ss in db.TB_STUDENT_SEAT
                        join r2 in db.TB_SCORE_ROUND_2 on ss.STUDENT_CODE equals r2.STD_CODE
                        join r1 in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r1.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2
                        orderby s.STD_LEVEL_ID ascending, r2.STD_CODE ascending, (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34) descending, r2.ROUND_1_SCORE descending
                        select new
                        {
                            s.STD_NATION,
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            namesurname = ss.TB_APPLICATION_STUDENT.STD_NAME + "" + ss.TB_APPLICATION_STUDENT.STD_SURNAME,

                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score1 = r1.ROUND_SCORE,
                            score2 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),

                            score21 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                            r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                            r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35),

                            score22 = (r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                            r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                            r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),
                            SCORE_1_11 = r2.SCORE_1_11,
                            SCORE_1_12 = r2.SCORE_1_12,
                            SCORE_1_13 = r2.SCORE_1_13,
                            SCORE_1_14 = r2.SCORE_1_14,
                            SCORE_1_15 = r2.SCORE_1_15,

                            SCORE_1_21 = r2.SCORE_1_21,
                            SCORE_1_22 = r2.SCORE_1_22,
                            SCORE_1_23 = r2.SCORE_1_23,
                            SCORE_1_24 = r2.SCORE_1_24,
                            SCORE_1_25 = r2.SCORE_1_25,

                            SCORE_1_31 = r2.SCORE_1_31,
                            SCORE_1_32 = r2.SCORE_1_32,
                            SCORE_1_33 = r2.SCORE_1_33,
                            SCORE_1_34 = r2.SCORE_1_34,
                            SCORE_1_35 = r2.SCORE_1_35,


                            SCORE_2_11 = r2.SCORE_2_11,
                            SCORE_2_12 = r2.SCORE_2_12,
                            SCORE_2_13 = r2.SCORE_2_13,
                            SCORE_2_14 = r2.SCORE_2_14,

                            SCORE_2_21 = r2.SCORE_2_21,
                            SCORE_2_22 = r2.SCORE_2_22,
                            SCORE_2_23 = r2.SCORE_2_23,
                            SCORE_2_24 = r2.SCORE_2_24,

                            SCORE_2_31 = r2.SCORE_2_31,
                            SCORE_2_32 = r2.SCORE_2_32,
                            SCORE_2_33 = r2.SCORE_2_33,
                            SCORE_2_34 = r2.SCORE_2_34,

                            prize_id = r2.PRIZE_ID,
                            remark = r2.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true

                        };
            if (model.stdNation > 0)
            {
                items = items.Where(x => x.STD_NATION == model.stdNation);
            }
            if (model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == model.studentLevel);
            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                if (!CommonUtils.isNumber(model.searchText))
                {
                    items = items.Where(x => x.namesurname.Contains(model.searchText) || x.schoolName.Contains(model.searchText));
                }
                else
                {
                    int stdCode = Convert.ToInt32(model.searchText);
                    items = items.Where(x => x.studentCode == stdCode);
                }
            }

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "" + (item.conernFlag ? "*" : "");
                rr.studentLevel = model.studentLevel + "";
                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
                rr.round1score = Convert.ToInt16(item.score1);
                rr.remark = item.remark;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                rr.rowScore21 = Convert.ToDouble(item.score21);
                rr.rowScore22 = Convert.ToDouble(item.score22);
                rr.score11 = (item.SCORE_1_11 != null) ? item.SCORE_1_11.ToString() : String.Empty;
                rr.score12 = (item.SCORE_1_12 != null) ? item.SCORE_1_12.ToString() : String.Empty;
                rr.score13 = (item.SCORE_1_13 != null) ? item.SCORE_1_13.ToString() : String.Empty;
                rr.score14 = (item.SCORE_1_14 != null) ? item.SCORE_1_14.ToString() : String.Empty;
                rr.score15 = (item.SCORE_1_15 != null) ? item.SCORE_1_15.ToString() : String.Empty;

                rr.score21 = (item.SCORE_1_21 != null) ? item.SCORE_1_21.ToString() : String.Empty;
                rr.score22 = (item.SCORE_1_22 != null) ? item.SCORE_1_22.ToString() : String.Empty;
                rr.score23 = (item.SCORE_1_23 != null) ? item.SCORE_1_23.ToString() : String.Empty;
                rr.score24 = (item.SCORE_1_24 != null) ? item.SCORE_1_24.ToString() : String.Empty;
                rr.score25 = (item.SCORE_1_25 != null) ? item.SCORE_1_25.ToString() : String.Empty;

                rr.score31 = (item.SCORE_1_31 != null) ? item.SCORE_1_31.ToString() : String.Empty;
                rr.score32 = (item.SCORE_1_32 != null) ? item.SCORE_1_32.ToString() : String.Empty;
                rr.score33 = (item.SCORE_1_33 != null) ? item.SCORE_1_33.ToString() : String.Empty;
                rr.score34 = (item.SCORE_1_34 != null) ? item.SCORE_1_34.ToString() : String.Empty;
                rr.score35 = (item.SCORE_1_35 != null) ? item.SCORE_1_35.ToString() : String.Empty;

                rr.score2_11 = (item.SCORE_2_11 != null) ? item.SCORE_2_11.ToString() : String.Empty;
                rr.score2_12 = (item.SCORE_2_12 != null) ? item.SCORE_2_12.ToString() : String.Empty;
                rr.score2_13 = (item.SCORE_2_13 != null) ? item.SCORE_2_13.ToString() : String.Empty;
                rr.score2_14 = (item.SCORE_2_14 != null) ? item.SCORE_2_14.ToString() : String.Empty;

                rr.score2_21 = (item.SCORE_2_21 != null) ? item.SCORE_2_21.ToString() : String.Empty;
                rr.score2_22 = (item.SCORE_2_22 != null) ? item.SCORE_2_22.ToString() : String.Empty;
                rr.score2_23 = (item.SCORE_2_23 != null) ? item.SCORE_2_23.ToString() : String.Empty;
                rr.score2_24 = (item.SCORE_2_24 != null) ? item.SCORE_2_24.ToString() : String.Empty;

                rr.score2_31 = (item.SCORE_2_31 != null) ? item.SCORE_2_31.ToString() : String.Empty;
                rr.score2_32 = (item.SCORE_2_32 != null) ? item.SCORE_2_32.ToString() : String.Empty;
                rr.score2_33 = (item.SCORE_2_33 != null) ? item.SCORE_2_33.ToString() : String.Empty;
                rr.score2_34 = (item.SCORE_2_34 != null) ? item.SCORE_2_34.ToString() : String.Empty;

                reportLists.Add(rr);
                index++;
            }


            //เรียงลำดับคะแนนใหม่กรณีมีคะแนนเท่ากัน
            //reOrderPrize(reportLists);

            //if (reportLists != null)
            //{
            //    if (reportLists.Count == 1)
            //    {
            //        reportLists[0].remark = "รางวัลเหรียญทอง";
            //    }
            //    if (reportLists.Count == 2)
            //    {
            //        reportLists[1].remark = "รางวัลเหรียญเงิน";
            //    }
            //    if (reportLists.Count == 3)
            //    {
            //        reportLists[2].remark = "รางวัลเหรียญทองแดง";
            //    }

            //}
            return reportLists;
        }
        private List<Round1Report01Model> GetRound2Lists1ByName(Round1ReportModelCriteria model)
        {

            List<Round1Report01Model> reportLists = null;
            int index = 1;



            reportLists = new List<Round1Report01Model>();
            var items = from ss in db.TB_STUDENT_SEAT
                        join r2 in db.TB_SCORE_ROUND_2 on ss.STUDENT_CODE equals r2.STD_CODE
                        join r1 in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r1.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2
                        orderby s.STD_LEVEL_ID ascending, s.STD_NAME ascending, (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34) descending, r2.ROUND_1_SCORE descending
                        select new
                        {
                            s.STD_NATION,
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score1 = r1.ROUND_SCORE,
                            score2 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),

                            score21 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                            r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                            r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35),

                            score22 = (r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                            r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                            r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),
                            SCORE_1_11 = r2.SCORE_1_11,
                            SCORE_1_12 = r2.SCORE_1_12,
                            SCORE_1_13 = r2.SCORE_1_13,
                            SCORE_1_14 = r2.SCORE_1_14,
                            SCORE_1_15 = r2.SCORE_1_15,

                            SCORE_1_21 = r2.SCORE_1_21,
                            SCORE_1_22 = r2.SCORE_1_22,
                            SCORE_1_23 = r2.SCORE_1_23,
                            SCORE_1_24 = r2.SCORE_1_24,
                            SCORE_1_25 = r2.SCORE_1_25,

                            SCORE_1_31 = r2.SCORE_1_31,
                            SCORE_1_32 = r2.SCORE_1_32,
                            SCORE_1_33 = r2.SCORE_1_33,
                            SCORE_1_34 = r2.SCORE_1_34,
                            SCORE_1_35 = r2.SCORE_1_35,


                            SCORE_2_11 = r2.SCORE_2_11,
                            SCORE_2_12 = r2.SCORE_2_12,
                            SCORE_2_13 = r2.SCORE_2_13,
                            SCORE_2_14 = r2.SCORE_2_14,

                            SCORE_2_21 = r2.SCORE_2_21,
                            SCORE_2_22 = r2.SCORE_2_22,
                            SCORE_2_23 = r2.SCORE_2_23,
                            SCORE_2_24 = r2.SCORE_2_24,

                            SCORE_2_31 = r2.SCORE_2_31,
                            SCORE_2_32 = r2.SCORE_2_32,
                            SCORE_2_33 = r2.SCORE_2_33,
                            SCORE_2_34 = r2.SCORE_2_34,

                            prize_id = r2.PRIZE_ID,
                            remark = r2.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true

                        };
            if (model.stdNation > 0)
            {
                items = items.Where(x => x.STD_NATION == model.stdNation);
            }
            if (model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == model.studentLevel);
            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentCode.Equals(model.searchText));

            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentFullName.Contains(model.searchText) || x.schoolName.Contains(model.searchText));
            }

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "" + (item.conernFlag ? "*" : "");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
                rr.round1score = Convert.ToInt16(item.score1);
                rr.remark = item.remark;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                rr.rowScore21 = Convert.ToDouble(item.score21);
                rr.rowScore22 = Convert.ToDouble(item.score22);
                rr.score11 = (item.SCORE_1_11 != null) ? item.SCORE_1_11.ToString() : String.Empty;
                rr.score12 = (item.SCORE_1_12 != null) ? item.SCORE_1_12.ToString() : String.Empty;
                rr.score13 = (item.SCORE_1_13 != null) ? item.SCORE_1_13.ToString() : String.Empty;
                rr.score14 = (item.SCORE_1_14 != null) ? item.SCORE_1_14.ToString() : String.Empty;
                rr.score15 = (item.SCORE_1_15 != null) ? item.SCORE_1_15.ToString() : String.Empty;

                rr.score21 = (item.SCORE_1_21 != null) ? item.SCORE_1_21.ToString() : String.Empty;
                rr.score22 = (item.SCORE_1_22 != null) ? item.SCORE_1_22.ToString() : String.Empty;
                rr.score23 = (item.SCORE_1_23 != null) ? item.SCORE_1_23.ToString() : String.Empty;
                rr.score24 = (item.SCORE_1_24 != null) ? item.SCORE_1_24.ToString() : String.Empty;
                rr.score25 = (item.SCORE_1_25 != null) ? item.SCORE_1_25.ToString() : String.Empty;

                rr.score31 = (item.SCORE_1_31 != null) ? item.SCORE_1_31.ToString() : String.Empty;
                rr.score32 = (item.SCORE_1_32 != null) ? item.SCORE_1_32.ToString() : String.Empty;
                rr.score33 = (item.SCORE_1_33 != null) ? item.SCORE_1_33.ToString() : String.Empty;
                rr.score34 = (item.SCORE_1_34 != null) ? item.SCORE_1_34.ToString() : String.Empty;
                rr.score35 = (item.SCORE_1_35 != null) ? item.SCORE_1_35.ToString() : String.Empty;

                rr.score2_11 = (item.SCORE_2_11 != null) ? item.SCORE_2_11.ToString() : String.Empty;
                rr.score2_12 = (item.SCORE_2_12 != null) ? item.SCORE_2_12.ToString() : String.Empty;
                rr.score2_13 = (item.SCORE_2_13 != null) ? item.SCORE_2_13.ToString() : String.Empty;
                rr.score2_14 = (item.SCORE_2_14 != null) ? item.SCORE_2_14.ToString() : String.Empty;

                rr.score2_21 = (item.SCORE_2_21 != null) ? item.SCORE_2_21.ToString() : String.Empty;
                rr.score2_22 = (item.SCORE_2_22 != null) ? item.SCORE_2_22.ToString() : String.Empty;
                rr.score2_23 = (item.SCORE_2_23 != null) ? item.SCORE_2_23.ToString() : String.Empty;
                rr.score2_24 = (item.SCORE_2_24 != null) ? item.SCORE_2_24.ToString() : String.Empty;

                rr.score2_31 = (item.SCORE_2_31 != null) ? item.SCORE_2_31.ToString() : String.Empty;
                rr.score2_32 = (item.SCORE_2_32 != null) ? item.SCORE_2_32.ToString() : String.Empty;
                rr.score2_33 = (item.SCORE_2_33 != null) ? item.SCORE_2_33.ToString() : String.Empty;
                rr.score2_34 = (item.SCORE_2_34 != null) ? item.SCORE_2_34.ToString() : String.Empty;

                reportLists.Add(rr);
                index++;
            }


            //เรียงลำดับคะแนนใหม่กรณีมีคะแนนเท่ากัน
            //reOrderPrize(reportLists);

            //if (reportLists != null)
            //{
            //    if (reportLists.Count == 1)
            //    {
            //        reportLists[0].remark = "รางวัลเหรียญทอง";
            //    }
            //    if (reportLists.Count == 2)
            //    {
            //        reportLists[1].remark = "รางวัลเหรียญเงิน";
            //    }
            //    if (reportLists.Count == 3)
            //    {
            //        reportLists[2].remark = "รางวัลเหรียญทองแดง";
            //    }

            //}
            return reportLists;
        }
        private List<Round1Report01Model> GetRound2Lists1BySchool(Round1ReportModelCriteria model)
        {

            List<Round1Report01Model> reportLists = null;
            int index = 1;



            reportLists = new List<Round1Report01Model>();
            var items = from ss in db.TB_STUDENT_SEAT
                        join r2 in db.TB_SCORE_ROUND_2 on ss.STUDENT_CODE equals r2.STD_CODE
                        join r1 in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r1.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2
                        orderby s.STD_LEVEL_ID ascending, s.TB_APPLICATION_SCHOOL.SCHOOL_NAME ascending, (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34) descending, r2.ROUND_1_SCORE descending
                        select new
                        {
                            s.STD_NATION,
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score1 = r1.ROUND_SCORE,
                            score2 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),

                            score21 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                            r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                            r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35),

                            score22 = (r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                            r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                            r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),
                            SCORE_1_11 = r2.SCORE_1_11,
                            SCORE_1_12 = r2.SCORE_1_12,
                            SCORE_1_13 = r2.SCORE_1_13,
                            SCORE_1_14 = r2.SCORE_1_14,
                            SCORE_1_15 = r2.SCORE_1_15,

                            SCORE_1_21 = r2.SCORE_1_21,
                            SCORE_1_22 = r2.SCORE_1_22,
                            SCORE_1_23 = r2.SCORE_1_23,
                            SCORE_1_24 = r2.SCORE_1_24,
                            SCORE_1_25 = r2.SCORE_1_25,

                            SCORE_1_31 = r2.SCORE_1_31,
                            SCORE_1_32 = r2.SCORE_1_32,
                            SCORE_1_33 = r2.SCORE_1_33,
                            SCORE_1_34 = r2.SCORE_1_34,
                            SCORE_1_35 = r2.SCORE_1_35,


                            SCORE_2_11 = r2.SCORE_2_11,
                            SCORE_2_12 = r2.SCORE_2_12,
                            SCORE_2_13 = r2.SCORE_2_13,
                            SCORE_2_14 = r2.SCORE_2_14,

                            SCORE_2_21 = r2.SCORE_2_21,
                            SCORE_2_22 = r2.SCORE_2_22,
                            SCORE_2_23 = r2.SCORE_2_23,
                            SCORE_2_24 = r2.SCORE_2_24,

                            SCORE_2_31 = r2.SCORE_2_31,
                            SCORE_2_32 = r2.SCORE_2_32,
                            SCORE_2_33 = r2.SCORE_2_33,
                            SCORE_2_34 = r2.SCORE_2_34,

                            prize_id = r2.PRIZE_ID,
                            remark = r2.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true

                        };
            if (model.stdNation > 0)
            {
                items = items.Where(x => x.STD_NATION == model.stdNation);
            }
            if (model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == model.studentLevel);
            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentCode.Equals(model.searchText));

            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentFullName.Contains(model.searchText) || x.schoolName.Contains(model.searchText));
            }

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "" + (item.conernFlag ? "*" : "");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
                rr.round1score = Convert.ToInt16(item.score1);
                rr.remark = item.remark;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                rr.rowScore21 = Convert.ToDouble(item.score21);
                rr.rowScore22 = Convert.ToDouble(item.score22);
                rr.score11 = (item.SCORE_1_11 != null) ? item.SCORE_1_11.ToString() : String.Empty;
                rr.score12 = (item.SCORE_1_12 != null) ? item.SCORE_1_12.ToString() : String.Empty;
                rr.score13 = (item.SCORE_1_13 != null) ? item.SCORE_1_13.ToString() : String.Empty;
                rr.score14 = (item.SCORE_1_14 != null) ? item.SCORE_1_14.ToString() : String.Empty;
                rr.score15 = (item.SCORE_1_15 != null) ? item.SCORE_1_15.ToString() : String.Empty;

                rr.score21 = (item.SCORE_1_21 != null) ? item.SCORE_1_21.ToString() : String.Empty;
                rr.score22 = (item.SCORE_1_22 != null) ? item.SCORE_1_22.ToString() : String.Empty;
                rr.score23 = (item.SCORE_1_23 != null) ? item.SCORE_1_23.ToString() : String.Empty;
                rr.score24 = (item.SCORE_1_24 != null) ? item.SCORE_1_24.ToString() : String.Empty;
                rr.score25 = (item.SCORE_1_25 != null) ? item.SCORE_1_25.ToString() : String.Empty;

                rr.score31 = (item.SCORE_1_31 != null) ? item.SCORE_1_31.ToString() : String.Empty;
                rr.score32 = (item.SCORE_1_32 != null) ? item.SCORE_1_32.ToString() : String.Empty;
                rr.score33 = (item.SCORE_1_33 != null) ? item.SCORE_1_33.ToString() : String.Empty;
                rr.score34 = (item.SCORE_1_34 != null) ? item.SCORE_1_34.ToString() : String.Empty;
                rr.score35 = (item.SCORE_1_35 != null) ? item.SCORE_1_35.ToString() : String.Empty;

                rr.score2_11 = (item.SCORE_2_11 != null) ? item.SCORE_2_11.ToString() : String.Empty;
                rr.score2_12 = (item.SCORE_2_12 != null) ? item.SCORE_2_12.ToString() : String.Empty;
                rr.score2_13 = (item.SCORE_2_13 != null) ? item.SCORE_2_13.ToString() : String.Empty;
                rr.score2_14 = (item.SCORE_2_14 != null) ? item.SCORE_2_14.ToString() : String.Empty;

                rr.score2_21 = (item.SCORE_2_21 != null) ? item.SCORE_2_21.ToString() : String.Empty;
                rr.score2_22 = (item.SCORE_2_22 != null) ? item.SCORE_2_22.ToString() : String.Empty;
                rr.score2_23 = (item.SCORE_2_23 != null) ? item.SCORE_2_23.ToString() : String.Empty;
                rr.score2_24 = (item.SCORE_2_24 != null) ? item.SCORE_2_24.ToString() : String.Empty;

                rr.score2_31 = (item.SCORE_2_31 != null) ? item.SCORE_2_31.ToString() : String.Empty;
                rr.score2_32 = (item.SCORE_2_32 != null) ? item.SCORE_2_32.ToString() : String.Empty;
                rr.score2_33 = (item.SCORE_2_33 != null) ? item.SCORE_2_33.ToString() : String.Empty;
                rr.score2_34 = (item.SCORE_2_34 != null) ? item.SCORE_2_34.ToString() : String.Empty;

                reportLists.Add(rr);
                index++;
            }


            //เรียงลำดับคะแนนใหม่กรณีมีคะแนนเท่ากัน
            //reOrderPrize(reportLists);

            //if (reportLists != null)
            //{
            //    if (reportLists.Count == 1)
            //    {
            //        reportLists[0].remark = "รางวัลเหรียญทอง";
            //    }
            //    if (reportLists.Count == 2)
            //    {
            //        reportLists[1].remark = "รางวัลเหรียญเงิน";
            //    }
            //    if (reportLists.Count == 3)
            //    {
            //        reportLists[2].remark = "รางวัลเหรียญทองแดง";
            //    }

            //}
            return reportLists;
        }
        private List<Round1Report01Model> GetRound2Lists1ByScore(Round1ReportModelCriteria model)
        {

            List<Round1Report01Model> reportLists = null;
            int index = 1;



            reportLists = new List<Round1Report01Model>();
            var items = from ss in db.TB_STUDENT_SEAT
                        join r2 in db.TB_SCORE_ROUND_2 on ss.STUDENT_CODE equals r2.STD_CODE
                        join r1 in db.TB_SCORE_ROUND_1 on ss.STUDENT_CODE equals r1.STD_CODE
                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
                        join c in
                            (from c in db.TB_CONCERN
                             select new
                             {
                                 c.STD_CODE,
                             }) on ss.STUDENT_CODE equals c.STD_CODE into cc
                        from concern in cc.DefaultIfEmpty()
                        where s.STD_NATION != 2
                        orderby s.STD_LEVEL_ID ascending, r2.PRIZE_ID ascending,
                        (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34) descending
                        select new
                        {
                            s.STD_NATION,
                            s.STD_LEVEL_ID,
                            studentCode = ss.STUDENT_CODE,
                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
                            score1 = r1.ROUND_SCORE,
                            score2 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),

                            score21 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
                            r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
                            r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35),

                            score22 = (r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
                            r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
                            r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),
                            SCORE_1_11 = r2.SCORE_1_11,
                            SCORE_1_12 = r2.SCORE_1_12,
                            SCORE_1_13 = r2.SCORE_1_13,
                            SCORE_1_14 = r2.SCORE_1_14,
                            SCORE_1_15 = r2.SCORE_1_15,

                            SCORE_1_21 = r2.SCORE_1_21,
                            SCORE_1_22 = r2.SCORE_1_22,
                            SCORE_1_23 = r2.SCORE_1_23,
                            SCORE_1_24 = r2.SCORE_1_24,
                            SCORE_1_25 = r2.SCORE_1_25,

                            SCORE_1_31 = r2.SCORE_1_31,
                            SCORE_1_32 = r2.SCORE_1_32,
                            SCORE_1_33 = r2.SCORE_1_33,
                            SCORE_1_34 = r2.SCORE_1_34,
                            SCORE_1_35 = r2.SCORE_1_35,


                            SCORE_2_11 = r2.SCORE_2_11,
                            SCORE_2_12 = r2.SCORE_2_12,
                            SCORE_2_13 = r2.SCORE_2_13,
                            SCORE_2_14 = r2.SCORE_2_14,

                            SCORE_2_21 = r2.SCORE_2_21,
                            SCORE_2_22 = r2.SCORE_2_22,
                            SCORE_2_23 = r2.SCORE_2_23,
                            SCORE_2_24 = r2.SCORE_2_24,

                            SCORE_2_31 = r2.SCORE_2_31,
                            SCORE_2_32 = r2.SCORE_2_32,
                            SCORE_2_33 = r2.SCORE_2_33,
                            SCORE_2_34 = r2.SCORE_2_34,

                            prize_id = r2.PRIZE_ID,
                            remark = r2.TB_M_PRIZE.PRIZE_NAME_TH,
                            conernFlag = (concern.STD_CODE == null) ? false : true

                        };
            if (model.stdNation > 0)
            {
                items = items.Where(x => x.STD_NATION == model.stdNation);
            }
            if (model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == model.studentLevel);
            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentCode.Equals(model.searchText));

            }
            if (!String.IsNullOrEmpty(model.searchText))
            {
                items = items.Where(x => x.studentFullName.Contains(model.searchText) || x.schoolName.Contains(model.searchText));
            }

            foreach (var item in items)
            {
                Round1Report01Model rr = new Round1Report01Model();
                rr.seq = index;
                rr.studentCode = item.studentCode + "" + (item.conernFlag ? "*" : "");
                rr.studentLevel = model.studentLevel + "";

                rr.studentFullName = item.studentFullName + "";
                rr.schoolName = item.schoolName;
                rr.province = item.province;
                rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
                rr.round1score = Convert.ToInt16(item.score1);
                rr.remark = item.remark;
                rr.prize_id = Convert.ToInt16(item.prize_id);
                rr.rowScore21 = Convert.ToDouble(item.score21);
                rr.rowScore22 = Convert.ToDouble(item.score22);
                rr.score11 = (item.SCORE_1_11 != null) ? item.SCORE_1_11.ToString() : String.Empty;
                rr.score12 = (item.SCORE_1_12 != null) ? item.SCORE_1_12.ToString() : String.Empty;
                rr.score13 = (item.SCORE_1_13 != null) ? item.SCORE_1_13.ToString() : String.Empty;
                rr.score14 = (item.SCORE_1_14 != null) ? item.SCORE_1_14.ToString() : String.Empty;
                rr.score15 = (item.SCORE_1_15 != null) ? item.SCORE_1_15.ToString() : String.Empty;

                rr.score21 = (item.SCORE_1_21 != null) ? item.SCORE_1_21.ToString() : String.Empty;
                rr.score22 = (item.SCORE_1_22 != null) ? item.SCORE_1_22.ToString() : String.Empty;
                rr.score23 = (item.SCORE_1_23 != null) ? item.SCORE_1_23.ToString() : String.Empty;
                rr.score24 = (item.SCORE_1_24 != null) ? item.SCORE_1_24.ToString() : String.Empty;
                rr.score25 = (item.SCORE_1_25 != null) ? item.SCORE_1_25.ToString() : String.Empty;

                rr.score31 = (item.SCORE_1_31 != null) ? item.SCORE_1_31.ToString() : String.Empty;
                rr.score32 = (item.SCORE_1_32 != null) ? item.SCORE_1_32.ToString() : String.Empty;
                rr.score33 = (item.SCORE_1_33 != null) ? item.SCORE_1_33.ToString() : String.Empty;
                rr.score34 = (item.SCORE_1_34 != null) ? item.SCORE_1_34.ToString() : String.Empty;
                rr.score35 = (item.SCORE_1_35 != null) ? item.SCORE_1_35.ToString() : String.Empty;

                rr.score2_11 = (item.SCORE_2_11 != null) ? item.SCORE_2_11.ToString() : String.Empty;
                rr.score2_12 = (item.SCORE_2_12 != null) ? item.SCORE_2_12.ToString() : String.Empty;
                rr.score2_13 = (item.SCORE_2_13 != null) ? item.SCORE_2_13.ToString() : String.Empty;
                rr.score2_14 = (item.SCORE_2_14 != null) ? item.SCORE_2_14.ToString() : String.Empty;

                rr.score2_21 = (item.SCORE_2_21 != null) ? item.SCORE_2_21.ToString() : String.Empty;
                rr.score2_22 = (item.SCORE_2_22 != null) ? item.SCORE_2_22.ToString() : String.Empty;
                rr.score2_23 = (item.SCORE_2_23 != null) ? item.SCORE_2_23.ToString() : String.Empty;
                rr.score2_24 = (item.SCORE_2_24 != null) ? item.SCORE_2_24.ToString() : String.Empty;

                rr.score2_31 = (item.SCORE_2_31 != null) ? item.SCORE_2_31.ToString() : String.Empty;
                rr.score2_32 = (item.SCORE_2_32 != null) ? item.SCORE_2_32.ToString() : String.Empty;
                rr.score2_33 = (item.SCORE_2_33 != null) ? item.SCORE_2_33.ToString() : String.Empty;
                rr.score2_34 = (item.SCORE_2_34 != null) ? item.SCORE_2_34.ToString() : String.Empty;

                reportLists.Add(rr);
                index++;
            }


            //เรียงลำดับคะแนนใหม่กรณีมีคะแนนเท่ากัน
            //reOrderPrize(reportLists);

            //if (reportLists != null)
            //{
            //    if (reportLists.Count == 1)
            //    {
            //        reportLists[0].remark = "รางวัลเหรียญทอง";
            //    }
            //    if (reportLists.Count == 2)
            //    {
            //        reportLists[1].remark = "รางวัลเหรียญเงิน";
            //    }
            //    if (reportLists.Count == 3)
            //    {
            //        reportLists[2].remark = "รางวัลเหรียญทองแดง";
            //    }

            //}
            return reportLists;
        }
        #endregion

        //private List<Round1Report01Model> GetRound2Lists(Round1ReportModelCriteria model)
        //{

        //    List<Round1Report01Model> reportLists = null;
        //    int index = 1;
        //    if (model.studentLevel == 0)
        //    {

        //        if (!String.IsNullOrWhiteSpace(model.searchText))
        //        {
        //            reportLists = new List<Round1Report01Model>();
        //            var items = from ss in db.TB_STUDENT_SEAT
        //                        join r2 in db.TB_SCORE_ROUND_2 on ss.STUDENT_CODE equals r2.STD_CODE
        //                        join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
        //                        orderby s.STD_LEVEL_ID ascending, (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
        //                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
        //                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
        //                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
        //                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
        //                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34) descending, r2.ROUND_1_SCORE descending
        //                        select new
        //                        {
        //                            studentCode = ss.STUDENT_CODE,
        //                            roomNo = ss.TB_ROOM.ROOM_NUMBER,
        //                            studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
        //                            schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
        //                            province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
        //                            score1 = r2.ROUND_1_SCORE,
        //                            score2 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
        //                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
        //                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
        //                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
        //                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
        //                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),

        //                            SCORE_1_11 = r2.SCORE_1_11,
        //                            SCORE_1_12 = r2.SCORE_1_12,
        //                            SCORE_1_13 = r2.SCORE_1_13,
        //                            SCORE_1_14 = r2.SCORE_1_14,
        //                            SCORE_1_15 = r2.SCORE_1_15,

        //                            SCORE_1_21 = r2.SCORE_1_21,
        //                            SCORE_1_22 = r2.SCORE_1_22,
        //                            SCORE_1_23 = r2.SCORE_1_23,
        //                            SCORE_1_24 = r2.SCORE_1_24,
        //                            SCORE_1_25 = r2.SCORE_1_25,

        //                            SCORE_1_31 = r2.SCORE_1_31,
        //                            SCORE_1_32 = r2.SCORE_1_32,
        //                            SCORE_1_33 = r2.SCORE_1_33,
        //                            SCORE_1_34 = r2.SCORE_1_34,
        //                            SCORE_1_35 = r2.SCORE_1_35,


        //                            SCORE_2_11 = r2.SCORE_2_11,
        //                            SCORE_2_12 = r2.SCORE_2_12,
        //                            SCORE_2_13 = r2.SCORE_2_13,
        //                            SCORE_2_14 = r2.SCORE_2_14,

        //                            SCORE_2_21 = r2.SCORE_2_21,
        //                            SCORE_2_22 = r2.SCORE_2_22,
        //                            SCORE_2_23 = r2.SCORE_2_23,
        //                            SCORE_2_24 = r2.SCORE_2_24,

        //                            SCORE_2_31 = r2.SCORE_2_31,
        //                            SCORE_2_32 = r2.SCORE_2_32,
        //                            SCORE_2_33 = r2.SCORE_2_33,
        //                            SCORE_2_34 = r2.SCORE_2_34,

        //                            prize_id = r2.PRIZE_ID,
        //                            remark = r2.TB_M_PRIZE.PRIZE_NAME_TH
        //                        };
        //            if (CommonUtils.isNumber(model.searchText))
        //            {
        //                int stdCode = Convert.ToInt32(model.searchText);
        //                var condition = from xx in items where xx.studentCode == stdCode select xx;
        //                foreach (var item in condition)
        //                {
        //                    Round1Report01Model rr = new Round1Report01Model();
        //                    rr.seq = index;
        //                    rr.studentCode = item.studentCode + "";
        //                    rr.studentFullName = item.studentFullName;
        //                    rr.schoolName = item.schoolName;
        //                    rr.province = item.province;
        //                    rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
        //                    rr.round1score = Convert.ToInt16(item.score1);
        //                    rr.remark = item.remark;
        //                    rr.prize_id = Convert.ToInt16(item.prize_id);


        //                    rr.score11 = item.SCORE_1_11 + "";
        //                    rr.score12 = item.SCORE_1_12 + "";
        //                    rr.score13 = item.SCORE_1_13 + "";
        //                    rr.score14 = item.SCORE_1_14 + "";
        //                    rr.score15 = item.SCORE_1_15 + "";

        //                    rr.score21 = item.SCORE_1_21 + "";
        //                    rr.score22 = item.SCORE_1_22 + "";
        //                    rr.score23 = item.SCORE_1_23 + "";
        //                    rr.score24 = item.SCORE_1_24 + "";
        //                    rr.score25 = item.SCORE_1_25 + "";

        //                    rr.score31 = item.SCORE_1_31 + "";
        //                    rr.score32 = item.SCORE_1_32 + "";
        //                    rr.score33 = item.SCORE_1_33 + "";
        //                    rr.score34 = item.SCORE_1_34 + "";
        //                    rr.score35 = item.SCORE_1_35 + "";


        //                    rr.score2_11 = item.SCORE_2_11 + "";
        //                    rr.score2_12 = item.SCORE_2_12 + "";
        //                    rr.score2_13 = item.SCORE_2_13 + "";
        //                    rr.score2_14 = item.SCORE_2_14 + "";

        //                    rr.score2_21 = item.SCORE_2_21 + "";
        //                    rr.score2_22 = item.SCORE_2_22 + "";
        //                    rr.score2_23 = item.SCORE_2_23 + "";
        //                    rr.score2_24 = item.SCORE_2_24 + "";

        //                    rr.score2_31 = item.SCORE_2_31 + "";
        //                    rr.score2_32 = item.SCORE_2_32 + "";
        //                    rr.score2_33 = item.SCORE_2_33 + "";
        //                    rr.score2_34 = item.SCORE_2_34 + "";

        //                    reportLists.Add(rr);
        //                    index++;
        //                }
        //            }
        //            else
        //            {
        //                var condition = from xx in items where xx.studentFullName.Contains(model.searchText) || xx.schoolName.Contains(model.searchText) select xx;
        //                foreach (var item in condition)
        //                {
        //                    Round1Report01Model rr = new Round1Report01Model();
        //                    rr.seq = index;
        //                    rr.studentCode = item.studentCode + "";
        //                    rr.studentFullName = item.studentFullName;
        //                    rr.schoolName = item.schoolName;
        //                    rr.province = item.province;
        //                    rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
        //                    rr.round1score = Convert.ToInt16(item.score1);
        //                    rr.remark = item.remark;
        //                    rr.prize_id = Convert.ToInt16(item.prize_id);


        //                    reportLists.Add(rr);
        //                    index++;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        reportLists = new List<Round1Report01Model>();
        //        var items = from ss in db.TB_STUDENT_SEAT
        //                    join r2 in db.TB_SCORE_ROUND_2 on ss.STUDENT_CODE equals r2.STD_CODE
        //                    join s in db.TB_APPLICATION_STUDENT on ss.STUDENT_ID equals s.STD_ID
        //                    where s.STD_LEVEL_ID == model.studentLevel
        //                    orderby s.STD_LEVEL_ID ascending, (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
        //                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
        //                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
        //                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
        //                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
        //                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34) descending, r2.ROUND_1_SCORE descending
        //                    select new
        //                    {
        //                        studentCode = ss.STUDENT_CODE,
        //                        roomNo = ss.TB_ROOM.ROOM_NUMBER,
        //                        studentFullName = ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH + "" + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME,
        //                        schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME,
        //                        province = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME,
        //                        score1 = r2.ROUND_1_SCORE,
        //                        score2 = (r2.SCORE_1_11 + r2.SCORE_1_12 + r2.SCORE_1_13 + r2.SCORE_1_14 + r2.SCORE_1_15 +
        //                                r2.SCORE_1_21 + r2.SCORE_1_22 + r2.SCORE_1_23 + r2.SCORE_1_24 + r2.SCORE_1_25 +
        //                                r2.SCORE_1_31 + r2.SCORE_1_32 + r2.SCORE_1_33 + r2.SCORE_1_34 + r2.SCORE_1_35 +
        //                                r2.SCORE_2_11 + r2.SCORE_2_12 + r2.SCORE_2_13 + r2.SCORE_2_14 +
        //                                r2.SCORE_2_21 + r2.SCORE_2_22 + r2.SCORE_2_23 + r2.SCORE_2_24 +
        //                                r2.SCORE_2_31 + r2.SCORE_2_32 + r2.SCORE_2_33 + r2.SCORE_2_34),

        //                        SCORE_1_11 = r2.SCORE_1_11,
        //                        SCORE_1_12 = r2.SCORE_1_12,
        //                        SCORE_1_13 = r2.SCORE_1_13,
        //                        SCORE_1_14 = r2.SCORE_1_14,
        //                        SCORE_1_15 = r2.SCORE_1_15,

        //                        SCORE_1_21 = r2.SCORE_1_21,
        //                        SCORE_1_22 = r2.SCORE_1_22,
        //                        SCORE_1_23 = r2.SCORE_1_23,
        //                        SCORE_1_24 = r2.SCORE_1_24,
        //                        SCORE_1_25 = r2.SCORE_1_25,

        //                        SCORE_1_31 = r2.SCORE_1_31,
        //                        SCORE_1_32 = r2.SCORE_1_32,
        //                        SCORE_1_33 = r2.SCORE_1_33,
        //                        SCORE_1_34 = r2.SCORE_1_34,
        //                        SCORE_1_35 = r2.SCORE_1_35,

        //                        SCORE_2_11 = r2.SCORE_2_11,
        //                        SCORE_2_12 = r2.SCORE_2_12,
        //                        SCORE_2_13 = r2.SCORE_2_13,
        //                        SCORE_2_14 = r2.SCORE_2_14,

        //                        SCORE_2_21 = r2.SCORE_2_21,
        //                        SCORE_2_22 = r2.SCORE_2_22,
        //                        SCORE_2_23 = r2.SCORE_2_23,
        //                        SCORE_2_24 = r2.SCORE_2_24,

        //                        SCORE_2_31 = r2.SCORE_2_31,
        //                        SCORE_2_32 = r2.SCORE_2_32,
        //                        SCORE_2_33 = r2.SCORE_2_33,
        //                        SCORE_2_34 = r2.SCORE_2_34,

        //                        prize_id = r2.PRIZE_ID,
        //                        remark = r2.TB_M_PRIZE.PRIZE_NAME_TH
        //                    };

        //        if (!String.IsNullOrWhiteSpace(model.searchText))
        //        {
        //            if (CommonUtils.isNumber(model.searchText))
        //            {
        //                int stdCode = Convert.ToInt32(model.searchText);
        //                var condition = from xx in items where xx.studentCode == stdCode select xx;
        //                foreach (var item in condition)
        //                {
        //                    Round1Report01Model rr = new Round1Report01Model();
        //                    rr.seq = index;
        //                    rr.studentCode = item.studentCode + "";
        //                    rr.studentFullName = item.studentFullName;
        //                    rr.schoolName = item.schoolName;
        //                    rr.province = item.province;
        //                    rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
        //                    rr.round1score = Convert.ToInt16(item.score1);
        //                    rr.remark = item.remark;
        //                    rr.prize_id = Convert.ToInt16(item.prize_id);

        //                    rr.score11 = item.SCORE_1_11 + "";
        //                    rr.score12 = item.SCORE_1_12 + "";
        //                    rr.score13 = item.SCORE_1_13 + "";
        //                    rr.score14 = item.SCORE_1_14 + "";
        //                    rr.score15 = item.SCORE_1_15 + "";

        //                    rr.score21 = item.SCORE_1_21 + "";
        //                    rr.score22 = item.SCORE_1_22 + "";
        //                    rr.score23 = item.SCORE_1_23 + "";
        //                    rr.score24 = item.SCORE_1_24 + "";
        //                    rr.score25 = item.SCORE_1_25 + "";

        //                    rr.score31 = item.SCORE_1_31 + "";
        //                    rr.score32 = item.SCORE_1_32 + "";
        //                    rr.score33 = item.SCORE_1_33 + "";
        //                    rr.score34 = item.SCORE_1_34 + "";
        //                    rr.score35 = item.SCORE_1_35 + "";

        //                    rr.score2_11 = item.SCORE_2_11 + "";
        //                    rr.score2_12 = item.SCORE_2_12 + "";
        //                    rr.score2_13 = item.SCORE_2_13 + "";
        //                    rr.score2_14 = item.SCORE_2_14 + "";

        //                    rr.score2_21 = item.SCORE_2_21 + "";
        //                    rr.score2_22 = item.SCORE_2_22 + "";
        //                    rr.score2_23 = item.SCORE_2_23 + "";
        //                    rr.score2_24 = item.SCORE_2_24 + "";

        //                    rr.score2_31 = item.SCORE_2_31 + "";
        //                    rr.score2_32 = item.SCORE_2_32 + "";
        //                    rr.score2_33 = item.SCORE_2_33 + "";
        //                    rr.score2_34 = item.SCORE_2_34 + "";

        //                    reportLists.Add(rr);
        //                    index++;
        //                }
        //            }
        //            else
        //            {
        //                var condition = from xx in items where xx.studentFullName.Contains(model.searchText) || xx.schoolName.Contains(model.searchText) select xx;
        //                foreach (var item in condition)
        //                {
        //                    Round1Report01Model rr = new Round1Report01Model();
        //                    rr.seq = index;
        //                    rr.studentCode = item.studentCode + "";
        //                    rr.studentFullName = item.studentFullName;
        //                    rr.schoolName = item.schoolName;
        //                    rr.province = item.province;
        //                    rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
        //                    rr.round1score = Convert.ToInt16(item.score1);
        //                    rr.remark = item.remark;
        //                    rr.prize_id = Convert.ToInt16(item.prize_id);

        //                    rr.score11 = item.SCORE_1_11 + "";
        //                    rr.score12 = item.SCORE_1_12 + "";
        //                    rr.score13 = item.SCORE_1_13 + "";
        //                    rr.score14 = item.SCORE_1_14 + "";
        //                    rr.score15 = item.SCORE_1_15 + "";

        //                    rr.score21 = item.SCORE_1_21 + "";
        //                    rr.score22 = item.SCORE_1_22 + "";
        //                    rr.score23 = item.SCORE_1_23 + "";
        //                    rr.score24 = item.SCORE_1_24 + "";
        //                    rr.score25 = item.SCORE_1_25 + "";

        //                    rr.score31 = item.SCORE_1_31 + "";
        //                    rr.score32 = item.SCORE_1_32 + "";
        //                    rr.score33 = item.SCORE_1_33 + "";
        //                    rr.score34 = item.SCORE_1_34 + "";
        //                    rr.score35 = item.SCORE_1_35 + "";


        //                    rr.score2_11 = item.SCORE_2_11 + "";
        //                    rr.score2_12 = item.SCORE_2_12 + "";
        //                    rr.score2_13 = item.SCORE_2_13 + "";
        //                    rr.score2_14 = item.SCORE_2_14 + "";

        //                    rr.score2_21 = item.SCORE_2_21 + "";
        //                    rr.score2_22 = item.SCORE_2_22 + "";
        //                    rr.score2_23 = item.SCORE_2_23 + "";
        //                    rr.score2_24 = item.SCORE_2_24 + "";

        //                    rr.score2_31 = item.SCORE_2_31 + "";
        //                    rr.score2_32 = item.SCORE_2_32 + "";
        //                    rr.score2_33 = item.SCORE_2_33 + "";
        //                    rr.score2_34 = item.SCORE_2_34 + "";

        //                    reportLists.Add(rr);
        //                    index++;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in items)
        //            {
        //                Round1Report01Model rr = new Round1Report01Model();
        //                rr.seq = index;
        //                rr.studentCode = item.studentCode + "";
        //                rr.studentFullName = item.studentFullName;
        //                rr.schoolName = item.schoolName;
        //                rr.province = item.province;
        //                rr.round2score = String.Format("{0:0.00}", (Convert.ToDouble(item.score2) / 3));
        //                rr.round1score = Convert.ToInt16(item.score1);
        //                rr.remark = item.remark;
        //                rr.prize_id = Convert.ToInt16(item.prize_id);

        //                rr.score11 = item.SCORE_1_11 + "";
        //                rr.score12 = item.SCORE_1_12 + "";
        //                rr.score13 = item.SCORE_1_13 + "";
        //                rr.score14 = item.SCORE_1_14 + "";
        //                rr.score15 = item.SCORE_1_15 + "";

        //                rr.score21 = item.SCORE_1_21 + "";
        //                rr.score22 = item.SCORE_1_22 + "";
        //                rr.score23 = item.SCORE_1_23 + "";
        //                rr.score24 = item.SCORE_1_24 + "";
        //                rr.score25 = item.SCORE_1_25 + "";

        //                rr.score31 = item.SCORE_1_31 + "";
        //                rr.score32 = item.SCORE_1_32 + "";
        //                rr.score33 = item.SCORE_1_33 + "";
        //                rr.score34 = item.SCORE_1_34 + "";
        //                rr.score35 = item.SCORE_1_35 + "";

        //                rr.score2_11 = item.SCORE_2_11 + "";
        //                rr.score2_12 = item.SCORE_2_12 + "";
        //                rr.score2_13 = item.SCORE_2_13 + "";
        //                rr.score2_14 = item.SCORE_2_14 + "";

        //                rr.score2_21 = item.SCORE_2_21 + "";
        //                rr.score2_22 = item.SCORE_2_22 + "";
        //                rr.score2_23 = item.SCORE_2_23 + "";
        //                rr.score2_24 = item.SCORE_2_24 + "";

        //                rr.score2_31 = item.SCORE_2_31 + "";
        //                rr.score2_32 = item.SCORE_2_32 + "";
        //                rr.score2_33 = item.SCORE_2_33 + "";
        //                rr.score2_34 = item.SCORE_2_34 + "";

        //                reportLists.Add(rr);
        //                index++;
        //            }
        //        }

        //        //เรียงลำดับคะแนนใหม่กรณีมีคะแนนเท่ากัน
        //        //reOrderPrize(reportLists);
        //    }
        //    //if (reportLists != null)
        //    //{
        //    //    if (reportLists.Count == 1)
        //    //    {
        //    //        reportLists[0].remark = "รางวัลเหรียญทอง";
        //    //    }
        //    //    if (reportLists.Count == 2)
        //    //    {
        //    //        reportLists[1].remark = "รางวัลเหรียญเงิน";
        //    //    }
        //    //    if (reportLists.Count == 3)
        //    //    {
        //    //        reportLists[2].remark = "รางวัลเหรียญทองแดง";
        //    //    }

        //    //}
        //    return reportLists;
        //}
        #endregion

        #region "STUDENT REPORT"
        /*
        * พิมพ์เอกสารยืนยันการสมัคร
        * */
        public ActionResult Report01()
        {

            if (Session["Phet10School"] == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("../");
            }

            //ตรวจสอบว่าสารถใช้งานเมนูนี้ได้หรือไม่
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                if (!isAnnounceDate)
                {
                    return View("ReportNotice01");
                }
            }


            CultureInfo ci = new CultureInfo("th");// (CultureInfo)this.Session["PhetCulture"];
            /*
             * DECLARE VARIABLE
             */
            CommonUtils util = new CommonUtils();

            TB_APPLICATION_SCHOOL tb_application_school = (TB_APPLICATION_SCHOOL)Session["Phet10School"];

            List<Report01Model> lists = new List<Report01Model>();

            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_SCHOOL_ID == tb_application_school.SCHOOL_ID select a;
            //var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == tb_application_school.SCHOOL_ID select a;




            if (tb_application_student != null)
            {
                List<TB_APPLICATION_STUDENT> students = tb_application_student.ToList();
                int schId = Convert.ToInt16(students[0].STD_SCHOOL_ID);
                TB_APPLICATION_SCHOOL school = db.TB_APPLICATION_SCHOOL.Where(s => s.SCHOOL_ID == schId).FirstOrDefault();
                foreach (TB_APPLICATION_STUDENT student in students)
                {
                    /*
                     * ที่นั่งสอบ
                     */
                    TB_STUDENT_SEAT studentSeat = db.TB_STUDENT_SEAT.SingleOrDefault(t => t.STUDENT_ID == student.STD_ID);
                    Report01Model report = new Report01Model();
                    report.map = CommonUtils.getByteImage(Convert.ToInt16(student.STD_LEVEL_ID));
                    if (studentSeat != null)
                    {
                        report.p_std_id = studentSeat.STUDENT_CODE + "";
                        report.p_exam_room = studentSeat.TB_ROOM.ROOM_NUMBER + "";
                        report.p_exam_seat = studentSeat.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(studentSeat.SIT_NUMBER).ToString("00");
                        report.p_exam_building = studentSeat.TB_ROOM.ROOM_BUILD + "";
                        report.p_exam_floor = studentSeat.TB_ROOM.ROOM_FLOOR + "";
                    }
                    else
                    {
                        report.p_std_id = "-";
                        report.p_exam_room = "-";
                        report.p_exam_seat = "-";
                        report.p_exam_building = "-";
                        report.p_exam_floor = "-";
                    }
                    /*
                     * บอร์ดที่ประกาศผลสอบ
                     */
                    TB_EXAM_ANOUNCE examAnounce = db.TB_EXAM_ANOUNCE.Where(t => t.ANOUNCE_FOR_LEVEL == student.STD_LEVEL_ID).FirstOrDefault();
                    if (examAnounce != null)
                    {
                        report.p_exam_announce_building = examAnounce.ANOUNCE_BUILDING + "";
                        report.p_exam_announce_board = examAnounce.ANOUNCE_BOARD;
                    }
                    else
                    {
                        report.p_exam_announce_building = "-";
                        report.p_exam_announce_board = "-";
                    }

                    report.p_exam_date = "9 สิงหาคม 2558";
                    report.p_exam_time = "09.00-10.00 น.";

                    report.p_student_name = util.getStudentFullName(student);
                    report.p_school_name = school.SCHOOL_NAME;
                    report.p_school_type = getSchoolType(school.SCHOOL_TYPE);
                    report.p_school_province = school.TB_M_PROVINCE.PROVINCE_NAME;
                    report.p_student_level = student.TB_M_LEVEL.LEVEL_NAME_TH.Split(' ')[0];// +" " + student.STD_GRADE;
                    report.p_student_grade = student.STD_GRADE + "";

                    String phoneLable = (ci.Name.ToUpper().Equals("TH")) ? "เบอร์ติดต่อ  " : "Phone Number  ";

                    var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == school.SCHOOL_ID select a;
                    //APPEND STAFF
                    if (tb_application_staff != null)
                    {
                        List<TB_APPLICATION_STAFF> staffs = tb_application_staff.ToList();

                        if (staffs.Count >= 1)
                        {
                            report.p_staff01_name = util.getStaffFullName(staffs[0]);
                            report.p_staff01_phone = phoneLable + staffs[0].STAFF_PHONE;
                        }
                        if (staffs.Count >= 2)
                        {
                            report.p_staff02_name = util.getStaffFullName(staffs[1]);
                            report.p_staff02_phone = phoneLable + staffs[1].STAFF_PHONE;
                        }
                        if (staffs.Count >= 3)
                        {
                            report.p_staff03_name = util.getStaffFullName(staffs[2]);
                            report.p_staff03_phone = phoneLable + staffs[2].STAFF_PHONE;
                        }
                        if (staffs.Count >= 4)
                        {
                            report.p_staff04_name = util.getStaffFullName(staffs[3]);
                            report.p_staff04_phone = phoneLable + staffs[3].STAFF_PHONE;
                        }
                        if (staffs.Count >= 5)
                        {
                            report.p_staff05_name = util.getStaffFullName(staffs[4]);
                            report.p_staff05_phone = phoneLable + staffs[4].STAFF_PHONE;
                        }
                    }
                    lists.Add(report);
                }
            }


            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Reports/Rpt01_" + ci.Name + ".rpt");
            rptH.Load();
            rptH.SetDataSource(lists);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }


        /*
         * พิมพ์ใบสมัคร
         * */
        public ActionResult Report02()
        {
            if (Session["Phet10School"] == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("../");
            }

            TB_APPLICATION_SCHOOL sessionSchool = (TB_APPLICATION_SCHOOL)Session["Phet10School"];
            int id = Convert.ToInt32(sessionSchool.SCHOOL_ID);
            TB_APPLICATION_SCHOOL tb_application_school = db.TB_APPLICATION_SCHOOL.Single(t => t.SCHOOL_ID == id);

            List<Report02Model> lists = new List<Report02Model>();
            var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == tb_application_school.SCHOOL_ID select a;
            if (tb_application_staff != null)
            {

                List<TB_APPLICATION_STAFF> staffs = tb_application_staff.ToList();
                foreach (TB_APPLICATION_STAFF staff in staffs)
                {

                    Report02Model tmp = new Report02Model();
                    if (tb_application_school.SCHOOL_TYPE == "4")
                    {
                        tmp.data1 = " อาจารย์ผู้ควบคุมดูแลนักศึกษา";
                    }
                    else
                    {
                        tmp.data1 = " ครูผู้ควบคุมดูแลนักเรียน";
                    }
                    tmp.data2 = "";// Resources.Common.Common.RTP003 + " : " + staff.STAFF_FOR_LEVEL;
                    tmp.data3 = staff.TB_M_TITLE.TITLE_NAME_TH + "" + staff.STAFF_NAME + "  " + staff.STAFF_SURNAME;
                    tmp.data4 = "โทร." + " : " + staff.STAFF_PHONE;
                    tmp.data5 = "";
                    lists.Add(tmp);
                }
            }
            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_SCHOOL_ID == tb_application_school.SCHOOL_ID orderby a.STD_LEVEL_ID, a.STD_GRADE select a;
            if (tb_application_student != null)
            {
                List<TB_APPLICATION_STUDENT> students = tb_application_student.ToList();
                int seq = 1;
                int tmpLevel = Convert.ToInt16(students[0].STD_LEVEL_ID);
                foreach (TB_APPLICATION_STUDENT student in students)
                {
                    if (tmpLevel != Convert.ToInt16(student.STD_LEVEL_ID))
                    {
                        tmpLevel = Convert.ToInt16(student.STD_LEVEL_ID);
                        seq = 1;
                    }
                    Report02Model tmp = new Report02Model();
                    if (tb_application_school.SCHOOL_TYPE == "4")
                    {
                        tmp.data1 = "ขอส่งนักศึกษาเข้าแข่งขันภาษาจีนเพชรยอดมงกุฎ ครั้งที่ 12  (นานาชาติ) ระดับอุดมศึกษา ดังนี้";
                    }
                    else
                    {
                        tmp.data1 = "ขอส่งนักเรียนเข้าแข่งขันภาษาจีนเพชรยอดมงกุฎ ครั้งที่ 12  (นานาชาติ) ดังนี้";
                    }
                    tmp.data2 = "ระดับชั้น" + " " + student.TB_M_LEVEL.LEVEL_NAME_TH + "";
                    tmp.data3 = seq + ". " + student.TB_M_TITLE.TITLE_NAME_TH + "" + student.STD_NAME + " " + student.STD_SURNAME;
                    tmp.data4 = student.TB_M_LEVEL.LEVEL_NAME_TH;
                    tmp.data5 = "ปีที่ " + student.STD_GRADE + "  " + "โทร." + student.STD_PHONE;
                    lists.Add(tmp);

                    seq++;


                }
            }

            CultureInfo ci = new CultureInfo("th");

            ReportClass rptH = new ReportClass();
            if (tb_application_school.SCHOOL_TYPE.Equals("4"))
            {
                rptH.FileName = Server.MapPath("~/Reports/Rpt02_" + ci.Name + "_4.rpt");
            }
            else
            {
                rptH.FileName = Server.MapPath("~/Reports/Rpt02_" + ci.Name + ".rpt");
            }
            rptH.Load();
            rptH.SetDataSource(lists);

            rptH.SetParameterValue("P_SCHOOL_ZONE_EDU", (tb_application_school.SCHOOL_ZONE_EDU == null) ? "" : tb_application_school.SCHOOL_ZONE_EDU);
            rptH.SetParameterValue("P_SCHOOL_ZONE", (tb_application_school.SCHOOL_ZONE == null) ? "" : tb_application_school.SCHOOL_ZONE);
            rptH.SetParameterValue("P_SCHOOL_TYPE_OTHER", (tb_application_school.SCHOOL_TYPE_OTHER == null) ? "" : tb_application_school.SCHOOL_TYPE_OTHER);
            rptH.SetParameterValue("P_SCHOOL_TYPE", (tb_application_school.SCHOOL_TYPE == null) ? "" : getSchoolType(tb_application_school.SCHOOL_TYPE) + "  " + ((tb_application_school.SCHOOL_TYPE_OTHER == null) ? "" : tb_application_school.SCHOOL_TYPE_OTHER));
            rptH.SetParameterValue("P_SCHOOL_PROVINCE", (tb_application_school.TB_M_PROVINCE.PROVINCE_NAME == null) ? "" : tb_application_school.TB_M_PROVINCE.PROVINCE_NAME);
            rptH.SetParameterValue("P_SCHOOL_NAME", (tb_application_school.SCHOOL_NAME == null) ? "" : tb_application_school.SCHOOL_NAME);
            rptH.SetParameterValue("P_SCHOOL_EMAIL", (tb_application_school.SCHOOL_EMAIL == null) ? "" : tb_application_school.SCHOOL_EMAIL);
            rptH.SetParameterValue("P_SCHOOL_ADDR_ZIPCODE", (tb_application_school.SCHOOL_ADDR_ZIPCODE == null) ? "" : tb_application_school.SCHOOL_ADDR_ZIPCODE);
            rptH.SetParameterValue("P_SCHOOL_ADDR_TOMBON", (tb_application_school.TB_M_DISTRICT.DISTRICT_NAME == null) ? "" : tb_application_school.TB_M_DISTRICT.DISTRICT_NAME);
            rptH.SetParameterValue("P_SCHOOL_ADDR_SOI", (tb_application_school.SCHOOL_ADDR_SOI == null) ? "" : tb_application_school.SCHOOL_ADDR_SOI);
            rptH.SetParameterValue("P_SCHOOL_ADDR_ROAD", (tb_application_school.SCHOOL_ADDR_ROAD == null) ? "" : tb_application_school.SCHOOL_ADDR_ROAD);
            rptH.SetParameterValue("P_SCHOOL_ADDR_PROVINCE", (tb_application_school.TB_M_PROVINCE.PROVINCE_NAME == null) ? "" : tb_application_school.TB_M_PROVINCE.PROVINCE_NAME);
            rptH.SetParameterValue("P_SCHOOL_ADDR_PHONE", (tb_application_school.SCHOOL_ADDR_PHONE == null) ? "" : tb_application_school.SCHOOL_ADDR_PHONE);
            rptH.SetParameterValue("P_SCHOOL_ADDR_FAX", (tb_application_school.SCHOOL_ADDR_FAX == null) ? "" : tb_application_school.SCHOOL_ADDR_FAX);
            rptH.SetParameterValue("P_SCHOOL_ADDR_AMPHUR", (tb_application_school.TB_M_AMPHUR.AMPHUR_NAME == null) ? "" : tb_application_school.TB_M_AMPHUR.AMPHUR_NAME);
            rptH.SetParameterValue("P_SCHOOL_ADDR", (tb_application_school.SCHOOL_ADDR == null) ? "" : tb_application_school.SCHOOL_ADDR);



            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        /*
         *ใบเซ็นชื่อนักเรียนเข้าสอบแข่งขัน ภาษาจีน เพชรยอดมงกุฏ ครั้งที่ 10 (นานาชาติ)
         */
        public List<Report04Model> Report04(int studentLevel, int roomNo)
        {
            CultureInfo ci = new CultureInfo("th");
            List<Report04Model> lists = new List<Report04Model>();

            //var varRoom = from a in db.TB_ROOM where a.ROOM_FOR_LEVEL == studentLevel && a.ROOM_NUMBER == roomNo orderby a.ROOM_FOR_LEVEL, a.ROOM_NUMBER select a;
            var varRoom = from a in db.TB_ROOM where a.ROOM_FOR_LEVEL == studentLevel orderby a.ROOM_FOR_LEVEL, a.ROOM_NUMBER select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            if (rooms != null)
            {

                int roomIndex = 1;
                foreach (TB_ROOM room in rooms)
                {
                    List<TB_STUDENT_SEAT> studentSeat = room.TB_STUDENT_SEAT.OrderBy(s => s.TB_APPLICATION_STUDENT.STD_LEVEL_ID).OrderBy(s => s.STUDENT_CODE).ToList();
                    foreach (TB_STUDENT_SEAT student in studentSeat)
                    {
                        Report04Model report = new Report04Model();
                        report.seq = roomIndex + ".";
                        report.id = student.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(student.SIT_NUMBER).ToString("00") + "/" + student.STUDENT_CODE + "";
                        report.name = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + student.TB_APPLICATION_STUDENT.STD_NAME + "  " + student.TB_APPLICATION_STUDENT.STD_SURNAME;
                        report.school = student.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        report.roomId = Convert.ToInt16(room.ROOM_ID);
                        report.roomNo = student.TB_ROOM.ROOM_NUMBER + "";
                        report.level = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_ZH);
                        report.grade = student.TB_APPLICATION_STUDENT.STD_GRADE + "";
                        report.sign = "";
                        report.remark = "" + rooms[0].ROOM_SEAT_COUNT;
                        lists.Add(report);
                        roomIndex++;
                    }
                }

            }

            return lists;

        }
        /*
         *ใบลงทะเบียนนักเรียนที่สมัครเข้าร่วมแข่งขัน ภาษาจีน เพชรยอดมงกุฏ ครั้งที่ 10 (นานาชาติ)
         */
        public List<Report05Model> Report05(int studentLevel, int roomNo)
        {
            CultureInfo ci = new CultureInfo("th");
            List<Report05Model> lists = new List<Report05Model>();

            var varRoom = from a in db.TB_ROOM where a.ROOM_FOR_LEVEL == studentLevel && a.ROOM_NUMBER == roomNo orderby a.ROOM_FOR_LEVEL, a.ROOM_NUMBER select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            if (rooms != null)
            {

                int roomIndex = 1;
                foreach (TB_ROOM room in rooms)
                {
                    List<TB_STUDENT_SEAT> studentSeat = room.TB_STUDENT_SEAT.ToList();

                    foreach (TB_STUDENT_SEAT student in studentSeat)
                    {
                        Report05Model report = new Report05Model();
                        report.seq = roomIndex + ".";
                        report.id = student.STUDENT_CODE + "";
                        report.name = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + student.TB_APPLICATION_STUDENT.STD_NAME + "  " + student.TB_APPLICATION_STUDENT.STD_SURNAME;
                        report.school = student.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        report.roomNo = student.TB_ROOM.ROOM_NUMBER + "";
                        report.level = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_ZH);
                        report.grade = student.TB_APPLICATION_STUDENT.STD_GRADE + "";
                        report.exam_building = student.TB_ROOM.ROOM_BUILD + "";
                        report.sign = "";
                        report.remark = "";
                        lists.Add(report);
                        roomIndex++;
                    }
                }

            }
            return lists;
        }

        #endregion

        #region "COMMITTEE REPORT"
        /*
         * รายงานใบเซ็นต์ชื่อกรรมการคุมสอบ
         */
        public List<Report08Model> Report08()
        {
            var varRoom = from a in db.TB_ROOM orderby a.ROOM_ID select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            List<Report08Model> lists = new List<Report08Model>();
            if (rooms != null)
            {

                int seq = 1;
                foreach (TB_ROOM room in rooms)
                {
                    List<TB_COMMITEE_ROOM> crs = room.TB_COMMITEE_ROOM.ToList();
                    //        List<TB_STUDENT_SEAT> studentSeat = db.TB_STUDENT_SEAT.Where(t => t.ROOM_ID == room.ROOM_ID).ToList();
                    foreach (TB_COMMITEE_ROOM cr in crs)
                    {
                        Report08Model tmp = new Report08Model();
                        tmp.seq = seq + ".";
                        tmp.room = Convert.ToInt16(cr.TB_ROOM.ROOM_NUMBER);
                        tmp.fullName = "คุณ" + cr.TB_COMMITEE.COMMITEE_NAME + "  " + cr.TB_COMMITEE.COMMITEE_SURNAME;
                        lists.Add(tmp);
                        seq++;
                    }
                    seq = 1;
                }

            }

            return lists;
        }

        /*
         * รายงานใบเซ็นต์ชื่อกรรมการคุมสอบ (แจ้งจำนวนผู้เข้าสอบ)
         */
        public List<Report09Model> Report09()
        {
            var varRoom = from a in db.TB_ROOM orderby a.ROOM_ID select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            List<Report09Model> lists = new List<Report09Model>();
            if (rooms != null)
            {

                int seq = 1;

                foreach (TB_ROOM room in rooms)
                {
                    List<TB_COMMITEE_ROOM> crs = room.TB_COMMITEE_ROOM.ToList();
                    foreach (TB_COMMITEE_ROOM cr in crs)
                    {
                        Report09Model tmp = new Report09Model();
                        tmp.seq = seq + ".";
                        tmp.room = Convert.ToInt16(cr.TB_ROOM.ROOM_NUMBER);
                        tmp.fullName = "คุณ" + cr.TB_COMMITEE.COMMITEE_NAME + "  " + cr.TB_COMMITEE.COMMITEE_SURNAME;
                        lists.Add(tmp);
                        seq++;
                    }
                    seq = 1;
                }
            }
            return lists;
        }
        /*
         * รายงานใบเซ็นต์ชื่อเข้าผู้เข้าสอบ และติดหน้าห้องสอบ
         */
        public List<Report10Model> Report10(int roomNo)
        {
            CultureInfo ci = new CultureInfo("th");
            var varRoom = from a in db.TB_ROOM orderby a.ROOM_ID where a.ROOM_NUMBER == roomNo select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            List<Report10Model> lists = new List<Report10Model>();
            if (rooms != null)
            {
                int seq = 1;
                foreach (TB_ROOM room in rooms)
                {
                    List<TB_STUDENT_SEAT> sss = room.TB_STUDENT_SEAT.ToList();
                    foreach (TB_STUDENT_SEAT ss in sss)
                    {
                        Report10Model tmp = new Report10Model();
                        tmp.seq = seq + ".";
                        tmp.roomNo = room.ROOM_NUMBER + "";
                        tmp.seatCount = Convert.ToInt16(room.ROOM_SEAT_COUNT);
                        tmp.seat = ss.SIT_NUMBER_PREFIX + "" + ss.SIT_NUMBER + " / " + ss.STUDENT_CODE;
                        tmp.stdcode = "";
                        tmp.fullName = ((ci.Name.Equals("th")) ? ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME;
                        tmp.schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        tmp.provinceName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME;
                        lists.Add(tmp);
                        seq++;
                    }
                    seq = 1;
                }
            }
            return lists;
        }
        /*
          * รายงานผังที่นั่งของผู้เข้าสอบ สำหรับติดหน้าห้องสอบ
          */
        public List<Report11Model> Report11(int roomNo)
        {

            CultureInfo ci = new CultureInfo("th");

            var varRoom = from a in db.TB_ROOM orderby a.ROOM_ID where a.ROOM_NUMBER == roomNo select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            List<Report11Model> lists = new List<Report11Model>();
            if (rooms != null)
            {
                int seq = 1;
                foreach (TB_ROOM room in rooms)
                {
                    List<TB_STUDENT_SEAT> sss = room.TB_STUDENT_SEAT.ToList();
                    foreach (TB_STUDENT_SEAT ss in sss)
                    {
                        Report11Model tmp = new Report11Model();
                        tmp.seq = seq + ".";
                        tmp.roomNo = room.ROOM_NUMBER + "";
                        tmp.seatCount = Convert.ToInt16(room.ROOM_SEAT_COUNT);
                        tmp.seat = ss.SIT_NUMBER_PREFIX + "" + ss.SIT_NUMBER + " / " + ss.STUDENT_CODE;
                        tmp.stdcode = "";
                        tmp.fullName = ((ci.Name.Equals("th")) ? ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME;
                        tmp.schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        tmp.provinceName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME;
                        lists.Add(tmp);
                        seq++;
                    }
                    seq = 1;
                }
            }
            return lists;
        }
        /*
           * รายงานรายชื่อกรรมการคุมสอบ ระบุตัวบุคคล
           */
        public List<Report11Model> Report12(int roomNo)
        {
            CultureInfo ci = new CultureInfo("th");

            var varRoom = from a in db.TB_ROOM orderby a.ROOM_ID where a.ROOM_NUMBER == roomNo select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            List<Report11Model> lists = new List<Report11Model>();
            if (rooms != null)
            {
                int seq = 1;
                foreach (TB_ROOM room in rooms)
                {
                    List<TB_STUDENT_SEAT> sss = room.TB_STUDENT_SEAT.ToList();
                    foreach (TB_STUDENT_SEAT ss in sss)
                    {
                        Report11Model tmp = new Report11Model();
                        tmp.seq = seq + ".";
                        tmp.roomNo = room.ROOM_NUMBER + "";
                        tmp.seatCount = Convert.ToInt16(room.ROOM_SEAT_COUNT);
                        tmp.seat = ss.SIT_NUMBER_PREFIX + "" + ss.SIT_NUMBER + " / " + ss.STUDENT_CODE;
                        tmp.stdcode = "";
                        tmp.fullName = ((ci.Name.Equals("th")) ? ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : ss.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + ss.TB_APPLICATION_STUDENT.STD_NAME + "  " + ss.TB_APPLICATION_STUDENT.STD_SURNAME;
                        tmp.schoolName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                        tmp.provinceName = ss.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME;
                        lists.Add(tmp);
                        seq++;
                    }
                    seq = 1;
                }
            }
            return lists;
        }
        /*
           * เอกสารแปะหน้าซองข้อสอบ
           */
        public List<Report13Model> Report13()
        {

            CultureInfo ci = new CultureInfo("th");
            var varRoom = from a in db.TB_ROOM orderby a.ROOM_FOR_LEVEL, a.ROOM_ID select a;
            List<TB_ROOM> rooms = varRoom.ToList();
            List<Report13Model> lists = new List<Report13Model>();
            if (rooms != null)
            {
                int seq = 1;
                foreach (TB_ROOM room in rooms)
                {
                    Report13Model tmp = new Report13Model();
                    tmp.level = ((ci.Name.Equals("th")) ? room.TB_M_LEVEL.LEVEL_NAME_TH : (ci.Name.Equals("en")) ? room.TB_M_LEVEL.LEVEL_NAME_EN : room.TB_M_LEVEL.LEVEL_NAME_ZH);
                    tmp.roomNo = Convert.ToInt16(room.ROOM_NUMBER);
                    tmp.count = room.TB_STUDENT_SEAT.Count;
                    lists.Add(tmp);
                    seq++;

                    seq = 1;
                }
            }
            return lists;
        }
        /*
           * เอกสารแปะหน้าซองข้อสอบ
           */
        public List<Report14Model> Report14(int studentLevel)
        {
            CultureInfo ci = new CultureInfo("th");
            var varStudent = from a in db.TB_STUDENT_SEAT orderby a.STUDENT_ID, a.ROOM_ID orderby a.STUDENT_ID, a.ROOM_ID where a.TB_APPLICATION_STUDENT.STD_LEVEL_ID == studentLevel select a;

            List<Report14Model> lists = new List<Report14Model>();
            List<TB_STUDENT_SEAT> studentSeats = varStudent.ToList();

            if (studentSeats != null)
            {
                int rowIndex = 1;
                foreach (TB_STUDENT_SEAT student in studentSeats)
                {
                    Report14Model report = new Report14Model();
                    report.seq = rowIndex + ".";
                    report.p_std_id = student.SIT_NUMBER + "";
                    report.p_student_level = student.TB_APPLICATION_STUDENT.STD_LEVEL_ID + "";
                    report.levelDescription = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_ZH);
                    report.p_student_name = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + student.TB_APPLICATION_STUDENT.STD_NAME + "  " + student.TB_APPLICATION_STUDENT.STD_SURNAME;
                    report.p_school_name = student.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                    report.p_school_province = student.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME;
                    lists.Add(report);
                    rowIndex++;
                }
            }
            return lists;
        }
        #endregion

        /*
           * ผังเลขที่นั่งสอบแข่งขัน
           */
        public List<Report15Model> Report15(int studentLevel)
        {
            CultureInfo ci = new CultureInfo("th");


            //var varRoom = from a in db.TB_ROOM where a.ROOM_FOR_LEVEL == level orderby a.ROOM_FOR_LEVEL, a.ROOM_NUMBER select a;
            var varRoom = from a in db.TB_ROOM orderby a.ROOM_FOR_LEVEL, a.ROOM_NUMBER select a;

            List<Report15Model> lists = new List<Report15Model>();
            List<TB_ROOM> rooms = varRoom.ToList();
            if (rooms != null)
            {
                int rowIndex = 1;
                foreach (TB_ROOM room in rooms)
                {
                    Report15Model report = new Report15Model();
                    report.seq = rowIndex + ".";
                    report.level = room.ROOM_FOR_LEVEL + "";
                    report.levelDescription = ((ci.Name.Equals("th")) ? room.TB_M_LEVEL.LEVEL_NAME_TH : (ci.Name.Equals("en")) ? room.TB_M_LEVEL.LEVEL_NAME_EN : room.TB_M_LEVEL.LEVEL_NAME_ZH);
                    report.examRoom = Convert.ToInt16(room.ROOM_NUMBER);

                    List<TB_STUDENT_SEAT> studentSeats = room.TB_STUDENT_SEAT.OrderBy(t => t.STUDENT_ID).ToList();
                    if (studentSeats.Count != 0)
                    {
                        report.seat = String.Format("{0}  -  {1}", studentSeats[0].STUDENT_CODE, studentSeats[studentSeats.Count - 1].STUDENT_CODE);
                    }
                    else
                    {
                        report.seat = "";
                    }
                    report.examStudentCount = studentSeats.Count;
                    report.building = Convert.ToInt16(room.ROOM_BUILD);
                    lists.Add(report);
                    rowIndex++;
                }

            }

            return lists;
        }


        public List<Report19Model> Report19(String _startDate, String _endDate, int periodIndex)
        {
            /**
             * รายงานสรุปจำนวนรวมของผู้สมัครในแต่ละในแต่ละวัน 
             * "วันที่สมัคร","ระดับชั้น", "สถานศึกษา", "จังหวัด", "ประเทศ" 
             */

            List<Report19Model> lists = new List<Report19Model>();
            int seq = 1;
            switch (periodIndex)
            {
                case 1:
                    List<Report19Model> tmpList = new List<Report19Model>();


                    var items = from p in db.TB_APPLICATION_SCHOOL
                                orderby p.SCHOOL_REGISTER_DATE ascending
                                group p by EntityFunctions.TruncateTime(p.SCHOOL_REGISTER_DATE)
                                    into g
                                    select new
                                    {
                                        registerDate = g.Key,
                                        amount = g.Sum(p => p.TB_APPLICATION_STUDENT.Count)
                                    };


                    StringBuilder sb = new StringBuilder();
                    foreach (var data in items)
                    {
                        Report19Model report = new Report19Model();
                        report.seq = seq;
                        report.description = Convert.ToDateTime(data.registerDate).ToString("dd/MM/yyyy", new CultureInfo("en-US")) + "";
                        report.date = Convert.ToInt32(Convert.ToDateTime(data.registerDate).ToString("yyyyMMdd", new CultureInfo("en-US")) + "");
                        report.count = data.amount;
                        tmpList.Add(report);
                        seq++;
                        sb.Append(data.registerDate);
                        sb.Append(",");
                    }
                    List<Report19Model> tmp = tmpList.OrderBy(o => o.date).ToList();
                    int tmpSeq = 1;
                    foreach (Report19Model rpt in tmp)
                    {
                        rpt.seq = tmpSeq;
                        tmpSeq++;
                    }
                    lists = tmp;
                    break;
                case 2:
                    List<TB_M_LEVEL> levelList = db.TB_M_LEVEL.Where(l => l.LEVEL_ID != 0).ToList();
                    if (levelList != null)
                    {
                        foreach (TB_M_LEVEL level in levelList)
                        {
                            Report19Model report = new Report19Model();
                            report.seq = seq;
                            report.description = level.LEVEL_NAME_TH;
                            report.count = level.TB_APPLICATION_STUDENT.Count;
                            lists.Add(report);

                            seq++;
                        }
                    }
                    break;
                case 3:
                    List<TB_APPLICATION_SCHOOL> schoolList = db.TB_APPLICATION_SCHOOL.ToList();
                    if (schoolList != null)
                    {
                        foreach (TB_APPLICATION_SCHOOL school in schoolList)
                        {
                            Report19Model report = new Report19Model();
                            report.seq = seq;
                            report.description = school.SCHOOL_NAME;
                            report.count = school.TB_APPLICATION_STUDENT.Count;
                            lists.Add(report);

                            seq++;
                        }
                    }
                    break;
                case 4:

                    List<Report19Model> tmpProvinceList = new List<Report19Model>();
                    var myQuery1 = from p in db.TB_APPLICATION_SCHOOL
                                   orderby p.SCHOOL_REGISTER_DATE descending
                                   group p by p.TB_M_PROVINCE.PROVINCE_NAME into g
                                   select new { province = g.Key, amount = g.Sum(p => p.TB_APPLICATION_STUDENT.Count) };
                    foreach (var data in myQuery1)
                    {
                        Report19Model report = new Report19Model();
                        report.seq = seq;
                        report.description = data.province;
                        report.count = data.amount;
                        tmpProvinceList.Add(report);
                        seq++;
                    }
                    List<Report19Model> tmp1 = tmpProvinceList.OrderBy(o => o.description).ToList();
                    int tmpSeq1 = 1;
                    foreach (Report19Model rpt in tmp1)
                    {
                        rpt.seq = tmpSeq1;
                        tmpSeq1++;
                    }
                    lists = tmp1;

                    break;
                case 5:
                    break;
            }



            return lists;
        }


        public List<Report19Model> Report20()
        {
            int seq = 1;
            List<Report19Model> list = new List<Report19Model>();

            List<TB_APPLICATION_SCHOOL> schoolList = db.TB_APPLICATION_SCHOOL.ToList();
            foreach (TB_APPLICATION_SCHOOL data in schoolList)
            {
                Report19Model report = new Report19Model();
                report.seq = seq;
                String msg = ((data.TB_APPLICATION_STUDENT.Count == 0) ? "นักเรียน," : "") + ((data.TB_APPLICATION_STAFF.Count == 0) ? "ผู้ควบคุม," : "");
                report.description = data.SCHOOL_NAME + "  " + (!String.IsNullOrEmpty(msg) ? "ไม่มีข้อมูล(" + msg + ")" : "");
                if (!String.IsNullOrEmpty(msg))
                {
                    list.Add(report);
                    seq++;
                }
            }
            return list;
        }

        #region "SCREEN ANNOUNCE"

        /*
        *ตรวจสอบรายชื่อเด็กที่ลงทะเบียน
        */
        public ActionResult Report03(Report03ModelCriteria _model)
        {

            var items = from s in db.TB_APPLICATION_STUDENT
                        join t in db.TB_M_TITLE on s.STD_TITLE_ID equals t.TITLE_ID
                        join sh in db.TB_APPLICATION_SCHOOL on s.STD_SCHOOL_ID equals sh.SCHOOL_ID
                        join p in db.TB_M_PROVINCE on sh.SCHOOL_PROVINCE equals p.PROVINCE_ID
                        select new
                        {
                            t.TITLE_NAME_TH,
                            s.STD_LEVEL_ID,
                            s.STD_NAME,
                            s.STD_SURNAME,
                            sh.SCHOOL_NAME,
                            p.PROVINCE_NAME,
                            sh.SCHOOL_TYPE,
                            sh.SCHOOL_TYPE_OTHER
                        };
            if (_model.studentLevel > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == _model.studentLevel);
            }
            if (!String.IsNullOrWhiteSpace(_model.schoolName))
            {
                items = items.Where(x => x.SCHOOL_NAME.Contains(_model.schoolName));
            }


            List<Report03Model> reportLists = new List<Report03Model>();

            int seq = 1;
            foreach (var item in items)
            {

                Report03Model tmp = new Report03Model();
                tmp.seq = seq + ".";
                tmp.fullName = item.TITLE_NAME_TH + "" + item.STD_NAME + "  " + item.STD_SURNAME;
                tmp.school = item.SCHOOL_NAME;
                tmp.province = item.PROVINCE_NAME;
                tmp.schoolType = item.SCHOOL_TYPE.Equals("5") ? item.SCHOOL_TYPE_OTHER : getSchoolType(item.SCHOOL_TYPE);
                reportLists.Add(tmp);
                seq++;
            }


            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 15);
                ViewBag.PageContent = "ตรวจสอบรายชื่อผู้สมัคร";
            }

            return View(_model);
        }

        /*
         *ประกาศเลขที่นั่งสอบ
         */
        public ActionResult Report06(Report06ModelCriteria _model)
        {
            //ตรวจสอบว่าสารถใช้งานเมนูนี้ได้หรือไม่
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                if (!isAnnounceDate)
                {
                    return View("ReportNotice03");
                }
            }

            var items = from st in db.TB_STUDENT_SEAT
                        join s in db.TB_APPLICATION_STUDENT on st.STUDENT_ID equals s.STD_ID
                        join lv in db.TB_M_LEVEL on s.STD_LEVEL_ID equals lv.LEVEL_ID
                        join r in db.TB_ROOM on st.ROOM_ID equals r.ROOM_ID
                        join b in db.TB_M_BUILDING on r.ROOM_BUILD equals b.BUILDING_ID
                        join t in db.TB_M_TITLE on s.STD_TITLE_ID equals t.TITLE_ID
                        join sh in db.TB_APPLICATION_SCHOOL on s.STD_SCHOOL_ID equals sh.SCHOOL_ID
                        orderby st.STUDENT_ID, st.ROOM_ID, st.STUDENT_ID, st.ROOM_ID
                        select new
                        {
                            t.TITLE_NAME_TH,
                            s.STD_LEVEL_ID,
                            s.STD_NAME,
                            s.STD_SURNAME,
                            sh.SCHOOL_NAME,
                            st.SIT_NUMBER_PREFIX,
                            st.SIT_NUMBER,
                            r.ROOM_NUMBER,
                            lv.LEVEL_NAME_TH,
                            s.STD_GRADE,
                            b.BUILDING_NAME,
                            st.STUDENT_CODE


                        };
            if (_model.levelId > 0)
            {
                items = items.Where(x => x.STD_LEVEL_ID == _model.levelId);
            }
            if (!String.IsNullOrWhiteSpace(_model.schoolName))
            {
                items = items.Where(x => x.SCHOOL_NAME.Contains(_model.schoolName));
            }

            List<Report06Model> reportLists = new List<Report06Model>();

            int rowIndex = 1;
            foreach (var item in items)
            {
                Report06Model report = new Report06Model();
                report.seq = rowIndex + ".";
                report.id = item.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(item.SIT_NUMBER).ToString("00");
                report.name = item.TITLE_NAME_TH + item.STD_NAME + "  " + item.STD_SURNAME;
                report.school = item.SCHOOL_NAME;
                report.roomNo = item.ROOM_NUMBER + "";
                report.level = item.LEVEL_NAME_TH;
                report.grade = item.STD_GRADE + "";
                report.exam_building = item.BUILDING_NAME + "";
                report.sign = "";
                report.remark = item.STUDENT_CODE + "";
                reportLists.Add(report);
                rowIndex++;
            }

            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 15);
                ViewBag.PageContent = "ประกาศเลขที่นั่งสอบ";
            }
            return View(_model);
        }

        /*
         *ประกาศห้องสอบ
         */
        public ActionResult Report07(Report07ModelCriteria _model)
        {

            //ตรวจสอบว่าสารถใช้งานเมนูนี้ได้หรือไม่
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                if (!isAnnounceDate)
                {
                    return View("ReportNotice03");
                }
            }

            var items = from r in db.TB_ROOM
                        join lv in db.TB_M_LEVEL on r.ROOM_FOR_LEVEL equals lv.LEVEL_ID
                        join b in db.TB_M_BUILDING on r.ROOM_BUILD equals b.BUILDING_ID
                        orderby r.ROOM_FOR_LEVEL, r.ROOM_NUMBER
                        select new
                        {
                            r.ROOM_ID,
                            r.ROOM_FOR_LEVEL,
                            r.ROOM_NUMBER,
                            r.ROOM_FLOOR,
                            lv.LEVEL_NAME_TH,
                            b.BUILDING_NAME,

                        };


            if (_model.level > 0)
            {
                items = items.Where(x => x.ROOM_FOR_LEVEL == _model.level);
            }


            List<Report07Model> reports = new List<Report07Model>();
            List<TB_STUDENT_SEAT> studentSeats = db.TB_STUDENT_SEAT.OrderBy(t => t.STUDENT_ID).ToList();
            int rowIndex = 1;
            foreach (var item in items)
            {
                Report07Model report07 = new Report07Model();
                report07.seq = rowIndex + ".";
                report07.exam_room = item.ROOM_NUMBER + "";
                report07.floor = Convert.ToInt16(item.ROOM_FLOOR);
                report07.forLevel = item.LEVEL_NAME_TH;
                List<TB_STUDENT_SEAT> ss = studentSeats.Where(x => x.ROOM_ID == item.ROOM_ID).ToList();
                if (ss != null && ss.Count > 0)
                {
                    report07.seatNumber = String.Format("{0}  -  {1}", ss[0].STUDENT_CODE, ss[ss.Count - 1].STUDENT_CODE);
                }
                report07.exam_building = item.BUILDING_NAME + "";
                report07.seatCount = ss.Count;
                reports.Add(report07);
                rowIndex++;
            }



            if (reports != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reports.ToPagedList(pageIndex, 15);
                ViewBag.PageContent = "ประกาศห้องสอบ";
            }

            return View(_model);
        }
        /*
         * ค้นหาเลขที่นั่งสอบ
         */
        public ActionResult Report07_1(Report07_1ModelCriteria _model)
        {
            //ตรวจสอบว่าสารถใช้งานเมนูนี้ได้หรือไม่
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                if (!isAnnounceDate)
                {
                    return View("ReportNotice03");
                }
            }

            List<TB_STUDENT_SEAT> studentSeats = null;

            //SEARCH CONDITION

            var varStudent = from a in db.TB_STUDENT_SEAT orderby a.STUDENT_ID, a.ROOM_ID orderby a.STUDENT_ID, a.ROOM_ID select a;
            if (!String.IsNullOrWhiteSpace(_model.searchText))
            {
                if (studentSeats == null || studentSeats.Count == 0)
                {
                    studentSeats = varStudent.OrderBy(r => r.STUDENT_ID).OrderBy(r => r.ROOM_ID).Where(r => r.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME.Contains(_model.searchText)).ToList();
                }
            }
            if (!String.IsNullOrWhiteSpace(_model.searchText))
            {
                if (studentSeats == null || studentSeats.Count == 0)
                {
                    studentSeats = varStudent.OrderBy(r => r.STUDENT_ID).OrderBy(r => r.ROOM_ID).Where(r => r.TB_APPLICATION_STUDENT.STD_NAME.Contains(_model.searchText)).ToList();
                }
            }
            if (!String.IsNullOrWhiteSpace(_model.searchText))
            {
                if (studentSeats == null || studentSeats.Count == 0)
                {
                    studentSeats = varStudent.OrderBy(r => r.STUDENT_ID).OrderBy(r => r.ROOM_ID).Where(r => r.TB_APPLICATION_STUDENT.STD_SURNAME.Contains(_model.searchText)).ToList();
                }
            }
            if (!String.IsNullOrWhiteSpace(_model.searchText))
            {
                if (studentSeats == null || studentSeats.Count == 0 && CommonUtils.isNumber(_model.searchText))
                {
                    int stdCode = Convert.ToInt32(_model.searchText);
                    studentSeats = varStudent.OrderBy(r => r.STUDENT_ID).OrderBy(r => r.ROOM_ID).Where(r => r.STUDENT_CODE == stdCode).ToList();
                }
            }

            List<Report07_1Model> reportLists = new List<Report07_1Model>();

            if (studentSeats != null)
            {
                CultureInfo ci = new CultureInfo("th");
                int rowIndex = 1;
                foreach (TB_STUDENT_SEAT student in studentSeats)
                {
                    Report07_1Model report = new Report07_1Model();
                    report.seq = rowIndex + ".";
                    report.studentId = Convert.ToInt32(student.STUDENT_ID);
                    report.id = student.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(student.SIT_NUMBER).ToString("00");
                    report.name = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_TITLE.TITLE_NAME_ZH) + student.TB_APPLICATION_STUDENT.STD_NAME + "  " + student.TB_APPLICATION_STUDENT.STD_SURNAME;
                    report.school = student.TB_APPLICATION_STUDENT.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                    report.roomNo = student.TB_ROOM.ROOM_NUMBER + "";
                    report.level = ((ci.Name.Equals("th")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_TH : (ci.Name.Equals("en")) ? student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_EN : student.TB_APPLICATION_STUDENT.TB_M_LEVEL.LEVEL_NAME_ZH);
                    report.grade = student.TB_APPLICATION_STUDENT.STD_GRADE + "";
                    report.exam_building = student.TB_ROOM.ROOM_BUILD + "";
                    report.sign = "";
                    report.remark = student.STUDENT_CODE + "";
                    reportLists.Add(report);
                    rowIndex++;
                }

                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 15);
            }
            ViewBag.PageContent = "ค้นหาเลขที่นั่งสอบ";
            ViewBag.doSearch = true;
            ViewBag.doFirst = false;
            return View(_model);
        }

        /*
         * ประกาศผลรอบเจียรไนเพชร
         */
        public ActionResult Report16(Round1ReportModelCriteria _model)
        {
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                if (!isAnnounceDate)
                {
                    return View("ReportNotice02");
                }
            }
            List<Round1Report01Model> reportLists = GetRound1All(_model);

            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 25);
            }
            ViewBag.PageContent = "ประกาศผลการแข่งขันภาษาจีนเพชรยอดมงกุฏ ครั้งที่ 12 ( นานาชาติ ) รอบเจียระไนเพชร";

            return View(_model);
        }
        /*
         * ประกาศผลรอบเพชรยอดมงกุฏ
         */
        public ActionResult Report17(Round1ReportModelCriteria _model)
        {
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                if (!isAnnounceDate)
                {
                    return View("ReportNotice02");
                }
            }
            List<Round1Report01Model> reportLists = GetRound2Lists1(_model);

            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                _model.reports = reportLists.ToPagedList(pageIndex, 25);
            }
            ViewBag.PageContent = "ประกาศผลการแข่งขันภาษาจีนเพชรยอดมงกุฏ ครั้งที่ 12 ( นานาชาติ ) รอบเพชรยอดมงกุฏ";

            return View(_model);
        }
        /*
         * แผนผังห้องสอบ
         */
        public ActionResult Report18()
        {

            //ตรวจสอบว่าสารถใช้งานเมนูนี้ได้หรือไม่
            if (Session["isAnnounceDate"] != null)
            {
                Boolean isAnnounceDate = (Boolean)Session["isAnnounceDate"];
                return View("ReportNotice04");
            }
            return View();
        }

        /*
         * รายงานผลการสมัคร
         */
        public ActionResult Report(ReportModelCriteria _model)
        {
            List<TB_APPLICATION_STUDENT> studentLists = null;

            //SEARCH CONDITION
            //if (_model.studentLevel == 0)
            //{
            //    var varRoom  = from a in db.TB_APPLICATION_STUDENT orderby a.STD_SCHOOL_ID, a.STD_ID  select a;
            //    if (!String.IsNullOrWhiteSpace(_model.schoolName))
            //    {
            //        studentLists = varRoom.Where(r => r.TB_APPLICATION_SCHOOL.SCHOOL_NAME.Contains(_model.schoolName)).ToList();
            //    }
            //    else
            //    {
            //        studentLists = varRoom.ToList();
            //    }
            //}
            //else {
            //var varRoom = from a in db.TB_APPLICATION_STUDENT orderby a.STD_SCHOOL_ID, a.STD_ID where a.STD_LEVEL_ID == _model.studentLevel select a;
            //    if (!String.IsNullOrWhiteSpace(_model.schoolName))
            //    {
            //studentLists = varRoom.Where(r => r.TB_APPLICATION_SCHOOL.SCHOOL_NAME.Contains(_model.schoolName)).ToList();
            //    }
            //    else
            //    {
            //        studentLists = varRoom.ToList();
            //    }
            //}

            List<Report19Model> reportLists = new List<Report19Model>();
            if (studentLists != null)
            {
                int seq = 1;
                foreach (TB_APPLICATION_STUDENT student in studentLists)
                {

                    Report19Model tmp = new Report19Model();
                    //tmp.seq = seq+".";
                    //tmp.fullName = student.TB_M_TITLE.TITLE_NAME_TH + "" + student.STD_NAME + "  " + student.STD_SURNAME;
                    //tmp.school = student.TB_APPLICATION_SCHOOL.SCHOOL_NAME;
                    //tmp.province = student.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME;
                    //if (student.TB_APPLICATION_SCHOOL.SCHOOL_TYPE.Equals("5"))
                    //{
                    //    tmp.schoolType = student.TB_APPLICATION_SCHOOL.SCHOOL_TYPE_OTHER;
                    //}
                    //else {
                    //    tmp.schoolType = getSchoolType(student.TB_APPLICATION_SCHOOL.SCHOOL_TYPE);
                    //}
                    reportLists.Add(tmp);
                    seq++;
                }
            }

            if (reportLists != null)
            {
                var pageIndex = _model.Page ?? 1;
                //_model.reports = reportLists.ToPagedList(pageIndex, 15);
                ViewBag.PageContent = "ตรวจสอบรายชื่อผู้สมัคร";
            }

            return View(_model);
        }
        #endregion


        public ActionResult PrintConfirmDocument(int id = 0)
        {

            CultureInfo ci = (CultureInfo)this.Session["PhetCulture"];
            /*
             * DECLARE VARIABLE
             */
            CommonUtils util = new CommonUtils();


            List<Report01Model> lists = new List<Report01Model>();

            var tb_application_student = from a in db.TB_APPLICATION_STUDENT where a.STD_ID == id select a;


            if (tb_application_student != null)
            {

                List<TB_APPLICATION_STUDENT> students = tb_application_student.ToList();
                int schId = Convert.ToInt16(students[0].STD_SCHOOL_ID);
                TB_APPLICATION_SCHOOL school = db.TB_APPLICATION_SCHOOL.Where(s => s.SCHOOL_ID == schId).FirstOrDefault();
                foreach (TB_APPLICATION_STUDENT student in students)
                {
                    /*
                     * ที่นั่งสอบ
                     */
                    TB_STUDENT_SEAT studentSeat = db.TB_STUDENT_SEAT.SingleOrDefault(t => t.STUDENT_ID == student.STD_ID);
                    Report01Model report = new Report01Model();
                    report.map = CommonUtils.getByteImage(Convert.ToInt16(student.STD_LEVEL_ID));
                    if (studentSeat != null)
                    {
                        report.p_std_id = studentSeat.STUDENT_CODE + "";
                        report.p_exam_room = studentSeat.TB_ROOM.ROOM_NUMBER + "";
                        report.p_exam_seat = studentSeat.SIT_NUMBER_PREFIX + "" + Convert.ToInt16(studentSeat.SIT_NUMBER).ToString("00");
                        report.p_exam_building = studentSeat.TB_ROOM.ROOM_BUILD + "";
                        report.p_exam_floor = studentSeat.TB_ROOM.ROOM_FLOOR + "";
                    }
                    else
                    {
                        report.p_std_id = "-";
                        report.p_exam_room = "-";
                        report.p_exam_seat = "-";
                        report.p_exam_building = "-";
                        report.p_exam_floor = "-";
                    }
                    /*
                     * บอร์ดที่ประกาศผลสอบ
                     */
                    TB_EXAM_ANOUNCE examAnounce = db.TB_EXAM_ANOUNCE.Where(t => t.ANOUNCE_FOR_LEVEL == student.STD_LEVEL_ID).FirstOrDefault();
                    if (examAnounce != null)
                    {
                        report.p_exam_announce_building = examAnounce.ANOUNCE_BUILDING + "";
                        report.p_exam_announce_board = examAnounce.ANOUNCE_BOARD;
                    }
                    else
                    {
                        report.p_exam_announce_building = "-";
                        report.p_exam_announce_board = "-";
                    }

                    report.p_exam_date = "9 สิงหาคม 2558";
                    report.p_exam_time = "09.00-10.00 น.";

                    report.p_student_name = util.getStudentFullName(student);
                    report.p_school_name = school.SCHOOL_NAME;
                    report.p_school_type = getSchoolType(school.SCHOOL_TYPE);
                    report.p_school_province = school.TB_M_PROVINCE.PROVINCE_NAME;
                    report.p_student_level = student.TB_M_LEVEL.LEVEL_NAME_TH.Split(' ')[0];// +" " + student.STD_GRADE;
                    report.p_student_grade = student.STD_GRADE + "";

                    String phoneLable = (ci.Name.ToUpper().Equals("TH")) ? "เบอร์ติดต่อ  " : "Phone Number  ";

                    var tb_application_staff = from a in db.TB_APPLICATION_STAFF where a.STAFF_SCHOOL_ID == school.SCHOOL_ID select a;
                    //APPEND STAFF
                    if (tb_application_staff != null)
                    {
                        List<TB_APPLICATION_STAFF> staffs = tb_application_staff.ToList();

                        if (staffs.Count >= 1)
                        {
                            report.p_staff01_name = util.getStaffFullName(staffs[0]);
                            report.p_staff01_phone = phoneLable + staffs[0].STAFF_PHONE;
                        }
                        if (staffs.Count >= 2)
                        {
                            report.p_staff02_name = util.getStaffFullName(staffs[1]);
                            report.p_staff02_phone = phoneLable + staffs[1].STAFF_PHONE;
                        }
                        if (staffs.Count >= 3)
                        {
                            report.p_staff03_name = util.getStaffFullName(staffs[2]);
                            report.p_staff03_phone = phoneLable + staffs[2].STAFF_PHONE;
                        }
                        if (staffs.Count >= 4)
                        {
                            report.p_staff04_name = util.getStaffFullName(staffs[3]);
                            report.p_staff04_phone = phoneLable + staffs[3].STAFF_PHONE;
                        }
                        if (staffs.Count >= 5)
                        {
                            report.p_staff05_name = util.getStaffFullName(staffs[4]);
                            report.p_staff05_phone = phoneLable + staffs[4].STAFF_PHONE;
                        }
                    }
                    lists.Add(report);
                }
            }


            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Reports/Rpt01_th.rpt");
            rptH.Load();
            rptH.SetDataSource(lists);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        #region "CHART"
        public ActionResult DailyReport()
        {
            List<Report19Model> report19Lists = Report19("", "", 1);

            List<String> cat = new List<string>();
            for (int i = 1; i <= 31; i++)
            {
                cat.Add(i + "");
            }
            int sumtotal = 0;
            Object[] dataMonth6 = new Object[31];
            Object[] dataMonth7 = new Object[31];
            Object[] dataMonth8 = new Object[31];

            for (int dayIndex = 0; dayIndex < 31; dayIndex++)
            {
                dataMonth6[dayIndex] = "0";
                dataMonth7[dayIndex] = "0";
                dataMonth8[dayIndex] = "0";
                for (int i = 0; i < report19Lists.Count; i++)
                {
                    Report19Model rpt = report19Lists[i];
                    if ("06".Equals(rpt.description.Split('/')[1]))
                    {
                        dataMonth6[Convert.ToInt16(rpt.description.Split('/')[0]) - 1] = rpt.count + "";
                    }
                    if ("07".Equals(rpt.description.Split('/')[1]))
                    {
                        dataMonth7[Convert.ToInt16(rpt.description.Split('/')[0]) - 1] = rpt.count + "";
                    }
                    if ("08".Equals(rpt.description.Split('/')[1]))
                    {
                        dataMonth8[Convert.ToInt16(rpt.description.Split('/')[0]) - 1] = rpt.count + "";
                    }
                }
            }

            foreach (Report19Model rpt in report19Lists)
            {
                sumtotal += Convert.ToInt16(rpt.count);
            }

            /*
             * BEGIN RENDER CHART
             */
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart");
            Series[] series = new Series[3];
            series[0] = new Series { Name = CommonUtils.getMonthName(Convert.ToInt16("06")), Data = new Data(dataMonth6) };
            series[1] = new Series { Name = CommonUtils.getMonthName(Convert.ToInt16("07")), Data = new Data(dataMonth7) };
            series[2] = new Series { Name = CommonUtils.getMonthName(Convert.ToInt16("08")), Data = new Data(dataMonth8) };
            chart.SetYAxis(new YAxis
            {
                Title = new YAxisTitle { Text = "จำนวน (คน)" },
            });
            chart.SetXAxis(new XAxis
            {
                Categories = cat.ToArray()
            }
            )
                .SetSeries(new[] { series[0], series[1], series[2] }

                );

            /*
             DESCRIPTION 
             */
            Title title = new Title();
            title.Text = "รายงานสรุปยอดผู้สมัครแยกตามวัน (ยอดสมัครทั้งหมด " + sumtotal + " คน)";
            chart.SetTitle(title);
            return View(chart);
        }

        public ActionResult DailyReport1()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Report19Model> report19Lists = Report19("", "", 1);

            List<String> cat = new List<string>();
            for (int i = 1; i <= 31; i++)
            {
                cat.Add(i + "");
            }
            int sumtotal = 0;
            Object[] dataMonth6 = new Object[31];
            Object[] dataMonth7 = new Object[31];
            Object[] dataMonth8 = new Object[31];

            for (int dayIndex = 0; dayIndex < 31; dayIndex++)
            {
                dataMonth6[dayIndex] = "0";
                dataMonth7[dayIndex] = "0";
                dataMonth8[dayIndex] = "0";
                for (int i = 0; i < report19Lists.Count; i++)
                {
                    Report19Model rpt = report19Lists[i];
                    if ("06".Equals(rpt.description.Split('/')[1]))
                    {
                        dataMonth6[Convert.ToInt16(rpt.description.Split('/')[0]) - 1] = rpt.count + "";
                    }
                    if ("07".Equals(rpt.description.Split('/')[1]))
                    {
                        dataMonth7[Convert.ToInt16(rpt.description.Split('/')[0]) - 1] = rpt.count + "";

                    }
                    if ("08".Equals(rpt.description.Split('/')[1]))
                    {
                        dataMonth8[Convert.ToInt16(rpt.description.Split('/')[0]) - 1] = rpt.count + "";

                    }
                }
            }

            foreach (Report19Model rpt in report19Lists)
            {
                sumtotal += Convert.ToInt16(rpt.count);
            }

            /*
             * BEGIN RENDER CHART
             */
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart");
            Series[] series = new Series[3];
            series[0] = new Series { Name = CommonUtils.getMonthName(Convert.ToInt16("06")), Data = new Data(dataMonth6) };
            series[1] = new Series { Name = CommonUtils.getMonthName(Convert.ToInt16("07")), Data = new Data(dataMonth7) };
            series[2] = new Series { Name = CommonUtils.getMonthName(Convert.ToInt16("08")), Data = new Data(dataMonth8) };

            chart.SetYAxis(new YAxis
                   {
                       Title = new YAxisTitle { Text = "จำนวน (คน)" },
                   });
            chart.SetXAxis(new XAxis
            {
                Categories = cat.ToArray()
            }
            )
                .SetSeries(new[] { series[0], series[1], series[2] }

                );

            /*
             DESCRIPTION 
             */
            Title title = new Title();
            title.Text = "รายงานสรุปยอดผู้สมัครแยกตามวัน (ยอดสมัครทั้งหมด " + sumtotal + " คน)";
            chart.SetTitle(title);
            return View(chart);
        }

        public ActionResult DailyReport2()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Report19Model> report19Lists = Report19("", "", 2);

            List<String> cat = new List<string>();

            int sumtotal = 0;
            foreach (Report19Model rpt in report19Lists)
            {
                cat.Add(rpt.description);
                sumtotal += Convert.ToInt16(rpt.count);
            }
            /*
             * BEGIN RENDER CHART
             */
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart");
            chart.InitChart(new Chart { DefaultSeriesType = ChartTypes.Pie });

            chart.SetSeries(new Series
                    {
                        Type = ChartTypes.Pie,
                        //Name = "Browser share",
                        Data = new Data(new object[]
                            {
                                new object[] { cat[0], report19Lists[0].count },
                                new object[] { cat[1], report19Lists[1].count},
                                new Point
                                    {
                                        Name = cat[2],
                                        Y = report19Lists[2].count,
                                        Sliced = true,
                                        Selected = true
                                    },
                                new object[] { cat[3],report19Lists[3].count },
                                new object[] { cat[4], report19Lists[4].count }
                            })
                    });


            /*
             DESCRIPTION 
             */
            Title title = new Title();
            title.Text = "รายงานสรุปยอดผู้สมัครแยกตามระดับชั้น (ยอดสมัครทั้งหมด " + sumtotal + " คน)";
            chart.SetTitle(title);
            return View("DailyReport1", chart);
        }
        public ActionResult DailyReport3()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("../Account/ManagementLogin");
            }

            List<Report19Model> report19Lists = Report19("", "", 3);

            List<String> cat = new List<string>();
            object[] data = new object[report19Lists.Count];// { cat[0], report19Lists[0].count }

            int index = 0;
            int sumtotal = 0;
            foreach (Report19Model rpt in report19Lists)
            {
                cat.Add(rpt.description);
                object tmp = new object[] { rpt.description, rpt.count };
                data[index] = tmp;
                sumtotal += Convert.ToInt16(rpt.count);
                index++;
            }
            /*
             * BEGIN RENDER CHART
             */
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart");
            chart.SetSeries(new Series
            {
                Type = ChartTypes.Pie,
                //Name = "Browser share",
                Data = new Data(data)
            });


            /*
             DESCRIPTION 
             */
            Title title = new Title();
            title.Text = "รายงานสรุปยอดผู้สมัครแยกตามระดับชั้น (ยอดสมัครทั้งหมด " + sumtotal + " คน)";
            chart.SetTitle(title);
            return View("DailyReport1", chart);
        }
        #endregion
        #region "UTILS"
        private String getSchoolType(String type)
        {
            String result = "";
            switch (Convert.ToInt16(type))
            {
                case 1:
                    result = Resources.Application.Application.SCHOOL_TYPE_01;
                    break;
                case 2:
                    result = Resources.Application.Application.SCHOOL_TYPE_02;
                    break;
                case 3:
                    result = Resources.Application.Application.SCHOOL_TYPE_03;
                    break;
                case 4:
                    result = Resources.Application.Application.SCHOOL_TYPE_04;
                    break;
                case 5:
                    result = Resources.Application.Application.SCHOOL_TYPE_OTHER;
                    break;
                default: break;
            }
            return result;
        }

        private String getReport19Type(int index)
        {
            String[] type = { "", "วันที่สมัคร", "ระดับชั้น", "สถานศึกษา", "จังหวัด", "ประเทศ" };
            return type[index];
        }
        private String getStudentLevel(int studentLevel)
        {
            String[] level ={ "" ,
                           "ช่วงชั้นที่ 1 ประถมศึกษาปีที่ 1-3",
                           "ช่วงชั้นที่ 2 ประถมศึกษาปีที่ 4-6" ,
                           "ช่วงชั้นที่ 3 มัธยมศึกษาปีที่ 1-3",
                           "ช่วงชั้นที่ 4 มัธยมศึกษาปีที่ 4-6",
                           "อุดมศึกษา"};
            return level[studentLevel];
        }


        #endregion

        #region STUDENT LEVEL CASCADE
        public ActionResult AsyncRoomForLevel(string studentLevel)
        {
            DelayResponse();
            var amphurs = repository.GetRoomForLevel(studentLevel).ToList().Select(a => new SelectListItem()
            {
                Text = a.ROOM_NUMBER + "",
                Value = a.ROOM_NUMBER.ToString(),
            });

            return Json(amphurs);
        }

        private static void DelayResponse()
        {
            System.Threading.Thread.Sleep(100);
        }
        #endregion



        #region "sum25"
        private int getschoolByLevelId(List<TB_APPLICATION_STUDENT> students, String cul)
        {
            int count = 1;
            int tmpSchoolId = Convert.ToInt16(students[0].STD_SCHOOL_ID);
            foreach (TB_APPLICATION_STUDENT std in students)
            {
                if (std.STD_SCHOOL_ID != tmpSchoolId && std.TB_APPLICATION_SCHOOL.SCHOOL_CULTURE.Equals(cul.ToUpper()))
                {
                    tmpSchoolId = Convert.ToInt16(std.TB_APPLICATION_SCHOOL.SCHOOL_ID);
                    count++;
                }
            }
            return count;
        }
        private String getschoolProvinceByLevelId(List<TB_APPLICATION_STUDENT> students)
        {
            List<String> tmps = new List<string>();
            StringBuilder sb = new StringBuilder();
            int tmpSchoolProvinceId = 0;
            foreach (TB_APPLICATION_STUDENT std in students)
            {
                if (std.TB_APPLICATION_SCHOOL.SCHOOL_PROVINCE != tmpSchoolProvinceId)
                {
                    tmps.Add(std.TB_APPLICATION_SCHOOL.TB_M_PROVINCE.PROVINCE_NAME);
                    tmpSchoolProvinceId = Convert.ToInt16(std.TB_APPLICATION_SCHOOL.SCHOOL_PROVINCE);
                }
            }

            List<string> xxx = new List<string>();
            xxx.AddRange(tmps.Distinct());

            foreach (String x in xxx)
            {
                sb.Append(x);
                sb.Append(",");
            }
            return sb.ToString();
        }
        #endregion
    }
}
