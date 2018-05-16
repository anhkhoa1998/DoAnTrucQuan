using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        IPEndPoint IP;
        Socket client;
        public Client()
        {
            InitializeComponent();
            Connect();

        }

        private void Connect()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6740);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Cannot connect to server" + MessageBoxButtons.OK + MessageBoxIcon.Error);
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }
        private void Dissconnect()
        {
            client.Close();
        }
        private void Send()
        {
            if (tbMessage.Text != string.Empty)
                client.Send(Serialize(tbMessage.Text));

        }
        private void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    client.Receive(data);
                    string message = Deserialize(data) as string;
                    AddMessage(message);
                }
            }
            catch
            {
                Dissconnect();
            }
        }
        private void AddMessage(string message)
        {
            lvMessage.Items.Add(new ListViewItem() { Text = message });
            tbMessage.Clear();
        }
        byte[] Serialize(string obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatte = new BinaryFormatter();
            formatte.Serialize(stream, obj);
            return stream.ToArray();
        }
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatte = new BinaryFormatter();
            return formatte.Deserialize(stream);

        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dissconnect();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
            AddMessage(tbMessage.Text);
        }
    }
   
}
