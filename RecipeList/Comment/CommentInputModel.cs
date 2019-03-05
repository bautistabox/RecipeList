namespace RecipeList.Recipe
{
    public class CommentInputModel
    {
        public int RecipeId { get; set; }
        public int CommentUploader { get; set; }
        public string CommentText { get; set; }
    }
}