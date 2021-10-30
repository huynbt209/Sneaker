using System.Collections.Generic;
using Sneaker.Models;
using Sneaker.ViewModel;

namespace Sneaker.Repository.Interface
{
    public interface IItemRepo
    {
        IEnumerable<Item> GetProducts();
        IEnumerable<Item> GetProductByTrademark(int trademarkId);
        IEnumerable<Trademark> GetTrademarks();
        ItemTrademarkViewModel ProductTrademarkViewModel();
        IEnumerable<Item> GetProductInTrademarkId(int id);
        ItemTrademarkViewModel CreateProduct(ItemTrademarkViewModel productTrademarkViewModel);
        ItemTrademarkViewModel CreateProductWithExcel(ItemTrademarkViewModel productTrademarkViewModel);
        ItemTrademarkViewModel EditProduct(ItemTrademarkViewModel productTrademarkViewModel);
        Item GetItemById(int id);
        FeedbackItemViewModel GetItemDetails(int id);
        TrendingHotSaleViewModel GetTrendHotSaleNewProducts();
        IEnumerable<Item> GetProductsSale(int id);
        IEnumerable<Item> GetProductsNew(int id);
        bool RemoveProduct(int id);
    }
}