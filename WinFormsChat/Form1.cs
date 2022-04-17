using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsChat
{
    public partial class Form1 : Form
    {
        bool alive = false;
        UdpClient client;
        const int LOCALPORT = 8001;
        const int REMOTEPORT = 8001;
        const int TTL = 20;
        const string HOST = "235.5.5.1";
        IPAddress groupAddress;
        string userName;

        public Form1()
        {
            InitializeComponent();

            loginButton.Enabled = true;
            logoutButton.Enabled = false;
            sendButton.Enabled = false;
            chatTextBox.ReadOnly = true;

            groupAddress = IPAddress.Parse(HOST);
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            userName = userNameTextBox.Text;
            userNameTextBox.ReadOnly = true;

            try
            {
                client = new UdpClient(LOCALPORT);
                client.JoinMulticastGroup(groupAddress, TTL);
                Task.Run(() =>
                {
                    ReceiveMessages();
                });

                string message = userName + " вошел в чат";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                client.Send(buffer, buffer.Length, HOST, REMOTEPORT);

                loginButton.Enabled = false;
                logoutButton.Enabled = true;
                sendButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            alive = true;

            try
            {
                while (alive)
                {
                    IPEndPoint remoteIp = null;
                    byte[] buffer = client.Receive(ref remoteIp);
                    string message = Encoding.UTF8.GetString(buffer);

                    this.Invoke(new MethodInvoker(() =>
                    {
                        string time = DateTime.Now.ToShortTimeString();
                        chatTextBox.Text = time + " " + message + "\r\n" + chatTextBox.Text;
                    }));
                }
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                {
                    return;
                }
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string message = $"{userName}: {messageTextBox.Text}";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                client.Send(buffer, buffer.Length, HOST, REMOTEPORT);
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            ExitChat();
        }

        private void ExitChat()
        {
            string message = userName + " покидает чат!";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            client.Send(buffer, buffer.Length, HOST, REMOTEPORT);
            client.DropMulticastGroup(groupAddress);

            alive = false;
            client.Close();

            loginButton.Enabled = true;
            logoutButton.Enabled = false;
            sendButton.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosedEventArgs e)
        {
            if (alive)
            {
                ExitChat();
            }
        }
    }
}
