using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace DPUWebPhet10.Models
{
    public partial class TB_APPLICATION_STUDENT
    {
        public int? Page { get; set; }
        public int? id { get; set; }
        //public IPagedList<TB_APPLICATION_STUDENT> reports { get; set; }
        public IPagedList<ConcernView> reports { get; set; }

    }

    public class ConcernView
    {
        public int seq { get; set; }
        public string SCHOOL_NAME { get; set; }
        public string STD_NAME { get; set; }
        public string STD_SURNAME { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_PHONE { get; set; }
        public string LEVEL_NAME { get; set; }
    }

}