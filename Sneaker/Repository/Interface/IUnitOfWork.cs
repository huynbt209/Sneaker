using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sneaker.Repository.Interface
{
    public interface IUnitOfWork
    {
        void UploadImage(IFormFile file);
        Task<string> SaveFileAsync(IFormFile file, string pathToUpload);
    }
}
