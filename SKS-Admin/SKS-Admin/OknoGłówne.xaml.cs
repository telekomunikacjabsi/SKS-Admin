using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;

namespace SKS_Admin
{
    /// <summary>
    /// Interaction logic for OknoGłówne.xaml
    /// </summary>
    public partial class OknoGłówne : Window
    {
        private TcpClient client;
        private List<string> black_list;
        private string[] black_list_table2;
        private string[] black_list_table;
        private string[] users_table;
        private string login;
        private string password;
        private Thread th;

        public OknoGłówne(TcpClient client, string login, string password)
        {
            InitializeComponent();
            this.password = password;
            this.login = login;
            this.client = client;
            black_list = new List<string>();
            GET_LIST();
            GET_USERS();
        }

        private void GET_LIST()
        {
            list_czarna.Items.Clear();
            Users user_login = new Users(3, "VERIFYLIST;", client);
            string Recive_message = user_login.ReceiveMessage();
            string temp = Recive_message.Remove(Recive_message.Length - 2);
            if (Recive_message=="OK;!$")
            {
                return;
            }
            else
            {
                black_list_table2 = Regex.Split(temp, ";");
                MessageBox.Show(black_list_table2[2]);
                black_list_table = Regex.Split(black_list_table2[2], ":");
                for (int i = 0; i < black_list_table.Length; i++)
                {
                    list_czarna.Items.Add(black_list_table[i]);
                    black_list.Add(black_list_table[i]);
                }
            }
        }

        private void GET_USERS()
        {
            Users user = new Users(2, client);
            string Recive_message_user = user.ReceiveMessage();
            string temp = Recive_message_user.Remove(Recive_message_user.Length-2);
            string [] users_table_temp = Regex.Split(temp, ";");
            users_table = Regex.Split(users_table_temp[1], "%1");
                        
             for (int i = 0; i < users_table.Length; i++)
             {
                 listBox.Items.Add(users_table[i]);
             }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.czarna_grid.Visibility = System.Windows.Visibility.Visible;
            this.textBox.Visibility = System.Windows.Visibility.Visible;
            this.Statystyki_grid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string temp = textBox.Text;
            if (!list_czarna.Items.Contains(temp))
            {
                list_czarna.Items.Add(temp);
            }
            if (black_list.Exists(x => x == temp))
            {
                return;
            }
            black_list.Add(temp);

            string tab = "";
            int i = 1;
            foreach (string black in black_list)
            {
                if (i == black_list.Count)
                {
                    tab += black;
                }
                else
                {
                    tab += black + ":";
                }
                i++;
            }
            MessageBox.Show(tab);
            Users user_login = new Users(4, client, "LIST", tab);
            //string Recive_message = user_login.ReceiveMessage();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            string [] help = null;
            Users user = new Users(2, client);
            string Recive_message_user = user.ReceiveMessage();
            string temp = Recive_message_user.Remove(Recive_message_user.Length - 2);
            string[] users_table_temp = Regex.Split(temp, ";");
            if (users_table_temp[0] == "NEW_CLIENT")
            {
                help = Regex.Split(users_table_temp[1], "%1");
                //listBox.Items.Add(help[0]);
            }
            users_table = Regex.Split(users_table_temp[1], "%1");
            for (int i = 0; i < users_table.Length; i++)
            {
                listBox.Items.Add(users_table[i]);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string curItem = listBox.SelectedItem.ToString();
            if (curItem != null)
            {
                string[] tab = Regex.Split(curItem, ":");
                TcpClient client2 = new TcpClient();
                IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(tab[0]), int.Parse(tab[1]));
                client2.Connect(IP_End);
                Client us = new Client(client2, "CONNECT", login, password);
                us.ReceiveMessage();

                us.SendMessage("SCREENSHOT!$");
                MessageBox.Show(""+us.ReceiveIMAGE());



            }
        }

        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            //MessageBox.Show("" + port);
            return port;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            this.Statystyki_grid.Visibility = System.Windows.Visibility.Visible;
            //this.czarna_grid.Visibility = System.Windows.Visibility.Hidden;
            this.textBox.Visibility = System.Windows.Visibility.Hidden;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            string curItem = listBox.SelectedItem.ToString();
            if (curItem != null)
            {
                string[] tab = null;
                tab = Regex.Split(curItem, ":");
                TcpClient client2 = new TcpClient();
                IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(tab[0]), int.Parse(tab[1]));
                //if (!client2.Client.Connected)
                //{
                client2.Connect(IP_End);
                    Client us = new Client(client2, "CONNECT", login, password);
                    us.ReceiveMessage();
                    
                //}
                //Client us2 = new Client(client2);
                us.SendMessage("MESSAGE;"+ textBox1.Text + "!$");
                //us.SendMessage("DISCONNECT!$");
                
                client2.Close();
            }
        }
    }
}



//$$$$$$$$$$$$$$$$$  DO ŁĄCZENIA SIĘ Z KLIENTAMI
/*
        public Users(TcpClient client, string communicat, string login, string password)
        {
            this.login = login;
            this.password = password;
            this.client = client;
            connect2(client);
        }

        public void connect2(TcpClient client)
        {
            try
            {
                string help = CalculateSHA256(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(login));
                String temp = "CONNECT;"  + help + ";1900!$";
                SendMessage(temp);
            }
            catch (Exception)
            {
                throw;
            }
        }
    */
