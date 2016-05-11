using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SKS_Admin
{
    class Client
    {
        private TcpClient pc_client;

        public Client(TcpClient pc_client)
        {
            this.pc_client = pc_client;
        }

        public void SendMessage(String temp)
        {
            NetworkStream stream = pc_client.GetStream();
            byte[] message = Encoding.UTF8.GetBytes(temp);
            stream.Write(message, 0, message.Length);
        }

        public string ReceiveMessage()
        {
            NetworkStream str = pc_client.GetStream();
            byte[] inStream = new byte[255];
            str.Read(inStream, 0, 255);
            string returndata = Encoding.UTF8.GetString(inStream);
            MessageBox.Show("(Class Client)"+returndata);
            return returndata.Substring(0, returndata.IndexOf('\0'));
        }
    }
}
