using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;

namespace CmsData.Email
{
    public class DefaultSmtpClient : IEmailClient
    {
        private SmtpClient client = new SmtpClient();

        public X509CertificateCollection ClientCertificates => client.ClientCertificates;
        public bool EnableSsl
        {
            get => client.EnableSsl;
            set => client.EnableSsl = value;
        }

        public string PickupDirectoryLocation
        {
            get => client.PickupDirectoryLocation;
            set => client.PickupDirectoryLocation = value;
        }

        public SmtpDeliveryFormat DeliveryFormat
        {
            get => client.DeliveryFormat;
            set => client.DeliveryFormat = value;
        }

        public SmtpDeliveryMethod DeliveryMethod
        {
            get => client.DeliveryMethod;
            set => client.DeliveryMethod = value;
        }

        public ServicePoint ServicePoint => client.ServicePoint;
        public int Timeout
        {
            get => client.Timeout;
            set => client.Timeout = value;
        }

        public ICredentialsByHost Credentials
        {
            get => client.Credentials;
            set => client.Credentials = value;
        }

        public bool UseDefaultCredentials
        {
            get => client.UseDefaultCredentials;
            set => client.UseDefaultCredentials = value;
        }

        public string TargetName
        {
            get => client.TargetName;
            set => client.TargetName = value;
        }

        public string Host
        {
            get => client.Host;
            set => client.Host = value;
        }

        public int Port
        {
            get => client.Port;
            set => client.Port = value;
        }

        public event SendCompletedEventHandler SendCompleted;

        public void Dispose() => client = null;

        public void Send(MailMessage message) => client.Send(message);

        public void Send(string from, string recipients, string subject, string body) => client.Send(from, recipients, subject, body);

        public void SendAsync(MailMessage message, object userToken) => client.SendAsync(message, userToken);

        public void SendAsync(string from, string recipients, string subject, string body, object userToken) => client.SendAsync(from, recipients, subject, body, userToken);

        public void SendAsyncCancel() => client.SendAsyncCancel();

        public System.Threading.Tasks.Task SendMailAsync(MailMessage message) => client.SendMailAsync(message);

        public System.Threading.Tasks.Task SendMailAsync(string from, string recipients, string subject, string body) => client.SendMailAsync(from, recipients, subject, body);
    }
}
