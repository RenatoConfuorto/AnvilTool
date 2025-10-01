using WPF_Core.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace LIB.UserControls
{
    /// <summary>
    /// Logica di interazione per IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {
        private SolidColorBrush TextColor;
        private SolidColorBrush AccentColor;

        public IconButton()
        {
            InitializeComponent();
            Btn.MouseEnter += Button_IsMouseDirectlyOverChanged;
            Btn.MouseLeave += Button_IsMouseDirectlyOverChanged;
            TextColor = (SolidColorBrush)FindResource("TextColor");
            AccentColor = (SolidColorBrush)FindResource("AccentColor");
            IconColor = TextColor;
        }

        #region Icon Source
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source),
                typeof(ImageSource),
                typeof(IconButton));

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        #region IconColor
        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register(nameof(IconColor),
                typeof(SolidColorBrush),
                typeof(IconButton));
        public SolidColorBrush IconColor
        {
            get => (SolidColorBrush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        } 
        #endregion
        #endregion

        #region Command
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command),
                typeof(RelayCommand),
                typeof(IconButton));


        public RelayCommand Command
        {
            get => (RelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        #endregion

        private void Button_IsMouseDirectlyOverChanged(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if(btn.IsMouseOver)
            {
                IconColor = AccentColor;
            }
            else
            {
                IconColor = TextColor;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged; 
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
