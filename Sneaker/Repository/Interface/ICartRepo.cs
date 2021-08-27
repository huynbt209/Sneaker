using Sneaker.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface ICartRepo
    {
        bool AddtoCart(Product product, int quantity, string userId);
        Cart cart(string userId);
        IEnumerable<Cart> GetCartItem(string userId);
        decimal GetCartTotal(string userId);
        int GetCount(string userId);
        bool RemoveCart(int id);
        bool ClearCart(int id);
        bool CreateOrder(Invoice invoice, string userId);
    }
}