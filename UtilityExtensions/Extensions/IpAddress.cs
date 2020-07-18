using System.Net;
using System.Net.Sockets;

namespace UtilityExtensions.Extensions
{
    public static class IpAddress
    {
        public static string GetIpAddress()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
                if (addr[i].AddressFamily == AddressFamily.InterNetwork)
                    return addr[i].ToString();

            return string.Empty;
        }
    }
}
