using System;

namespace RecipeList.Shopping
{
    public class ShoppingListItems
    {
       
        
        public int listId { get; set; }
        public string listOwner { get; set; }
        public int listOwnerId { get; set; }
        public string listName { get; set; }
        public string[] listItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}