using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeList.Authentication;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc.Filters;


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

            var passwordHasher = new PasswordHasher();
            user.Password = passwordHasher.HashPassword(user.Password);
            
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

            // send email
            var fromAddress = new MailAddress("infinity.test.email@gmail.com");
            const string fromPassword = "Password1@3";
            var toEmail = new MailAddress(user.Email);
            const string subject = "RecipeList Confirmation Email";
            var body = "Click the link below to confirm your email and gain access to the site!"
                       + "\n\nhttps://localhost:5001/account/verify/" + user.Id + "/" + emailVerification.GuId;

            var clientDetails = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toEmail)
            {
                Subject = subject,
                Body = body
            })

            clientDetails.Send(message);

            Console.WriteLine("Email Sent");

            return RedirectToAction("AwaitingVerification");
        }

        [HttpGet]
        public IActionResult AwaitingVerification()
        {
            return View();
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

            var passwordHasher = new PasswordHasher();
            if (!passwordHasher.VerifyPassword(model.Password, dbUser.Password))
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