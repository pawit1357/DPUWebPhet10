using System;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class Report03ModelCriteria
    {
        public int? Page { get; set; }
        public int studentLevel { get; set; }
        public String schoolName { get; set; }

        public IPagedList<Report03Model> reports { get; set; }

    }
}