using System;

namespace RecipeList.Shopping
{
    public class ShoppingListItems
    {
       
        
        public int ListId { get; set; }
        public string ListOwner { get; set; }
        public int ListOwnerId { get; set; }
        public string ListName { get; set; }
        public string[] ListItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}