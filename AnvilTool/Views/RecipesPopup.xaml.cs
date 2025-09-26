using AnvilTool.Entities.StoredData;
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

using static AnvilTool.Constants.Consts;

namespace AnvilTool.Views
{
    /// <summary>
    /// Logica di interazione per RecipesPopup.xaml
    /// </summary>
    public partial class RecipesPopup : Window
    {
        RecipesPopupViewModel vm;
        public RecipesPopup(RecipesMode mode)
        {
            InitializeComponent();
            DataContext = vm = new RecipesPopupViewModel(this.Close, mode);
        }

        public object Open()
        {
            this.ShowDialog();
            return vm.ReturnValue;
        }

        public void OpenSave(Product product)
        {
            vm.ProductToSave = product;
            this.ShowDialog();
        }
    }
}
