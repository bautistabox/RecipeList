using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Shopping
{
    [Table("list_items")]
    public class ListItem
    {
        [Column("id")] [Key] public int Id { get; set; }

        [Column("list_id")] public int ListId { get; set; }

        [Column("item_name")] public string ItemName { get; set; }
    }
}