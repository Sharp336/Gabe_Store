using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Gabe_Store.Shared;
using static MudBlazor.CategoryTypes;

namespace Gabe_Store.Services.EmailProvider
{
    public class EmailProvider : IEmailProvider
    {
        public void SendGoodEmail(Good g, string email)
        {
            using (FileStream fs = new FileStream("EmailSenderCredentials.json", FileMode.OpenOrCreate))
            {
                EmailSenderCredentials? credentials = JsonSerializer.Deserialize<EmailSenderCredentials>(fs);

                string sending_email = credentials.login;
                string sending_password = credentials.password;

                MailMessage message = new MailMessage();

                try
                {
                    message.From = new MailAddress(sending_email);
                    message.To.Add(new MailAddress(email));
                    message.Subject = "Ur purchased good";
                    message.Body = g.Product;

                    SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com");
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Credentials = new NetworkCredential(sending_email, sending_password);

                    Console.Clear();
                    Console.WriteLine("hui8");

                    DirectoryInfo licenceFolderPath = new(Environment.CurrentDirectory + @"\LICENCE\");
                    Console.WriteLine($"licenceFolderPath is {licenceFolderPath.FullName}");
                    foreach (var file in licenceFolderPath.GetFiles())
                    {
                        Console.WriteLine($"Вы прекрепили Name - {file.Name}");
                        Console.WriteLine($"Вы прекрепили FullName - {file.FullName}");
                        message.Attachments.Add(new Attachment(file.FullName));
                    }

                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка при отправке письма: " + ex.Message);
                }
            }

        }
    }
}
