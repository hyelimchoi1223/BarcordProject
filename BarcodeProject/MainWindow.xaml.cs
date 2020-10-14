using CIMON_Helper;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BarcodeProject
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int TagCount = 5;

        const bool CaptureOnlyInForeground = true;
        const string DeviceLabelName = "DeviceName_Label";
        const string BarcodeValueTextBoxName = "BarcodeValue_TextBox";
        const string TagValueLabelName = "TagName_Label";

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
        }

        private void CheckProcess()
        {
            while (!this._isStopThread)
            {
                Thread.Sleep(200);
                Process[] viewProcess = Process.GetProcessesByName("GitHubDesktop"); // 프로세스이름 수정해야함.
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
            }
        }
        private void BarcodeValue_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                string tagIndex = textBox.Name.Replace(BarcodeValueTextBoxName, "");
                Label control = (Label)this.FindName(string.Format("{0}{1}", TagValueLabelName, tagIndex));
                cmx.SetTagVal(control.Content.ToString(), textBox.Text);
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
