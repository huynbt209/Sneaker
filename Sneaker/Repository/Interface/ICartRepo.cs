﻿using Sneaker.Models;
using Sneaker.ViewModel;
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
        bool RemoveCart(int id, string userId);
        void EmptyCart(string userId);
        Cart GetCartById(int id);
        bool CreateOrder(CartViewModel cartViewModel, string userId);
        bool CreateOrderDetail(Invoice invoice, string userId);
        Task<bool> SubmitOrder(string paymentId, string payerId, CartViewModel cartViewModel);
    }
}