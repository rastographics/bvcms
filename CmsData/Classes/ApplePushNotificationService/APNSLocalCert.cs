using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.ApplePushNotificationService
{
	public class APNSLocalCert
	{
		public static X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			if (acceptableIssuers != null && acceptableIssuers.Length > 0 && localCertificates != null && localCertificates.Count > 0) {
				foreach (X509Certificate certificate in localCertificates) {
					string issuer = certificate.Issuer;

					if (Array.IndexOf(acceptableIssuers, issuer) != -1) {
						return certificate;
					}
				}
			}

			if (localCertificates != null && localCertificates.Count > 0) {
				return localCertificates[0];
			}

			return null;
		}
	}
}
