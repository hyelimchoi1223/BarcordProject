using System;
namespace CIMON_Helper
{
    public interface IFunction
    {
        string CurrentProjectName { get; }
        bool Visible { get; set; }
        string CurrentUserName { get; }
        string CurrentUserID { get; }
        string SystemPath { get; }
        string CurrentProjectPath { get; }

        object GetTagVal(string TagName);
        bool SetTagVal(string TagName, object Value);
        bool OpenPage(string PageName);
        bool ClosePage(string PageName);
        bool OpenProject(string PrjName);
        bool CloseProject();
        bool RunScript(string ScriptName);
        void StopScript(string ScriptName);
        void ViewDB();
        void ViewLoginDlg();
        void ViewAlarm();
        void ViewSysStatus();
        void Quit();      // 지원하지 않음
        void SetTitle(string TitleName);
        bool UserLogIn(string UserName, string Password);
        void UserLogOut();
        void ViewChangePasswordDlg();
        string GetTagDevName(string TagName);
        string GetTagAddress(string TagName);
        void ViewMenu(bool fView);
        void ViewToolbar(bool fView);
        void ViewStatusbar(bool fView);
        void ViewNetwork();
        string[] GetTagList(string Group);
        object InitOpcSvc();
        object GetOpcData();
        void QuitOpcSvc();
    }
}
