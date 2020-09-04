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
    /// SettingsPopupView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsPopupView : Window
    {
        public SettingsPopupView()
        {
            InitializeComponent();
        }

        private void BarcodeName_TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectDeviceNamePopupView popup = new SelectDeviceNamePopupView();
            popup.ShowDialog();
            if (popup.DialogResult == true)
            {

            }
        }
    }
}
