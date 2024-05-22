using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace CarParkingSystem.EmailSender
{
    public class EmailSender : IMailSender
    {
        public void MessageSend(Message message)
        {
            try
            {
                var mail = new MailMessage();
                var Client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("futurevilleuniversity@gmail.com", "nyaysttgphfkeizh"),
                    EnableSsl = true,
                };
                mail.From = new MailAddress("futurevilleuniversity@gmail.com", "Car Parking System");
                mail.To.Add(message.Messageto);
                mail.Subject = message.Subject;
                var htmlView = AlternateView.CreateAlternateViewFromString(message.Content, null, MediaTypeNames.Text.Html);
                mail.AlternateViews.Add(htmlView);
                Client.Send(mail);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
