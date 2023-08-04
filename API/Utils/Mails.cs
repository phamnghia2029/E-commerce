using System.Net;
using System.Net.Mail;

namespace API.Utils;

public class Mails
{
    public static void SendTo(String email, string subject, string body, List<Attachment>? attachments = null)
    {
        try
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "quizpractice05@gmail.com",
                    Password = "edhyvvslleaftpim"
                };

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                var message = new MailMessage();

                if (attachments != null)
                {
                    attachments.ForEach(a => message.Attachments.Add(a));
                }

                message.To.Add(email);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress("quizpractice05@gmail.com");
                smtp.Send(message);
            }
        }
        catch (Exception)
        {
        }

    }
}