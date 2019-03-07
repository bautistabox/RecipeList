using System;
using Microsoft.AspNetCore.Mvc;

namespace RecipeList.Comment
{
    public class CommentsController : Controller
    {
        private readonly RecipesDbContext _db;

        public CommentsController(RecipesDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Save(CommentInputModel model)
        {
            var comment = new Comment
            {
                UserId = model.CommentUploader,
                RecipeId = model.RecipeId,
                CommentText = model.CommentText,
                CreatedAt = DateTime.Now
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();
            
            return RedirectToAction("Page", "Recipes", new {recipeId = model.RecipeId});
        }
    }
}