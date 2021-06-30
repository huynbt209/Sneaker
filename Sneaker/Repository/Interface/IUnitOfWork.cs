using Microsoft.AspNetCore.Http;

namespace Sneaker.Repository.Interface
{
    public interface IUnitOfWork
    {
        void UploadImage(IFormFile file);
    }
}
