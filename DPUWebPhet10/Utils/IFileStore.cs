using System;
using System.Web;

namespace DPUWebPhet10.Models
{
    public interface IFileStore
    {
        Guid SaveUploadedFile(HttpPostedFileBase fileBase);
    }
}