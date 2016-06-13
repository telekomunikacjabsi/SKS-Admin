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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SKS_Admin
{
    public class Client
    {
        public TcpClient client;
        private string login;
        private string password;
        private string communicat;
        string message;
        readonly string packetEndSign = "!$";
        public byte[] toBytes = null;
        public byte[] bytessss = null;
        public List<byte> message_que = new List<byte>();
        public NetworkStream stream;


        public Client(string ip, string port, string login, string password)
        {
            this.login = login;
            this.password = password;
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            client.Connect(IP_End);
            stream = client.GetStream();
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

        public void connect()
        {
            try
            {
                string help = CalculateSHA256(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(login));
                String temp = "CONNECT;" + help + "!$";
                SendMessage(temp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SendMessage(String temp)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] message = Encoding.UTF8.GetBytes(temp);
                stream.Write(message, 0, message.Length);
            }
            catch (Exception)
            {
                return;
            }
        }

        /*public string ReceiveMessage()
        {
            NetworkStream str = client.GetStream();
            byte[] inStream = new byte[255];
            str.Read(inStream, 0, 255);
            string returndata = Encoding.UTF8.GetString(inStream);
            returndata += "\0";
            //MessageBox.Show("(Class Client)"+returndata);
            str.Flush();
            return returndata.Substring(0, returndata.IndexOf('\0'));
        }*/

        public string ReceiveMessage(bool recurrentCall = false)
        {
            string[] messages = null;
            int i;
            byte[] bytes = new byte[256];
            if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                message += Encoding.UTF8.GetString(bytes, 0, i);
                if (message.IndexOf(packetEndSign) == -1) // jeśli odczytana wiadomość nie zawiera znacznika końca wiadomości
                    ReceiveMessage(true);
                if (recurrentCall)
                    return message;
                messages = SplitMessages(message);
                message = messages[0];
            }
            //stream.Flush();
            return message;
        }

        public byte[] ReceiveMessageIMG() 
        {
            NetworkStream stream = client.GetStream();
            int dataLength = GetDataLength(client.GetStream());
            if (dataLength == 0)
            {
                byte[] x = { 0};
                return x;
            }
            //toBytes = null;
            byte[] bytes = new byte[dataLength];
            stream.Read(bytes, 0, bytes.Length);
            //toBytes = bytes;
            stream.Flush();
            return bytes;
        }

        private int GetDataLength(NetworkStream stream)
        {
            Queue<byte> headerBytes = new Queue<byte>();
            byte[] buffer = new byte[1];
            byte argumentSeparator = Encoding.UTF8.GetBytes(";")[0];
            while (stream.Read(buffer, 0, 1) >= 0)
            {
                if (buffer[0] == argumentSeparator)
                    return BytesToInt(headerBytes.ToArray());
                headerBytes.Enqueue(buffer[0]);
            }
            throw new EndOfStreamException();
        }

        private int BytesToInt(byte[] bytes)
        {
            string numberString = Encoding.UTF8.GetString(bytes);
            int number;
            if (Int32.TryParse(numberString, out number))
                return number;
            else
                return 0;
               // throw new FormatException();
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