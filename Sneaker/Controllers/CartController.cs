using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sneaker.Helpers;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sneaker.ViewModel;

namespace Sneaker.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly IAdminRepo _adminRepo;
        private readonly ICartRepo _cartRepo;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public CartController(ILogger<UserController> logger, IProductRepo productRepo, IAdminRepo adminRepo,
            ICartRepo cartRepo, IConfiguration configuration)
        {
            _logger = logger;
            _productRepo = productRepo;
            _adminRepo = adminRepo;
            _cartRepo = cartRepo;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _cartRepo.GetCartItem(user);
            var cartViewModel = new CartViewModel
            {
                Carts = items,
                CartTotal = _cartRepo.GetCartTotal(user)
            };
            int count = _cartRepo.GetCount(user);
            ViewBag.cartCount = count;
            return View(cartViewModel);
        }

        public IActionResult AddCart(int id)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var cart = _cartRepo.cart(user);
            var product = _productRepo.GetProductById(id);
            if (product != null)
            {
                _cartRepo.AddtoCart(product, 1, user);
                return Json(nameof(Index));
            }
            return View(cart);
        }

        public IActionResult RemoveCart(int id)
        {
            var product = _productRepo.GetProductById(id);
            if (product != null)
            {
                _cartRepo.RemoveCart(id);
            }
            return RedirectToAction("Index");
        }

        public IActionResult CreateOrder()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateOrder(Invoice invoice)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _cartRepo.GetCartItem(user);
            if (items == null)
            {
                ModelState.AddModelError("", "Add Product First");
            }
            else
            {
                _cartRepo.CreateOrder(invoice, user);
            }
            return View();
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(double total)
        {
            var paypalAPI = new PayPalAPI(_configuration);
            string url = await paypalAPI.getRedirectURLToPayPal(total, "USD");
            return Redirect(url);
        }

        public async Task<IActionResult> Success([FromQuery(Name = "paymentId")] string paymentId, [FromQuery(Name = "PayerId")] string payerId)
        {
            var paypalAPI = new PayPalAPI(_configuration);
            PayPalPaymentExecutedResponse result = await paypalAPI.ExecutedPayment(paymentId, payerId);
            Debug.WriteLine("Transaction Details");
            Debug.WriteLine("cart: " + result.cart);
            Debug.WriteLine("CreateAt: " + result.create_time.ToLongDateString());
            Debug.WriteLine("id: " + result.id);
            Debug.WriteLine("intent: " + result.intent);
            Debug.WriteLine("links 0 - href: " + result.links[0].href);
            Debug.WriteLine("links 0 - method: " + result.links[0].method);
            Debug.WriteLine("links 0 - rel: " + result.links[0].rel);
            Debug.WriteLine("payer_info - first_name: " + result.payer.payer_info.first_name);
            Debug.WriteLine("payer_info - last_name: " + result.payer.payer_info.last_name);
            Debug.WriteLine("payer_info - email: " + result.payer.payer_info.email);
            Debug.WriteLine("payer_info - billing_address: " + result.payer.payer_info.billing_address);
            Debug.WriteLine("payer_info - country_code: " + result.payer.payer_info.country_code);
            Debug.WriteLine("payer_info - shipping_address: " + result.payer.payer_info.shipping_address);
            Debug.WriteLine("payer_info - payer_id: " + result.payer.payer_info.payer_id);
            Debug.WriteLine("state: " + result.state);
            ViewBag.result = result;
            return View("Success");
        }
    }
}