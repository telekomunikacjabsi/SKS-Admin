using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;

namespace SKS_Admin
{
    /// <summary>
    /// Interaction logic for OknoGłówne.xaml
    /// </summary>
    public partial class OknoGłówne : Window
    {
        private TcpClient client;
        private string[] users_table;
        private string login;
        private string password;
        public bool IsStop = false;
        public bool IsCancel = false;
        public bool IsCancelSmall = false;
        public List<Client> client_list;
        public List<string> name_clients;


        public OknoGłówne(TcpClient client, string login, string password)
        {
            InitializeComponent();
            this.password = password;
            this.login = login;
            this.client = client;
            GET_USERS();
            SEARCHING_START();
        }

        public void SEARCHING_START()
        {
            new Thread(() => SEARCH_NEW_CLIENT()).Start();
        }

        public void SEARCH_NEW_CLIENT()
        {
            //Thread.Sleep(5000);
            while (true)
            {
                Users user = new Users(2, client);
                string Recive_message_user = user.ReceiveMessage();
                if (Recive_message_user != "USERS;!$")
                {
                    Dispatcher.Invoke(() => { refresh(); });
                }
                Thread.Sleep(20000);
            }
        }

        private void GET_USERS()
        {
            //IsCancelSmall = false;
            Users user = new Users(2, client);
            string Recive_message_user = user.ReceiveMessage();
            string temp = Recive_message_user.Remove(Recive_message_user.Length - 2);
            if (temp=="USERS;")
            {
                return;
            }
            string[] users_table_temp = Regex.Split(temp, ";");
            users_table = Regex.Split(users_table_temp[1], "%1");
            IsStop = false;
            IsCancel = false;
            IsCancelSmall = false;
            conn_client(); //połączenie klientów z adminem
        }

        public void conn_client()
        {
            name_clients = new List<string>();
            client_list = new List<Client>();
            int j = 1;
            for (int i = 0; i < users_table.Length; i++)
            {
                string[] tab = Regex.Split(users_table[i], ":");
                Client cli = new Client(tab[0], tab[1], login, password);
                cli.connect();
                string x = cli.ReceiveMessage();
                string temp = x.Remove(x.Length - 2);
                string[] name_table = Regex.Split(temp, ";");
                name_clients.Add(name_table[2]);
                listBox1.Items.Add("Połączono: "+name_table[2]);
                listBox.Items.Add(j + ". " +name_table[2]);
                client_list.Add(cli);
                j++;
            }
            new Thread(() => SmallWindow()).Start();
           // new Thread(() => Getscreen(client_list[0])).Start();
        }

