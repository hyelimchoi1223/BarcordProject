using CIMON_Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace BarcodeProject_WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public static CimonXClass cmx = new CimonXClass();
        public static MainWindow window;
        public static App application;

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Process[] viewProcess = Process.GetProcessesByName("CimonX");

            if (viewProcess != null && viewProcess.Length == 1)
            {
                application = new App();
                application.InitializeComponent();

                DispatcherTimer timer = new DispatcherTimer();    //객체생성

                timer.Interval = TimeSpan.FromTicks(5000);    //시간간격 설정
                timer.Tick += new EventHandler(CimonXConnection);          //이벤트 추가
                timer.Start();

                application.Run();

            }
            else
            {
                MessageBox.Show("CimonX가 실행되어 있어야 합니다.");
                //System.Windows.Application.Current.Shutdown();
            }
        }

        public static void CimonXConnection(Object state, EventArgs eventArgs)
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
                main.Window_Loaded(null,null);

                return;
            }

            CimonXClass.bird = null;
        }
    }
}
