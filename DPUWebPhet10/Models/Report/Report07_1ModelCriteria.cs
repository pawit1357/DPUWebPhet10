using System;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class Report07_1ModelCriteria
    {
        public int? Page { get; set; }
        public String searchText { get; set; }
        public IPagedList<Report07_1Model> reports { get; set; }

    }
}