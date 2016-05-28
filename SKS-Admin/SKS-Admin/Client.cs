using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        string message;
        readonly string packetEndSign = "!$";
        public byte[] toBytes = null;
        public byte[] bytessss = null;
        public List<byte> message_que = new List<byte>();

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

        public byte[] get_byte()
        {
            return toBytes;
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
            //MessageBox.Show("(Class Client)"+returndata);
            return returndata.Substring(0, returndata.IndexOf('\0'));
        }

         /*public byte[] ReceiveIMAGE()
         {
             NetworkStream str = client.GetStream();
            // int x = client.ReceiveBufferSize();
             byte[] inStream = new byte[85600];
             str.Read(inStream, 0, 85600);
             return inStream;
         }*/

        public void ReceiveMessageIMG(bool recurrentCall = false)
        {
            NetworkStream stream = client.GetStream();
            int i;
            bool EndPart = false;
            byte [] znak = Encoding.UTF8.GetBytes("!$");
            byte[] bytes = new byte[256];
            if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                for (int j = 0; j < bytes.Length; j++)
                {
                    message_que.Add(bytes[j]);
                    if (j < bytes.Length-1)
                    {
                        if (bytes[j] == znak[0] && bytes[j + 1] == znak[1])
                        {
                            EndPart = true;
                        }
                    }
                }
                //message_que.CopyTo(bytes, 0);
                message_que.IndexOf(znak[0]);

                if (EndPart == false) // jeśli odczytana wiadomość nie zawiera znacznika końca wiadomości
                    ReceiveMessageIMG(true);
                if (recurrentCall)
                    return;
            }
            toBytes = message_que.Concat(bytes).ToArray<byte>();
            //string[] users_table_temp = Regex.Split(message, ";");
            //toBytes = Encoding.UTF8.GetBytes(bytes);
            File.WriteAllBytes("admin.txt", toBytes);
            MessageBox.Show(" ");
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

        private string[] SplitMessages(string message)
        {
            string[] array;
            int index = message.IndexOf(packetEndSign);
            if (index == -1)
            {
                array = new string[1];
                array[0] = message;
                return array;
            }
            array = new string[2];
            array[0] = message.Substring(0, index);
            array[1] = message.Substring(index + packetEndSign.Length);
            return array;
        }

        private void ReplaceInArray(string[] array, string replaceFrom, string replaceTo)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = array[i].Replace(replaceFrom, replaceTo);
        }

    }
}





/*
public void ReceiveMessageIMG(bool recurrentCall = false)
{
    NetworkStream stream = client.GetStream();
    string[] messages = null;
    int i;
    byte[] bytes = new byte[256];
    if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
    {
        message += Encoding.UTF8.GetString(bytes, 0, i);
        if (message.IndexOf(packetEndSign) == -1) // jeśli odczytana wiadomość nie zawiera znacznika końca wiadomości
            ReceiveMessageIMG(true);
        if (recurrentCall)
            return;
        messages = SplitMessages(message);
        message = messages[0];
    }
    string result = string.Join(string.Empty, message.Skip(11));
    //string[] users_table_temp = Regex.Split(message, ";");
    toBytes = Encoding.UTF8.GetBytes(result);
    File.WriteAllBytes("admin.txt", toBytes);
    MessageBox.Show(" ");
}*/