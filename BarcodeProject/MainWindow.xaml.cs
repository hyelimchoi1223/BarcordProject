﻿using CIMON_Helper;
using RawInput_dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace BarcodeProject
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int TagCount = 5;

        const bool CaptureOnlyInForeground = false;
        const string DeviceLabelName = "DeviceName_Label";
        const string TagValueLabelName = "TagName_Label";

        private RawInput _rawinput;

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

            if (!GetSettingInitialize())
                ShowSettingPopup();

            this._checkProcess = new Thread(CheckProcess);
            this._checkProcess.IsBackground = true;
            this._checkProcess.Start();
        }

        private void CheckProcess()
        {
            while (!this._isStopThread)
            {
                Thread.Sleep(200);
                Process[] viewProcess = Process.GetProcessesByName("CimonX"); // 프로세스이름 수정해야함.
                if (viewProcess != null && viewProcess.Length >= 1)
                {
                    CimonXConnection();
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
        private void CimonXConnection()
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

                return;
            }

            CimonXClass.bird = null;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this._isStopThread = true;
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingPopupWindow settingPopup = new SettingPopupWindow();
            settingPopup.ShowDialog();
            if (settingPopup.DialogResult.HasValue && settingPopup.DialogResult.Value)
            {
                GetSettingInitialize();
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }
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
            keyNames.Clear();
            base.OnSourceInitialized(e);
        }
        List<string> keyNames = new List<string>();
        /// <summary>
        /// Custom KeyDown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyPressed(object sender, RawInputEventArg e)
        {
            int controlIndex = GetLabelIndex(e.KeyPressEvent.DeviceName);
            if (controlIndex != -1)
            {
                int result = -1;
                if(e.KeyPressEvent.VKeyName != "ENTER" && int.TryParse(e.KeyPressEvent.VKeyName.Replace("D", ""), out result))
                {
                    Label control = (Label)this.FindName(string.Format("{0}{1}", TagValueLabelName, controlIndex));
                    cmx.SetTagVal(control.Content.ToString(), string.Empty);
                    cmx.SetTagVal(string.Format("{0}_D", control.Content.ToString()), "0");
                    keyNames.Add(result.ToString());
                }
                if (e.KeyPressEvent.VKeyName == "ENTER" && keyNames.Count != 0)
                {
                    for(int i = 0;i < keyNames.Count;i++)
                    {
                        if(i%2==0)
                        {
                            keyNames.RemoveAt(i);
                        }
                    }

                    Label control = (Label)this.FindName(string.Format("{0}{1}", TagValueLabelName, controlIndex));
                    cmx.SetTagVal(control.Content.ToString(), String.Join("", keyNames.ToArray()));
                    cmx.SetTagVal(string.Format("{0}_D", control.Content.ToString()), "1");

                    keyNames.Clear();
                }
            }
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (null == ex) return;

            Debug.WriteLine("Unhandled Exception: " + ex.Message);
            Debug.WriteLine("Unhandled Exception: " + ex);
            MessageBox.Show(ex.Message);
        }
    }
}
