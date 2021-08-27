﻿using Microsoft.AspNetCore.Http;
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
            int count = _cartRepo.GetCount(user);
            ViewBag.cartCount = count;
            var items = _cartRepo.GetCartItem(user);

            var cartViewModel = new CartViewModel
            {
                Carts = items,
                CartTotal = _cartRepo.GetCartTotal(user)
            };

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

        [HttpPost]
        public IActionResult SubmitOrder(CartViewModel invoiceVM)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _cartRepo.GetCartItem(user);
            var cartViewModel = new CartViewModel
            {
                Carts = items,
                CartTotal = _cartRepo.GetCartTotal(user)
            };
            _cartRepo.CreateOrder(invoiceVM, user);
            return View(cartViewModel);
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(double total)
        {
            var paypalAPI = new PayPalAPI(_configuration);
            string url = await paypalAPI.getRedirectURLToPayPal(total, "USD");
            return Redirect(url);
        }

        public async Task<IActionResult> SubmitOrder([FromQuery(Name = "paymentId")] string paymentId, [FromQuery(Name = "PayerId")] string payerId, CartViewModel invoiceVM)
        {
            await _cartRepo.SubmitOrder(paymentId, payerId, invoiceVM);
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _adminRepo.GetUserId(currentUser).Result;
            var items = _cartRepo.GetCartItem(user);
            var cartViewModel = new CartViewModel
            {
                Carts = items,
                CartTotal = _cartRepo.GetCartTotal(user),
                PaymentId = paymentId
            };
            int count = _cartRepo.GetCount(user);
            ViewBag.cartCount = count;
            return View(cartViewModel);
        }
    }
}