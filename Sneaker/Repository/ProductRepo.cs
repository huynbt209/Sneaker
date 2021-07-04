using Microsoft.EntityFrameworkCore;
using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class ProductRepo : IProductRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Product> GetProductByTrademark(int trademarkId)
        {
            var products = _dbContext.Products.Include(p => p.Trademark).Where(p => p.TrademarkId == trademarkId).ToList();
            return products;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _dbContext.Products.ToList();
        }

        public IEnumerable<Trademark> GetTrademarks()
        {
            return _dbContext.Trademarks.ToList();
            
        }

        public ProductTrademarkViewModel CreateProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            var newProduct = new ProductTrademarkViewModel
            {
                Product = productTrademarkViewModel.Product,               
                Trademarks = _dbContext.Trademarks.ToList(),
            };
            _dbContext.Add(newProduct);
            _dbContext.SaveChanges();
            return newProduct;
        }

        public IEnumerable<Product> GetProductInTrademarkId(int id)
        {
            var listProductIdInSelectedTrademark = _dbContext.Products.Where(t => t.TrademarkId == id).Select(p => p.Id).ToArray();
            var list = _dbContext.Products.Where(p => listProductIdInSelectedTrademark.Any(t => t.Equals(p.Id))).ToList();
            return list;
        }

        public ProductTrademarkViewModel ProductTrademarkViewModel()
        {
            var productViewModel = new ProductTrademarkViewModel
            {
                Product = new Product(),
                Trademarks = _dbContext.Trademarks.OrderBy(t => t.TrademarkName).Distinct().ToList(),
            };
            return productViewModel;
        }

        public ProductTrademarkViewModel ImportProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            return null;
        }
    }
}
