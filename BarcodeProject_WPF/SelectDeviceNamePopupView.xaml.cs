using RawInput_dll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BarcodeProject_WPF
{
    /// <summary>
    /// SelectDeviceNamePopupView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectDeviceNamePopupView : Window
    {
        private readonly RawInput _rawinput;
        const bool CaptureOnlyInForeground = true;

        public SelectDeviceNamePopupView()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(new WindowInteropHelper(this).Handle, CaptureOnlyInForeground);
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();
            Dictionary<IntPtr, KeyPressEvent> devices = RawInput.GetKeyboardDriver().GetDeviceList();
            var items = new List<string>();
            foreach (var key in devices.Keys)
            {
                items.Add(devices[key].DeviceName);
            }

            DeviceList.ItemsSource = items;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (null == ex) return;

            // Log this error. Logging the exception doesn't correct the problem but at least now
            // you may have more insight as to why the exception is being thrown.
            Debug.WriteLine("Unhandled Exception: " + ex.Message);
            Debug.WriteLine("Unhandled Exception: " + ex);
            MessageBox.Show(ex.Message);
        }

        private void DeviceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
