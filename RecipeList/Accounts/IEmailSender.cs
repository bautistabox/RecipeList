namespace RecipeList.Accounts
{
    public interface IEmailSender
    {
        void SendEmail(User user, EmailVerification emailVerification);
    }
}