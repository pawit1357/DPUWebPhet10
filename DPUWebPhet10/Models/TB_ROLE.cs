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
    
    public partial class TB_ROLE
    {
        public TB_ROLE()
        {
            this.TB_ROLE_PERMISSION = new HashSet<TB_ROLE_PERMISSION>();
            this.TB_USER = new HashSet<TB_USER>();
        }
    
        public decimal ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTON { get; set; }
        public Nullable<decimal> STATUS { get; set; }
    
        public virtual TB_M_STATUS TB_M_STATUS { get; set; }
        public virtual ICollection<TB_ROLE_PERMISSION> TB_ROLE_PERMISSION { get; set; }
        public virtual ICollection<TB_USER> TB_USER { get; set; }
    }
}