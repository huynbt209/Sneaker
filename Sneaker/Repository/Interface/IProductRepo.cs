using Sneaker.Models;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetProducts();
        IEnumerable<Product> GetProductByTrademark(int trademarkId);
        IEnumerable<Trademark> GetTrademarks();
        ProductTrademarkViewModel ImportProduct(ProductTrademarkViewModel productTrademarkViewModel);
        ProductTrademarkViewModel ProductTrademarkViewModel();
        IEnumerable<Product> GetProductInTrademarkId(int id);
        ProductTrademarkViewModel CreateProduct(ProductTrademarkViewModel productTrademarkViewModel);
        ProductTrademarkViewModel EditProduct(ProductTrademarkViewModel productTrademarkViewModel);
        IEnumerable<Product> GetProductsSale(int id);
        IEnumerable<Product> GetProductsNew(int id);
        Product GetProductById (int id);
    }
}
