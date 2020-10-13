using RawInput_dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarcordProject
{
    public partial class SelectDevicePopupForm : Form
    {
        public string DeviceName { get; set; }
        private readonly RawInput _rawinput;
        public delegate void SendDeviceName(string deviceName);
        public event SendDeviceName SendMsg;

        const bool CaptureOnlyInForeground = true;

        public SelectDevicePopupForm()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(Handle, CaptureOnlyInForeground);
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("DeviceType"));
            table.Columns.Add(new DataColumn("DeviceName"));
            table.Columns.Add(new DataColumn("etc"));

            Dictionary<IntPtr, KeyPressEvent> devices = RawInput.GetKeyboardDriver().GetDeviceList();
            foreach (var item in devices.Keys)
            {
                if (Win32.GetDeviceType(devices[item].DeviceType) != DeviceType.RimTypekeyboard) continue;

                DataRow row = table.NewRow();
                row["DeviceType"] = devices[item].DeviceType;
                row["DeviceName"] = devices[item].DeviceName;
                row["etc"] = devices[item];
                table.Rows.Add(row);
            }

            dataGridView1.DataSource = table;
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (null == ex) return;

            // Log this error. Logging the exception doesn't correct the problem but at least now
            // you may have more insight as to why the exception is being thrown.
            Debug.WriteLine("Unhandled Exception: " + ex.Message);
            Debug.WriteLine("Unhandled Exception: " + ex);
            MessageBox.Show(ex.Message);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DeviceName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            SendMsg(DeviceName);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }       
    }    
}
