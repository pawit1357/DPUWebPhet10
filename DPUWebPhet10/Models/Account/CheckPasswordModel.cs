using System;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class CheckPasswordModel
    {
        public int? Page { get; set; }
        public String school { get; set; }

        public IPagedList<TB_APPLICATION_SCHOOL> schools { get; set; }
    }

}