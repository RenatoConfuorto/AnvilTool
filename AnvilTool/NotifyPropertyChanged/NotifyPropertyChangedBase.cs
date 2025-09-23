using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.NotifyPropertyChanged
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event triggered by notify the view of the property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Triggers the <event cref="PropertyChanged"/> for the given property name
        /// </summary>
        /// <param name="propertyChanged">Name of the property to be notifyed</param>
        /// <remarks>If the method is called insed the set or inti of a property there is no need to specify the propertyChanged parameter</remarks>
        public void RaisePropertyChanged([CallerMemberName] string propertyChanged = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
        /// <summary>
        /// Sets the new value to the property field and automatically triggers the event <event cref="PropertyChanged"/>
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Property field passad as referece</param>
        /// <param name="value">The new value to assign to the field</param>
        /// <param name="propertyName">The name of the property</param>
        /// <remarks>If the method is called insed the set or inti of a property there is no need to specify the propertyChanged parameter</remarks>
        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            RaisePropertyChanged(propertyName);
        }
    }
}
