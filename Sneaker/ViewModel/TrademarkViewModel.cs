using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class TrademarkViewModel
    {
        [Required]
        [Display(Name = "Trademark Name")]
        public string TrademarkName { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
