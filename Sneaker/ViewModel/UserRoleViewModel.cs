using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.ViewModel
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string DateCreate { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutStatus { get; set; }
    }
}
