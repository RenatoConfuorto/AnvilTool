using LIB.Views;
using LIB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Helpers
{
    public class MessageDialogHelper
    {
        public static void ShowInfoMessage(string message)
        {
            MessageBox messagebox = new MessageBox(MessageType.Info, message);
            messagebox.ShowDialog();
        }

        public static bool ShowConfirmationRequestMessage(string message)
        {
            MessageBox messageBox = new MessageBox(MessageType.Confirmation, message);
            messageBox.ShowDialog();
            return messageBox.result;
        }
    }
}
