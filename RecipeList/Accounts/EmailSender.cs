using System;
using System.Net;
using System.Net.Mail;

namespace RecipeList.Accounts
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(User user, EmailVerification emailVerification)
        {
            // send email
            var fromAddress = new MailAddress("infinity.test.email@gmail.com");
            const string fromPassword = "Password1@3";
            var toEmail = new MailAddress(user.Email);
            const string subject = "RecipeList Confirmation Email";
            var body = "Click the link below to confirm your email and gain access to the site!"
                       + "\n\nhttps://localhost:5001/account/verify/" + user.Id + "/" + emailVerification.GuId;

            var clientDetails = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toEmail)
            {
                Subject = subject,
                Body = body
            })

                clientDetails.Send(message);
        }
    }
}