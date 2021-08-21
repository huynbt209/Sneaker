using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sneaker.Models;

namespace Sneaker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Trademark> Trademarks { get; set; }
        public DbSet<FeedbackProduct> FeedbackProducts { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }

        public DbSet<Invoice> Invoice { get; set; }
    }
}
