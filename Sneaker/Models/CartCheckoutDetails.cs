using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sneaker.Models
{
    public class CheckoutDetails
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Invoice")]
        public int CheckoutId { get; set; }

        [ForeignKey("CheckoutId")]
        public virtual Checkout Checkout { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public CheckoutDetails()
        {
            CreateAt = DateTime.Now;
        }
    }
}