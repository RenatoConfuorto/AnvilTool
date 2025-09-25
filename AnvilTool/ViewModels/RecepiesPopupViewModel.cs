using AnvilTool.Commands;
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

        #region SelectedItems
        private Server _selectedServer;
        public Server SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }

        private Material _selectedMaterial;
        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set => SetProperty(ref _selectedMaterial, value);
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public object SelectedItem 
        { 
            get; 
            set; 
        }

        public RelayCommand SelectedItemCommand { get; private set; }
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

            SelectedItemCommand = new RelayCommand(OnSelectedItem);
        }

        #region Commands
        private void RaiseCanExecuteChanged()
        {
            RelayCommand.RaiseCanExecuteAll(this);
        }
        #endregion

        private void OnSelectedItem(object obj)
        {
            if (obj is Server s)
                SelectedServer = s;
            else if (obj is Material m)
                SelectedMaterial = m;
            else if (obj is Product p)
                SelectedProduct = p;
            else if(obj == null)
            {
                SelectedServer = null;
                SelectedMaterial = null; 
                SelectedProduct = null;
            }
                RaiseCanExecuteChanged();
        }
        #endregion
    }
}
