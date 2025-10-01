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
    /// Logica di interazione per ServerTreeCtrl.xaml
    /// </summary>
    public partial class ServerTreeCtrl : UserControl
    {
        public ServerTreeCtrl()
        {
            InitializeComponent();
        }

        #region Server
        public Server Server
        {
            get { return (Server)GetValue(ServerProperty); }
            set { SetValue(ServerProperty, value); }
        }

        public static readonly DependencyProperty ServerProperty =
            DependencyProperty.Register(nameof(Server)
                , typeof(Server)
                , typeof(ServerTreeCtrl)
                , new PropertyMetadata(null, new PropertyChangedCallback(OnServerChanged)));

        private static void OnServerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ServerTreeCtrl c = d as ServerTreeCtrl;
        }
        #endregion
    }
}
