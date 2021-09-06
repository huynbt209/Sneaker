using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class ProductTrademarkViewModel
    {
        public Product Products { get; set; }
        public IEnumerable<Trademark> Trademarks { get; set; }
        public string StatusMessage { get; set; }
    }
}
