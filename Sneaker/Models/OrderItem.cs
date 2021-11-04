using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sneaker.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public Decimal Price { get; set; }
        public string Note { get; set; }
    }
}