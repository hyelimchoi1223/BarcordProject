using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CIMON_Helper;

namespace BarcordProject
{
    static class Program
    {
        public static CimonXClass cmx = new CimonXClass();
        public static Form1 fmd;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Process[] viewProcess = Process.GetProcessesByName("CimonX");

            if (viewProcess != null && viewProcess.Length == 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 주기적으로 CimonX 실행 여부 확인
                Timer tt = new Timer();
                tt.Interval = 5000;
                tt.Tick += new EventHandler(CimonXConnectionCheck);
                tt.Start();

                fmd = new Form1();
                Application.Run(fmd);
            }
            else
            {
                MessageBox.Show("CimonX가 실행되어 있어야 합니다.");
                Application.Exit();
            }

        }

        private static void CimonXConnectionCheck(object sender, EventArgs e)
        {
            bool isConnected = cmx.CimonXConnection();
            if (!isConnected)
            {
                fmd.PageLoad(cmx, null);
            }
        }
    }
}
