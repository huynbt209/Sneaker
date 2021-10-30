using System.ComponentModel.DataAnnotations;
using System;


namespace Sneaker.Models
{
    public class Order
    {
        public int Id { get; set; } 
        [Required]
        public string OwnerId { get; set; }
        public bool Status { get; set; }
        public bool IsTeamOrder { get; set; }
        public bool OrderLocked { get; set; }
        public DateTime CreateAt { get; set; }
        
        public string Note { get; set; }

        public Order()
        {
            CreateAt = DateTime.Now;
        }
        
    }
}
