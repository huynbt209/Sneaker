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
    }
}
