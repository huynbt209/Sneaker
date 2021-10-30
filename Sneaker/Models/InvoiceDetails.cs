using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Sneaker.Models
{
    public class InvoiceDetails
    {      
        public int Id { get; set; }
        [Required]
        public string UserId {get; set;}
        
        [Required]
        [Display(Name = "Invoice")]
        public int InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public InvoiceDetails()
        {
            CreateAt = DateTime.Now;
        }
    }
}