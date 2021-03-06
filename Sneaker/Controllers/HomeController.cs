using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITrademarkRepo _trademarkRepo;
        private readonly IItemRepo _itemRepo;


        public HomeController(ILogger<HomeController> logger, ITrademarkRepo trademarkRepo, IItemRepo itemRepo)
        {
            _logger = logger;
            _trademarkRepo = trademarkRepo;
            _itemRepo = itemRepo;
        }

        public IActionResult Index()
        {
            return View(_itemRepo.GetTrendHotSaleNewProducts());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}