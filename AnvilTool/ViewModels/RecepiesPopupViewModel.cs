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
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.TextFormatting;

using static AnvilTool.Constants.Consts;

namespace AnvilTool.ViewModels
{
    public class RecipesPopupViewModel : NotifyPropertyChangedBase
    {
        #region ReturnValue
        public object ReturnValue { get; private set; }
        #endregion

        protected RecipesMode Mode { get; private set; }
        protected Action CloseDel { get; private set; }

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

        #region Commands
        public ICommand SelectedItemCommand { get; private set; }
        public ICommand SelectCmd { get; private set; }
        public ICommand SaveCmd { get; private set; }
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
            SelectCmd = new RelayCommand(Select, CanSelect);
            SaveCmd = new RelayCommand(Save, CanSave);
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

        #region Save
        private void Save(object param)
        {

        }
        private bool CanSave(object param) => SelectedMaterial != null;
        #endregion

        #endregion

        private bool AskConfirmation(string message)
        {
            return MessageBox.Show(message, "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
