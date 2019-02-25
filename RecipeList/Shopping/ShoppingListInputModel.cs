using System.ComponentModel.DataAnnotations;

namespace RecipeList.Shopping
{
//    public class ShoppingListDataInput
//    {
//        public string Data { get; set; }
//    }
    public class ShoppingListInputModel
    {
        public string Name { get; set; }
        public int Len { get; set; }
        public string[] Items { get; set; }
        public int ListId { get; set; }
    }
}