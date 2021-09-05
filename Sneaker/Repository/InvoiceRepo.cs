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
            var invoiceInDb = _dbContext.Invoice.ToList();
            return invoiceInDb;
        }

        public IEnumerable<InvoiceDetails> GetDetailsByInvoiceId(int invoiceId)
        {
            return _dbContext.InvoiceDetails.Include(i => i.Invoice).Where(i => i.InvoiceId == invoiceId).ToList();
        }

        public Invoice GetInvoiceById(int id)
        {
            return _dbContext.Invoice.SingleOrDefault(i => i.Id == id);
        }

        public InvoiceDetailsViewModel GetInvoiceDetails(int id)
        {
            var invoice = _dbContext.Invoice.FirstOrDefault(i => i.Id == id);
            var invoiceDetails = _dbContext.InvoiceDetails.Include(d => d.Product).Where(d => d.InvoiceId == id).ToList();
            var model = new InvoiceDetailsViewModel
            {
                Invoice = invoice,
                Details = invoiceDetails,
            };
            return model;
        }


        public IEnumerable<Invoice> GetUserInvoices(string userId)
        {
            return _dbContext.Invoice.Where(i => i.OwnerId == userId).ToList();
        }

        public bool UpdateInvoiceStatus(int id, bool status, string message, string changeStatus)
        {
            var invoiceInDb = _dbContext.Invoice.FirstOrDefault(a => a.Id == id);
            if (invoiceInDb == null) return false;
            invoiceInDb.Status = status;
            invoiceInDb.StatusMessage = message;
            invoiceInDb.ChangeStatusBy = changeStatus;
            _dbContext.Invoice.Update(invoiceInDb);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
