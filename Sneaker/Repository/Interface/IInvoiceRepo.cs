using Sneaker.Models;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface IInvoiceRepo
    {
        IEnumerable<Invoice> GetUserInvoices (string userId);
        IEnumerable<Invoice> GetAllInvoices();
        Invoice GetInvoiceById (int id);
        IEnumerable<InvoiceDetails> GetDetailsByInvoiceId (int invoiceId);
        
    }
}
