using System.Collections.Generic;
using Sneaker.Models;

namespace Sneaker.ViewModel
{
    public class OrderItemViewModel
    {
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public Order Orders { get; set; }
    }
}