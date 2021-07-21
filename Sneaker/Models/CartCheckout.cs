using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sneaker.Models
{
    public class Checkout
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public bool Status { get; set; }
        public DateTime CreateAt { get; set; }
        
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        
        public virtual ICollection<CheckoutDetails> CheckoutDetailses { get; set; }

        
        public Checkout()
        {
            CreateAt = DateTime.Now;
            CheckoutDetailses = new HashSet<CheckoutDetails>();
        }
    }
}