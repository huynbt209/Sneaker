using Microsoft.EntityFrameworkCore;
using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Product> GetProductsSaleUser(int id)
        {
            return _dbContext.Products.Include(p => p.Trademark).Where(p => p.PriceOld != null && p.TrademarkId == id).ToList();
        }
    }
}