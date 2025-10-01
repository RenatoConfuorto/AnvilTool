using LIB.ViewModels;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LIB.Views
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public bool result = false;
        public MessageBox(MessageType type, string Message)
        {
            MessageBoxViewModel viewModel = new MessageBoxViewModel(type, Message);
            viewModel.CloseBox += (object sender, EventArgs e) =>
            {
                result = ((MessageBoxViewModel)DataContext).MessageResult;
                this.Close();
            };
            this.DataContext = viewModel;
            InitializeComponent();
        }
    }
}