        public void SmallWindow()
        {
            while (true)
            {
                for (int i = 0; i < client_list.Count; i++)
                {
                    Getscreen_little(client_list[i], i);
                }
                /*if (IsCancelSmall == true)
                {
                    return;
                }*/
                Thread.Sleep(6000);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            IsStop = true;
            IsCancel = true;
            IsCancelSmall = true;
            for (int i = 0; i < client_list.Count; i++)
            {
                client_list[i].SendMessage("DISCONNECT!$");
            }
            clear();
            Thread.Sleep(300);
            GET_USERS();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            /*string temp = textBox.Text;
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
            //string Recive_message = user_login.ReceiveMessage();*/
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            string[] help = null;
            Users user = new Users(2, client);
            string Recive_message_user = user.ReceiveMessage();
            string temp = Recive_message_user.Remove(Recive_message_user.Length - 2);
            string[] users_table_temp = Regex.Split(temp, ";");
            if (users_table_temp[0] == "NEW_CLIENT")
            {
                help = Regex.Split(users_table_temp[1], "%1");
            }
            users_table = Regex.Split(users_table_temp[1], "%1");
            for (int i = 0; i < users_table.Length; i++)
            {
                listBox.Items.Add(users_table[i]);
            }
        }

        public void Getscreen(Client us)
        {
            while (true)
            {
                if (IsStop == false)
                {
                    us.SendMessage("SCREENSHOT!$");
                    Dispatcher.Invoke(() => {image.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
                    Thread.Sleep(1200);
                }
                if (IsCancel == true)
                {
                    return;
                }
            }
        }

        public void refresh()
        {
            IsStop = true;
            IsCancel = true;
            IsCancelSmall = true;
            for (int i = 0; i < client_list.Count; i++)
            {
                client_list[i].SendMessage("DISCONNECT!$");
            }
            Thread.Sleep(500);
            clear();
            GET_USERS();
        }

        public void Getscreen_little(Client us, int i)
        {
            us.SendMessage("SCREENSHOT!$");
            if (us.client.Connected == false)
            {
                string temp = name_clients[i];
                Dispatcher.Invoke(() => { refresh(); });
                Dispatcher.Invoke(() => { listBox1.Items.Add("Odłączono: " + temp); });
                return;
            }
            if (i == 0)
            {
                Dispatcher.Invoke(() =>{ image_Copy.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
                Dispatcher.Invoke(() => { label_image1.Content = name_clients[i]; });
            }
            else if (i == 1)
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        image_Copy1.Source = byteArrayToImage(us.ReceiveMessageIMG());
                    }
                    catch (Exception)
                    {
                        return;
                    }
                     });
                Dispatcher.Invoke(() => { label_image2.Content = name_clients[i]; });
            }
            else if (i == 2)
            {
                Dispatcher.Invoke(() => {
                    try
                    {
                        image_Copy2.Source = byteArrayToImage(us.ReceiveMessageIMG());
                    }
                    catch (Exception)
                    {
                        return ;
                    }
                     });
                Dispatcher.Invoke(() => { label_image3.Content = name_clients[i]; });
            }
            else if (i == 3)
            {
                Dispatcher.Invoke(() => { image_Copy3.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
                Dispatcher.Invoke(() => { label_image4.Content = name_clients[i]; });
            }
            else if (i == 4)
            {
                Dispatcher.Invoke(() => { image_Copy4.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
                Dispatcher.Invoke(() => { label_image5.Content = name_clients[i]; });
            }
            /*else if(i == 5)
                Dispatcher.Invoke(() => { image_Copy5.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            else if(i == 6)
                Dispatcher.Invoke(() => { image_Copy6.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            else if(i == 7)
                Dispatcher.Invoke(() => { image_Copy7.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 8)
                Dispatcher.Invoke(() => { image_Copy8.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 9)
                Dispatcher.Invoke(() => { image_Copy9.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 10)
                Dispatcher.Invoke(() => { image_Copy10.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 11)
                Dispatcher.Invoke(() => { image_Copy11.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 12)
                Dispatcher.Invoke(() => { image_Copy12.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 13)
                Dispatcher.Invoke(() => { image_Copy13.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 14)
                Dispatcher.Invoke(() => { image_Copy14.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 15)
                Dispatcher.Invoke(() => { image_Copy15.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
            if (i == 16)
                Dispatcher.Invoke(() => { image_Copy16.Source = byteArrayToImage(us.ReceiveMessageIMG()); });
                */      
        }

        public ImageSource byteArrayToImage(byte[] byteArrayIn)
        {
            BitmapImage image = null;
            if (byteArrayIn != null)
            {
                MemoryStream stream = new MemoryStream(byteArrayIn);
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
            }
             return image;            
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = true;
            this.Statystyki_grid.Visibility = System.Windows.Visibility.Hidden;
            this.button6.Visibility = System.Windows.Visibility.Hidden;
            this.button7.Visibility = System.Windows.Visibility.Hidden;
            this.Scroll.Visibility = System.Windows.Visibility.Visible;
            this.MainWindow.Visibility = System.Windows.Visibility.Visible;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string curItem = listBox.SelectedItem.ToString();
                if (curItem != null)
                {
                    string[] tab = Regex.Split(curItem, ". ");
                    int x = Int32.Parse(tab[0]);
                    client_list[x - 1].SendMessage("MESSAGE;" + textBox1.Text + "!$");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Zaznacz użytkownika do którego chcesz wysłać wiadomość.");
            }

        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Png Image|*.png";
            dialog.ShowDialog();
            if (dialog.FileName != "")
            {
                //System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
                String nameOfFile = dialog.FileName;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                using (FileStream stream = new FileStream(nameOfFile, FileMode.Create))
                    encoder.Save(stream);
                MessageBox.Show("Screen został zapisany");
            }
        }

        private void button7_Click_1(object sender, RoutedEventArgs e)
        {
            if (IsStop == false)
            {
                IsStop = true;
                button7.Content = "Start";
            }
            else
            {
                IsStop = false;
                button7.Content = "Stop";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(1);
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            this.czarna_grid.Visibility = System.Windows.Visibility.Hidden;
            this.textBox.Visibility = System.Windows.Visibility.Hidden;
            this.Statystyki_grid.Visibility = System.Windows.Visibility.Hidden;
            this.button5.Visibility = System.Windows.Visibility.Visible;
            this.label3_Copy.Visibility = System.Windows.Visibility.Visible;
            this.textBox1.Visibility = System.Windows.Visibility.Visible;
        }

        private void clear()
        {
            label_image1.Content = "Offline";
            label_image2.Content = "Offline";
            label_image3.Content = "Offline";
            label_image4.Content = "Offline";
            label_image5.Content = "Offline";
            label_image6.Content = "Offline";
            label_image7.Content = "Offline";
            label_image8.Content = "Offline";
            label_image9.Content = "Offline";
            label_image10.Content = "Offline";
            label_image11.Content = "Offline";
            label_image12.Content = "Offline";
            label_image13.Content = "Offline";
            label_image14.Content = "Offline";
            label_image15.Content = "Offline";
            label_image16.Content = "Offline";
            image_Copy.Source = null;
            image_Copy1.Source = null;
            image_Copy2.Source = null;
            image_Copy3.Source = null;
            image_Copy4.Source = null;
            image_Copy5.Source = null;
            image_Copy6.Source = null;
            image_Copy7.Source = null;
            image_Copy8.Source = null;
            image_Copy9.Source = null;
            image_Copy10.Source = null;
            image_Copy11.Source = null;
            image_Copy12.Source = null;
            image_Copy13.Source = null;
            image_Copy14.Source = null;
            image_Copy15.Source = null;
            listBox.Items.Clear();
            listBox1.Items.Clear();
        }

        public void START_BIG(int i)
        {
            new Thread(() => Getscreen(client_list[i])).Start();
        }

        public void SwitchBigVideo(int i)
        {
            if (i >= client_list.Count)
            {
                return;
            }
            this.Statystyki_grid.Visibility = System.Windows.Visibility.Visible;
            this.MainWindow.Visibility = System.Windows.Visibility.Hidden;
            this.Scroll.Visibility = System.Windows.Visibility.Hidden;
            this.button6.Visibility = System.Windows.Visibility.Visible;
            this.button7.Visibility = System.Windows.Visibility.Visible;
            IsCancel = false;
            /*client_list[i].SendMessage("PROCESSES!$");
            string Recive_message = client_list[i].ReceiveMessage();
            string temp = Recive_message.Remove(Recive_message.Length - 2);
            MessageBox.Show(temp);*/
            Dispatcher.Invoke(() => { START_BIG(i); });
        }

        private void button8_Click_1(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(0);
        }

        private void button8_Click_2(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(1);
        }

        private void button8_Click_3(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(2);
        }

        private void button8_Click_4(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(3);
        }

        private void button8_Click_5(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(4);
        }

        private void button8_Click_6(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(5);
        }

        private void button8_Click_7(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(6);
        }

        private void button8_Click_8(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(7);
        }

        private void button8_Click_9(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(8);
        }

        private void button8_Click_10(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(9);
        }

        private void button8_Click_11(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(10);
        }

        private void button8_Click_12(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(11);
        }

        private void button8_Click_13(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(12);
        }

        private void button8_Click_14(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(13);
        }

        private void button8_Click_15(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(14);
        }

        private void button8_Click_16(object sender, RoutedEventArgs e)
        {
            SwitchBigVideo(15);
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
