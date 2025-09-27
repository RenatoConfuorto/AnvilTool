using AnvilTool.Commands;
using AnvilTool.Entities;

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
    /// Logica di interazione per MoveButton.xaml
    /// </summary>
    public partial class MoveButton : UserControl
    {
        public MoveButton()
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
                , typeof(MoveButton)
                , new PropertyMetadata(null));
        #endregion

        #region MoveClickCommand
        public RelayCommand MoveClickCommand
        {
            get { return (RelayCommand )GetValue(MoveClickCommandProperty); }
            set { SetValue(MoveClickCommandProperty, value); }
        }

        public static readonly DependencyProperty MoveClickCommandProperty =
            DependencyProperty.Register(nameof(MoveClickCommand)
                , typeof(RelayCommand)
                , typeof(MoveButton));

        #endregion
    }
}
