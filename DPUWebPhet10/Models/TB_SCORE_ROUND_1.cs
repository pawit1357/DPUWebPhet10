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
    
    public partial class TB_SCORE_ROUND_1
    {
        public decimal STD_CODE { get; set; }
        public Nullable<decimal> ROUND_SCORE { get; set; }
        public Nullable<decimal> PRIZE_ID { get; set; }
    
        public virtual TB_M_PRIZE TB_M_PRIZE { get; set; }
    }
}
