using System.IO; // Stream için
using System.Collections.Generic; // IList için
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // IFormFile için burada olması gerekiyor

namespace ECommerce.Application.Interfaces.Storage
{
    public interface IFileStorageService
    {
        // Tek bir dosyayı yükler ve dosyanın URL'sini veya yolunu döndürür.
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName);

        // Birden fazla dosyayı yükler ve yollarının listesini döndürür.
        Task<List<string>> UploadFilesAsync(IList<(Stream fileStream, string fileName)> files, string folderName);

        // Bir dosyayı siler
        void DeleteFile(string filePath);

        // AZ ÖNCE HATAYA NEDEN OLAN VE GEREKSİZ OLAN METOT BURADAN SİLİNDİ!
        // Task<IEnumerable<object>> UploadFilesAsync(List<IFormFile> newImages, string v); // <-- BU SATIR BURADAN SİLİNDİ!
    }
}   