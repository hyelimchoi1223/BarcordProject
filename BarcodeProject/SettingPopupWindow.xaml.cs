using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BarcodeProject
{
    /// <summary>
    /// SettingPopupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPopupWindow : Window
    {
        public SettingPopupWindow()
        {
            InitializeComponent();
            GetSettingInitialize();
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
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DeviceName = GetControlValue(typeof(TextBox), "DeviceName_TextBox");
            Properties.Settings.Default.TagName = GetControlValue(typeof(TextBox), "TagName_TextBox");
            Properties.Settings.Default.Save();
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string GetControlValue(Type type, string controlName)
        {
            string values = string.Empty;
            char separator = Properties.Settings.Default.Separator;
            if (type == typeof(TextBox))
            {
                for (int i = 1; i <= MainWindow.TagCount; i++)
                {
                    TextBox control = (TextBox)this.FindName(string.Format("{0}{1}", controlName, i));
                    values += control.Text;
                    if (i != MainWindow.TagCount)
                        values += separator;
                }
            }

            return values;
        }
        private bool GetSettingInitialize()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DeviceName) || string.IsNullOrWhiteSpace(Properties.Settings.Default.TagName))
                return false;

            char separator = Properties.Settings.Default.Separator;
            List<string> devices = Properties.Settings.Default.DeviceName.Split(separator).ToList();
            SetControlValueBinding(typeof(TextBox), "DeviceName_TextBox", devices);
            List<string> tags = Properties.Settings.Default.TagName.Split(separator).ToList();
            SetControlValueBinding(typeof(TextBox), "TagName_TextBox", tags);

            return true;
        }
        private void SetControlValueBinding(Type type, string controlName, List<string> values)
        {
            if (type == typeof(TextBox))
            {
                for (int i = 0; i < values.Count; i++)
                {
                    TextBox control = (TextBox)this.FindName(string.Format("{0}{1}", controlName, i + 1));
                    control.Text = values[i];
                }
            }
        }
    }
}
