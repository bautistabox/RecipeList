using System.Net;
using System.Net.Mail;

namespace RecipeList.Accounts
{
    public class EmailSender : IEmailSender
    {
//        public void SendVerificationEmail(User user, EmailVerification emailVerification)
//        {
//            // send email
//            var fromAddress = new MailAddress("infinity.test.email@gmail.com");
//            const string fromPassword = "Password1@3";
//            var toEmail = new MailAddress(user.Email);
//            const string subject = "RecipeList Confirmation Email";
//            var body = "Click the link below to confirm your email and gain access to the site!"
//                       + "\n\nhttps://localhost:5001/account/verify/" + user.Id + "/" + emailVerification.GuId;
//
//            var clientDetails = new SmtpClient
//            {
//                Port = 587,
//                Host = "smtp.gmail.com",
//                EnableSsl = true,
//                DeliveryMethod = SmtpDeliveryMethod.Network,
//                UseDefaultCredentials = false,
//                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
//            };
//
//            using (var message = new MailMessage(fromAddress, toEmail)
//            {
//                Subject = subject,
//                Body = body
//            })
//
//                clientDetails.Send(message);
//        }
//
//        public void SendUsernameEmail(string email, string username)
//        {
//            var fromAddress = new MailAddress("infinity.test.email@gmail.com");
//            const string fromPassword = "Password1@3";
//            var toEmail = new MailAddress(email);
//            const string subject = "RecipeList Username Recovery";
//            var body = "The username associated with this email is: \n\n\t";
//        }

        public bool SendEmail(string to, string toName, string from, string fromName, string subject,
            string body, bool isBodyHTML)
        {
            try
            {
                var fromAddress = new MailAddress("infinity.test.email@gmail.com");
                const string fromPassword = "Password1@3";
                var fromAddr = new MailAddress(from, fromName, System.Text.Encoding.UTF8);
                var toAddr = new MailAddress(to, toName, System.Text.Encoding.UTF8);
                var smtp = new SmtpClient
                {
                    Port = 587,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddr, toAddr)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHTML,
                    BodyEncoding = System.Text.Encoding.UTF8
                })
                {
                    smtp.Send(message);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}