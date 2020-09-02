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
            this.Close();
        }
        internal void SetBarcodeInfo(List<BarcodeInfo> values)
        {
            info = values;
        }

        internal List<BarcodeInfo> GetBarcodeInfo()
        {
            return info;
        }
    }
}
