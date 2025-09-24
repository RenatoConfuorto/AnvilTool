using AnvilTool.Entities.Base;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.Entities.StoredData
{
    public class Server : EntityBase
    {
        #region Materials
        private ObservableCollection<Material> _materials;
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set => SetProperty(ref _materials, value);
        } 
        #endregion
    }
}
