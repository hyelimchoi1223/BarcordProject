using CIMON_Helper;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace BarcodeProject_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static CimonXClass cmx = new CimonXClass();

        public MainWindow()
        {
            InitializeComponent();
            //CimonProcessCheck();
        }

        #region Cimon 실행여부 체크
        private void CimonProcessCheck()
        {
            Process[] viewProcess = Process.GetProcessesByName("CimonX");

            if (viewProcess != null && viewProcess.Length == 1)
            {
                DispatcherTimer timer = new DispatcherTimer();    //객체생성

                timer.Interval = TimeSpan.FromMilliseconds(5000);    //시간간격 설정
                timer.Tick += new EventHandler(CimonXConnectionCheck);          //이벤트 추가
                timer.Start();
            }
            else
            {
                MessageBox.Show("CimonX가 실행되어 있어야 합니다.");
                Environment.Exit(0);
            }
        }
        private void CimonXConnectionCheck(object sender, EventArgs e)
        {
            bool isConnected = cmx.CimonXConnection();
            if (!isConnected)
            {
                MessageBox.Show("CimonX가 실행되어 있어야 합니다.");
                Environment.Exit(0);
            }
        }
        #endregion

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopupView popup = new SettingsPopupView();
            popup.ShowDialog();
            if(popup.DialogResult == true)
            {

            }
        }
    }
}
