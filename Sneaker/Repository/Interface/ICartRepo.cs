using Sneaker.Models;
using System;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface ICartRepo
    {
        void CreateOrder(Order checkout);
        //UserCheckoutViewModel UserCheckoutViewModel(string userId);
        public Task<string> GetUserId(string userId);
    }
}