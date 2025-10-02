using LIB.Entities.Base;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Entities.StoredData
{
    public class Product : EntityBase
    {
        public int MaterialId { get; set; }
        #region Final Sequence
        private ObservableCollection<Move> _finalSeq;
        public ObservableCollection<Move> FinalSeq
        {
            get => _finalSeq;
            set => SetProperty(ref _finalSeq, value);
        }
        #endregion

        #region Target
        private int _target;
        public int Target
        {
            get => _target;
            set => SetProperty(ref _target, value);
        }
        #endregion

        public override string ToString()
        {
            return $"{Name} - {Target}";
        }
    }
}
