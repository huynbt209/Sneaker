using Sneaker.Data;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository
{
    public class FeedbackProductRepo : IFeedbackProductRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public FeedbackProductRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SaveComment(int productId, string userId, string message)
        {
            var newFeedback = new FeedbackItem()
            {
                ItemId = productId,
                UserId = userId,
                Message = message
            };
            await _dbContext.FeedbackItems.AddAsync(newFeedback);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
