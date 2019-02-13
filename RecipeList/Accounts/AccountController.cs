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
            var user = new User
            {
                Username = model.Username,
                Password = model.Password
            };

            var uNameExists = _db.Users.Any(u => u.Username == user.Username);
            if (!uNameExists)
            {
                return RedirectToAction("Login");
            }

            var dbUser = _db.Users.Single(u => u.Username == user.Username);
            if (user.Password != dbUser.Password)
            {
                Console.WriteLine("Passwords Do Not Match");
                return RedirectToAction("Login");
            }
            
            dbUser.LastLoginAt = DateTime.Now;
            _db.SaveChanges();

            HttpContext.Session.SetString("_Username", user.Username);
            
            return RedirectToAction("Home");
        }

        [HttpGet]
        public IActionResult Home()
        {
            return View("Login");
        }
    }
}