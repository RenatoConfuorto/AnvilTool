using System.Windows;

namespace AnvilTool
{
    /// <summary>
    /// Logica di interazione per MainWindowNew.xaml
    /// </summary>
    public partial class MainWindowNew : Window
    {
        public MainWindowNew()
        {
            InitializeComponent();
            DataContext = new MainVM();
        }
    }
}
