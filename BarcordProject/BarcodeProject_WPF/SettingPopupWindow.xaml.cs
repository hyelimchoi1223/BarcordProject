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

namespace BarcodeProject_WPF
{
    /// <summary>
    /// SettingPopupForm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPopupWindow : Window
    {
        public string test { get; set; }
        public SettingPopupWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            test = DeviceName_TextBox1.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
