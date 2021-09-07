using System.Collections.Generic;
using Sneaker.Models;

namespace Sneaker.ViewModel
{
    public class FeedbackItemViewModel
    {
        public Item Items {get; set;}
        public IEnumerable<FeedbackItem> FeedbackItems {get; set;}
    }
}