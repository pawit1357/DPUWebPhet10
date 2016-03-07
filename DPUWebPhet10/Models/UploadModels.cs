
using System;
using System.ComponentModel.DataAnnotations;
namespace DPUWebPhet10.Models
{
    public class UploadModel
    {
        public Guid? photo_guid { get; set; }
        [Required(ErrorMessage = "ยังไม่ได้เลือกไฟล์เอกสาร")]
        public string photo_filename { get; set; }

        public TB_APPLICATION_SCHOOL school { get; set; }
    }

}
