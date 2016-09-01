using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WakeOnLan.exe
{
    public static class Power
    {
        private const int UDP_PORT = 7;

        public static void WakeUp(byte[] bytes)
        {
            int udpPort = UDP_PORT;
            int.TryParse(ConfigurationManager.AppSettings["UDP_PORT"], out udpPort);

            List<byte> packet = new List<byte>();

            for (int i = 0; i < 6; i++)
                packet.Add(0xFF);
    
            for (int i = 0; i < 16; i++)
                packet.AddRange(bytes);

            UdpClient client = new UdpClient();

            List<IPAddress> targetIpAddresses = ConfigurationManager.AppSettings["TargetIpAddresses"].Split(',').Select(IPAddress.Parse).ToList();
            targetIpAddresses.Add(IPAddress.Broadcast);

            foreach (IPAddress targetIpAddress in targetIpAddresses)
            {
                client.Connect(targetIpAddress, udpPort);
                client.Send(packet.ToArray(), packet.Count);
            }
        }
    }
}
