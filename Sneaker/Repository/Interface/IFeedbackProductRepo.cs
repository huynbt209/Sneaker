using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface IFeedbackProductRepo
    {
        Task<bool> SaveComment(int productId, string userId, string message);
    }
}
