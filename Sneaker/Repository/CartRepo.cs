﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sneaker.Data;
using Sneaker.Helpers;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class CartRepo : ICartRepo
    {
        private readonly ApplicationDbContext _dbContext;
        private IHttpContextAccessor _httpContextAccessor;
        public CartRepo(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
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
            return _dbContext.Carts.Where(c => c.UserId == userId).Count();
        }

        public bool RemoveCart(int id)
        {
            var CartItem = _dbContext.Carts.SingleOrDefault(c => c.Products.Id == id);
            var localAmount = 0;
            if (CartItem != null)
            {
                if (CartItem.Quantity > 1)
                {
                    CartItem.Quantity--;
                    localAmount = CartItem.Quantity;
                }
                else
                {
                    _dbContext.Carts.Remove(CartItem);
                }
            }
            _dbContext.SaveChanges();
            return true;
        }

        public bool ClearCart(int id)
        {
            var cartItems = _dbContext.Carts.Where(c => c.Id == id);
            _dbContext.Carts.RemoveRange(cartItems);
            _dbContext.SaveChanges();
            return true;
        }

        public bool CreateOrder(CartViewModel invoiceVM, string userId)
        {
            invoiceVM.Invoices.CreateAt = DateTime.Now;
            invoiceVM.Invoices.OwnerId = userId;
            _dbContext.Invoice.Add(invoiceVM.Invoices);
            decimal orderTotal = 0;
            var cartItems = GetCartItem(userId);
            foreach (var item in cartItems)
            {
                var orderDetail = new InvoiceDetails
                {
                    ProductId = item.Products.Id,
                    InvoiceId = invoiceVM.Invoices.Id,
                    Price = item.Products.Price,
                    Quantity = item.Quantity,
                    UserId = item.UserId,
                };
                orderTotal += (item.Quantity * item.Products.Price);
                _dbContext.InvoiceDetails.Add(orderDetail);
            }
            invoiceVM.Invoices.OrderTotal = orderTotal;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
