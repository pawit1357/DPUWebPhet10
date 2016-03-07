using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DPUWebPhet10.Models
{

    [Serializable]
    public class ApplicationAdhocModel
    {
        public String SCHOOL_NAME { get; set; }
        public int STD_SCHOOL_ID { get; set; }
        public int STD_LEVEL_ID { get; set; }
        public int STD_GRADE { get; set; }
        public int STD_TITLE_ID { get; set; }

        public String STUDENT_CODE { get; set; }
        public String STD_NAME { get; set; }
        public String STD_SURNAME { get; set; }


        public int STD_NATION { get; set; }

        public int SIT_NUMBER { get; set; }
        public String SIT_NUMBER_PREFIX { get; set; }
        public int ROOM_NUMBER { get; set; }

    }


}