using WPF_Core.Commands;
using LIB.Constants;
using WPF_Core.Interfaces.Navigation;
using LIB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Core.Attributes;
using AnvilTool.Views;
using WPF_Core.ViewModels;

namespace AnvilTool.ViewModels
{
    [ViewRef(typeof(HomeView))]
    public class HomeViewModel : ViewModelBase
    {
        #region Private Fields
        #endregion

        #region Command
        #endregion

        #region Public Properties
        #endregion

        #region Constructor
        public HomeViewModel() : base(ViewNames.Home) { }
        #endregion

        #region Override Methods
        protected override void InitCommands()
        {
            base.InitCommands();
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
