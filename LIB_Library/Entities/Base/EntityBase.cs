using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WPF_Core.Entities;

namespace LIB.Entities.Base
{
    public abstract class EntityBase : NotifyerPropertyChangedBase
    {
        #region Id
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        #endregion

        #region Name
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        #endregion
    }
}
