using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sneaker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Models
{
    public class CartItem
    {
        private readonly ApplicationDbContext _dbContext;
        private CartItem(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public string Id { get; set; }
        public List<Cart> Carts { get; set; }

        public static CartItem GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<ApplicationDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
            return new CartItem(context) { Id = cartId };
        }

        public void AddToCart(Product product, int quantity)
        {
            var cart = _dbContext.Carts.SingleOrDefault(c => c.Products.Id == product.Id && c.CartItemId == Id);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartItemId = Id,
                    Products = product,
                    Quantity = quantity,
                };
                _dbContext.Carts.Add(cart);
            }
            else
            {
                cart.Quantity++;
            }
            _dbContext.SaveChanges();          
        }
        public void UpdateCart(Product product, int quantity)
        {
            var cart = _dbContext.Carts.SingleOrDefault(c => c.Products.Id == product.Id && c.CartItemId == Id);
            for (var i = 0; i < cart.Quantity; i++)
            {
                cart = new Cart
                {
                    Quantity = quantity
                };
                _dbContext.Carts.Update(cart);
            }
        }
        public int RemoveCart(Product product)
        {
            var CartItem = _dbContext.Carts.SingleOrDefault(c => c.Products.Id == product.Id && c.CartItemId == Id);

            var localAmount = 0;

            if (CartItem != null)
            {
                if (CartItem.Quantity > 1)
                {
                    CartItem.Quantity--;
                    localAmount = CartItem.Quantity;
                }
                else
                {
                    _dbContext.Carts.Remove(CartItem);
                }
            }
            _dbContext.SaveChanges();
            return localAmount;
        }

        public List<Cart> GetCartItems()
        {
            return Carts ?? (Carts = _dbContext.Carts.Where(c => c.CartItemId == Id).Include(p => p.Products).ToList());
        }

        public void ClearCart()
        {
            var cartItems = _dbContext.Carts.Where(c => c.CartItemId == Id);
            _dbContext.Carts.RemoveRange(cartItems);
            _dbContext.SaveChanges();
        }

        public decimal GetCartTotal()
        {
            var total = _dbContext.Carts.Where(c => c.CartItemId == Id).Select(c => c.Products.Price * c.Quantity).Sum();
            return total;
        }
    }
}