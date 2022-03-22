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


      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=assign6ashish;AccountKey=CyFQZthGyu+yLfmq4nevIu0Tqhv/jNFbAgGoSOuC590Ych0N79DikatdzCFlTDMuR7D/y+uoAF8H+AStq0AUjw==;EndpointSuffix=core.windows.net";
            var containerName = "excel-store";

            //await blobClient.UploadAsync(file.OpenReadStream());

            String uploadStatus;

            if (file == null || file.Length == 0)
                TempData["uploadStatus"] = "!!! Please select a excel file !!!";
            else
            {
                if (isExcelFile(file) && isFileSizeCorrect(file))
                {
                    BlobClient blobClient = new BlobClient(connectionString: connectionString, blobContainerName: containerName, blobName: getFileName(file));
                    var status = await blobClient.UploadAsync(file.OpenReadStream());

                    if (status.GetRawResponse().Status == 201)
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
            }


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
