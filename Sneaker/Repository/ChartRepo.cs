using System;
using System.Collections.Generic;
using System.Linq;
using Sneaker.Data;
using Sneaker.Repository.Interface;

namespace Sneaker.Repository
{
    public class ChartRepo : IChartRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public ChartRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<double> CountItemOfTrademark()
        {
            List<double> items = new List<double>();
            var trademarkList = _dbContext.Trademarks.OrderBy(t => t.TrademarkName).Distinct().ToList();
            var itemList = _dbContext.Items.ToList();
            var currentYear = DateTime.Now.Year;

            for (var i = 0; i < trademarkList.Count; i++)
            {
                var trademarkId = trademarkList[i].Id;
                var itemCount = itemList.Count(p => p.TrademarkId == trademarkId && p.CreateAt.Year == currentYear);
                items.Add(itemCount);
            }

            return items;
        }

        public List<double> CountProductInInvoiceOfTrademark()
        {
            List<double> items = new List<double>();
            var trademarkList = _dbContext.Trademarks.OrderBy(t => t.TrademarkName).Distinct().ToList();
            var itemInvoiceList = _dbContext.InvoiceDetails.ToList();
            var currentYear = DateTime.Now.Year;
            

            for (var i = 0; i < trademarkList.Count; i++)
            {
                var trademarkId = trademarkList[i].Id;   
                var itemCount = itemInvoiceList.Count(p => p.Item.TrademarkId == trademarkId && p.CreateAt.Year == currentYear);
                items.Add(itemCount);
            }
            return items;
        }

        public List<string> GetTrademarkList()
        {
            List<string> trademarks = new List<string>();

            var trademarkList = _dbContext.Trademarks.OrderBy(t => t.TrademarkName).Distinct().ToList();
            for (var i = 0; i < trademarkList.Count; i++)
            {
                trademarks.Add(trademarkList[i].TrademarkName);
            }

            return trademarks;
        }
    }
}