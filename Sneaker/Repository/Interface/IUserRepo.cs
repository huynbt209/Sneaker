using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface IUserRepo
    {
        IEnumerable<Product> GetProductsSaleUser(int id);
    }
}
