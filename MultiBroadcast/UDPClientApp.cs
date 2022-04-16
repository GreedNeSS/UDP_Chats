using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MultiBroadcast
{
    internal class UDPClientApp: IDisposable
    {
        private UdpClient sender;
        private UdpClient receiver;
        private string userName;
        private IPAddress remoteAddress;
        public int LocalPort { get; private set; }
        public int RemotePort { get; private set; }
        public string Address { get; private set; }

        public UDPClientApp(string userName)
        {
            this.userName = userName;
            LocalPort = 8001;
            RemotePort = 8001;
            Address = "235.0.0.20";
            remoteAddress = IPAddress.Parse(Address);

            receiver = new UdpClient(LocalPort);
            receiver.JoinMulticastGroup(remoteAddress, 50);
            sender = new UdpClient(Address, RemotePort);
        }

        public void SendMessage(string message)
        {
            try
            {
                message = $"{userName}: {message}";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                sender.Send(buffer, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ReceiveMessage()
        {
            Task.Run(() =>
            {
                try
                {
                    IPEndPoint remoteIp = null;
                    string localAddress = LocalIPAddress();

                    while (true)
                    {
                        byte[] buffer = receiver.Receive(ref remoteIp);

                        if (remoteIp.Address.ToString().Equals(localAddress))
                            continue;

                        string message = Encoding.UTF8.GetString(buffer);
                        Console.WriteLine($"{remoteIp.Address}: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    receiver.Close();
                }
            });
        }

        private string LocalIPAddress()
        {
            string localIP = string.Empty;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            return localIP;
        }

        public void Dispose()
        {
            if (sender != null)
            {
                sender.Close();
            }

            if (receiver != null)
            {
                receiver.Close();
            }
        }
    }
}
