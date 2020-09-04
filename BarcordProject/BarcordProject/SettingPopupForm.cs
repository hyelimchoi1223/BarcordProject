using BarcordProject.Model;
using BarcordProject.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarcordProject
{
    public partial class SettingPopupForm : Form
    {
        List<BarcodeInfo> info;
        public SettingPopupForm()
        {            
            InitializeComponent();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            Settings.Default.DeviceName = string.Format("{0}|{1}|{2}|{3}", textBox1.Text, textBox3.Text, textBox5.Text, textBox7.Text);
            Settings.Default.Save();
            this.Close();
        }
        internal void SetBarcodeInfo(List<BarcodeInfo> values)
        {
            info = values;
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var popup = new SelectDevicePopupForm();
            popup.SendMsg += new SelectDevicePopupForm.SendDeviceName(SetDeviceName);
            DialogResult result = popup.ShowDialog();
            this.Refresh();

            //SelectDevicePopupForm popup = new SelectDevicePopupForm();            
            //popup.ShowDialog();

            //if (popup.DialogResult == DialogResult.OK)
            //{
                
            //    popup.Close();
            //}
        }
        void SetDeviceName(string deviceName)
        {
            textBox1.Text = deviceName;
        }
    }
}
