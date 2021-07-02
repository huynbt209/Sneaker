﻿using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface ITrademarkRepo
    {
        IEnumerable<Trademark> GetTrademarks();
        Trademark GetTrademarkById(int Id);
        bool CreateNewTrademark(Trademark trademark);
        bool EditTrademark(Trademark trademark);
        bool RemoveTrademark(int Id);
    }
}