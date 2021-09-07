using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sneaker.Repository.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
using Sneaker.Data;
using Sneaker.Models;

namespace Sneaker.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(IWebHostEnvironment webHostEnvironment, ApplicationDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
            Item = new ItemRepo(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
            this.Dispose();
        }
        public IItemRepo Item {get;}
        
        public async Task<string> SaveFileAsync(IFormFile file, string pathToUpload)
        {
            pathToUpload = Path.Combine(this._webHostEnvironment.WebRootPath, pathToUpload);
            if (!Directory.Exists(pathToUpload))
                System.IO.Directory.CreateDirectory(pathToUpload); //Create Path of not exists
            var pathFile = Path.Combine(pathToUpload, file.FileName);
            await using(var fileStream = new FileStream(pathFile, FileMode.Create)) 
            {
                await file.CopyToAsync(fileStream);
            }
            var imageUrl = Path.GetFileNameWithoutExtension(pathFile);
            return imageUrl;
        }

        public async void UploadImage(IFormFile file)
        {
            var fileName = file.FileName.Trim('"');
            fileName = EnsureFileName(fileName);
            var buffer = new byte[16 * 1024];
            await using var output = System.IO.File.Create(GetPathAndFileName(fileName));
            await using var input = file.OpenReadStream();
            int readBytes;
            while ((readBytes = await input.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, readBytes));
            }
        }

        private string GetPathAndFileName(string fileName)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Path.Combine(path, fileName);
        }

        private static string EnsureFileName(string fileName)
        {
            if (fileName.Contains("\\"))
                fileName = fileName.Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            return fileName;
        }
    }
}
