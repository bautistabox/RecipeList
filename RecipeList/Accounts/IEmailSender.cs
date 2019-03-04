namespace RecipeList.Accounts
{
    public interface IEmailSender
    {
//        void SendVerificationEmail(User user, EmailVerification emailVerification);

        bool SendEmail(string to, string toName, string from, string fromName, string subject,
            string body, bool isBodyHTML);
    }
}