using System;

namespace DPUWebPhet10.Models
{
    /*
     * รายงานสรุปจำนวนรวมของผู้สมัครในแต่ละในแต่ละวัน / ห้องสอบ  / ระดับชั้น / สถานศึกษา / จังหวัด / ประเทศ
     */
    public class Report19Model 
    {
        public int date { get; set; }
        public int seq { get; set; }
        public String description { get; set; }
        public int count { get; set; }

    }

}