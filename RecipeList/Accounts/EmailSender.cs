using System.Net;
using System.Net.Mail;

namespace RecipeList.Accounts
{
    public class EmailSender : IEmailSender
    {    
        public bool SendEmail(string to, string toName, string from, string fromName, string subject,
            string body, bool isBodyHTML)
        {
            try
            {
                var fromAddress = new MailAddress("infinity.test.email@gmail.com");
                const string fromPassword = "Password1@3";
                var fromAddr = new MailAddress(from, fromName, System.Text.Encoding.UTF8);
                var toAddr = new MailAddress(to, toName, System.Text.Encoding.UTF8);
                using (var smtp = new SmtpClient
                {
                    Port = 587,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)                 
                })

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