using System.Net.Mail;
using System.Net;

namespace WareHouseManagement.Middleware
{
    public class EmailSender
    {
        public static async Task<bool> SendEmail(string toEmail,string subject,string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("dkwebsoftware@gmail.com", "tnmx nqah rtlq ekwp");
                    client.EnableSsl = true;
                    using (var message = new MailMessage("dkwebsoftware@gmail.com", toEmail))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.IsBodyHtml = true;
                        await client.SendMailAsync(message);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
