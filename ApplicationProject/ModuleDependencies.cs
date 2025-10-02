using AnvilTool.ViewModels;
using AnvilTool.Views;
using WPF_Core.Dependency;
using LIB.Constants;
using WPF_Core.Interfaces.Navigation;
using WPF_Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Core.Interfaces.ViewModels;

namespace AnvilTool
{
    public class ModuleDependencies : DependencyInjectionBase
    {
        public override void InjectDependencies()
        {
            AddDependency<IViewModelBase, MainWindowViewModel>(ViewNames.MainWindow);
            AddDependency<IViewModelBase, HomeViewModel>(ViewNames.Home);
            AddDependency<IPopupViewModelBase, RecepiesPopupViewModel>(ViewNames.RecipePopup);
        }
    }
}
