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
            string serverIP = textBox_Copy.Text;
            IPAddress address;

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

            if (IPAddress.TryParse(serverIP, out address))
            {
                try
                {
                    TcpClient client = new TcpClient(textBox_Copy.Text, 5000);
                    string password = passwordBox.Password;

                    Users user_login = new Users(1, textBox.Text, password, client);
                    string Recive_message = user_login.ReceiveMessage();

                    if (Recive_message == "AUTH;SUCCESS!$")
                    {
                        OknoGłówne ok = new OknoGłówne(client, textBox.Text, password);
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
                catch (Exception)
                {
                    MessageBox.Show("Problem połączenia z serwerem!");
                    return;
                }
            }

          
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

