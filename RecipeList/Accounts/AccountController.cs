using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;

namespace RecipeList.Accounts
{
    public class AccountController : Controller
    {
        private readonly RecipesDbContext _db;

        public AccountController(RecipesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessRegister(RegisterInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register");
            }
            
            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                DisplayName = model.DisplayName,
                RegisteredAt = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessLogin(LoginInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }
            
            var dbUser = _db.Users.FirstOrDefault(u => u.Username == model.Username);
            if (dbUser == null)
            {
                ModelState.AddModelError("Username", "The specified user or password is incorrect.");
                return View("Login");
            }
            
            if (model.Password != dbUser.Password)
            {
                ModelState.AddModelError("Password", "The specified user or password is incorrect.");
                return View("Login");
            }

            dbUser.LastLoginAt = DateTime.Now;
            _db.SaveChanges();

            HttpContext.Session.SetInt32("_Userid", dbUser.Id);
            HttpContext.Session.SetString("_Username", dbUser.Username);

            return RedirectToAction("Home");
        }

        [HttpGet]
        public IActionResult Home()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            if (sessionUName != null)
            {
                return View();
            }

            return RedirectToAction("Login");
        }
    }
}