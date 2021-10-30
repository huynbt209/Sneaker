using Microsoft.AspNetCore.Mvc;
using Sneaker.Repository.Interface;

namespace Sneaker.Controllers
{
    public class ChartController : Controller
    {
        private readonly IChartRepo _chartRepo;

        public ChartController(IChartRepo chartRepo)
        {
            _chartRepo = chartRepo;
        }
        // GET
        public IActionResult Index()
        {
            ViewBag.TrademarkList = _chartRepo.GetTrademarkList();
            ViewBag.CountItemOfTrademark = _chartRepo.CountItemOfTrademark();
            return View();
        }

        [HttpPost]
        public IActionResult Index(int selectedYear)
        {
            ViewBag.TrademarkList = _chartRepo.GetTrademarkList();
            ViewBag.CountItemOfTrademark = _chartRepo.CountItemOfTrademark();
            ViewBag.Year = selectedYear;
            return View();
        }
    }
}