using System.Collections.Generic;
using Sneaker.Models;

namespace Sneaker.ViewModel
{
    public class OrderItemViewModel
    {
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public Item Items { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
        public Order Orders { get; set; }
    }
}