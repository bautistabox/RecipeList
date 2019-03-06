using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using RecipeList.Authentication;
using RecipeList.Recipe;

namespace RecipeList.Accounts
{
    public class AccountController : Controller
    {
        private readonly RecipesDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailSender _emailSender;

        public AccountController(RecipesDbContext db, IPasswordHasher passwordHasher, IEmailSender emailSender)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
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
            
            var bio = new UserBio
            {
                UserId = user.Id,
                Bio = user.DisplayName + "'s bio"
            };

            var dbUserName = _db.Users.SingleOrDefault(u => u.Username == user.Username);
            var dbUserEmail = _db.Users.SingleOrDefault(u => u.Email == user.Email);

            if (dbUserName != null || dbUserEmail != null || user.Password != model.ConfirmPassword)
            {
                if (dbUserName != null)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                }

                if (dbUserEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }

                if (user.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("Password", "Passwords do not match");
                }

                return View("Register");
            }

            user.Password = _passwordHasher.HashPassword(user.Password);

            _db.Users.Add(user);
            _db.UserBios.Add(bio);
            _db.SaveChanges();

            var uniqueIdentifier = new UniqueIdentifiers
            {
                UserId = user.Id,
                UniqueId = Guid.NewGuid(),
                IsVerified = false
            };

            _db.UniqueIdentifiers.Add(uniqueIdentifier);
            _db.SaveChanges();

            const string from = "infinity.test.email@gmail.com";
            const string fromName = "RecipeList";
            const string subject = "RecipeList Confirmation Email";
            var body = "Click <a href='https://localhost:5001/account/verify/" + user.Id + "/" +
                       uniqueIdentifier.UniqueId + "'>Here</a> to confirm your email and gain access to the site!";

