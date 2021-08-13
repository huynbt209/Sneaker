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
            List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
            ViewBag.cart = carts;
            ViewBag.Total = carts.Sum(c => c.Products.Price * c.Quantity);
            return View();
        }

        [HttpGet]
        public IActionResult AddCart(int id)
        {
            var product = _productRepo.GetProductById(id);
            if (SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart") == null)
            {
                List<Cart> carts = new List<Cart>();
                carts.Add(new Cart
                {
                    Products = product,
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", carts);
            }
            else
            {
                List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
                int index = exists(id, carts);
                if (index == -1)
                {
                    carts.Add(new Cart
                    {
                        Products = product,
                        Quantity = 1
                    });
                }
                else
                {
                    carts[index].Quantity++;
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", carts);
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult AddCart(int id, int quantity)
        {
            var product = _productRepo.GetProductById(id);
            if (SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart") == null)
            {
                List<Cart> carts = new List<Cart>();
                carts.Add(new Cart
                {
                    Products = product,
                    Quantity = quantity
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", carts);
            }
            else
            {
                List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
                int index = exists(id, carts);
                if (index == -1)
                {
                    carts.Add(new Cart
                    {
                        Products = product,
                        Quantity = quantity
                    });
                }
                else
                {
                    carts[index].Quantity += quantity;
                }

                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", carts);
            }

            return RedirectToAction("Index", "Cart");
        }

        private int exists(int id, List<Cart> carts)
        {
            for (var i = 0; i < carts.Count; i++)
            {
                if (carts[i].Products.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        [HttpPost]
        public IActionResult UpdateCart(int[] quantity)
        {
            List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
            for (var i = 0; i < carts.Count; i++)
            {
                carts[i].Quantity = quantity[i];
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", carts);
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult DeleteCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Products.Id == id)
                    {
                        dataCart.RemoveAt(i);
                    }
                }

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
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