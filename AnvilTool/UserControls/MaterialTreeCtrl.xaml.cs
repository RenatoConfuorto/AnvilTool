using AnvilTool.Entities.StoredData;

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

namespace AnvilTool.UserControls
{
    /// <summary>
    /// Logica di interazione per MaterialTreeCtrl.xaml
    /// </summary>
    public partial class MaterialTreeCtrl : UserControl
    {
        public MaterialTreeCtrl()
        {
            InitializeComponent();
        }

        #region Material
        public Material Material
        {
            get { return (Material)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register(nameof(Material)
                , typeof(Material)
                , typeof(MaterialTreeCtrl)
                , new PropertyMetadata(null, new PropertyChangedCallback(OnMaterialChanged)));

        private static void OnMaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaterialTreeCtrl c = d as MaterialTreeCtrl;
        }
        #endregion

    }
}
