using Microsoft.AspNetCore.Hosting;
using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class TrademarkRepo : ITrademarkRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public TrademarkRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    //
        public bool CreateNewTrademark(Trademark trademark)
        {
            if(CheckExist(trademark.TrademarkName))
            {
                return false;
            }
            var newTrademark = new Trademark
            {
                TrademarkName = trademark.TrademarkName,
                Description = trademark.Description
            };
            _dbContext.Trademarks.Add(newTrademark);
            _dbContext.SaveChanges();
            return true;
        }
    //
        private bool CheckExist(string trademarkName)
        {
            return _dbContext.Trademarks.Any(tr => tr.TrademarkName.Contains(trademarkName));
        }
    //
        public bool EditTrademark(Trademark trademark)
        {
            if(CheckExist(trademark.TrademarkName))
            {
                return false;
            }
            var newTrademark = new Trademark
            {
                TrademarkName = trademark.TrademarkName,
                Description = trademark.Description
            };
            _dbContext.Trademarks.Update(newTrademark);
            _dbContext.SaveChanges();
            return true;
        }
    //
        public Trademark GetTrademarkById(int id)
        {
            return _dbContext.Trademarks.SingleOrDefault(tr => tr.Id == id);
        }

        public IEnumerable<Trademark> GetTrademarks()
        {
            return _dbContext.Trademarks.ToList();
        }
    //
        public bool RemoveTrademark(int id)
        {
            var trademarkInDb = GetTrademarkById(id);
            if (trademarkInDb == null)
            {
                return false;
            }
            _dbContext.Trademarks.Remove(trademarkInDb);
            _dbContext.SaveChanges();
            return true;
        }
    //
    }
}
