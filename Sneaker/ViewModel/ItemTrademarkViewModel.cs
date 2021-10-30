using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sneaker.Models;

namespace Sneaker.ViewModel
{
    public class ItemTrademarkViewModel
    {
        public Item Items { get; set; }
        public IEnumerable<Trademark> Trademarks { get; set; }
        public string StatusMessage { get; set; }
        
        public IFormFile FormFile { get; set; }
    }
}