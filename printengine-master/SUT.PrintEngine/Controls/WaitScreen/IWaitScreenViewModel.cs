// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Controls.WaitScreen
{
    public interface IWaitScreenViewModel:IViewModel
    {
        bool Hide();
        bool Show();
        bool Show(string message);
        bool Show(string message, bool disableParent);
        string Message { get; set; }
        void HideNow();
    }
}
