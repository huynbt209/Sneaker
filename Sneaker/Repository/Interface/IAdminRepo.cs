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
        public Task<string> GetUserName(string userId);
        public IEnumerable<UserRoleViewModel> GetAllUserInDb();
        bool RemoveAccountUser(string Id);
        ApplicationUser GetUserById(string Id);
        bool EditAccountUser(ApplicationUser applicationUser);
        bool LockAccountUser(string Id);
        bool UnlockAccountUser(string Id);
        
    }
}
