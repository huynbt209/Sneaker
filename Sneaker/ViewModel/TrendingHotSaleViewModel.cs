using System.Collections.Generic;
using Sneaker.Models;

namespace Sneaker.ViewModel
{
    public class TrendingHotSaleViewModel
    {
        public IEnumerable<Product> trendingProducts { get; set; }
        
        public IEnumerable<Product> hotProducts { get; set; }
    }
}