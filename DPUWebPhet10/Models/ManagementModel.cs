using PagedList;

namespace DPUWebPhet10.Models
{
    public class ManagementModel
    {
        public int? id { get; set; }
        public int? Page { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string approveStatus { get; set; }
        public string schoolName { get; set; }

        public IPagedList<TB_APPLICATION_SCHOOL> schools { get; set; }

    }
}