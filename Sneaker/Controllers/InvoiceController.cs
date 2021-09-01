using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sneaker.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepo _invoiceRepo;
        private readonly IAdminRepo _adminRepo;
        private readonly ILogger<InvoiceController> _logger;
        public InvoiceController(IInvoiceRepo invoiceRepo, IAdminRepo adminRepo, ILogger<InvoiceController> logger)
        {
            _invoiceRepo = invoiceRepo;
            _adminRepo = adminRepo;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var invoice = _invoiceRepo.GetUserInvoices(user);
            return View(invoice);
        }

        public IActionResult GetUserInvoice()
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var invoice = _invoiceRepo.GetUserInvoices(user);
            return new JsonResult(invoice);
        }

        [HttpPost]
        public IActionResult UpdateInvoiceStatus(int invoiceId, bool status, string statusMessage)
        {   
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserName(currentUser).Result;
            if (statusMessage == null)
            {
                _logger.LogWarning("Message is empty when update invoice status!");
                return Json(new {success = false, message = "Please add a message!"});
            }

            if (_invoiceRepo.UpdateInvoiceStatus(invoiceId, status, statusMessage, user))
            {
                _logger.LogInformation("The invoice has been Transaction Confirmation!");
                return Json(new {success = true, message = "The invoice has been Transaction Confirmation!"});
            }
            _logger.LogInformation("There are some error when confirmation!");
            return Json(new {success = false, message = "There are some error when confirmation!"});
        }

        public IActionResult InvoiceDetail(int id)
        {
            var invoice = _invoiceRepo.GetInvoiceDetails(id);
            if (invoice.Invoice != null)
            _logger.LogInformation($"Open Invoice: {invoice.Invoice.PaymentId}");
            return View(invoice);
        }
    }
}
