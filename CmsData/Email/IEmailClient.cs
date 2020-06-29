using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;

namespace CmsData.Email
{
    public interface IEmailClient : IDisposable
    {
        X509CertificateCollection ClientCertificates { get; }
        bool EnableSsl { get; set; }
        string PickupDirectoryLocation { get; set; }
        SmtpDeliveryFormat DeliveryFormat { get; set; }
        SmtpDeliveryMethod DeliveryMethod { get; set; }
        ServicePoint ServicePoint { get; }
        int Timeout { get; set; }
        ICredentialsByHost Credentials { get; set; }
        bool UseDefaultCredentials { get; set; }
        string TargetName { get; set; }
        string Host { get; set; }
        int Port { get; set; }

        event SendCompletedEventHandler SendCompleted;

        void Send(MailMessage message);
        void Send(string from, string recipients, string subject, string body);
        void SendAsync(MailMessage message, object userToken);
        void SendAsync(string from, string recipients, string subject, string body, object userToken);
        void SendAsyncCancel();
        System.Threading.Tasks.Task SendMailAsync(MailMessage message);
        System.Threading.Tasks.Task SendMailAsync(string from, string recipients, string subject, string body);
    }
}
