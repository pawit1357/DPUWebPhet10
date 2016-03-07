using System;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class Report07ModelCriteria
    {
        public int? Page { get; set; }
        public int level { get; set; }
        public String searchText { get; set; }

        public IPagedList<Report07Model> reports { get; set; }

    }
}