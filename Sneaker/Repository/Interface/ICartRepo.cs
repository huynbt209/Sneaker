using Sneaker.Models;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface ICartRepo
    {
        //bool SaveCheckout(string userId);
        void CreateOrder(Order checkout);
        //UserCheckoutViewModel UserCheckoutViewModel(string userId);
        public Task<string> GetUserId(string userId);
    }
}