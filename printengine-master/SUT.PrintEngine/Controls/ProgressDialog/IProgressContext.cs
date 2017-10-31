// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace SUT.PrintEngine.Controls.ProgressDialog
{
    public interface IProgressContext
    {
        void SetProgress(double value);
        void SetMaxProgressValue(double value);
    }
}