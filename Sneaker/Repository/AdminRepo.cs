using Microsoft.EntityFrameworkCore;
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
    public class AdminRepo : IAdminRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public AdminRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> GetUserId(string userId)
        {
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? "Unknown" : user.Id;
        }
        public async Task<string> GetUserName(string userId)
        {
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? "Unknown" : user.FullName;
        }

        public IEnumerable<UserRoleViewModel> GetAllUserInDb()
        {
            var listUsers = (from user in _dbContext.ApplicationUsers
                             join userRoles in _dbContext.UserRoles on user.Id equals userRoles.UserId
                             join role in _dbContext.Roles on userRoles.RoleId equals role.Id
                             select new
                             {
                                 UserId = user.Id,
                                 FullName = user.FullName,
                                 Email = user.Email,
                                 RoleName = role.Name,
                                 DateCreate = user.CreateAt,
                                 LockUser = user.LockoutEnd
                             })
                .ToList().Select(u => new UserRoleViewModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.RoleName,
                    DateCreate = u.DateCreate.ToString("HH:mm - MM/dd/yyyy"),
                    LockoutEnd = u.LockUser,
                    LockoutStatus = CheckUserLockOrNot(u.LockUser)
                });
            return listUsers;
        }

        private static bool CheckUserLockOrNot(DateTimeOffset? lockUser)
        {
            if (lockUser == null)
            {
                return true;
            }
            return lockUser < DateTime.Now;
        }

        public bool RemoveAccountUser(string Id)
        {
            var accountInDb = GetUserById(Id);
            if (accountInDb == null) return false;
            _dbContext.ApplicationUsers.Remove(accountInDb);
            _dbContext.SaveChanges();
            return true;
        }

        public ApplicationUser GetUserById(string Id)
        {
            return _dbContext.ApplicationUsers.SingleOrDefault(u => u.Id == Id);
        }

        public bool EditAccountUser(ApplicationUser applicationUser)
        {
            var accountInDb = GetUserById(applicationUser.Id);
            if (accountInDb == null) return false;

            accountInDb.FullName = applicationUser.FullName;
            accountInDb.Email = applicationUser.Email;
            _dbContext.ApplicationUsers.Update(accountInDb);
            _dbContext.SaveChanges();
            return true;
        }

        public bool LockAccountUser(string Id)
        {
            var user = GetUserById(Id);
            if (user == null)
            {
                return false;
            }

            user.LockoutEnd = DateTime.Now.AddYears(1000);
            _dbContext.SaveChanges();

            return true;
        }

        public bool UnlockAccountUser(string Id)
        {
            var user = GetUserById(Id);

            if (user == null)
            {
                return false;
            }

            user.LockoutEnd = DateTime.Now;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
