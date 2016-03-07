using System;
using System.Collections.Generic;
using PagedList;

namespace DPUWebPhet10.Models
{
    public class Round1ReportModelCriteria
    {
        public int? Page { get; set; }
        public int? id { get; set; }
        public String searchText { get; set; }
        public int studentLevel { get; set; }
        public int roomNo { get; set; }
        public int exportFormat { get; set; }
        public int OrderType { get; set; }
        public int stdNation { get; set; }
        public String action { get; set; }
        public IPagedList<Round1Report01Model> reports { get; set; }

        public List<String> SelectedStudentIDs { get; set; }
        public List<String> SelectedPrizeValueIDs { get; set; }
    }

    public class Round1Report01Model : ScoreRound21Model
    {
        public int seq { get; set; }
        public String roomNo { get; set; }
        public int roomId { get; set; }
        //public String studentCode { get; set; }
        public String studentLevel { get; set; }
        public String studentFullName { get; set; }
        public String schoolName { get; set; }
        public String province { get; set; }
        public String round2score { get; set; }
        public double round1score { get; set; }
        public String remark { get; set; }
        public String phone { get; set; }
        public int prize_id { get; set; }
        public double rowScore21 { get; set; }
        public double rowScore22 { get; set; }




        public String score2_11 { get; set; }
        public String score2_12 { get; set; }
        public String score2_13 { get; set; }
        public String score2_14 { get; set; }
                           
        public String score2_21 { get; set; }
        public String score2_22 { get; set; }
        public String score2_23 { get; set; }
        public String score2_24 { get; set; }
                           
        public String score2_31 { get; set; }
        public String score2_32 { get; set; }
        public String score2_33 { get; set; }
        public String score2_34 { get; set; }





    }
}