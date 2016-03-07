using System;
using System.ComponentModel.DataAnnotations;

namespace DPUWebPhet10.Models
{
    public class ScoreRound1Model
    {
        public String studentLevel { get; set; }

        [Required(ErrorMessage = "ยังไม่ได้ป้อนรหัสนักเรียน/นักศึกษา")]
        public String studentCode { get; set; }
        public String actionName { get; set; }
        //public TB_STUDENT_SEAT student { get; set; }

        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        //public String score1 { get; set; }
        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        //public String score2 { get; set; }
        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        //public String score3 { get; set; }
        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        //public String score4 { get; set; }
        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        //public String score5 { get; set; }

    }

}