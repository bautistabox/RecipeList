using System;
using Microsoft.AspNetCore.Mvc;

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
    }
}