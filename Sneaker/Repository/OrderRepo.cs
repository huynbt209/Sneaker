using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return _dbContext.OrderItems.Where(c => c.UserId == userId).Include(p => p.Order).ToList();
        }

        public decimal GetCartTotal(string userId)
        {
            var total = _dbContext.OrderItems.Where(c => c.UserId == userId).Select(c => c.Item.Price * c.Quantity)
                .Sum();
            return total;
        }

        public string CreateOrderGroup(string userId)
        {
            Order orderGroup;
            var orderGrCode = RandomGroupCode();
            if (FindOrderExistInDb(userId) != null)
            {
                orderGroup = FindOrderExistInDb(userId);
                if (orderGroup.orderGroupCode == null)
                {
                    orderGroup.orderGroupCode = orderGrCode;
                }
                else
                {
                    orderGrCode = orderGroup.orderGroupCode;
                }
            }
            else
            {
                orderGroup = new Order()
                {
                    OwnerId = userId,
                    IsTeamOrder = true,
                    orderGroupCode = orderGrCode,
                    OrderLocked = false
                };
                _dbContext.Orders.Add(orderGroup);
            }
            _dbContext.SaveChanges();
            
            return orderGrCode;
        }

        private Order FindOrderExistInDb(string userId)
        {
            return _dbContext.Orders.FirstOrDefault(o => o.OwnerId == userId);
        }

        private string RandomGroupCode()
        {
            int length = 15;
      
            // creating a StringBuilder object()
            StringBuilder stringBuilder = new StringBuilder();  
            Random random = new Random();  

            char letter;  

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                stringBuilder.Append(letter);  
            }

            return stringBuilder.ToString();
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
                OwnerId = userId,
                OrderLocked = true
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
        
        public IEnumerable<Order> GetUserOrder(string userId)
        {
            return _dbContext.Orders.Where(i => i.OwnerId == userId).ToList();
        }

        public OrderItemViewModel GetOrderItem(int id)
        {
            var orderInDb = _dbContext.Orders.FirstOrDefault(i => i.Id == id);
            var orderItem = _dbContext.OrderItems.Include(d => d.Item).Where(o => o.OrderId == id).ToList();
            var model = new OrderItemViewModel()
            {
                Orders = orderInDb,
                OrderItems = orderItem,
            };
            return model;
        }
    }
}