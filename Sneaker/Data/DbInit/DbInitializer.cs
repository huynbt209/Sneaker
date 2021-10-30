using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sneaker.Data.DbInit;
using Sneaker.Data;
using System.Linq;
using Sneaker.Utility;
using Sneaker.Models;

namespace Sneaker.Data.DbInit
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async void Initializer()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine((ex.Message));
                throw new ApplicationException("Error when Migrate Db", ex);
            }

            List<string> ListRoles = new List<string>(new string[]
            {
                SystemRoles.Administrator,
                SystemRoles.Manager,
                SystemRoles.Member,
            });
            foreach (var role in ListRoles)
            {
                if (_db.Roles.Any(r => r.Name == role)) continue;
                _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
            }

            if (_userManager.FindByEmailAsync("sneakeradmin@gmail.com").Result == null)
            {
                var result = _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "sneakeradmin@gmail.com",
                    FullName = "Admin Sneaker",
                    Email = "sneakeradmin@gmail.com",
                    EmailConfirmed = true
                }, "Sneaker@123").GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == "sneakeradmin@gmail.com");
                    await _userManager.AddToRoleAsync(user, SystemRoles.Administrator);
                }
            }


            // if (_userManager.FindByEmailAsync("sneakermanager@gmail.com").Result == null)
            // {
            //     var result = _userManager.CreateAsync(new ApplicationUser
            //     {
            //         UserName = "sneakermanager@gmail.com",
            //         FullName = "Manager Sneaker",
            //         Email = "sneakermanager@gmail.com",
            //         EmailConfirmed = true
            //     }, "Sneaker@123").GetAwaiter().GetResult();
            //     if (result.Succeeded)
            //     {
            //         var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == "sneakermanager@gmail.com");
            //         await _userManager.AddToRoleAsync(user, SystemRoles.Manager);
            //     }
            // }
            //
            //
            // if (_userManager.FindByEmailAsync("sneakermember@gmail.com").Result == null)
            // {
            //     var result = _userManager.CreateAsync(new ApplicationUser
            //     {
            //         UserName = "sneakermember@gmail.com",
            //         FullName = "Member Sneaker",
            //         Email = "sneakermember@gmail.com",
            //         EmailConfirmed = true
            //     }, "Sneaker@123").GetAwaiter().GetResult();
            //     if (result.Succeeded)
            //     {
            //         var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == "sneakermember@gmail.com");
            //         await _userManager.AddToRoleAsync(user, SystemRoles.Member);
            //     }
            // }
            //
            //
            // if (_userManager.FindByEmailAsync("huynbt09@gmail.com").Result == null)
            // {
            //     var result = _userManager.CreateAsync(new ApplicationUser
            //     {
            //         UserName = "huynbt09@gmail.com",
            //         FullName = "Huy Sneaker",
            //         Email = "huynbt09@gmail.com",
            //         EmailConfirmed = true
            //     }, "Sneaker@123").GetAwaiter().GetResult();
            //     if (result.Succeeded)
            //     {
            //         var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == "huynbt09@gmail.com");
            //         await _userManager.AddToRoleAsync(user, SystemRoles.Administrator);
            //     }
            // }
        }
    }
}
