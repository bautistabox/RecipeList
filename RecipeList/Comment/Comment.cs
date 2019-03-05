using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Recipe
{
    [Table("comments")]
    public class Comment
    {
        [Column("id")][Key] public int Id { get; set; }
        [Column("recipe_id")] public int RecipeId { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("comment_text")] public string CommentText { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; }
    }
}