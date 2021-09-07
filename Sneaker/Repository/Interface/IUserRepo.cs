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
        IEnumerable<Item> GetProductsSaleUser(int id);
        IEnumerable<Item> GetProductsNewUser(int id);
        IEnumerable<Item> GetProductByTrademark(int trademarkId);

    }
}
