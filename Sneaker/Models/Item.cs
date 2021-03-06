using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sneaker.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Display(Name = "Title URL")]
        public string TitleUrl { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Image1 { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public Decimal Price { get; set; }
        public string Badge { get; set; }
        public string Category { get; set; }
        [Display(Name = "Colors")]
        public string ProductCard { get; set; }
        public bool Status { get; set; }
        public string StatusMessage { get; set; }
        public string ChangeStatusBy { get; set; }
        public DateTime CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        [Display(Name = "Trademark")]
        public int TrademarkId { get; set; }
        [ForeignKey("TrademarkId")]
        public virtual Trademark Trademark { get; set; }

        public Item()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }
    }
}
