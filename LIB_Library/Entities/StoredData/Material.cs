using LIB.Entities.Base;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Entities.StoredData
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

        public override string ToString()
        {
            return $"{Name}";
        }

        public Product GetProduct(int id) => Products?.FirstOrDefault(p => p.Id == id);
    }
}
