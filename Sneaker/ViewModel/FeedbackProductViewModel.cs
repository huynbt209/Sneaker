using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class FeedbackProductViewModel
    {
        public Product Product {get ;set;}
        public IEnumerable<FeedbackProduct> feedbackProducts {get;set;}
        
    }
}
