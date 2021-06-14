using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sneaker.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full Name")] public string FullName { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string ImagePath { get; set; }

        public ApplicationUser()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }
    }
}