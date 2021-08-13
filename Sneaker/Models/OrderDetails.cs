using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Invoice")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public OrderDetails()
        {
            CreateAt = DateTime.Now;
        }
    }
}