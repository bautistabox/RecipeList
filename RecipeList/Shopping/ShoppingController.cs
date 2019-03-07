using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecipeList.Authentication;
using RecipeList.Ingredient;

namespace RecipeList.Shopping
{
    [Authorize]
    public class ShoppingController : Controller
    {
        private readonly RecipesDbContext _db;

        public ShoppingController(RecipesDbContext db)
        {
            _db = db;
        }

        public JsonResult GetSearchValue(string search)
        {
            var allItems = _db.ListItems.Where(i => i.ItemName.Contains(search)).Select(i => new ListItem
            {
                Id = i.Id,
                ItemName = i.ItemName,
                ListId = i.ListId
            }).ToList();

            return new JsonResult(allItems);
        }

        [HttpGet]
        public IActionResult Lists()
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var sessionUName = HttpContext.Session.GetString("_Username");

            var model = _db
                .Lists
                .Where(u => u.UserId == sessionUId)
                .Select(u => new ShoppingListItems
                {
                    listId = u.Id,
                    listName = u.Name,
                    listOwner = sessionUName,
                    listOwnerId = sessionUId.Value,
                    UpdatedAt = u.UpdatedAt ?? DateTime.Now,
                    CreatedAt = u.CreatedAt
                })
                .ToList();

            foreach (var mod in model)
            {
                var items = _db
                    .ListItems
                    .Where(l => l.ListId == mod.listId)
                    .Select(i => i.ItemName)
                    .ToList();

                mod.listItems = items.ToArray();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult New()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            var dupeList = new List<string>();
            var user = _db.Users.FirstOrDefault(u => u.Username == sessionUName);
            
            // this block will grab only the shopping list items this user has enter before
            if (user != null)
            {
                var usersLists = _db.Lists.Where(l => l.UserId == user.Id).Select(l => l.Id).ToList();
                var listItems = _db.ListItems.Where(li => usersLists.Contains(li.ListId)).ToList();

                foreach (var item in listItems)
                {
                    var flag = false;

                    for (var i = 0; i != dupeList.Count; i++)
                        if (dupeList[i].Equals(item.ItemName))
                            flag = true;

                    if (!flag || dupeList.Count == 0)
                        dupeList.Add(item.ItemName);
                }
            }

            ViewData["items"] = dupeList;
            ViewData["username"] = sessionUName;
            return View();
        }

        [HttpPost]
        [Route("shopping/create-list")]
        public IActionResult Create(IngredientListInputModel model)
        {
            var dbRecipe = _db.Recipes.FirstOrDefault(r => r.Id == model.RecipeId);
            if (dbRecipe == null)
            {
                return View("New");
            }
            
            var shoppingList = new ShoppingList
            {
                Name = dbRecipe.Name,
                UserId = HttpContext.Session.GetInt32("_Userid").Value,
                CreatedAt = DateTime.Now
            };

            _db.Lists.Add(shoppingList);
            _db.SaveChanges();

            var recipeIngredientIds = _db.RecipeIngredients
                .Where(r => r.RecipeId == model.RecipeId)
                .Select(i => i.IngredientId)
                .ToArray();


            foreach (var recipeIngredientId in recipeIngredientIds)
            {
                var ingredient = _db.Ingredients
                    .Where(i => i.Id == recipeIngredientId)
                    .Select(i => i.Name)
                    .Single();

                var listItem = new ListItem
                {
                    ListId = shoppingList.Id,
                    ItemName = ingredient
                };

                _db.ListItems.Add(listItem);
            }

            _db.SaveChanges();
            return RedirectToAction("View", new {listId = shoppingList.Id});
        }

        [HttpGet]
        public IActionResult View(int listId)
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var sessionUName = HttpContext.Session.GetString("_Username");

            var model = _db
                .Lists
                .Where(u => u.Id == listId)
                .Select(u => new ShoppingListItems
                {
                    listId = u.Id,
                    listName = u.Name,
                    listOwner = sessionUName,
                    listOwnerId = sessionUId.Value
                }).SingleOrDefault();
            var items = _db
                .ListItems
                .Where(l => l.ListId == model.listId)
                .Select(i => i.ItemName)
                .ToList();
            if (model != null)
            {
                model.listItems = items.ToArray();
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int listId)
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var sessionUName = HttpContext.Session.GetString("_Username");

            var model = _db
                .Lists
                .Where(u => u.Id == listId)
                .Select(u => new ShoppingListItems
                {
                    listId = u.Id,
                    listName = u.Name,
                    listOwner = sessionUName,
                    listOwnerId = sessionUId.Value
                })
                .SingleOrDefault();
            var items = _db
                .ListItems
                .Where(l => l.ListId == model.listId)
                .Select(i => i.ItemName)
                .ToList();

            if (model != null)
            {
                model.listItems = items.ToArray();
            }

            var dupeList = new List<string>();
            foreach (var item in _db.ListItems.ToList())
            {
                var flag = false;

                for (var i = 0; i != dupeList.Count; i++)
                    if (dupeList[i].Equals(item.ItemName))
                        flag = true;

                if (!flag || dupeList.Count == 0)
                    dupeList.Add(item.ItemName);
            }

            ViewData["items"] = dupeList.ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(DataInput dataInput)
        {
            var slim = JsonConvert.DeserializeObject<ShoppingListInputModel>(dataInput.Data);
            Console.WriteLine(slim.ListId);
            for (var i = 0; i != slim.Items.Length; i++)
                Console.WriteLine(slim.Items[i]);
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

            return RedirectToAction("view", "shopping", new {slim.ListId});
        }

        [HttpPost]
        public IActionResult Process(DataInput dataInput)
        {
            var uId = HttpContext.Session.GetInt32("_Userid");
            if (uId == -1)
            {
                return RedirectToAction("Login", "Account");
            }

            var slim = JsonConvert.DeserializeObject<ShoppingListInputModel>(dataInput.Data);

            var shoppingList = new ShoppingList
            {
                Name = slim.Name,
                UserId = uId.Value,
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

            return RedirectToAction("lists");
        }

        [HttpPost]
        public IActionResult Delete(ShoppingListItems model)
        {
            var deleteListItems = from listItem in _db.ListItems
                where listItem.ListId == model.listId
                select listItem;

            foreach (var listItem in deleteListItems)
                _db.ListItems.Remove(listItem);

            var deleteList = from list in _db.Lists
                where list.Id == model.listId
                select list;

            foreach (var list in deleteList)
                _db.Lists.Remove(list);

            _db.SaveChanges();

            return RedirectToAction("lists");
        }
    }
}