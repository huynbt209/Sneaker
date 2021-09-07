using System.Collections.Generic;

namespace Sneaker.Models
{
    public class Cart
    {
        public int Id {get;set;}
        //public Product Products { get; set; }
        public Item Items { get; set; }
        public int Quantity { get; set; }
        public string UserId {get; set;}
    }
}