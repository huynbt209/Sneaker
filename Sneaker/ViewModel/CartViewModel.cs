using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class CartViewModel
    {
        public CartItem CartItem { get; set; }
        public decimal CartTotal { get; set; }
    }
}
