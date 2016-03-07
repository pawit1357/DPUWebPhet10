using System;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class ReportModelCriteria
    {
        public int? Page { get; set; }
        public int reportType { get; set; }
        public int studentLevel { get; set; }
        public int roomNo { get; set; }
        public String startDate { get; set; }
        public String endDate { get; set; }
        public int periodIndex { get; set; }

        public String searchText { get; set; }

        public IPagedList<Report19Model> reports { get; set; }

    }
}