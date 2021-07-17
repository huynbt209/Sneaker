﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly ILogger<UserController> _logger;
        private readonly ITrademarkRepo _trademarkRepo;
        private readonly IProductRepo _productRepo;
        public UserController(IUserRepo userRepo, ILogger<UserController> logger, ITrademarkRepo trademarkRepo, IProductRepo productRepo)
        {
            _userRepo = userRepo;
            _logger = logger;
            _trademarkRepo = trademarkRepo;
            _productRepo = productRepo;
        }
        public IActionResult Index()
        {
            return View(_productRepo.GetProducts());
        }
        public IActionResult GetListProducts(int id)
        {
            var listProducts = _productRepo.GetProductByTrademark(id);
            _logger.LogInformation("Display list products!");
            return new JsonResult(listProducts);
        }
        public IActionResult ListProducts(int id)
        {
            var listProducts = _productRepo.GetProductByTrademark(id);
            _logger.LogInformation("Display list products!");
            return View(listProducts);
        }

        public IActionResult GetProductSaleUser(int id)
        {
            var listSale = _userRepo.GetProductsSaleUser(id);
            _logger.LogInformation("Display list products sale!");
            return new JsonResult(listSale);
        }
        public IActionResult ListSale(int id)
        {
            var listSaleUser = _userRepo.GetProductsSaleUser(id);
            _logger.LogInformation("Display list products sale for user!");
            return View(listSaleUser);
        }

        public IActionResult GetProductNewUser(int id)
        {
            var listNew = _productRepo.GetProductsNew(id);
            _logger.LogInformation("Display list products new!");
            return new JsonResult(listNew);
        }
        public IActionResult ListNew(int id)
        {
            var listNew = _productRepo.GetProductsNew(id);
            _logger.LogInformation("Display list products new!");
            return View(listNew);
        }
    }
}