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
            return _dbContext.Products.Include(p => p.Trademark).ToList();
        }

        public IEnumerable<Trademark> GetTrademarks()
        {
            return _dbContext.Trademarks.ToList();

        }

        public ProductTrademarkViewModel CreateProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            var checkExists = _dbContext.Products.Include(p => p.Trademark).Where(p => p.ProductName == productTrademarkViewModel.Products.ProductName
            && p.Trademark.Id == productTrademarkViewModel.Products.TrademarkId);
            if (checkExists.Any())
            {
                //
            }
            else
            {
                productTrademarkViewModel.Products.CreateAt = DateTime.Now;
                _dbContext.Products.Add(productTrademarkViewModel.Products);
                _dbContext.SaveChanges();
            }
            var newProduct = new ProductTrademarkViewModel
            {
                Products = productTrademarkViewModel.Products,
                Trademarks = _dbContext.Trademarks.ToList(),
                StatusMessage = "Error: " + _dbContext.Trademarks.SingleOrDefault(t => t.Id == productTrademarkViewModel.Products.TrademarkId).TrademarkName.ToString()
            };
            return newProduct;
        }
        public Product GetProductById(int id)
        {
            return _dbContext.Products.SingleOrDefault(p => p.Id == id);
        }

        public ProductTrademarkViewModel EditProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            var checkExists = _dbContext.Products.Include(p => p.Trademark).Where(p => p.ProductName == productTrademarkViewModel.Products.ProductName
            && p.Trademark.Id == productTrademarkViewModel.Products.TrademarkId);
            var productInDb = GetProductById(productTrademarkViewModel.Products.Id);
            if (checkExists.Any())
            {
                //
            }
            else
            {
                productInDb.ProductName = productTrademarkViewModel.Products.ProductName;
                productInDb.Badge = productTrademarkViewModel.Products.Badge;
                productInDb.Title = productTrademarkViewModel.Products.Title;
                productInDb.TitleURL = productTrademarkViewModel.Products.TitleURL;
                productInDb.Image = productTrademarkViewModel.Products.Image;
                productInDb.Image1 = productTrademarkViewModel.Products.Image1;
                productInDb.Quantity = productTrademarkViewModel.Products.Quantity;
                productInDb.Badge = productTrademarkViewModel.Products.Badge;
                productInDb.Price = productTrademarkViewModel.Products.Price;
                productInDb.Status = productTrademarkViewModel.Products.Status;
                productInDb.StatusMessage = productTrademarkViewModel.Products.StatusMessage;
                _dbContext.SaveChanges();
                return null;
            }

            var model = new ProductTrademarkViewModel
            {
                Products = productTrademarkViewModel.Products,
                StatusMessage = "Error: " + _dbContext.Trademarks.SingleOrDefault(t => t.Id == productTrademarkViewModel.Products.TrademarkId).TrademarkName.ToString()
            };
            return model;
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
                Products = new Product(),
                Trademarks = _dbContext.Trademarks.OrderBy(t => t.TrademarkName).Distinct().ToList(),
            };
            return productViewModel;
        }

        public ProductTrademarkViewModel ImportProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            return null;
        }

        public IEnumerable<Product> GetProductsSale(int id)
        {
            return _dbContext.Products.Include(p => p.Trademark).Where(p => p.Status == true && p.Trademark.Id == id).ToList();
        }

        public CartProductViewModel getTrendingHotSaleProducts()
        {
            //category
            IEnumerable<Product> _trendingProducts = _dbContext.Products.Include(p => p.Trademark).Where(p => p.ChangeStatusBy != null).ToList();
            // list invoices ->> count each product(count not duplicate name) in invoices -> list product have productId + product sales(count each product in invoices) + list information bt productId List
            IEnumerable<Product> _hotProducts = _dbContext.Products.Include(p => p.Trademark).Where(p => p.StatusMessage != null).ToList();

            var newTrendingHotSaleViewModel = new CartProductViewModel()
            {
                trendingProducts = _trendingProducts,
                hotProducts = _hotProducts
            };
            return newTrendingHotSaleViewModel;
        }

        public IEnumerable<Product> GetProductsNew(int id)
        {
            return _dbContext.Products.Include(p => p.Trademark).Where(p => p.Badge != null && p.Trademark.Id == id).ToList();
        }

        public FeedbackProductViewModel GetProductDetail(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            var listFeedBack = _dbContext.FeedbackProducts.Include(f => f.ApplicationUser).Include(p => p.Product).Where(i => i.ProductId == id);
            var model = new FeedbackProductViewModel
            {
                Product = product,
                feedbackProducts = listFeedBack
            };
            return model;
        }
    }
}
