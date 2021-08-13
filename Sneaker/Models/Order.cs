using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public int PostalCode { get; set; }
        public decimal OrderTotal {get; set;}
        
        public bool Status { get; set; }
        public DateTime CreateAt { get; set; }        
        
        public List<OrderDetails> CheckoutDetailses { get; set; }
        
        public Order()
        {
            CreateAt = DateTime.Now;
        }
    }
}

