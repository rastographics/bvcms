using CmsData.Email;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UtilityExtensions;

namespace SharedTestFixtures.Network
{
    public class MockEmailClient : IEmailClient
    {
        public Action<string, string, string, string> Receive { get; set; }

        public X509CertificateCollection ClientCertificates { get; }
        public bool EnableSsl { get; set; }
        public string PickupDirectoryLocation { get; set; }
        public SmtpDeliveryFormat DeliveryFormat { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public ServicePoint ServicePoint { get; }
        public int Timeout { get; set; }
        public ICredentialsByHost Credentials { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string TargetName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public event SendCompletedEventHandler SendCompleted;

        public void Send(string from, string recipients, string subject, string body)
        {
            Receive.Invoke(recipients, from, subject, body);
        }

        public void Send(MailMessage message)
        {
            var altView = message.AlternateViews.FirstOrDefault();
            long length = altView?.ContentStream?.Length ?? 0;
            var bytes = new byte[length];
            altView?.ContentStream?.Read(bytes, 0, (int)length);
            var altViewBody = Encoding.Default.GetString(bytes);
            Send(message.From.Address, message.To.Select(t =>t.Address).FirstOrDefault(), message.Subject, Util.PickFirst(altViewBody, message.Body));
        }

        public void SendAsync(MailMessage message, object userToken)
        {
            Send(message);
        }

        public void SendAsync(string from, string recipients, string subject, string body, object userToken)
        {
            Send(from, recipients, subject, body);
        }

        public void SendAsyncCancel()
        {
            throw new NotImplementedException();
        }

        public Task SendMailAsync(MailMessage message)
        {
            Send(message);
            return null;
        }

        public Task SendMailAsync(string from, string recipients, string subject, string body)
        {
            Send(from, recipients, subject, body);
            return null;
        }

        public void Dispose() { }
    }
}
