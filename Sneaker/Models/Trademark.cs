using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Models
{
    public class Trademark
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Trademark Name")]
        public string TrademarkName { get; set; }
        [Required]
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public Trademark()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }
    }
}
