using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RecipeList.Home;
using WebApiContrib.Formatting;

namespace RecipeList.Shopping
{
    public class ShoppingController : Controller
    {
        private readonly RecipesDbContext _db;

        public ShoppingController(RecipesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Lists()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            var sessionUId = HttpContext.Session.GetInt32("_Userid") ?? -1;
            if (sessionUName == null || sessionUId == -1)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = _db
                .Lists
                .Where(u => u.UserId == sessionUId)
                .Select(u => new ShoppingListItems
                {
                    listId = u.Id,
                    listName = u.Name,
                    listOwner = sessionUName,
                    listOwnerId = sessionUId
                })
                .SingleOrDefault();

            var items = _db
                .ListItems
                .Where(l => l.ListId == model.listId)
                .Select(i => i.ItemName)
                .ToList();

            model.listItems = items.ToArray();

            return View(model);
        }

        [HttpGet]
        public IActionResult New()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            var sessionUId = HttpContext.Session.GetInt32("_Userid") ?? -1;
            if (sessionUName == null || sessionUId == -1)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            var sessionUId = HttpContext.Session.GetInt32("_Userid") ?? -1;
            if (sessionUName == null || sessionUId == -1)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = _db
                .Lists
                .Where(u => u.UserId == sessionUId)
                .Select(u => new ShoppingListItems
                {
                    listId = u.Id,
                    listName = u.Name,
                    listOwner = sessionUName,
                    listOwnerId = sessionUId
                })
                .SingleOrDefault();
            var items = _db
                .ListItems
                .Where(l => l.ListId == model.listId)
                .Select(i => i.ItemName)
                .ToList();

            model.listItems = items.ToArray();

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(ShoppingListDataInput sldi)
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            var uId = HttpContext.Session.GetInt32("_Userid") ?? -1;
            if (sessionUName == null || uId == -1)
            {
                return RedirectToAction("Login", "Account");
            }

            var slim = JsonConvert.DeserializeObject<ShoppingListInputModel>(sldi.Data);
            Console.WriteLine(slim.ListId);

            var query =
                from l in _db.Lists
                where l.Id == slim.ListId
                select l;

            foreach (var q in query)
            {
                q.Name = slim.Name;
                q.UpdatedAt = DateTime.Now;
            }

            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var query2 =
                from l in _db.ListItems
                where l.ListId == slim.ListId
                select l;

            foreach (var q in query2)
            {
                _db.Remove(q);
            }

            foreach (var item in slim.Items)
            {
                var listItem = new ListItem
                {
                    ListId = slim.ListId,
                    ItemName = item
                };

                _db.ListItems.Add(listItem);
            }

            _db.SaveChanges();

            return View("Process");
        }

        [HttpPost]
        public IActionResult Process(ShoppingListDataInput sldi)
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            if (sessionUName == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var uId = HttpContext.Session.GetInt32("_Userid") ?? -1;
            if (uId == -1)
            {
                return RedirectToAction("Login", "Account");
            }

            var slim = JsonConvert.DeserializeObject<ShoppingListInputModel>(sldi.Data);

            var shoppingList = new ShoppingList
            {
                Name = slim.Name,
                UserId = uId,
                CreatedAt = DateTime.Now
            };

            _db.Lists.Add(shoppingList);
            _db.SaveChanges();

            foreach (var item in slim.Items)
            {
                var listItem = new ListItem
                {
                    ListId = shoppingList.Id,
                    ItemName = item
                };

                _db.ListItems.Add(listItem);
            }

            _db.SaveChanges();

            return View();
        }
    }
}