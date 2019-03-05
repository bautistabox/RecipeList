namespace RecipeList.Accounts
{
    public class ProfileViewModel
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public decimal Rating { get; set; }
        public int NumberRecipes { get; set; }
        public int NumberRatings { get; set; }
        public string Bio { get; set; }
    }
}