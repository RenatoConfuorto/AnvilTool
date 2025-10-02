using LIB.Entities.StoredData;

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LIB.UserControls.StoredDataControls
{
    /// <summary>
    /// Logica di interazione per ProductTreeCtrl.xaml
    /// </summary>
    public partial class ProductTreeCtrl : UserControl
    {
        public ProductTreeCtrl()
        {
            InitializeComponent();
        }
        
        #region Product
        public Product Product
        {
            get { return (Product)GetValue(ProductProperty); }
            set { SetValue(ProductProperty, value); }
        }

        public static readonly DependencyProperty ProductProperty =
            DependencyProperty.Register(nameof(Product)
                , typeof(Product)
                , typeof(ProductTreeCtrl)
                , new PropertyMetadata(null, new PropertyChangedCallback(OnProductChanged)));

        private static void OnProductChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProductTreeCtrl c = d as ProductTreeCtrl;
        }
        #endregion

    }
}
