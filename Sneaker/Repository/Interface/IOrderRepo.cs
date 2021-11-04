using System;
using System.Collections.Generic;
using Sneaker.Models;
using Sneaker.ViewModel;

namespace Sneaker.Repository.Interface
{
    public interface IOrderRepo
    {
        Order GetOrderOfUser(string userId, int id);
        // bool CreateNewOrderItem(OrderItemViewModel orderItemViewModel, string userId);

        bool CreateNewOrder(Order order, string userId);
        bool AddOrderItem( string userId, Item item, int quantity, Order order);

        OrderItem OrderItem(string userId);
        IEnumerable<OrderItem> GetCartItem(string userId);
        decimal GetCartTotal(string userId);

        string CreateOrderGroup(string userId);

        IEnumerable<Order> GetUserOrder(string userId);

        OrderItemViewModel GetOrderItem(int id);

    }
}