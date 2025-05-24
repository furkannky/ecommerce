using ECommerce.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Http; // Eğer IFormFile'ı başka bir yerde kullanmıyorsanız bu using'i kaldırabilirsiniz.
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IHostEnvironment _env;
        private readonly string _uploadFolder = "uploads";

        public LocalFileStorageService(IHostEnvironment env)
        {
            _env = env;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var absolutePath = Path.Combine(_env.ContentRootPath, "wwwroot", filePath.TrimStart('/'));

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName)
        {
            if (fileStream == null || fileStream.Length == 0)
                return null;

            var uploadPath = Path.Combine(_env.ContentRootPath, "wwwroot", _uploadFolder, folderName);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var output = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(output);
            }

            return $"/{_uploadFolder}/{folderName}/{uniqueFileName}";
        }

        public async Task<List<string>> UploadFilesAsync(IList<(Stream fileStream, string fileName)> files, string folderName)
        {
            var uploadedPaths = new List<string>();
            foreach (var fileTuple in files)
            {
                var path = await UploadFileAsync(fileTuple.fileStream, fileTuple.fileName, folderName);
                if (path != null)
                {
                    uploadedPaths.Add(path);
                }
            }
            return uploadedPaths;
        }
    }
}