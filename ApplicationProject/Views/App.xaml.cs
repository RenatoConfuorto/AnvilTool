using ApplicationProject.ViewModels;
using ApplicationProject.Views;
using LIB.Constants;
using WPF_Core.Dependency;
using WPF_Core.Interfaces.ViewModels;
using WPF_Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using WPF_Core.Helpers;

namespace ApplicationProject
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SplashScreen _splashScreen;
        private IUnityContainer _container;
        public IUnityContainer Container
        {
            get
            {
                if(_container == null )_container = UnityHelper.Current.GetLocalContainer();
                return _container;
            }
        }
        public App()
        {
            
        }
        public SplashScreen SplashScreen
        {
            set => _splashScreen = value;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            if (_splashScreen != null) _splashScreen.Show(autoClose: true);
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });
            DependencyInjectionBase.InitDependencies();
            NavigateToMainView();

            base.OnStartup(e);
        }

        public void NavigateToMainView()
        {
            IUnityContainer container = UnityHelper.Current.GetLocalContainer();
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = container.Resolve<IViewModelBase>(ViewNames.MainWindow);
            mainWindow.Show();

        }
    }
}
