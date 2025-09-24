using AnvilTool.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnvilTool.Views
{
    /// <summary>
    /// Logica di interazione per RecepiesPopup.xaml
    /// </summary>
    public partial class RecepiesPopup : Window
    {
        RecepiesPopupViewModel vm;
        public RecepiesPopup()
        {
            InitializeComponent();
            DataContext = vm = new RecepiesPopupViewModel();
        }

        public object Open()
        {
            this.ShowDialog();
            return vm.ReturnValue;
        }
    }
}
