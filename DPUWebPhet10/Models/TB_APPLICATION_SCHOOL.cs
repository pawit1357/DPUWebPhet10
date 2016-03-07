//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DPUWebPhet10.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_APPLICATION_SCHOOL
    {
        public TB_APPLICATION_SCHOOL()
        {
            this.TB_APPLICATION_STUDENT = new HashSet<TB_APPLICATION_STUDENT>();
            this.TB_APPLICATION_STAFF = new HashSet<TB_APPLICATION_STAFF>();
        }
    
        public decimal SCHOOL_ID { get; set; }
        public string SCHOOL_ZONE_EDU { get; set; }
        public string SCHOOL_ZONE { get; set; }
        public string SCHOOL_TYPE { get; set; }
        public string SCHOOL_TYPE_OTHER { get; set; }
        public string SCHOOL_ADDR { get; set; }
        public string SCHOOL_ADDR_SOI { get; set; }
        public string SCHOOL_ADDR_ROAD { get; set; }
        public Nullable<decimal> SCHOOL_ADDR_TOMBON { get; set; }
        public Nullable<decimal> SCHOOL_ADDR_AMPHUR { get; set; }
        public Nullable<decimal> SCHOOL_ADDR_PROVINCE { get; set; }
        public string SCHOOL_ADDR_ZIPCODE { get; set; }
        public string SCHOOL_ADDR_PHONE { get; set; }
        public string SCHOOL_ADDR_FAX { get; set; }
        public string SCHOOL_NAME { get; set; }
        public Nullable<decimal> SCHOOL_PROVINCE { get; set; }
        public string SCHOOL_PASSWORD { get; set; }
        public string SCHOOL_DOC_PATH { get; set; }
        public Nullable<System.DateTime> SCHOOL_REGISTER_DATE { get; set; }
        public Nullable<decimal> SCHOOL_APPROVED_STATUS { get; set; }
        public string SCHOOL_EMAIL { get; set; }
        public string SCHOOL_CULTURE { get; set; }
        public Nullable<decimal> SCHOOL_ROUND { get; set; }
    
        public virtual ICollection<TB_APPLICATION_STUDENT> TB_APPLICATION_STUDENT { get; set; }
        public virtual TB_M_PROVINCE TB_M_PROVINCE { get; set; }
        public virtual TB_M_DISTRICT TB_M_DISTRICT { get; set; }
        public virtual TB_M_AMPHUR TB_M_AMPHUR { get; set; }
        public virtual TB_M_PROVINCE TB_M_PROVINCE1 { get; set; }
        public virtual TB_M_STATUS TB_M_STATUS { get; set; }
        public virtual ICollection<TB_APPLICATION_STAFF> TB_APPLICATION_STAFF { get; set; }
    }
}
