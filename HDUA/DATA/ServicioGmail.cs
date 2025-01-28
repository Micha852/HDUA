using MimeKit.Text;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace PROYECTO.SERVICIOS
{
    public class ServicioGmail
    {
        public void SendEmailGmail(String receptor, String asunto, String mensaje)
        {
            var aux = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            MimeMessage mail = new MimeMessage();

            String usermail = aux.GetSection("gmail:usuariogmail").Value;

            String passwordmail = aux.GetSection("gmail:pwordgmail").Value;

            mail.From.Add(MailboxAddress.Parse(usermail));
            mail.To.Add(MailboxAddress.Parse(receptor));
            mail.Subject = asunto;
            mail.Body = new TextPart(TextFormat.Html)
            {
                Text = mensaje
            };
            String smtpserver = aux.GetSection("gmail:hostgmail").Value;
            int port = int.Parse(aux.GetSection("gmail:port").Value);
            bool ssl = bool.Parse(aux.GetSection("gmail:ssl").Value);
            bool defaultcreadentials = bool.Parse(aux.GetSection("gmail:defaultcredencial").Value);

            using var smtpClient = new SmtpClient();
            smtpClient.Connect(
                smtpserver,
                port,
                SecureSocketOptions.StartTls
                );
            smtpClient.Authenticate(usermail, passwordmail);
            smtpClient.Send(mail);
            smtpClient.Disconnect(true);
        }
    }
}
