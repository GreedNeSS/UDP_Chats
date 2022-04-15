using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleChat
{
    internal class UDPClientApp: IDisposable
    {
        private UdpClient sender;
        private UdpClient receiver;
        public int LocalPort { get; private set; }
        public int RemotePort { get; private set; }
        public string Address { get; private set; }

        public UDPClientApp(int localPort, int remotePort, string ipAddress)
        {
            LocalPort = localPort;
            RemotePort = remotePort;
            Address = ipAddress;
            receiver = new UdpClient(localPort);
            sender = new UdpClient(Address, remotePort);
        }

        public void SendMessage(string message)
        {
            try
            {
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

                    while (true)
                    {
                        byte[] buffer = receiver.Receive(ref remoteIp);
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
