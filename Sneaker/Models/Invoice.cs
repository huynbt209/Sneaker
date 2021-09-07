using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Models
{
    public partial class Invoice
    {
        [Key]
        public int Id { get; set; }

        public string PaymentId { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public string OwnerId { get; set; }

        public bool IsTeamOrder { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
            ErrorMessage = "The email address is not entered in a correct format")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please enter your Phone Number")]
        public int PhoneNumber { get; set; }

        [Display(Name = "Country")]
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your Country")]
        public string Country { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your State / City")]
        public string State { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your District")]
        public string District { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your Wards")]
        public string Wards { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Please enter your Address")]
        public string Address { get; set; }

        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Please enter your Postal Code")]
        public int PostalCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryFee { get; set; }

        public bool Status { get; set; }

        public string StatusMessage { get; set; }
        public string ChangeStatusBy { get; set; }
        public string Note { get; set; }

        public DateTime CreateAt { get; set; }


        public Invoice()
        {
            CreateAt = DateTime.Now;
        }
    }
}

