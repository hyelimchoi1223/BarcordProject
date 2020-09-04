﻿using BarcordProject.Model;
using BarcordProject.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
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
            if (popup.DialogResult == DialogResult.OK)
            {
                string[] devices = Settings.Default.DeviceName.Split('|');
                label1.Text = devices[0];
                label2.Text = devices[1];
                label3.Text = devices[2];
                label4.Text = devices[3];
            }            
        }

        public void PageLoad(object sender, EventArgs e)
        {

        }

        private void Barcode_KeyDown1(object sender, KeyEventArgs e)
        {
            TextBox control = (TextBox)sender;
            if (e.KeyCode == Keys.Enter)
            {
                string test = control.Text;
            }
        }

        private void barcordRead_textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
