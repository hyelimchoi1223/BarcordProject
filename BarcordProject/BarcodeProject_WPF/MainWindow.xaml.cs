using CIMON_Helper;
using RawInput_dll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BarcodeProject_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private RawInput _rawinput;
        const bool CaptureOnlyInForeground = true;
        const string DeviceLabelName = "DeviceName_Label";
        const string BarcodeValueTextBoxName = "BarcodeValue_TextBox";


        public MainWindow()
        {
            InitializeComponent();            
          
            if (!GetSettingInitialize())
                ShowSettingPopup();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // I am new to WPF and I don't know where else to call this function.
            // It has to be called after the window is created or the handle won't
            // exist yet and the function will throw an exception.
            IntPtr hwnd = IntPtr.Zero;
            Window myWin = Application.Current.MainWindow;
            try
            {
                hwnd = new WindowInteropHelper(myWin).Handle;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Initialized Exception: " + ex.Message);
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(hwnd, CaptureOnlyInForeground);
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();

            _rawinput.KeyPressed += OnKeyPressed;

            base.OnSourceInitialized(e);
        }

        public void OnKeyPressed(object sender, RawInputEventArg e)
        {
            int controlIndex = GetLabelIndex(e.KeyPressEvent.DeviceName);
            if (controlIndex == -1) throw new Exception("이 장비는 등록된 장비가 아닙니다.");
            FocusTextBox(controlIndex);            
        }

        private void FocusTextBox(int controlIndex)
        {
            TextBox real = (TextBox)this.FindName(string.Format("{0}{1}", BarcodeValueTextBoxName, controlIndex));
            real.Focus();
        }

        private int GetLabelIndex(string deviceName)
        {
            for (int i = 1; i <= 4; i++)
            {
                Label control = (Label)this.FindName(string.Format("{0}{1}", DeviceLabelName, i));
                if (control.Content.Equals(deviceName))
                    return i;
            }
            return -1;
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

        private void ShowSettingPopup()
        {
            SettingPopupWindow settingPopup = new SettingPopupWindow();
            settingPopup.ShowDialog();
            if (settingPopup.DialogResult.HasValue && settingPopup.DialogResult.Value)
            {
                GetSettingInitialize();
            }
        }

        private bool GetSettingInitialize()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DeviceName) || string.IsNullOrWhiteSpace(Properties.Settings.Default.TagName))
                return false;

            char separator = Properties.Settings.Default.Separator;
            List<string> devices = Properties.Settings.Default.DeviceName.Split(separator).ToList();
            SetControlValueBinding(typeof(Label), "DeviceName_Label", devices);
            List<string> tags = Properties.Settings.Default.TagName.Split(separator).ToList();
            SetControlValueBinding(typeof(Label), "TagName_Label", tags);
            return true;
        }

        private void SetControlValueBinding(Type type, string controlName, List<string> values)
        {
            if (type == typeof(Label))
            {
                for (int i = 0; i < values.Count; i++)
                {
                    Label control = (Label)this.FindName(string.Format("{0}{1}", controlName, i + 1));
                    control.Content = values[i];
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingPopup();
        }

        private void BarcodeValue_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                CimonXClass cimon = new CimonXClass();
                cimon.GetTagVal("");
            }
        }
    }
}
