using Microsoft.AspNetCore.Mvc;
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
        public UserController(IUserRepo userRepo, ILogger<UserController> logger, ITrademarkRepo trademarkRepo)
        {
            _userRepo = userRepo;
            _logger = logger;
            _trademarkRepo = trademarkRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetProductSaleUser()
        {
            var listSale = _userRepo.GetProductsSaleUser();
            _logger.LogInformation("Display list products sale!");
            return new JsonResult(listSale);
        }
        public IActionResult ListSale()
        {
            var listSaleUser = _userRepo.GetProductsSaleUser();
            _logger.LogInformation("Display list products sale for user!");
            return View(listSaleUser);
        }
    }
}
