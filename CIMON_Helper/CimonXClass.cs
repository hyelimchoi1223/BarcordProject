using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CIMON_Helper
{
    public class CimonXClass : IFunction
    {
        public string progID;
        object bird;

        //--------------//
        // 생성자 구현  //
        //--------------//

        public CimonXClass()
        {

        }

        public void CimonXConnection(Object state, EventArgs eventArgs)
        {
            // 정상 연결이 된 경우
            if (bird != null && isCimonXRun()) return;

            // CimonX가 중간에 종료된 경우
            if (bird != null && !isCimonXRun())
            {
                Marshal.ReleaseComObject(bird);
                bird = null;

                //Program.fmd.frmMainDlg_Load(this, null);
                return;
            }

            // 초기 OLE 연결 설정
            if (bird == null && isCimonXRun())
            {
                progID = "CimonX.Document";

                var type = Type.GetTypeFromProgID(progID);
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
                    bird = Marshal.GetObjectForIUnknown(ppv);
                }

                //Program.fmd.frmMainDlg_Load(this, null);

                return;
            }

            bird = null;

        }

        //-----------------------//
        // 인터페이스 함수 구현  //
        //-----------------------//

        public string CurrentProjectName
        {
            get
            {
                if (bird == null) return "";
                else
                {
                    string projectName = (string)bird.GetType().InvokeMember("CurrentProjectName",
                    BindingFlags.GetProperty, null, bird, null);
                    return projectName;
                }

            }
        }

        public bool Visible
        {
            get
            {
                if (bird == null) return false;
                else
                {
                    bool visible = (bool)bird.GetType().InvokeMember("Visible",
                    BindingFlags.GetProperty, null, bird, null);
                    return visible;
                }
            }
            set
            {
                if (bird == null) return;

                bird.GetType().InvokeMember("Visible",
                BindingFlags.SetProperty, null, bird, new Object[] { value });
            }
        }

        public string CurrentUserName
        {
            get
            {
                if (bird == null) return "";
                else
                {
                    string userName = (string)bird.GetType().InvokeMember("CurrentUserName",
                    BindingFlags.GetProperty, null, bird, null);
                    return userName;
                }
            }
        }

        public string CurrentUserID
        {
            get
            {
                if (bird == null) return "";
                else
                {
                    string userID = (string)bird.GetType().InvokeMember("CurrentUserID",
                    BindingFlags.GetProperty, null, bird, null);
                    return userID;
                }
            }
        }

        public string SystemPath
        {
            get
            {
                if (bird == null) return "";
                else
                {
                    string systemPath = (string)bird.GetType().InvokeMember("SystemPath",
                    BindingFlags.GetProperty, null, bird, null);
                    return systemPath;
                }
            }
        }

        public string CurrentProjectPath
        {
            get
            {
                if (bird == null) return "";
                else
                {
                    string projectPath = (string)bird.GetType().InvokeMember("CurrentProjectPath",
                    BindingFlags.GetProperty, null, bird, null);
                    return projectPath;
                }
            }
        }

        public object GetTagVal(string TagName)
        {
            if (bird == null) return null;
            else
            {
                object returnVal = bird.GetType().InvokeMember("GetTagVal",
                BindingFlags.InvokeMethod, null, bird, new Object[] { TagName });

                return returnVal;
            }
        }

        public bool SetTagVal(string TagName, object Value)
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("SetTagVal",
                BindingFlags.InvokeMethod, null, bird, new Object[] { TagName, Value });

                return returnVal;
            }
        }

        public bool OpenPage(string PageName)
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("OpenPage",
                BindingFlags.InvokeMethod, null, bird, new Object[] { PageName });
                return returnVal;
            }
        }

        public bool ClosePage(string PageName)
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("ClosePage",
                BindingFlags.InvokeMethod, null, bird, new Object[] { PageName });
                return returnVal;
            }
        }

        public bool OpenProject(string PrjName)
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("OpenProject",
                BindingFlags.InvokeMethod, null, bird, new Object[] { PrjName });
                return returnVal;
            }
        }

        public bool CloseProject()
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("CloseProject",
                BindingFlags.InvokeMethod, null, bird, null);
                return returnVal;
            }
        }

        public bool RunScript(string ScriptName)
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("RunScript",
                BindingFlags.InvokeMethod, null, bird, new Object[] { ScriptName });
                return returnVal;
            }
        }

        public void StopScript(string ScriptName)
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("StopScript",
                BindingFlags.InvokeMethod, null, bird, new Object[] { ScriptName });
            }
        }

        public void ViewDB()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewDB",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public void ViewLoginDlg()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewLoginDlg",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public void ViewAlarm()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewAlarm",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public void ViewSysStatus()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewSysStatus",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public void Quit()
        {
            //             지원하지 않음
            //             bird.GetType().InvokeMember("Quit",
            //             BindingFlags.InvokeMethod, null, bird, null);
        }

        public void SetTitle(string TitleName)
        {
            //             지원하지 않음
            //             bird.GetType().InvokeMember("SetTitle",
            //             BindingFlags.InvokeMethod, null, bird, new Object[] { TitleName });
        }

        public bool UserLogIn(string UserName, string Password)
        {
            if (bird == null) return false;
            else
            {
                bool returnVal = (bool)bird.GetType().InvokeMember("UserLogIn",
                BindingFlags.InvokeMethod, null, bird, new Object[] { UserName, Password });
                return returnVal;
            }
        }

        public void UserLogOut()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("UserLogOut",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public void ViewChangePasswordDlg()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewChangePasswordDlg",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public string GetTagDevName(string TagName)
        {
            if (bird == null) return "";
            else
            {
                string tagDevName = (string)bird.GetType().InvokeMember("GetTagDevName",
                BindingFlags.GetProperty, null, bird, new Object[] { TagName });
                return tagDevName;
            }
        }

        public string GetTagAddress(string TagName)
        {
            if (bird == null) return "";
            else
            {
                string tagAddress = (string)bird.GetType().InvokeMember("GetTagAddress",
                BindingFlags.GetProperty, null, bird, new Object[] { TagName });
                return tagAddress;
            }
        }

        public void ViewMenu(bool fView)
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewMenu",
                BindingFlags.InvokeMethod, null, bird, new Object[] { fView });
            }
        }

        public void ViewToolbar(bool fView)
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewToolbar",
                BindingFlags.InvokeMethod, null, bird, new Object[] { fView });
            }
        }

        public void ViewStatusbar(bool fView)
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewStatusbar",
                BindingFlags.InvokeMethod, null, bird, new Object[] { fView });
            }
        }

        public void ViewNetwork()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("ViewNetwork",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        public string[] GetTagList(string Group)
        {
            if (bird == null) return null;
            else
            {
                string[] returnVal = (string[])bird.GetType().InvokeMember("GetTagList",
                BindingFlags.InvokeMethod, null, bird, new Object[] { Group });
                return returnVal;
            }
        }

        public object InitOpcSvc()
        {
            if (bird == null) return null;
            else
            {
                object returnVal = bird.GetType().InvokeMember("InitOpcSvc",
                BindingFlags.InvokeMethod, null, bird, null);
                return returnVal;
            }
        }

        public object GetOpcData()
        {
            if (bird == null) return null;
            else
            {
                object returnVal = bird.GetType().InvokeMember("GetOpcData",
                BindingFlags.InvokeMethod, null, bird, null);
                return returnVal;
            }
        }

        public void QuitOpcSvc()
        {
            if (bird == null) return;
            else
            {
                bird.GetType().InvokeMember("QuitOpcSvc",
                BindingFlags.InvokeMethod, null, bird, null);
            }
        }

        //-----------------------------------//
        // CimonX 실행되고 있는지 여부 판단  //
        //-----------------------------------//

        public bool isCimonXRun()
        {
            Process[] viewProcess = Process.GetProcessesByName("CimonX");

            if (viewProcess != null && viewProcess.Length == 1)
            {
                return true;
            }
            else return false;
        }
    }
}
