using System;

namespace DPUWebPhet10.Models
{
    /*
     * รายงานใบเซ็นต์ชื่อเข้าผู้เข้าสอบ และติดหน้าห้องสอบ
     */
    public class Report10Model
    {
        public String seq { get; set; }
        public String roomNo { get; set; }
        public int seatCount { get; set; }
        public String seat { get; set; }
        public String stdcode { get; set; }
        public String fullName { get; set; }
        public String schoolName { get; set; }
        public String provinceName { get; set; }
    }

}