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
        private readonly RawInput _rawinput;

        const bool CaptureOnlyInForeground = true;

        public SelectDevicePopupForm()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(Handle, CaptureOnlyInForeground);
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Column1"));
            table.Columns.Add(new DataColumn("Column2"));

            Dictionary<IntPtr, KeyPressEvent> test = RawInput.GetKeyboardDriver().GetDeviceList();

            foreach(var item in test.Keys)
            {
                
                DataRow row = table.NewRow();
                row["Column1"] = item;
                row["Column2"] = test[item];
                
                if (GetDeviceType(test[item].DeviceType) == DeviceType.RimTypekeyboard)
                {
                    table.Rows.Add(row);
                }
                
            }
            dataGridView1.DataSource = table;
        }

        private int GetDeviceType(string deviceType)
        {            
            switch (deviceType)
            {
                case "MOUSE": return DeviceType.RimTypemouse;
                case "KEYBOARD": return DeviceType.RimTypekeyboard;
                case "HID": return DeviceType.RimTypeHid;
                default: return DeviceType.RimTypedefault;
            }
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
    }
}
