using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeList.Authentication;

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
            _db.SaveChanges();

            var emailVerification = new EmailVerification
            {
                UserId = user.Id,
                GuId = Guid.NewGuid(),
                IsVerified = false
            };

            _db.EmailVerifications.Add(emailVerification);
            _db.SaveChanges();
//
//            _emailSender.SendVerificationEmail(user, emailVerification);

            const string from = "infinity.test.email@gmail.com";
            const string fromName = "RecipeList";
            const string subject = "RecipeList Confirmation Email";
            var body = "Click <a href='https://localhost:5001/account/verify/" + user.Id + "/" +
                       emailVerification.GuId + "'>Here</a> to confirm your email and gain access to the site!";

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
        [Route("account/verify/{id}/{guid}")]
        public IActionResult Verify(int id, Guid guid)
        {
            var dbEmailVer = _db.EmailVerifications.FirstOrDefault(e => e.UserId == id);
            var dbExpiredGuid = _db.ExpiredGuids.Any(e => e.ExpiredGuId == guid);
            if (dbExpiredGuid)
            {
                return View("ExpiredGuidView");
            }

            if (guid != dbEmailVer.GuId)
            {
                return View("/");
            }

            dbEmailVer.IsVerified = true;
            var expiredGuid = new ExpiredGuid
            {
                ExpiredGuId = guid
            };
            _db.ExpiredGuids.Add(expiredGuid);
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

            var dbEmailVer = _db.EmailVerifications.FirstOrDefault(e => e.UserId == dbUser.Id);
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

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }
    }
}