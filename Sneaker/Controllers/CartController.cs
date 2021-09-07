using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Helpers;
using Sneaker.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sneaker.ViewModel;

namespace Sneaker.Controllers
{
    public class CartController : Controller
    {
        private readonly IAdminRepo _adminRepo;
        private readonly ICartRepo _cartRepo;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IItemRepo _itemRepo;
        

        public CartController(ILogger<UserController> logger, IAdminRepo adminRepo,
            ICartRepo cartRepo, IConfiguration configuration, IItemRepo itemRepo)
        {
            _logger = logger;
            _adminRepo = adminRepo;
            _cartRepo = cartRepo;
            _configuration = configuration;
            _itemRepo = itemRepo;
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

            return View(cartViewModel);
        }

        [AllowAnonymous]
        public IActionResult GetCartCountUser()
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var countCart = _cartRepo.GetCount(user);
            return new JsonResult(countCart);
        }

        public IActionResult AddCart(int id)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var cart = _cartRepo.cart(user);
            var product = _itemRepo.GetItemById(id);
            if (product != null)
            {
                _cartRepo.AddtoCart(product, 1, user);
                return Json(nameof(Index));
            }
            return View(cart);
        }

        public IActionResult RemoveCart(int id)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var product = _itemRepo.GetItemById(id);
            if (product != null)
            {
                _cartRepo.RemoveCart(id, user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(double total)
        {
            var paypalAPI = new PayPalAPI(_configuration);
            string url = await paypalAPI.getRedirectURLToPayPal(total, "USD");
            return Redirect(url);
        }

        public async Task<IActionResult> SubmitOrder([FromQuery(Name = "paymentId")] string paymentId, [FromQuery(Name = "PayerId")] string payerId, CartViewModel cartViewModel)
        {
            await _cartRepo.SubmitOrder(paymentId, payerId, cartViewModel);
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _cartRepo.GetCartItem(user);
            var newCartViewModel = new CartViewModel
            {
                Carts = items,
                CartTotal = _cartRepo.GetCartTotal(user),
                PaymentId = paymentId
            };
            return View(newCartViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder(CartViewModel cartViewModel)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _cartRepo.GetCartItem(user);
            var newCartViewModel = new CartViewModel
            {
                Carts = items,
                CartTotal = _cartRepo.GetCartTotal(user)
            };
            _cartRepo.CreateOrder(cartViewModel, user);
            if (items != null)
            {
                _cartRepo.EmptyCart(user);
            }
            return View(newCartViewModel);
        }
    }
}