using BarcordProject.Model;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            titleLabel.Text = "타이틀 텍스트입니다.";
        }

        public void SetTestText(string value)
        {
            barcordRead_textBox1.Text = value;
        }

        private void OnSettingButtonClick(object sender, EventArgs e)
        {
            SettingPopupForm popup = new SettingPopupForm();
            List<BarcodeInfo> list = new List<BarcodeInfo>();
            popup.SetBarcodeInfo(list);
            popup.ShowDialog();
            if(popup.DialogResult == DialogResult.OK)
            {
                popup.GetBarcodeInfo();
            }
        }
    }
}
