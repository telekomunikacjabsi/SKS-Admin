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
        private string[] black_list_table;

        public OknoGłówne(TcpClient client)
        {
            InitializeComponent();
            this.client = client;
            black_list = new List<string>();
            ADD_LIST();
        }

        private void ADD_LIST()
        {
            Users user_login = new Users(3, "VERIFYLIST;", client);
            string Recive_message = user_login.ReceiveMessage();
            //black_list_table = Regex.Split(Recive_message, ";");
            /*listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC3");
            listBox.Items.Add("PC4");
            listBox.Items.Add("PC5");*/
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
            black_list.Add(temp);
            if (!list_czarna.Items.Contains(temp))
            {
                list_czarna.Items.Add(temp);
            }
            //list_czarna.Items.Add(temp);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Users user_login = new Users(2, client);
            string Recive_message = user_login.ReceiveMessage();
            listBox.Items.Clear();

        }
    }
}