            _emailSender.SendEmail(user.Email, user.Username, from, fromName, subject, body, true);
            return RedirectToAction("AwaitingVerification");
        }

        [HttpGet]
        public IActionResult AwaitingVerification()
        {
            return View();
        }

        [HttpGet]
        [Route("account/forgot-username")]
        public IActionResult ForgotUsername()
        {
            return View();
        }

        [HttpGet]
        [Route("account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("account/forgot/password")]
        public IActionResult ProcessForgotPassword(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            const string from = "infinity.test.email@gmail.com";
            const string fromName = "RecipeList";
            const string subject = "RecipeList Password Recovery";
            var linkId = Guid.NewGuid();
            var unique_identifier = new UniqueIdentifiers
            {
                UserId = user.Id,
                UniqueId = linkId,
                IsVerified = false
            };
            _db.UniqueIdentifiers.Add(unique_identifier);
            _db.SaveChanges();

            var body = "Hello " + user.DisplayName +
                       ",<br/><br/>Click this <a href='https://localhost:5001/account/recovery/" + user.Id + "/" +
                       linkId + "'>link</a> to reset your password.";
            _emailSender.SendEmail(user.Email, user.Username, from, fromName, subject, body, true);
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("account/forgot/username")]
        public IActionResult ProcessForgotUsername(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return RedirectToAction("ForgotUsername");
            }

            const string from = "infinity.test.email@gmail.com";
            const string fromName = "RecipeList";
            const string subject = "RecipeList Username Recovery";
            var body = "Hello " + user.DisplayName + ",<br/><br/>The username associated with this email is: <b>" +
                       user.Username + "</b>";

            _emailSender.SendEmail(user.Email, user.Username, from, fromName, subject, body, true);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("account/recovery/{id}/{uniqueId}")]
        public IActionResult Recovery(int id, Guid uniqueId)
        {
            var dbUniqueId = _db.UniqueIdentifiers.FirstOrDefault(u => uniqueId == uniqueId);
            if (dbUniqueId == null)
            {
                return RedirectToAction("Login");
            }

            dbUniqueId.IsVerified = true;
            ViewData["user"] = id;
            return View("Recovery");
        }

        [HttpPost]
        public IActionResult Recover(string firstPassword, int user)
        {
            var dbUser = _db.Users.FirstOrDefault(u => u.Id == user);

            if (dbUser == null)
            {
                return RedirectToAction("Login");
            }

            dbUser.Password = _passwordHasher.HashPassword(firstPassword);
            _db.SaveChanges();

            return RedirectToAction("RecoverySuccess");
        }

        [HttpGet]
        [Route("account/recover/success")]
        public IActionResult RecoverySuccess()
        {
            return View();
        }
        
        [HttpGet]
        [Route("account/verify/{id}/{uniqueId}")]
        public IActionResult Verify(int id, Guid uniqueId)
        {
            var dbEmailVer = _db.UniqueIdentifiers.FirstOrDefault(e => e.UserId == id);
            if (dbEmailVer.IsVerified)
            {
                return View("ExpiredEmailVerifyView");
            }

            if (uniqueId != dbEmailVer.UniqueId)
            {
                return View("/");
            }

            dbEmailVer.IsVerified = true;
            _db.SaveChanges();
            return View();
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


            if (!_passwordHasher.VerifyPassword(model.Password, dbUser.Password))
            {
                ModelState.AddModelError("Password", "The specified user or password is incorrect.");
                return View("Login");
            }

            var dbEmailVer = _db.UniqueIdentifiers.FirstOrDefault(e => e.UserId == dbUser.Id);
            if (!dbEmailVer.IsVerified)
            {
                ModelState.AddModelError("Username", "The specified user has not yet verified their email");
                return View("Login");
            }

            dbUser.LastLoginAt = DateTime.Now;
            _db.SaveChanges();

            HttpContext.Session.SetInt32("_Userid", dbUser.Id);
            HttpContext.Session.SetString("_Username", dbUser.Username);


            if (dbUser.Username.Equals("admin"))
            {
                return RedirectToAction("Home", "Admin");
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("account/user/{id}")]
        public IActionResult UserProfile(int id)
        {
            var dbUser = _db.Users.FirstOrDefault(u => u.Id == id);
            if (dbUser == null)
            {
                RedirectToAction("NoneFound");
            }
            
            var dbRecipes = _db.Recipes.Where(r => r.UploaderId == dbUser.Id).ToList();
            var recipeCount = 0;
            var recipeIds = new List<int>();
            if (dbRecipes.Count > 0)
            {
                foreach (var recipe in dbRecipes)
                {
                    recipeIds.Add(recipe.Id);
                }
                recipeCount = dbRecipes.Count;
            }

            var dbUserBio = _db.UserBios.FirstOrDefault(b => b.UserId == dbUser.Id);
            var bio = "";
            if (dbUserBio != null)
            {
                bio = dbUserBio.Bio;
            }
            
            var dbUserRatings = _db.RecipeRatings.Where(r => recipeIds.Contains(r.RecipeId)).ToList();
            var numberRatings = 0;
            var avgRating = 0;
            if (dbUserRatings.Count > 0)
            {
                numberRatings = dbUserRatings.Count;
                foreach (var rating in dbUserRatings)
                {
                    avgRating += rating.Rating;
                }

                avgRating = avgRating / dbUserRatings.Count;
            }
            
            var model = new ProfileViewModel
            {
                DisplayName = dbUser.DisplayName,
                Username = dbUser.Username,
                UserId = dbUser.Id,
                Email = dbUser.Email,
                Rating = avgRating,
                NumberRecipes = recipeCount,
                NumberRatings = numberRatings,
                Bio = bio
            };

            ViewData["Recipes"] = dbRecipes;
           
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var dbUser = _db.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetInt32("_Userid"));
            if (dbUser == null)
            {
                return RedirectToAction("Login");
            }

            var dbRecipes = _db.Recipes.Where(r => r.UploaderId == dbUser.Id).ToList();
            var recipeCount = 0;
            var recipeIds = new List<int>();
            if (dbRecipes.Count > 0)
            {
                foreach (var recipe in dbRecipes)
                {
                    recipeIds.Add(recipe.Id);
                }
                recipeCount = dbRecipes.Count;
            }

            var dbUserBio = _db.UserBios.FirstOrDefault(b => b.UserId == dbUser.Id);
            var bio = "";
            if (dbUserBio != null)
            {
                bio = dbUserBio.Bio;
            }
            
            var dbUserRatings = _db.RecipeRatings.Where(r => recipeIds.Contains(r.RecipeId)).ToList();
            var numberRatings = 0;
            var avgRating = 0;
            if (dbUserRatings.Count > 0)
            {
                numberRatings = dbUserRatings.Count;
                foreach (var rating in dbUserRatings)
                {
                    avgRating += rating.Rating;
                }

                avgRating = avgRating / dbUserRatings.Count;
            }
            
            var model = new ProfileViewModel
            {
                DisplayName = dbUser.DisplayName,
                Username = dbUser.Username,
                UserId = dbUser.Id,
                Email = dbUser.Email,
                Rating = avgRating,
                NumberRecipes = recipeCount,
                NumberRatings = numberRatings,
                Bio = bio
            };

            ViewData["Recipes"] = dbRecipes;
           
            return View(model);
        }

        [HttpPost]
        [Route("/account/profile/update")]
        public IActionResult UpdateProfile(string updatedDisplayName, string updatedBio, int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            var bio = _db.UserBios.FirstOrDefault(ub => ub.UserId == user.Id);
            if (bio == null)
            {
                return RedirectToAction("Profile");
            }

            user.DisplayName = updatedDisplayName;
            bio.Bio = updatedBio;
            _db.SaveChanges();
            
            return RedirectToAction("Profile");
        }
        

        [HttpGet]
        public IActionResult NoneFound()
        {
            return View();
        }
    }
}