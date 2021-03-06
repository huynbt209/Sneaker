using Sneaker.Models;
using Sneaker.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface ICartRepo
    {
        bool AddToCart(Item item, int quantity, string userId);
        bool CreateShoppingCart(ShoppingCart shoppingCart, string userId);
        Cart Cart(string userId);
        IEnumerable<Cart> GetCartItem(string userId);
        decimal GetCartTotal(string userId);
        int GetCount(string userId);
        bool RemoveCart(int id, string userId);
        void EmptyCart(string userId);
        Cart GetCartById(int id);
        bool CreateOrder(CartViewModel cartViewModel, string userId);
        Task<bool> SubmitOrder(string paymentId, string payerId, CartViewModel cartViewModel);
    }
}