using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sneaker.Models
{
    public class FeedbackItem
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public int Rating {get; set;}
        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
        
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public FeedbackItem()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }
    }
}