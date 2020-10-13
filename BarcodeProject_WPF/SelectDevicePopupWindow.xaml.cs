using RawInput_dll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// SelectDevicePopupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectDevicePopupWindow : Window
    {
        private string _deviceName;
        private readonly RawInput _rawinput;
        const bool CaptureOnlyInForeground = true;

        public SelectDevicePopupWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(Process.GetCurrentProcess().MainWindowHandle, CaptureOnlyInForeground);
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();

            DataTable table = new DataTable();
            table.Columns.Add("DeviceType", typeof(string));
            table.Columns.Add("DeviceName", typeof(string));
            table.Columns.Add("etc", typeof(string));

            Dictionary<IntPtr, KeyPressEvent> devices = RawInput.GetKeyboardDriver().GetDeviceList();
            foreach (var item in devices.Keys)
            {
                if (Win32.GetDeviceType(devices[item].DeviceType) != DeviceType.RimTypekeyboard) continue;

                table.Rows.Add(new string[] { devices[item].DeviceType, devices[item].DeviceName, devices[item].ToString() });
            }

            DeviceGrid.ItemsSource = table.DefaultView;            
        }

        private void DeviceGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    //This is the code which helps to show the data when the row is double clicked.
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    DataRowView dr = (DataRowView)dgr.Item;

                    _deviceName = dr[1].ToString();
                    this.DialogResult = true;
                    
                }
            }
            else
            {
                this.DialogResult = false;
            }
            this.Close();
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

        public string DeviceName
        {
            get { return _deviceName; }
        }
    }
}
