using AnvilTool.Commands;
using AnvilTool.Constants;
using AnvilTool.DbEngine;
using AnvilTool.Entities;
using AnvilTool.Entities.StoredData;
using AnvilTool.Helpers;
using AnvilTool.NotifyPropertyChanged;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.TextFormatting;

using static AnvilTool.Constants.Consts;

namespace AnvilTool.ViewModels
{
    public class RecipesPopupViewModel : NotifyPropertyChangedBase
    {
        private SQLiteProxy proxy = SQLiteHelper.Proxy;
        #region ReturnValue
        public object ReturnValue { get; private set; }
        #endregion

        protected RecipesMode Mode { get; private set; }
        protected Action CloseDel { get; private set; }
        public Product ProductToSave { get; set; }

        public bool IsOpenSelect { get => Mode == RecipesMode.SelectRecipe; }
        public bool IsOpenSave { get => Mode == RecipesMode.SaveRecipe; }
        public Visibility SelectButtonVisibility { get => IsOpenSelect ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility SaveButtonVisibility { get => IsOpenSave ? Visibility.Visible : Visibility.Collapsed; }

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

        #endregion

        #region New Server/Material/Product
        private string _newServerName;
        public string NewServerName
        {
            get => _newServerName;
            set
            {
                SetProperty(ref _newServerName, value);
                RaiseCanExecuteChanged();
            }
        }
        
        private string _newMaterialName;
        public string NewMaterialName
        {
            get => _newMaterialName;
            set
            {
                SetProperty(ref _newMaterialName, value);
                RaiseCanExecuteChanged();
            }
        }
        
        private string _newProductName;
        public string NewProductName
        {
            get => _newProductName;
            set
            {
                SetProperty(ref _newProductName, value);
                RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands
        public ICommand SelectedItemCommand { get; private set; }
        public ICommand SelectCmd { get; private set; }
        public ICommand AddProductCmd { get; private set; }
        public ICommand AddServerCmd { get; private set; }
        public ICommand AddMaterialCmd { get; private set; }
        #endregion

        #region Constructor
        public RecipesPopupViewModel(Action closeDel, RecipesMode mode)
        {
            this.Mode = mode;
            this.CloseDel = closeDel;

            //RaisePropertyChanged(nameof(IsOpenSave));
            //RaisePropertyChanged(nameof(IsOpenSelect));
            //RaisePropertyChanged(nameof(SelectButtonVisibility));
            //RaisePropertyChanged(nameof(SaveButtonVisibility));

            //#region TEST - To remove
            //ObservableCollection<Server> servers = new ObservableCollection<Server>();
            //for (int i = 0; i < 2; i++)
            //{
            //    Server s = new Server() { Id = i + 1, Name = $"Mondo_{i + 1}" };
            //    s.Materials = new ObservableCollection<Material>();
            //    for (int j = 0; j < 3; j++)
            //    {
            //        Material m = new Material() { Id = j + 1, Name = $"Material_{j + 1}", ServerId = s.Id };
            //        m.Products = new ObservableCollection<Product>();

            //        for (int k = 0; k < 5; k++)
            //        {
            //            Product p = new Product()
            //            {
            //                Id = k + 1,
            //                Name = $"Product_{k + 1}",
            //                MaterialId = m.Id,
            //                Target = 50 + k + j + i,
            //                FinalSeq = new ObservableCollection<Move>() { Consts.Moves[k], Consts.Moves[k + 1] }
            //            };
            //            m.Products.Add(p);
            //        }

            //        s.Materials.Add(m);
            //    }

            //    servers.Add(s);
            //}

            //Servers = servers;
            //#endregion


            Servers = new ObservableCollection<Server>(proxy.GetData());

            SelectedItemCommand = new RelayCommand(OnSelectedItem);
            SelectCmd = new RelayCommand(Select, CanSelect);
            AddProductCmd = new RelayCommand(AddProduct, CanAddProduct);
            AddServerCmd = new RelayCommand(AddServer, CanAddServer);
            AddMaterialCmd = new RelayCommand(AddMaterial, CanAddMaterial);
        }

        #region Commands
        private void RaiseCanExecuteChanged()
        {
            RelayCommand.RaiseCanExecuteAll(this);
        }
        #endregion

        #region SelectedItemCommand
        private void OnSelectedItem(object obj)
        {
            if (obj is Server s)
                SelectedServer = s;
            else if (obj is Material m)
                SelectedMaterial = m;
            else if (obj is Product p)
                SelectedProduct = p;
            else if (obj == null)
            {
                SelectedServer = null;
                SelectedMaterial = null;
                SelectedProduct = null;
            }
            RaiseCanExecuteChanged();
        }
        #endregion

        #region Select
        private void Select(object param)
        {
            if (AskConfirmation($"Caricare la seguente ricetta? {SelectedProduct}"))
            {
                ReturnValue = SelectedProduct;
                this.CloseDel();
            }
        }
        private bool CanSelect(object param) => SelectedProduct != null;
        #endregion

        #region Add Server
        private void AddServer(object param)
        {
            Server s = new Server()
            {
                Id = Servers.Count == 0 ? 1 : Servers.Max(_s => _s.Id) + 1,
                Name = NewServerName,
                Materials = new ObservableCollection<Material>()
            };

            if (proxy.InsertServer(s))
                Servers.Add(s);
            else ShowInfo("Insert fallito");
                
        }
        private bool CanAddServer(object param) => !String.IsNullOrEmpty(NewServerName);
        #endregion

        #region Add Material
        private void AddMaterial(object param)
        {
            Material m = new Material()
            {
                Id = SelectedServer.Materials.Count == 0 ? 1 : SelectedServer.Materials.Max(_m => _m.Id) + 1,
                Name = NewMaterialName,
                ServerId = SelectedServer.Id,
                Products = new ObservableCollection<Product>()
            };
            if (proxy.InsertMaterial(m))
                SelectedServer.Materials.Add(m);
            else ShowInfo("Insert fallito");
        }
        private bool CanAddMaterial(object pram) => SelectedServer != null && !String.IsNullOrEmpty(NewMaterialName);
        #endregion

        #region AddProduct
        private void AddProduct(object param)
        {
            Product p = new Product()
            {
                Id = SelectedMaterial.Products.Count == 0 ? 1 : SelectedMaterial.Products.Max(_m => _m.Id) + 1,
                Name = NewProductName,
                Target = ProductToSave.Target,
                MaterialId = SelectedMaterial.Id,
                FinalSeq = ProductToSave.FinalSeq,
            };

            if (proxy.InsertProduct(p))
                SelectedMaterial.Products.Add(p);
            else ShowInfo("Insert fallito");
        }
        private bool CanAddProduct(object param) => SelectedMaterial != null && !String.IsNullOrEmpty(NewProductName);
        #endregion

        #endregion

        private bool AskConfirmation(string message)
        {
            return MessageBox.Show(message, "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
