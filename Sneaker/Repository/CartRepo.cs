﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sneaker.Data;
using Sneaker.Helpers;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class CartRepo : ICartRepo
    {
        private readonly ApplicationDbContext _dbContext;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public CartRepo(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public Cart cart(string userId)
        {
            var cart = new Cart
            {
                UserId = userId
            };
            return cart;
        }
        public bool AddtoCart(Product product, int quantity, string userId)
        {
            var cart = _dbContext.Carts.SingleOrDefault(c => c.Products.Id == product.Id && c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Products = product,
                    Quantity = quantity,
                    UserId = userId
                };
                _dbContext.Carts.Add(cart);
            }
            else
            {
                cart.Quantity++;
            }
            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<Cart> GetCartItem(string userId)
        {
            return _dbContext.Carts.Where(c => c.UserId == userId).Include(p => p.Products).ToList();
        }

        public decimal GetCartTotal(string userId)
        {
            var total = _dbContext.Carts.Where(c => c.UserId == userId).Select(c => c.Products.Price * c.Quantity).Sum();
            return total;
        }
        public int GetCount(string userId)
        {
            return _dbContext.Carts.Where(c => c.UserId == userId).Sum(c => c.Quantity);
        }

        public bool RemoveCart(int id)
        {
            var CartItem = _dbContext.Carts.SingleOrDefault(c => c.Products.Id == id);
            if (CartItem != null)
            {
                _dbContext.Carts.Remove(CartItem);
            }
            _dbContext.SaveChanges();
            return true;
        }

        public bool ClearCart(int id)
        {
            var cartItems = _dbContext.Carts.Find(id);
            _dbContext.Carts.RemoveRange(cartItems);
            _dbContext.SaveChanges();
            return true;
        }

        public bool CreateOrder(CartViewModel invoiceVM, string userId)
        {
            invoiceVM.Invoices.CreateAt = DateTime.Now;
            invoiceVM.Invoices.OwnerId = userId;
            _dbContext.Invoice.Add(invoiceVM.Invoices);
            _dbContext.SaveChanges();
            decimal orderTotal = 0;
            var cartItems = GetCartItem(userId);
            foreach (var item in cartItems)
            {
                var invoiceDetail = new InvoiceDetails
                {
                    ProductId = item.Products.Id,
                    InvoiceId = invoiceVM.Invoices.Id,
                    Price = item.Products.Price * item.Quantity,
                    Quantity = item.Quantity,
                    UserId = item.UserId,
                };
                orderTotal += (item.Quantity * item.Products.Price);
                _dbContext.InvoiceDetails.Add(invoiceDetail);
            }
            invoiceVM.Invoices.OrderTotal = orderTotal;
            _dbContext.SaveChanges();
            return true;
        }

        public async Task<bool> SubmitOrder(string paymentId, string payerId, CartViewModel invoiceVM)
        {
            var paypalAPI = new PayPalAPI(_configuration);
            await paypalAPI.ExecutedPayment(paymentId, payerId); 
            return true;
        }
    }
}
