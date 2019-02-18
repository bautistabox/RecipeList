using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Process(ShoppingListDataInput sldi)
        {
            var slim = JsonConvert.DeserializeObject<ShoppingListInputModel>(sldi.Data);

            var shoppingList = new ShoppingList
            {
                Name = slim.Name,
                UserId = HttpContext.Session.GetInt32("_Userid") ?? -1,
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