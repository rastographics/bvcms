using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;
using Newtonsoft.Json;

namespace CmsData.Classes.ApplePushNotificationService
{
	public class APNSNotification
	{
		private byte[] command = new byte[1] { 0x02 };
		private byte[] commandBytes;

		private byte[] tokenID = new byte[1] { 0x01 };
		private byte[] tokenSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(32)));
		private byte[] tokenData;

		private byte[] messageID = new byte[1] { 0x02 };
		private byte[] messageSize;
		private byte[] messageData;

		private byte[] priorityID = new byte[1] { 0x05 };
		private byte[] prioritySize = new byte[2] { 0x00, 0x01 };
		private byte[] priorityData;

		public APNSNotification(APNSMessage message, bool priority)
		{
			Dictionary<string, Object> aps = new Dictionary<string, Object>();
			aps.Add("aps", message.getDictionary());

			string json = JsonConvert.SerializeObject(aps);

			messageSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(json.Length)));
			messageData = Encoding.UTF8.GetBytes(json);

			if (priority) {
				priorityData = new byte[1] { 0x10 };
			} else {
				priorityData = new byte[1] { 0x05 };
			}

			// ID + Token Size + Token + Message ID + Message Size + Message Data Length
			int commandSize = 1 + 2 + 32 + 1 + messageSize.Length + messageData.Length;

			commandBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(commandSize));
		}

		public void setDeviceToken(string token64)
		{
			tokenData = Convert.FromBase64String(token64);
		}

		public void sendViaConnection(APNSConnection connection)
		{
			if (tokenData == null) return;
			if (tokenData.Length != 32) return;

			SslStream stream = connection.getStream();

			stream.Write(command);
			stream.Write(commandBytes);

			stream.Write(tokenID);
			stream.Write(tokenSize);
			stream.Write(tokenData);

			stream.Write(messageID);
			stream.Write(messageSize);
			stream.Write(messageData);
		}
	}
}
