using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CIMON_Helper;

namespace BarcordProject
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            int count = 0;
            Task t1 = Task.Run(() => Application.Run(new Form1()));
            count++;

            if (count == 100)
                t1.Wait();

            
            //CimonXClass cimonX = new CimonXClass();
            //if(cimonX.CimonXConnection(null, null))
            //{

            //}
            //Program.Form1.frmMainDlg_Load(this, null);
        }
    }
}
