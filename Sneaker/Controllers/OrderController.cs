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
            var order = _orderRepo.GetUserOrder(user);
            return View(order);
        }
        
        public IActionResult OrderItem(int id)
        {
            var order = _orderRepo.GetOrderItem(id);
            if (order.Orders != null)
            {
                
            }
            _logger.LogInformation($"Open Invoice: {order.Orders.Id}");
            return View(order);
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
        
        
        public IActionResult ShareOrder()
        {   
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _adminRepo.GetUserId(currentUser).Result;

            var groupOrderCode = _orderRepo.CreateOrderGroup(userId);
            if (groupOrderCode!= null)
            {
                _logger.LogInformation("Create group order Successfully!");
            }

            return RedirectToAction("Index");
            /*if (_orderRepo.createOrderGroup(orderId, isTeamOrder, note, user))
            {
                _logger.LogInformation("Order has been opened!");
                return Json(new {success = true, message = "Invite friends to shop together!"});
            }
            _logger.LogInformation("There are some error when confirmation!");
            return Json(new {success = false, message = "There are some error when confirmation!"});*/
        }
    }
}