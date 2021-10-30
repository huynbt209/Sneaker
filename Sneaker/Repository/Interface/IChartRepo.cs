using System;
using System.Collections.Generic;

namespace Sneaker.Repository.Interface
{
    public interface IChartRepo
    {
        List<double> CountItemOfTrademark();
        List<String> GetTrademarkList();
        List<double> CountProductInInvoiceOfTrademark();

    }
}