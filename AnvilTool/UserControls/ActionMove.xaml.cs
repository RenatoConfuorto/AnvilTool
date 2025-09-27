using AnvilTool.Entities;
using AnvilTool.ValueConverters;

using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logica di interazione per ActionMove.xaml
    /// </summary>
    public partial class ActionMove : UserControl
    {
        public ActionMove()
        {
            InitializeComponent();
        }

        #region Move
        public Move Move
        {
            get { return (Move)GetValue(MoveProperty); }
            set { SetValue(MoveProperty, value); }
        }

        public static readonly DependencyProperty MoveProperty =
            DependencyProperty.Register(nameof(Move)
                , typeof(Move)
                , typeof(ActionMove)
                , new PropertyMetadata(null, new PropertyChangedCallback(OnMoveChange))); 

        private static void OnMoveChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActionMove c = (ActionMove)d;
            BitmapToBitmapImageConverter conv = new BitmapToBitmapImageConverter();

            if (c.Move != null)
                c.iconImg.Source = (BitmapImage)conv.Convert(c.Move.MoveImage, null, null, CultureInfo.CurrentCulture);
        }
        #endregion
    }
}
