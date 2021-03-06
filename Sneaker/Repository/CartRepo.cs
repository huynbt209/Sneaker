using Microsoft.AspNetCore.Http;
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

        public bool CreateShoppingCart(ShoppingCart shoppingCart, string userId)
        {
            var newShoppingCart = new ShoppingCart()
            {
                OwnerName = shoppingCart.OwnerName,
                Note = shoppingCart.Note,
                OwnerId = userId,
                CreateAt = DateTime.Now
            };
            _dbContext.ShoppingCarts.Add(newShoppingCart);
            _dbContext.SaveChanges();
            return true;
        }

        public Cart Cart(string userId)
        {
            var cart = new Cart
            {
                UserId = userId
            };
            return cart;
        }
        public bool AddToCart(Item item, int quantity, string userId)
        {
            var cart = _dbContext.Carts.SingleOrDefault(c => c.Items.Id == item.Id && c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Items = item,
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
            return _dbContext.Carts.Where(c => c.UserId == userId).Include(p => p.Items).ToList();
        }

        public decimal GetCartTotal(string userId)
        {
            var total = _dbContext.Carts.Where(c => c.UserId == userId).Select(c => c.Items.Price * c.Quantity).Sum();
            return total;
        }
        public int GetCount(string userId)
        {
            return _dbContext.Carts.Where(c => c.UserId == userId).Sum(c => c.Quantity);
        }

        public bool RemoveCart(int id, string userId)
        {
            var cartItem = _dbContext.Carts.SingleOrDefault(c => c.Items.Id == id && c.UserId == userId);
            if (cartItem != null)
            {
                _dbContext.Carts.Remove(cartItem);
            }
            _dbContext.SaveChanges();
            return true;
        }

        public void EmptyCart(string userId)
        {
            var cartItems = _dbContext.Carts.Where(c => c.UserId == userId);
            foreach (var cartItem in cartItems)
            {
                _dbContext.Carts.RemoveRange(cartItem);
            }
            _dbContext.SaveChanges();
        }

        public Cart GetCartById(int id)
        {
            return _dbContext.Carts.Find(id);
        }

        private Invoice CreateInvoice(Invoice invoice)
        {
            var result= _dbContext.Invoices.Add(invoice);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        
        public bool CreateOrder(CartViewModel cartViewModel, string userId)
        {
            cartViewModel.Invoices.CreateAt = DateTime.Now;
            cartViewModel.Invoices.OwnerId = userId;
            var invoice = CreateInvoice(cartViewModel.Invoices); 
            CreateOrderDetail(invoice, userId);
            return true;
        }
        

        private void CreateOrderDetail(Invoice invoice, string userId)
        {
            decimal orderTotal = 0;
            List<InvoiceDetails> detailsList = new List<InvoiceDetails>();
            var cartItems = GetCartItem(userId);
            foreach (var item in cartItems)
            {
                var invoiceDetail = new InvoiceDetails()
                {
                    ItemId = item.Items.Id,
                    InvoiceId = invoice.Id,
                    Price = item.Items.Price * item.Quantity,
                    Quantity = item.Quantity,
                    UserId = item.UserId,
                };
                detailsList.Add(invoiceDetail);
                orderTotal += (item.Quantity * item.Items.Price);
            }
            _dbContext.InvoiceDetails.AddRange(detailsList);
            invoice.OrderTotal = orderTotal;
            _dbContext.SaveChanges();
        }

        public async Task<bool> SubmitOrder(string paymentId, string payerId, CartViewModel cartViewModel)
        {
            var paypalAPI = new PayPalAPI(_configuration);
            await paypalAPI.ExecutedPayment(paymentId, payerId); 
            return true;
        }
    }
}
