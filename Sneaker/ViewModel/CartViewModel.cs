using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class CartViewModel
    {
        public decimal CartTotal { get; set; }
        public IEnumerable<Cart> Carts {get ; set;}
        public IEnumerable<Invoice> Invoices {get; set;}
    }
}
