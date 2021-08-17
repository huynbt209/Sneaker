using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class CartProductViewModel
    {
        public decimal CartTotal { get; set; }
        public IEnumerable<Product> trendingProducts { get; set; }
        
        public IEnumerable<Product> hotProducts { get; set; }
    }
}
