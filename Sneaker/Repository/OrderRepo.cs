using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;

namespace Sneaker.Repository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Order GetOrderOfUser(string userId, int id)
        {
            return _dbContext.Orders.Find(id);
        }

        public OrderItem OrderItem(string userId)
        {
            var orderItem = new OrderItem()
            {
                UserId = userId
            };
            return orderItem;
        }

        public IEnumerable<OrderItem> GetCartItem(string userId)
        {
            return _dbContext.OrderItems.Where(c => c.UserId == userId).Include(p => p.Item).ToList();
        }

        public decimal GetCartTotal(string userId)
        {
            var total = _dbContext.OrderItems.Where(c => c.UserId == userId).Select(c => c.Item.Price * c.Quantity)
                .Sum();
            return total;
        }
        
        public bool CreateNewOrder(Order order, string userId)
        {
            var orderUserExists = _dbContext.Orders.Where(o => o.OwnerId == userId);
            if(orderUserExists.Any())
            {
                return false;
            }
            var newOrder = new Order()
            {
                Note = order.Note,
                OwnerId = userId
            };
            _dbContext.Orders.Add(newOrder);
            _dbContext.SaveChanges();
            return true;
        }

        public bool AddOrderItem( string userId, Item item, int quantity, Order order)
        {
            var getOrderIdOfUser = _dbContext.Orders.Where(o => o.OwnerId == userId).ToList();
            var orderItem = _dbContext.OrderItems.SingleOrDefault(o => o.Item.Id == item.Id && o.UserId == userId);
            if (orderItem == null)
            {
                orderItem = new OrderItem()
                {
                    Item = item,
                    UserId = userId,
                    Quantity = quantity,
                    Price = item.Price,
                    OrderId = getOrderIdOfUser[0].Id,
                };
                _dbContext.OrderItems.Add(orderItem);
            }
            else
            {
                orderItem.Quantity++;
            }

            _dbContext.SaveChanges();
            return true;
        }
        
    }
}