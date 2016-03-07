using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DPUWebPhet10.Models
{
    public class MetaModel
    {
        //[MetadataType(typeof(TB_APPLICATION_SCHOOLMetaData))]
         //public partial class TB_APPLICATION_SCHOOL : EntityObject
        public class TB_APPLICATION_SCHOOLMetaData
        {

            [Required(ErrorMessageResourceName = "ERROR_MSG_01", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            [Remote("checkExistSchoolName", "Application", HttpMethod = "POST", ErrorMessageResourceName = "ERROR_MSG_02", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            public object SCHOOL_NAME { get; set; }
            
            //[Required(ErrorMessage = "You must enter an type!")]
            //public object SCHOOL_TYPE { get; set; }

            [Required(ErrorMessageResourceName = "ERROR_MSG_03", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            public object SCHOOL_ADDR { get; set; }

            [RegularExpression(@"^\d+$", ErrorMessageResourceName = "ERROR_MSG_04", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            [Required(ErrorMessageResourceName = "ERROR_MSG_05", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            public object SCHOOL_ADDR_PHONE { get; set; }

            [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceName = "ERROR_MSG_06", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            [Required(ErrorMessageResourceName = "ERROR_MSG_07", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            [Remote("checkEmailExist", "Application", HttpMethod = "POST", ErrorMessageResourceName = "ERROR_MSG_08", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            public object SCHOOL_EMAIL { get; set; }

            [RegularExpression(@"^\d+$", ErrorMessageResourceName = "ERROR_MSG_09", ErrorMessageResourceType = typeof(Resources.Application.Application))]
            public object SCHOOL_ADDR_ZIPCODE { get; set; }


        }

    }
}