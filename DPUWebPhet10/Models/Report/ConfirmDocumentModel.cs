using System;
using System.Collections.Generic;

namespace DPUWebPhet10.Models
{
    /*
     * รายงานสรุปจำนวนรวมของผู้สมัครในแต่ละในแต่ละวัน / ห้องสอบ  / ระดับชั้น / สถานศึกษา / จังหวัด / ประเทศ
     */
    public class ConfirmDocumentModel 
    {
        public int? Page { get; set; }
        public String schoolName { get; set; }
        public String action { get; set; }
        public List<String> names { get; set; }
        public List<String> surnames { get; set; }
        public List<int> SelectedLevelIDs { get; set; }
        public List<int> STD_TITLE_IDs { get; set; }

        public List<TB_APPLICATION_STUDENT> students { get; set; }
    }

}