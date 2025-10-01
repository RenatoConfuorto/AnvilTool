using AnvilTool.Views;

using LIB.Constants;
using LIB.Helpers;

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using WPF_Core.Attributes;
using WPF_Core.Commands;
using WPF_Core.EventArgs;
using WPF_Core.Helpers;
using WPF_Core.Interfaces.Navigation;
using WPF_Core.ViewModels;

namespace AnvilTool.ViewModels
{
    [ViewRef(typeof(MainWindow))]
    public class MainWindowViewModel : ViewModelBase
    {
        #region Private Fields
        #endregion

        #region Public Properties
        #endregion

        #region Commands
        public RelayCommand ShoutDownCommand { get; set; }
        public RelayCommand PreviousPageCommand { get; set; }
        public RelayCommand ReloadPageCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel() 
            : base(ViewNames.MainWindow)
        {
            // Main Window needs the NavigateToView bacause its viewChangedEvent is null
            NavigateToView(ViewNames.Home);
        }
        #endregion

        #region Override Methods
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        protected override void InitCommands()
        {
            base.InitCommands();
            ShoutDownCommand = new RelayCommand(ShoutDownCommandExecute);
            PreviousPageCommand = new RelayCommand(PreviousPageCommandExecute, PreviousPageCommandCanExecute);
            ReloadPageCommand = new RelayCommand(ReloadPageCommandExecute);
        }
        protected override void SetCommandExecutionStatus()
        {
            base.SetCommandExecutionStatus();
            PreviousPageCommand.RaiseCanExecuteChanged();
            ReloadPageCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Command Methods
        private void ShoutDownCommandExecute(object param)
        {
            if(MessageDialogHelper.ShowConfirmationRequestMessage("Uscire dall'applicazione?"))
            {
                Application.Current.Shutdown();
            }
        }
        private void PreviousPageCommandExecute(object param) 
        {
            if (String.IsNullOrEmpty(Navigation.ParentViewName))
            {
                ChangeView(ViewNames.Home);
            }
            else
            {
                ChangeView(Navigation.ParentViewName);
            }
        }
        private bool PreviousPageCommandCanExecute(object param) => !String.IsNullOrEmpty(Navigation.ParentViewName);
        private void ReloadPageCommandExecute(object param) 
        {
            Navigation.CurrentView.InitViewModel();
        }
        #endregion

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            NavigateToView(e.viewToCall, e.viewParam);
        }
        private void NavigateToView(string viewName, object param = null)
        {
            if (Navigation.CurrentView != null)
            {
                Navigation.CurrentView.viewChangedEvent -= OnViewChanged;
            }

            Navigation.NavigateTo(viewName, param);
            if (Navigation.CurrentView != null)
            {
                Navigation.CurrentView.viewChangedEvent += OnViewChanged;
            }

            SetCommandExecutionStatus();
        }
    }
}
