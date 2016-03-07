using System.IO;
using System.Web;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class FilesController : Controller
    {
        public FileDownloadResult Download(string file)
        {
            try
            {
                var fileData = file.GetFileData(Server.MapPath("~/FileUpload/documents"));
                return new FileDownloadResult(file, fileData);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, string.Format("The file {0} was not found.", file) );
            }
        }
        public FileDownloadResult Download1(string file)
        {
            try
            {
                var fileData = file.GetFileData(Server.MapPath("~/FileUpload/uploads"));
                return new FileDownloadResult(file, fileData);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, string.Format("The file {0} was not found.", file));
            }
        }
    }
}
