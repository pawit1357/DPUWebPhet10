using System;

namespace DPUWebPhet10.Models
{
    /*
     * รายงานรายชื่อนักเรียนที่สมัครเข้าร่วมแข่งขัน
     */
    public class Report15Model 
    {
        public String seq { get; set; }
        public String level { get; set; }
        public String levelDescription { get; set; }

        public int building { get; set; }
        public int examRoom { get; set; }
        public String seat { get; set; }
        public int examStudentCount { get; set; }
    }

}