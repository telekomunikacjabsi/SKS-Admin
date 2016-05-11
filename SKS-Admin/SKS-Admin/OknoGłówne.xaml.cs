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

        public OknoGłówne(TcpClient client)
        {
            InitializeComponent();
            this.client = client;
            black_list = new List<string>();
            ADD_LIST();
            GET_USERS();
        }

        private void ADD_LIST()
        {
            Users user_login = new Users(3, "VERIFYLIST;", client);
            string Recive_message = user_login.ReceiveMessage();
            if (Recive_message=="OK;!$")
            {
                return;
            }
            else
            {
                black_list_table2 = Regex.Split(Recive_message, ";");
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
            users_table = Regex.Split(Recive_message_user, ";");
            
             for (int i = 1; i < users_table.Length; i++)
             {
                 listBox.Items.Add(users_table[i]);
             }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

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
            Users user_login = new Users(2, client);
            string Recive_message = user_login.ReceiveMessage();
            listBox.Items.Clear();
            //usunięcie poprzednich danych po odświeżeniu
            users_table = Regex.Split(Recive_message, ";");

            for (int i = 1; i < users_table.Length; i++)
            {
                listBox.Items.Add(users_table[i]);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string[] temp_table = Regex.Split(users_table[1], ":");
            TcpClient pc_client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(temp_table[0]), Int32.Parse(temp_table[1]));
            pc_client.Connect(IP_End);
            
            Client pc = new Client(pc_client);
        }
    }
}
