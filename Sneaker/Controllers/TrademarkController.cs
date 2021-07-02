using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.Utility;
using System.Linq;

namespace Sneaker.Controllers
{
    [Authorize(Roles = (SystemRoles.Administrator))]
    public class TrademarkController : Controller
    {
        private readonly ITrademarkRepo _trademarkRepo;
        private readonly ILogger<TrademarkController> _logger;

        public TrademarkController(ITrademarkRepo trademarkRepo, ILogger<TrademarkController> logger)
        {
            _trademarkRepo = trademarkRepo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_trademarkRepo.GetTrademarks().ToList());
        }

        public IActionResult GetTrademarkInDb()
        {
            var trademarks = _trademarkRepo.GetTrademarks();
            return new JsonResult(trademarks);
        }

        [HttpGet]
        public IActionResult CreateTrademark()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTrademark(Trademark trademark)
        {
            if(ModelState.IsValid)
            {
                _trademarkRepo.CreateNewTrademark(trademark);
            }
            _logger.LogInformation("...");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditTrademark (int id)
        {
            var trademarkId = _trademarkRepo.GetTrademarkById(id);
            return View(trademarkId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTrademark(Trademark trademark)
        {
            if(ModelState.IsValid)
            {
                _trademarkRepo.EditTrademark(trademark);
            }

            _logger.LogInformation("...");
            return RedirectToAction("Index");
        }

        //
        public IActionResult RemoveTrademark(int id)
        {
            _trademarkRepo.RemoveTrademark(id);
            _logger.LogInformation("////");
            return RedirectToAction("Index");
        }
    }
}
