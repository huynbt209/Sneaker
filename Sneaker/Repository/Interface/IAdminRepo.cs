using Sneaker.Models;
using Sneaker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Repository.Interface
{
    public interface IAdminRepo
    {
        Task<string> GetUserId(string userId);
        Task<string> GetUserName(string userId);
        Task<string> GetUserFullName(string userId);
        IEnumerable<UserRoleViewModel> GetAllUserInDb();
        bool RemoveAccountUser(string Id);
        ApplicationUser GetUserById(string Id);
        bool EditAccountUser(ApplicationUser applicationUser);
        bool LockAccountUser(string Id);
        bool UnlockAccountUser(string Id);

    }
}
