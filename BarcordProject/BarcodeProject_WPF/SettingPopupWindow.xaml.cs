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
            Properties.Settings.Default.DeviceName = GetControlValue(typeof(TextBox), "DeviceName_TextBox");
            Properties.Settings.Default.TagName = GetControlValue(typeof(TextBox), "TagName_TextBox");
            Properties.Settings.Default.Save();
            this.DialogResult = true;
            this.Close();
        }

        private string GetControlValue(Type type, string controlName)
        {
            string values = string.Empty;
            char separator = Properties.Settings.Default.Separator;
            if (type == typeof(TextBox))
            {
                for (int i = 1; i <= 4; i++)
                {
                    TextBox control = (TextBox)this.FindName(string.Format("{0}{1}", controlName, i));
                    values += control.Text;
                    if(i != 4)
                        values += separator;
                }
            }

            return values;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeviceName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox control = (TextBox)sender;

            SelectDevicePopupWindow popup = new SelectDevicePopupWindow();
            popup.ShowDialog();
            if (popup.DialogResult.HasValue && popup.DialogResult.Value)
            {
                control.Text = popup.DeviceName;
            }
        }
    }
}
