using Sneaker.Models;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface IUserRepo
    {
        IEnumerable<Product> GetProductsSaleUser(int id);
        IEnumerable<Product> GetProductsNewUser(int id);
        FeedbackProductViewModel GetProductDetail(int id);
        IEnumerable<Product> GetProductByTrademark(int trademarkId);

    }
}
