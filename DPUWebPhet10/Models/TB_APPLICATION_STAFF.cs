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
    
    public partial class TB_APPLICATION_STAFF
    {
        public decimal STAFF_ID { get; set; }
        public Nullable<decimal> STAFF_SCHOOL_ID { get; set; }
        public Nullable<decimal> STAFF_POSITION { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_SURNAME { get; set; }
        public string STAFF_PHONE { get; set; }
        public Nullable<decimal> STAFF_TITLE_ID { get; set; }
        public Nullable<decimal> STAFF_FOR_LEVEL { get; set; }
        public Nullable<decimal> STAFF_NATION { get; set; }
    
        public virtual TB_APPLICATION_SCHOOL TB_APPLICATION_SCHOOL { get; set; }
        public virtual TB_M_TITLE TB_M_TITLE { get; set; }
        public virtual TB_M_LEVEL TB_M_LEVEL { get; set; }
    }
}