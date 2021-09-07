using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;

namespace Sneaker.Repository
{
    public class ItemRepo : IItemRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public ItemRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Item> GetProducts()
        {
            return _dbContext.Items.Include(p => p.Trademark).ToList();
        }

        public IEnumerable<Item> GetProductByTrademark(int trademarkId)
        {
            var products = _dbContext.Items.Include(p => p.Trademark).Where(p => p.TrademarkId == trademarkId).ToList();
            return products;
        }

        public IEnumerable<Trademark> GetTrademarks()
        {
            return _dbContext.Trademarks.ToList();
        }


        public ItemTrademarkViewModel ProductTrademarkViewModel()
        {
            var productViewModel = new ItemTrademarkViewModel()
            {
                Items = new Item(),
                Trademarks = _dbContext.Trademarks.OrderBy(t => t.TrademarkName).Distinct().ToList(),
            };
            return productViewModel;
        }

        public IEnumerable<Item> GetProductInTrademarkId(int id)
        {
            var listProductIdInSelectedTrademark =
                _dbContext.Items.Where(t => t.TrademarkId == id).Select(p => p.Id).ToArray();
            var list = _dbContext.Items.Where(p => listProductIdInSelectedTrademark.Any(t => t.Equals(p.Id))).ToList();
            return list;
        }

        public ItemTrademarkViewModel CreateProduct(ItemTrademarkViewModel productTrademarkViewModel)
        {
            var checkExists = _dbContext.Items.Include(p => p.Trademark).Where(p =>
                p.ProductName == productTrademarkViewModel.Items.ProductName
                && p.Trademark.Id == productTrademarkViewModel.Items.TrademarkId);
            if (checkExists.Any())
            {
                //
            }
            else
            {
                productTrademarkViewModel.Items.CreateAt = DateTime.Now;
                _dbContext.Items.Add(productTrademarkViewModel.Items);
                _dbContext.SaveChanges();
            }

            var newProduct = new ItemTrademarkViewModel()
            {
                Items = productTrademarkViewModel.Items,
                Trademarks = _dbContext.Trademarks.ToList(),
                StatusMessage = "Error: " + _dbContext.Trademarks
                    .SingleOrDefault(t => t.Id == productTrademarkViewModel.Items.TrademarkId).TrademarkName.ToString()
            };
            return newProduct;
        }

        public Item GetItemById(int id)
        {
            return _dbContext.Items.SingleOrDefault(i => i.Id == id);
        }

        public FeedbackItemViewModel GetItemDetails(int id)
        {
            var product = _dbContext.Items.FirstOrDefault(p => p.Id == id);
            var listFeedBack = _dbContext.FeedbackItems.Include(f => f.ApplicationUser)
                .Include(p => p.Item).Where(i => i.ItemId == id);
            var model = new FeedbackItemViewModel()
            {
                Items = product,
                FeedbackItems = listFeedBack
            };
            return model;
        }

        public IEnumerable<Item> GetProductsSale(int id)
        {
            return _dbContext.Items.Include(p => p.Trademark).Where(p => p.Status == true && p.Trademark.Id == id)
                .ToList();
        }

        public IEnumerable<Item> GetProductsNew(int id)
        {
            return _dbContext.Items.Include(p => p.Trademark).Where(p => p.Badge != null && p.Trademark.Id == id)
                .ToList();
        }

        public bool RemoveProduct(int id)
        {
            var productInDb = GetItemById(id);
            if (productInDb == null)
            {
                return false;
            }

            _dbContext.Items.Remove(productInDb);
            _dbContext.SaveChanges();
            return true;
        }

        public TrendingHotSaleViewModel GetTrendHotSaleNewProducts()
        {
            IEnumerable<Item> _trendingProducts =
                _dbContext.Items.Include(p => p.Trademark).Where(p => p.ChangeStatusBy != null).ToList();
            // list invoices ->> count each product(count not duplicate name) in invoices -> list product have productId + product sales(count each product in invoices) + list information bt productId List
            IEnumerable<Item> _hotProducts =
                _dbContext.Items.Include(p => p.Trademark).Where(p => p.StatusMessage != null).ToList();
            IEnumerable<Item> _saleProducts =
                _dbContext.Items.Include(p => p.Trademark).Where(p => p.Status == true).ToList();
            IEnumerable<Item> _newProducts =
                _dbContext.Items.Include(p => p.Trademark).Where(p => p.Badge != null).ToList();


            var newTrendingHotSaleViewModel = new TrendingHotSaleViewModel()
            {
                trendingProducts = _trendingProducts,
                hotProducts = _hotProducts,
                saleProducts = _saleProducts,
                newProducts = _newProducts
            };
            return newTrendingHotSaleViewModel;
        }

        public ItemTrademarkViewModel EditProduct(ItemTrademarkViewModel itemTrademarkViewModel)
        {
            var checkExists = _dbContext.Items.Include(p => p.Trademark)
                .Where(p => p.ProductName == itemTrademarkViewModel.Items.ProductName
                            && p.Trademark.Id == itemTrademarkViewModel.Items.TrademarkId);

            var itemInDb = GetItemById(itemTrademarkViewModel.Items.Id);
            if (checkExists.Any() && itemInDb.ProductName != itemTrademarkViewModel.Items.ProductName)
            {
                //
            }
            else
            {
                itemInDb.ProductName = itemTrademarkViewModel.Items.ProductName;
                itemInDb.Badge = itemTrademarkViewModel.Items.Badge;
                itemInDb.Title = itemTrademarkViewModel.Items.Title;
                itemInDb.TitleURL = itemTrademarkViewModel.Items.TitleURL;
                itemInDb.Image = itemTrademarkViewModel.Items.Image;
                itemInDb.Image1 = itemTrademarkViewModel.Items.Image1;
                itemInDb.Quantity = itemTrademarkViewModel.Items.Quantity;
                itemInDb.Badge = itemTrademarkViewModel.Items.Badge;
                itemInDb.Price = itemTrademarkViewModel.Items.Price;
                itemInDb.Status = itemTrademarkViewModel.Items.Status;
                itemInDb.StatusMessage = itemTrademarkViewModel.Items.StatusMessage;
                _dbContext.SaveChanges();
                return null;
            }

            var model = new ItemTrademarkViewModel()
            {
                Items = itemTrademarkViewModel.Items,
                StatusMessage = "Error: " + _dbContext.Trademarks
                    .SingleOrDefault(t => t.Id == itemTrademarkViewModel.Items.TrademarkId).TrademarkName.ToString()
            };
            return model;
        }
    }
}