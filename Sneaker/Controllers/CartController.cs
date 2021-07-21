using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Sneaker.Helpers;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<UserController> _logger;

        public CartController(ILogger<UserController> logger, IProductRepo productRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
        }

        public IActionResult Index()
        {
            List<Cart> carts = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
            ViewBag.cart= carts;
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
    }
}