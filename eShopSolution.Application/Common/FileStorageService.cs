using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Common
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "Uploads";

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, USER_CONTENT_FOLDER_NAME);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public string GetFileUrl(string fileName)
        {
            return Path.GetFullPath(fileName);
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string filePath)
        {
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var basePath = SetBasePath();

            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            originalFileName = originalFileName.Replace(" ", "_");
            var fileName = $"{originalFileName}{Path.GetExtension(originalFileName)}";
            var filePath = Path.Combine(basePath, originalFileName);
            await SaveFileAsync(file.OpenReadStream(), filePath);
            return filePath;
        }

        private string SetBasePath()
        {
            var basePath = "wwwroot/" + USER_CONTENT_FOLDER_NAME + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";
            //Create Folder
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            return basePath;
        }
    }
}