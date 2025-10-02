using LIB.Commands;
using LIB.Entities;

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

using WPF_Core.Commands;

namespace LIB.UserControls
{
    /// <summary>
    /// Logica di interazione per MoveButtonNew.xaml
    /// </summary>
    public partial class MoveButtonNew : UserControl
    {
        public MoveButtonNew()
        {
            InitializeComponent();
        }

        #region MoveProperty
        public Move Move
        {
            get { return (Move)GetValue(MoveProperty); }
            set { SetValue(MoveProperty, value); }
        }

        public static readonly DependencyProperty MoveProperty =
            DependencyProperty.Register(nameof(Move)
                , typeof(Move)
                , typeof(MoveButtonNew)
                , new PropertyMetadata(null));
        #endregion

        #region MoveClickCommand
        public RelayCommand MoveClickCommand
        {
            get { return (RelayCommand)GetValue(MoveClickCommandProperty); }
            set { SetValue(MoveClickCommandProperty, value); }
        }

        public static readonly DependencyProperty MoveClickCommandProperty =
            DependencyProperty.Register(nameof(MoveClickCommand)
                , typeof(RelayCommand)
                , typeof(MoveButtonNew));

        #endregion
    }
}
