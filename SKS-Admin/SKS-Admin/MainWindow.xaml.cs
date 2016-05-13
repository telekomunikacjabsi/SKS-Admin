using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace SKS_Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();          
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.ToString() == "")
            {
                MessageBox.Show("Wproadź nazwę grupy!");
                return;
            }
            string temp = passwordBox.Password;
            if (temp == "")
            {
                MessageBox.Show("Wproadź hasło!");
                return;
            }

            TcpClient client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

            try
            {
                client.Connect(IP_End);
            }
            catch (Exception)
            {
                MessageBox.Show("Problem połączenia z serwerem!");
                return;
            }

            string password = passwordBox.Password;

            Users user_login = new Users(1, textBox.Text, password, client);
            string Recive_message = user_login.ReceiveMessage();

            if (Recive_message == "AUTH;SUCCESS!$")
                {
                    OknoGłówne ok = new OknoGłówne(client);
                    ok.Show();                   
                    this.Close();
                    //stream.Close();
                }
                else
                {
                    MessageBox.Show("Error . . .");
                    this.Close();
                    return;
                }    
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

