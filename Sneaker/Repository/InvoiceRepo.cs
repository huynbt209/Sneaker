using Microsoft.EntityFrameworkCore;
using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class InvoiceRepo : IInvoiceRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public InvoiceRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Invoice> GetAllInvoices()
        {
            return _dbContext.Invoice.ToList();
        }

        public IEnumerable<InvoiceDetails> GetDetailsByInvoiceId(int invoiceId)
        {
            return _dbContext.InvoiceDetails.Include(i => i.Invoice).Where(i => i.InvoiceId == invoiceId).ToList();
        }

        public Invoice GetInvoiceById(int id)
        {
            return _dbContext.Invoice.SingleOrDefault(i => i.Id == id);
        }

        public IEnumerable<Invoice> GetUserInvoices(string userId)
        {
            return _dbContext.Invoice.Where(i => i.OwnerId == userId).ToList();
        }
    }
}
