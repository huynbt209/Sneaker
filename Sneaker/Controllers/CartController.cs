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
        private readonly CartItem _cartItem;

        public CartController(ILogger<UserController> logger, IProductRepo productRepo, IAdminRepo adminRepo,
            ICartRepo cartRepo, IConfiguration configuration, CartItem cartItem)
        {
            _logger = logger;
            _productRepo = productRepo;
            _adminRepo = adminRepo;
            _cartRepo = cartRepo;
            _configuration = configuration;
            _cartItem = cartItem;
        }
        public ViewResult Index()
        {
            var items = _cartItem.GetCartItems();
            _cartItem.Carts = items;
            var cartViewModel = new CartViewModel
            {
                CartItem = _cartItem,
                CartTotal = _cartItem.GetCartTotal()
            };

            return View(cartViewModel);
        }

        public RedirectToActionResult AddCart(int id)
        {
            var product = _productRepo.GetProductById(id);
            if (product != null)
            {
                _cartItem.AddToCart(product, 1);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveCart(int id)
        {
            var product = _productRepo.GetProductById(id);
            if (product != null)
            {
                _cartItem.RemoveCart(product);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Order()
        {
            return View();
        }
        public IActionResult Order(Order order)
        {
            List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
            ViewBag.cart = carts;
            ViewBag.Total = carts.Sum(c => c.Products.Price * c.Quantity);
            foreach (var item in carts)
            {
                if (item == null)
                {
                    ModelState.AddModelError("", "Your card is empty, add some products first!");
                }
                if (ModelState.IsValid)
                {
                    _cartRepo.CreateOrder(order);
                    return RedirectToAction("Success");
                }
            }
            return View(order);
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