using System;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class Report06ModelCriteria
    {
        public int? Page { get; set; }
        public int levelId { get; set; }
        public String schoolName { get; set; }

        public IPagedList<Report06Model> reports { get; set; }

    }
}