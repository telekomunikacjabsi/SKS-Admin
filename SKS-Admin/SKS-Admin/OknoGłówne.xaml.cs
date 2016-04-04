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

namespace SKS_Admin
{
    /// <summary>
    /// Interaction logic for OknoGłówne.xaml
    /// </summary>
    public partial class OknoGłówne : Window
    {
        public OknoGłówne()
        {
            InitializeComponent();
            ADD_LIST();
        }

        private void ADD_LIST()
        {
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
            listBox.Items.Add("PC1");
            listBox.Items.Add("PC2");
        }
    }
}
