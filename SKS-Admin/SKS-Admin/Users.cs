﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Security.Cryptography;

namespace SKS_Admin
{
    class Users
    {
        private TcpClient client;
        private string login;
        private string password;
        private string communicat;
        private string new_item;

        public Users(int kom_inf, string login, string password, TcpClient client)
        {
            this.client = client;
            this.login = login;
            this.password = password;
            connect(kom_inf);
        }

        public Users(int kom_inf, TcpClient client)
        {
            this.client = client;
            connect(kom_inf);
        }

        public Users(int kom_inf, string communicat, TcpClient client)
        {
            this.client = client;
            this.communicat = communicat;
            connect(kom_inf);
        }

        public Users(int kom_inf, TcpClient client, string communicat, string item)
        {
            this.client = client;
            this.new_item = item;
            this.communicat = communicat;
            connect(kom_inf);
        }
        
        public void connect(int kom_inf)
        {
            try
            {
                if (kom_inf == 1)
                {
                    string help = CalculateSHA256(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(login));
                    String temp = "CONNECT;ADMIN;" + login + ";" + help+"!$";
                    SendMessage(temp);
                }
                else if(kom_inf == 2)
                {
                    String temp = "USERS!$";
                    SendMessage(temp);
                }
                else if (kom_inf == 3)
                {
                    String temp = communicat;
                    SendMessage("VERIFYLIST;" + 0 + ";" + -1+ "!$");
                }
                else if (kom_inf == 4)
                {
                    String temp = communicat;
                    //MessageBox.Show(new_item);
                    SendMessage("LIST;" + 0 + ";" + new_item+"!$");
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Problem z połączeniem do serwera (class Usres)");
                //throw x;
                return;
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
            //MessageBox.Show(returndata);
            return returndata.Substring(0, returndata.IndexOf('\0'));
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
