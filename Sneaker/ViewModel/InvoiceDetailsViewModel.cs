using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class InvoiceDetailsViewModel
    {
        public Invoice Invoice {get ;set;}
        public IEnumerable<InvoiceDetails> Details {get; set;}
    }
}
