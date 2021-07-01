using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sneaker.Models;
using Sneaker.Repository.Interface;
using Sneaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Controllers
{
    [Authorize(Roles =(SystemRoles.Administrator))]
    public class AdminController : Controller
    {
        private readonly IAdminRepo _adminRepo;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepo adminRepo, ILogger<AdminController> logger)
        {
            _adminRepo = adminRepo;
            _logger = logger;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

         public IActionResult Index()
        {
            var users = _adminRepo.GetAllUserInDb();
            return View(users);
        }
        public IActionResult GetAccountUser()
        {
            var users = _adminRepo.GetAllUserInDb();
            return new JsonResult(users);
        }

        public IActionResult RemoveAccountUser(string id)
        {
            if(!_adminRepo.RemoveAccountUser(id))
            {
                throw new ArgumentException("Error");
            }

            _logger.LogInformation($"Remove user account with ID: {id}");
            return RedirectToAction(nameof(Index));
        }
        //
        public IActionResult EditAccountUser(string id)
        {
            var accountInDb = _adminRepo.GetUserById(id);
            if (accountInDb == null)
            {
                return NotFound();
            }
            return View(accountInDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccountUser(ApplicationUser applicationUsers)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!_adminRepo.EditAccountUser(applicationUsers))
            {
                throw new ArgumentException("Error");
            }

            _logger.LogInformation($"User account updated!");
            return RedirectToAction(nameof(Index));
        }
        //
        public IActionResult LockAccountUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if(!_adminRepo.LockAccountUser(id))
            {
                 return NotFound();
            }

            _logger.LogInformation($"Lock account user with id: {id}");
            return RedirectToAction(nameof(Index));
        }
        //
        public IActionResult UnlockAccountUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if(!_adminRepo.UnlockAccountUser(id))
            {
                 return NotFound();
            }

            _logger.LogInformation($"Unlock account user with id: {id}");
            return RedirectToAction(nameof(Index));
        }
        //
        [AllowAnonymous]
        public IActionResult Profile(string userId)
        {
            var user = _adminRepo.GetUserById(userId);
            return Json(user.ImagePath == null ? new {profile = "avatar.jpeg"} : new {profile = user.ImagePath});
        }
    }
}
