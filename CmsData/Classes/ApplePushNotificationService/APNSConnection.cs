using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.ApplePushNotificationService
{
	public class APNSConnection
	{
		private SslStream stream;
		private TcpClient client;

		public void open()
		{
			try {
				X509Store store = new X509Store(StoreLocation.LocalMachine);
				store.Open(OpenFlags.ReadOnly);

				X509CertificateCollection certificates = new X509CertificateCollection();
				certificates.AddRange(store.Certificates);

				client = new TcpClient();
				client.Connect("gateway.sandbox.push.apple.com", 2195);

				LocalCertificateSelectionCallback localCallBack = new LocalCertificateSelectionCallback(APNSLocalCert.SelectLocalCertificate);

				stream = new SslStream(client.GetStream(), false, (sender, cert, chain, sslPolicyErrors) => true, localCallBack);
				stream.AuthenticateAsClient("gateway.sandbox.push.apple.com", certificates, System.Security.Authentication.SslProtocols.Tls, false);
			} catch (Exception ex) { Console.Write(ex.Message); }
		}

		public SslStream getStream()
		{
			return stream;
		}

		public void close()
		{
			try {
				stream.Close();
				stream.Dispose();

				client.Client.Close();
				client.Client.Dispose();
			} catch (Exception ex) { Console.Write(ex.Message); }
		}
	}
}
