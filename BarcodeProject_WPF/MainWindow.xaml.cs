﻿using CIMON_Helper;
using RawInput_dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

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
        const string TagValueLabelName = "TagName_Label";

        public const int TagCount = 5;

        Thread _checkProcess;
        bool _isStopThread = false;

        Mutex mutex = null;

        public static CimonXClass cmx = new CimonXClass();

        public MainWindow()
        {
            InitializeComponent();

            mutex = new Mutex(true, "BarcordProject", out bool data);

            if (!data)
            {
                MessageBox.Show("BarcordProject is running", "Warning", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
            this._checkProcess = new Thread(CheckProcess);
            this._checkProcess.IsBackground = true;
            this._checkProcess.Start();

            //if (!GetSettingInitialize())
            //    ShowSettingPopup();
        }

        private void CheckProcess()
        {
            while (!this._isStopThread)
            {
                Thread.Sleep(200);
                Process[] viewProcess = Process.GetProcessesByName("GitHubDesktop");
                if (viewProcess != null && viewProcess.Length >= 1)
                {
                    DispatcherTimer timer = new DispatcherTimer();    //객체생성

                    timer.Interval = TimeSpan.FromTicks(5000);    //시간간격 설정
                    timer.Tick += new EventHandler(CimonXConnection);          //이벤트 추가
                    timer.Start();
                }
                else
                {
                    MessageBox.Show("Please start CimonX first.");
                    Thread.Sleep(1000);
                }

                if (this._isStopThread)
                    break;
            }
        }
        private void CimonXConnection(Object state, EventArgs eventArgs)
        {
            // 정상 연결이 된 경우
            if (CimonXClass.bird != null && cmx.isCimonXRun()) return;

            // CimonX가 중간에 종료된 경우
            if (CimonXClass.bird != null && !cmx.isCimonXRun())
            {
                Marshal.ReleaseComObject(CimonXClass.bird);
                CimonXClass.bird = null;
                return;
            }

            // 초기 OLE 연결 설정
            if (CimonXClass.bird == null && cmx.isCimonXRun())
            {
                cmx.progID = "CimonX.Document";

                var type = Type.GetTypeFromProgID(cmx.progID);
                if (type == null)
                {
                    throw new Exception("Invalid ProgID.");
                }

                var obj = Activator.CreateInstance(type);
                IntPtr pIUnk = Marshal.GetIUnknownForObject(obj);
                IntPtr ppv;
                Guid IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");
                Int32 result = Marshal.QueryInterface(pIUnk, ref IID_IDispatch, out ppv);
                if (result < 0)
                { throw new Exception("Invalid QueryInterface."); }
                else
                {
                    CimonXClass.bird = Marshal.GetObjectForIUnknown(ppv);
                }
                var main = App.Current.MainWindow as MainWindow; // If not a static method, this.MainWindow would work
                main.Window_Loaded(null, null);

                return;
            }

            CimonXClass.bird = null;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this._isStopThread = true;
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

        /// <summary>
        /// Custom KeyDown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyPressed(object sender, RawInputEventArg e)
        {
            int controlIndex = GetLabelIndex(e.KeyPressEvent.DeviceName);
            if (controlIndex == -1) throw new Exception("이 장비는 등록된 장비가 아닙니다.");
            FocusTextBox(controlIndex);            
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SystemPath = cmx.SystemPath;
            Properties.Settings.Default.CurrentProjectPath = cmx.CurrentProjectPath;
            Properties.Settings.Default.CurrentProjectName = cmx.CurrentProjectName;
            Properties.Settings.Default.Save();

            if (!GetSettingInitialize())
                ShowSettingPopup();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingPopup();
        }

        private void BarcodeValue_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                string tagIndex = textBox.Name.Replace(BarcodeValueTextBoxName, "");
                Label control = (Label)this.FindName(string.Format("{0}{1}", TagValueLabelName, tagIndex));
                //CimonXClass cimon = new CimonXClass();
                cmx.SetTagVal(control.Content.ToString(), textBox.Text);
            }
        }

        private void FocusTextBox(int controlIndex)
        {
            TextBox real = (TextBox)this.FindName(string.Format("{0}{1}", BarcodeValueTextBoxName, controlIndex));
            real.Focus();
        }

        private int GetLabelIndex(string deviceName)
        {
            for (int i = 1; i <= TagCount; i++)
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
    }
}
