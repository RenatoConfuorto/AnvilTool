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
        }
    }
}
