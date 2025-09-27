using AnvilTool.Helpers;

using System.Data;
using System.Windows;

namespace AnvilTool
{
    public partial class App : Application
    {
        public App()
        {
            SQLiteHelper.CheckDataFile();
#if DEBUG
            MainWindowNew mv = new MainWindowNew();
            mv.Show();
#else
            MainWindow mw = new MainWindow();
            mw.Show();
#endif
        }
    }
}
