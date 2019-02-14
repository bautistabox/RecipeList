using Microsoft.AspNetCore.Mvc;

namespace RecipeList.Shopping
{
    public class ShoppingController : Controller
    {
        public IActionResult Lists()
        {
            return View();
        } 
    }
}