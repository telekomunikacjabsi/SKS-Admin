using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SKS_Admin
{
    class Client
    {
        private TcpClient client;
        private string login;
        private string password;
        private string communicat;

        public Client(TcpClient client)
        {
            this.client = client;
        }

      /*  public Client(TcpClient client, string communicat)
        {
            this.communicat = communicat;
            this.client = client;
        }*/

        public Client(TcpClient client, string communicat)
        {
            this.communicat = communicat;
            this.client = client;
            screen();
        }

        public Client(TcpClient client, string communicat, string login, string password)
        {
            this.login = login;
            this.password = password;
            this.client = client;
            connect(client);
        }

        public void screen()
        {         
            try
            {
                SendMessage(communicat);             
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public void connect(TcpClient client)
        {
            try
            {
                string help = CalculateSHA256(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(login));
                String temp = "CONNECT;" + help  + "!$";
                SendMessage(temp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SendMessage(String temp)
        {
            NetworkStream stream = client.GetStream();
            byte[] message = Encoding.UTF8.GetBytes(temp);
            stream.Write(message, 0, message.Length);
        }

        public string ReceiveMessage()
        {
            NetworkStream str = client.GetStream();
            byte[] inStream = new byte[255];
            str.Read(inStream, 0, 255);
            string returndata = Encoding.UTF8.GetString(inStream);
            MessageBox.Show("(Class Client)"+returndata);
            return returndata.Substring(0, returndata.IndexOf('\0'));
        }

        public byte[] ReceiveIMAGE()
        {
            NetworkStream str = client.GetStream();
           // int x = client.ReceiveBufferSize();
            byte[] inStream = new byte[255];
            str.Read(inStream, 0, 255);
            return inStream;
        }


        private string CalculateSHA256(byte[] text, byte[] salt)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(text.Concat(salt).ToArray(), 0, text.Length + salt.Length);
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }       
    }
}
