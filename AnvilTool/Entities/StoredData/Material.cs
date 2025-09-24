using AnvilTool.Entities.Base;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.Entities.StoredData
{
    public class Material : EntityBase
    {
        public int ServerId { get; set; }
        #region Products
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }
        #endregion
    }
}
