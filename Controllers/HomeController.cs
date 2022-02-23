using Azure.Storage.Blobs;
using ExcelFileUploaderToAzureStroage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelFileUploaderToAzureStroage.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var connectionString = "DefaultEndpointsProtocol=https;AccountName=ashishassignment2sa;AccountKey=ZpHzp8IrMemxn43Wr3cgJkGtgtpynjUIBvdy+At1vE8lRHHph9Vv5LQDZM+vVmAIYY5VufdODy0N+ASttU5ZnQ==;EndpointSuffix=core.windows.net";
            var containerName = "excel-23022022";

            //await blobClient.UploadAsync(file.OpenReadStream());

            String uploadStatus;

            
            if (isExcelFile(file) && isFileSizeCorrect(file))
            {
                BlobClient blobClient = new BlobClient(connectionString: connectionString, blobContainerName: containerName, blobName: getFileName(file));
                var status  = await blobClient.UploadAsync(file.OpenReadStream());

                if(status.GetRawResponse().Status == 201)
                {
                    uploadStatus = "!!! Uploaded Successfully !!!";
                }
                else
                {
                    uploadStatus = "!!! Upload Failed !!!";
                }
            }
            else
            {
                uploadStatus = "!!! Only excel file of size >=1MB is allowed !!!";
            }

            TempData["uploadStatus"] = uploadStatus;

            //return Content($"{file.ContentType}");

            return RedirectToAction("Index");

        }


        private String getFileName(IFormFile file)
        {
            String fileName = file.FileName.Split(".")[0] + "@" +Guid.NewGuid();
            return fileName;
        }

        private bool isExcelFile(IFormFile file)
        {
            if(file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                && file.ContentType != "application/vnd.ms-excel") return false;
            return true;
        }

        private bool isFileSizeCorrect(IFormFile file)
        {
            if (file.Length < 1000000) return false;
            return true;
        }
    }
}
