using System.Collections.Generic;
using Sneaker.Models;

namespace Sneaker.ViewModel
{
    public class TrendingHotSaleViewModel
    {
        public IEnumerable<Item> trendingProducts { get; set; }
        public IEnumerable<Item> hotProducts { get; set; }
        public IEnumerable<Item> saleProducts { get; set; }
        public IEnumerable<Item> newProducts { get; set; }
        public IEnumerable<InvoiceDetails> hotBuyProduct { get; set; }
    }
}