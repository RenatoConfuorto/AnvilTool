using LIB.Constants;
using LIB.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using WPF_Core;
using WPF_Core.Attributes;
using WPF_Core.Dependency;
using WPF_Core.Helpers;
using WPF_Core.Interfaces.ViewModels;

namespace AnvilTool
{
    public class Programm
    {
        [STAThread]
        public static void Main(string[] args)
        {
            WpfApp app = new WpfApp();
            try
            {
                if (File.Exists("./Images/SplashScreen.png"))
                    app.SplashScreen = new SplashScreen(Assembly.GetEntryAssembly(), "./Images/SplashScreen.png");

                app.CommonStyles = "/AnvilTool;component/ResourceDictionary/CommonStyles.xaml";

                //app.MustBeUniqueProcess = false;

                app.ResizeMode = ResizeMode.NoResize;
                app.WindowStyle = WindowStyle.None;

                app.LoggersConfig = Properties.Settings.Default.Loggers;

                app.MainViewName = ViewNames.MainWindow;

                app.Run();
            }catch (Exception ex)
            {
                MessageDialogHelper.ShowInfoMessage(ex.Message);
            }
        }
    }
}
