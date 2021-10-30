using Microsoft.AspNetCore.Http;
using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface ITrademarkRepo
    {
        IEnumerable<Trademark> GetTrademarks();
        Trademark GetTrademarkById(int id);
        bool CreateNewTrademark(Trademark trademark);
        bool EditTrademark(Trademark trademark);
        bool RemoveTrademark(int id);
    }
}
