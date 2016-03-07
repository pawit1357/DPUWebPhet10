using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DPUWebPhet10.Models
{

    [Serializable]
    public class ApplicationModel
    {
        public String selectLabel { get; set; }

        [Required(ErrorMessageResourceName = "ERROR_MSG_10", ErrorMessageResourceType = typeof(Resources.Application.Application))]
        public String idSelectedSchoolType { get; set; }
        [Required(ErrorMessageResourceName = "ERROR_MSG_11", ErrorMessageResourceType = typeof(Resources.Application.Application))]
        public String SCHOOL_ADDR_PROVINCE { get; set; }
        [Required(ErrorMessageResourceName = "ERROR_MSG_12", ErrorMessageResourceType = typeof(Resources.Application.Application))]
        public String SCHOOL_ADDR_AMPHUR { get; set; }
        [Required(ErrorMessageResourceName = "ERROR_MSG_13", ErrorMessageResourceType = typeof(Resources.Application.Application))]
        public String SCHOOL_ADDR_TOMBON { get; set; }

        #region LIST
        public IEnumerable<SelectListItem> rSchoolTypes { get; set; }
        public IEnumerable<SelectListItem> provinceLists { get; set; }
        public IEnumerable<SelectListItem> tumbonLists { get; set; }
        public IEnumerable<SelectListItem> amphurLists { get; set; }

        public IList<TB_APPLICATION_STAFF> Staffs { get; set; }
        public IList<TB_APPLICATION_STUDENT> Students { get; set; }
        public TB_APPLICATION_SCHOOL school { get; set; }
        #endregion

        #region STAFF
        //[Required(ErrorMessage = "ยังไม่ได้เลือกคำนำหน้าชื่อ")]
        public String TMP_STAFF_TITLE_ID { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนชื่อ")]
        public String TMP_STAFF_NAME { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนนามสกุล")]
        public String TMP_STAFF_SURNAME { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนเบอร์ติดต่อ")]
        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        public String TMP_STAFF_PHONE { get; set; }
        #endregion

        #region STUDENT
        //[Required(ErrorMessage = "ยังไม่ได้เลือกระดับการศึกษา")]
        public String TMP_STD_LEVEL_ID { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนชั้นปี")]
        public String TMP_STD_GRADE { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้เลือกคำนำหน้าชื่อ")]
        public String TMP_STD_TITLE_ID { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนชื่อ")]
        public String TMP_STD_NAME { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนนามสกุล")]
        public String TMP_STD_SURNAME { get; set; }
        //[Required(ErrorMessage = "ยังไม่ได้ป้อนเบอร์โทรศัพท์")]
        //[RegularExpression(@"^\d+$", ErrorMessage = "กรุณาป้อนเฉพาะตัวเลข")]
        public String TMP_STD_PHONE { get; set; }
        public String TMP_STD_EMAIL { get; set; }
        public String TMP_STD_BIRTH_DAY { get; set; }
        public String TMP_STD_NATION { get; set; }
        public String TMP_STD_NATION_OTHER { get; set; }
        #endregion

    }


}