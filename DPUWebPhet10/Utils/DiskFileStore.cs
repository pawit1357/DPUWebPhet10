using System;
using System.Web;
using DPUWebPhet10.Models;
using System.IO;
using System.Web.Hosting;

namespace DPUWebPhet10.Models
{
    internal class DiskFileStore : IFileStore
    {
        private string _uploadsFolder = HostingEnvironment.MapPath("~/FileUpload/uploads");

        public Guid SaveUploadedFile(HttpPostedFileBase fileBase)
        {
            var identifier = Guid.NewGuid();
            FileInfo fileInfo = new FileInfo(fileBase.FileName);
            fileBase.SaveAs(GetDiskLocation(fileBase.FileName));
            return identifier;
        }

        private string GetDiskLocation(Guid identifier)
        {
            return Path.Combine(_uploadsFolder, identifier.ToString());
        }
        private string GetDiskLocation(String identifier)
        {
            return Path.Combine(_uploadsFolder, identifier.ToString());
        }
    }
}