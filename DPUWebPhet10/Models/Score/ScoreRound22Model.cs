using System;
using System.ComponentModel.DataAnnotations;

namespace DPUWebPhet10.Models
{
    public class ScoreRound22Model
    {
        [Required(ErrorMessage = "ยังไม่ได้ป้อนรหัสนักเรียน/นักศึกษา")]
        public String studentCode { get; set; }
        public String studentName { get; set; }
        public String actionName { get; set; }
        public TB_STUDENT_SEAT student { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score11 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score12 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score13 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score14 { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score21 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score22 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score23 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score24 { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score31 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score32 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score33 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        [Required(ErrorMessage = "ยังไม่ได้ป้อนคะแนน")]
        public String score34 { get; set; }
    }

}