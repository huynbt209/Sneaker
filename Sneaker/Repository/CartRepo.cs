using Microsoft.AspNetCore.Http;
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

        public void CreateOrder(Order order)
        {
            order.CreateAt = DateTime.Now;
            _dbContext.Orders.Add(order);
            List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(_httpContextAccessor.HttpContext.Session, "cart");
            foreach (var item in carts)
            {
                var checkoutDetails = new OrderDetails
                {
                    OrderId = order.Id,
                    ProductId = item.Id,
                    Price = carts.Sum(c => c.Products.Price * c.Quantity),
                    Quantity = item.Quantity
                };
                _dbContext.OrderDetails.Add(checkoutDetails);
            }
            _dbContext.SaveChanges();
        }

        public async Task<string> GetUserId(string userId)
        {
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? "Unknown" : user.Id;
        }

        public IEnumerable<Product> Products => _dbContext.Products.Include(c => c.Trademark);

        //public UserCheckoutViewModel UserCheckoutViewModel(string userId)
        //{
        //    var userCheckoutViewModel = new UserCheckoutViewModel
        //    {
        //        Checkout = new Checkout(),
        //        UserId = userId
        //    };
        //    return userCheckoutViewModel;
        //}
    }
}
