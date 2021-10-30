using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;

namespace Sneaker.Controllers
{
    public class OrderController : Controller
    {
        private readonly IAdminRepo _adminRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly ILogger<UserController> _logger;
        private readonly IItemRepo _itemRepo;
        
        public OrderController(ILogger<UserController> logger, IAdminRepo adminRepo,
            IOrderRepo orderRepo, IItemRepo itemRepo)
        {
            _logger = logger;
            _adminRepo = adminRepo;
            _orderRepo = orderRepo;
            _itemRepo = itemRepo;
        }
        // GET
        public IActionResult Index()
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _orderRepo.GetCartItem(user);

            var orderViewModel = new OrderItemViewModel()
            {
                OrderItems = items,
            };

            return View(orderViewModel);
        }
        public IActionResult CreateNewOrder()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNewOrder(Order order)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            if(!ModelState.IsValid)
            {
                _orderRepo.CreateNewOrder(order, user);
            }
            _logger.LogInformation("...");
            return RedirectToAction("Index");
        }
        
        
        public IActionResult AddCart(int id, Order order)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var cart = _orderRepo.OrderItem(user);
            if(order != null && user != null)
            {
                _orderRepo.CreateNewOrder(order, user);
                var product = _itemRepo.GetItemById(id);
                if (product != null)
                {
                    _orderRepo.AddOrderItem(user, product,1, order);
                    return Json(nameof(Index));
                }
            }
            _logger.LogInformation("...");
            return View(cart);
        }
    }
}