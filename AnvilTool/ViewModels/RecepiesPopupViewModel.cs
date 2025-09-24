using AnvilTool.Constants;
using AnvilTool.Entities;
using AnvilTool.Entities.StoredData;
using AnvilTool.NotifyPropertyChanged;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.ViewModels
{
    public class RecepiesPopupViewModel : NotifyPropertyChangedBase
    {
        #region ReturnValue
        public object ReturnValue { get; private set; }
        #endregion

        #region Servers
        private ObservableCollection<Server> _servers;
        public ObservableCollection<Server> Servers
        {
            get => _servers;
            set => SetProperty(ref _servers, value);
        }
        #endregion

        #region Constructor
        public RecepiesPopupViewModel()
        {
            #region TEST - To remove
            ObservableCollection<Server> servers = new ObservableCollection<Server>();
            for (int i = 0; i < 2; i++)
            {
                Server s = new Server() { Id = i + 1, Name = $"Mondo_{i + 1}" };
                s.Materials = new ObservableCollection<Material>();
                for (int j = 0; j < 3; j++)
                {
                    Material m = new Material() { Id = j + 1, Name = $"Material_{j + 1}", ServerId = s.Id };
                    m.Products = new ObservableCollection<Product>();

                    for (int k = 0; k < 5; k++)
                    {
                        Product p = new Product()
                        {
                            Id = k + 1,
                            Name = $"Product_{k + 1}",
                            MaterialId = m.Id,
                            Target = 50 + k + j + i,
                            FinalSeq = new ObservableCollection<Move>() { Consts.Moves[k], Consts.Moves[k + 1] }
                        };
                        m.Products.Add(p);
                    }

                    s.Materials.Add(m);
                }

                servers.Add(s);
            }

            Servers = servers; 
            #endregion
        }
        #endregion
    }
}
