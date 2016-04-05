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
            if (passwordBox.ToString() == null)
            {
                MessageBox.Show("Wproadź hasło!");
                return;
            }
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 5000);
            TcpListener listener = new TcpListener(ip);
            TcpClient client = new TcpClient();
            //IPEndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

           try
            {
                client.Connect("127.0.0.1", 5000);
            }
            catch (Exception)
            {
                MessageBox.Show("Problem z połączeniem z serwerem . . .");
                return;
            }
            NetworkStream stream = client.GetStream();
            WriteMessage(stream, "CONNECT;ADMIN;aka");
            while (true)
            {
                string[] message = ReceiveMessage(stream);
                if (message[1] == "SUCCESS")
                {
                    OknoGłówne ok = new OknoGłówne();
                    ok.Show();                   
                    this.Close();
                    return;
                    //stream.Close();
                }
                if (message[1] == "FAIL")
                {
                    MessageBox.Show("Error . . .");
                    return;
                }
                
                //textBox.AppendText(x[0]);
            }
            //client.Close();

            /*BinaryWriter writer = new BinaryWriter(client.GetStream());
            writer.Write("CONNECT;ADMIN;ala");*/
            /* try
             {
                 client.Connect(point);
                 if (client.Connected)
                 {
                     textBox1.AppendText("Udało sie");
                 }
             }
             catch (Exception)
             {

                 throw;
             }
             /*
             OknoGłówne ok = new OknoGłówne();
             ok.Show();
             this.Close();*/
        }

        private string[] ReceiveMessage(NetworkStream stream)
        {
            int i;
            byte[] bytes = new byte[256];
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string msg = Encoding.ASCII.GetString(bytes, 0, i);
                return Regex.Split(msg, ";"); // automatyczny podział komunikatu na argumenty
            }
            return new string[] { String.Empty };
        }

        private void WriteMessage(NetworkStream stream, params string[] message)
        {
            WriteMessage(stream, String.Join(";", message));
        }

        private void WriteMessage(NetworkStream stream, string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
