using AnvilTool.Compute;
using AnvilTool.Views;

using LIB.Constants;
using LIB.Entities;
using LIB.Entities.StoredData;
using LIB.Interfaces;
using LIB.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using WPF_Core.Attributes;
using WPF_Core.Commands;
using WPF_Core.Interfaces.Navigation;
using WPF_Core.ViewModels;
using WPF_Core.Views;

namespace AnvilTool.ViewModels
{
    [ViewRef(typeof(HomeView))]
    public class HomeViewModel : ViewModelBase
    {
        #region Private Fields
        private ObservableCollection<Move> _finalSequence;
        public ObservableCollection<Move> FinalSequence
        {
            get => _finalSequence;
            set => SetProperty(ref _finalSequence, value);
        }
        public ObservableCollection<Move> ComputedSequence { get; private set; } = new ObservableCollection<Move>();

        IComputeShortest computeShortest;
        #endregion

        #region Command
        #region Commands Definitions
        public ICommand PressMoveCmd { get; }
        public ICommand ToggleExploreCmd { get; }
        public ICommand MarkDirectionChangeCmd { get; }
        public ICommand FoundExactCmd { get; }
        public ICommand ComputeShortestCmd { get; }
        public ICommand ResetCmd { get; }
        public ICommand SaveRecipeCmd { get; }
        public ICommand LoadRecipeCmd { get; }
        #endregion
        #endregion

        #region Public Properties
        #region Current Pos
        private int _currentPos = 0;
        public int CurrentPos
        {
            get => _currentPos;
            set
            {
                // Clamp value between min and max
                SetProperty(ref _currentPos, Math.Max(Cnst.MIN_POS, Math.Min(Cnst.MAX_POS, value)));
            }
        }
        #endregion

        #region ActualTarget
        private int _actualTarget;
        public int ActualTarget
        {
            get => _actualTarget;
            set => SetProperty(ref _actualTarget, value);
        }
        #endregion

        #region Min/Max Possible
        private int _minPossible = Cnst.MIN_POS;
        public int MinPossible
        {
            get => _minPossible;
            set => SetProperty(ref _minPossible, value);
        }

        private int _maxPossible = Cnst.MAX_POS;
        public int MaxPossible
        {
            get => _maxPossible;
            set => SetProperty(ref _maxPossible, value);
        }

        #endregion
        #endregion

        private Dictionary<int, int> markers;
        private int lastMoveDelta = 0;

        public bool IsRecordingFinal { get; private set; } = false;
        public bool IsTargetKnown => MinPossible == MaxPossible;
        private bool explorationActive = false;

        #region Constructor
        public HomeViewModel() 
            : base(ViewNames.Home) 
        {
            //computeShortest = new BfsComputeShortes();
            computeShortest = new DirectStepCalcComputeShortest();
            FinalSequence = new ObservableCollection<Move>(); // stored in input order, left is last

            PressMoveCmd = new RelayCommand(PressMove, CanPressMove);
            ToggleExploreCmd = new RelayCommand(ToggleExplore, CanToggleExplore);
            MarkDirectionChangeCmd = new RelayCommand(MarkDirectionChange, CanMarkDirectionChanged);
            FoundExactCmd = new RelayCommand(FoundExact, CanFoundExact);
            ComputeShortestCmd = new RelayCommand(ComputeShortest, CanComputeShortest);
            ResetCmd = new RelayCommand(Reset, CanReset);
            SaveRecipeCmd = new RelayCommand(SaveRecipe, CanSaveRecipe);
            LoadRecipeCmd = new RelayCommand(LoadRecipe, CanLoadRecipe);

            SetInitialConditions();
        }
        #endregion

        #region Override Methods
        protected override void InitCommands()
        {
            base.InitCommands();
        }
        #endregion

        #region Private Methods
        private void SetInitialConditions()
        {
            IsRecordingFinal = true;
            explorationActive = false;
            MinPossible = Cnst.MIN_POS;
            MaxPossible = Cnst.MAX_POS;
            CurrentPos = MinPossible;
            ActualTarget = MinPossible;
            lastMoveDelta = 0;

            markers = new Dictionary<int, int>() { { -1, Cnst.MIN_POS }, { 1, Cnst.MAX_POS } };
            FinalSequence = new ObservableCollection<Move>();
            ComputedSequence = new ObservableCollection<Move>();
            NotifyPropertyChanged(nameof(ComputedSequence));

            UpdateApplicationData();
            RaiseCommandExecutionChanged();
        }

        private void UpdateApplicationData()
        {
            NotifyPropertyChanged(nameof(CurrentPos));
        }
        #endregion

        #region Commands methods

        private void RaiseCommandExecutionChanged()
        {
            RelayCommand.RaiseCanExecuteAll(this);
        }

        #region Press Move
        public void PressMove(object param)
        {
            Move _m = (Move)param;
            // if recording final sequence: record (we record deltas; left=last)
            if (IsRecordingFinal)
            {
                if (FinalSequence.Count >= Cnst.MAX_FINAL_SEQ_NUM)
                    FinalSequence.RemoveAt(0);

                FinalSequence.Add(_m);
                NotifyPropertyChanged(nameof(FinalSequence));
                return;
            }
            else
            {
                // apply move normally
                lastMoveDelta += _m.Delta;
                CurrentPos += _m.Delta;
            }
            UpdateApplicationData();
            RaiseCommandExecutionChanged();
        }

        private bool CanPressMove(object param) => true;
        #endregion

        #region ToggleExplore
        private void ToggleExplore(object param)
        {
            explorationActive = true;
            CurrentPos = Cnst.MIN_POS;
            MinPossible = Cnst.MIN_POS;
            MaxPossible = Cnst.MAX_POS;
            IsRecordingFinal = false;
            UpdateApplicationData();
            RaiseCommandExecutionChanged();
        }

        private bool CanToggleExplore(object param) => IsRecordingFinal;
        #endregion

        #region MarkDirectionchanged
        public void MarkDirectionChange(object param)
        {
            if (lastMoveDelta == 0)
            {
                MessageBox.Show("Nessun movimento recente: premi un bottone prima di marcare cambio.");
                return;
            }
            int sign = Math.Sign(lastMoveDelta);

            // Save Marker
            if (sign == 1)
                markers[sign] = Math.Min(markers[sign], CurrentPos);
            else
                markers[sign] = Math.Max(markers[sign], CurrentPos);

            MinPossible = markers[-1];
            MaxPossible = markers[1];

            if (ActualTarget < MinPossible) ActualTarget = MinPossible;
            if (ActualTarget > MaxPossible) ActualTarget = MaxPossible;

            UpdateApplicationData();
            lastMoveDelta = 0;
            RaiseCommandExecutionChanged();
        }

        private bool CanMarkDirectionChanged(object param) => explorationActive;
        #endregion

        #region FoundExact
        public void FoundExact(object param)
        {
            // user signals the exact value is CurrentPos
            MinPossible = CurrentPos;
            MaxPossible = CurrentPos;
            UpdateApplicationData();
            RaiseCommandExecutionChanged();
        }
        private bool CanFoundExact(object param) => explorationActive;
        #endregion

        #region ComputeShortest
        private void ComputeShortest(object param)
        {

            var seq = computeShortest.Compute(CurrentPos, ActualTarget, FinalSequence.ToList());

            if (seq == null)
            {
                MessageBox.Show("Impossibile calcolare la sequenza", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ComputedSequence = new ObservableCollection<Move>(seq);
            NotifyPropertyChanged(nameof(ComputedSequence));

            // Apply the sequence to update current position
            CurrentPos += ComputedSequence.Sum(s => s.Delta);
            RaiseCommandExecutionChanged();
        }
        private bool CanComputeShortest(object param) => explorationActive;
        #endregion

        #region Reset
        private void Reset(object param)
        {
            if (MessageBox.Show("Resettare tutti i dati?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SetInitialConditions();
            }
        }
        private bool CanReset(object param) => true;
        #endregion

        #region Save
        private void Save(object param)
        {
            if (MessageBox.Show("Salvare la configurazione attuale?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //SetInitialConditions();
            }
        }
        private bool CanSave(object param) => explorationActive;
        #endregion

        #region Save
        private void SaveRecipe(object param)
        {
            //RecipesPopup p = new RecipesPopup(Cnst.RecipesMode.SaveRecipe);
            Product prod = new Product()
            {
                Target = ActualTarget,
                FinalSeq = FinalSequence
            };
            //p.OpenSave(prod);


            Dictionary<string, object> popParam = new Dictionary<string, object>()
            {
                {"Operation", Cnst.RecipesMode.SaveRecipe },
                {"Product", prod },
            };
            PopUp p = new PopUp(ViewNames.RecipePopup, popParam);
            if (!p.IsInitialized)
                return;

            object popResult = p.Show();
            // TO CHECK;
        }
        private bool CanSaveRecipe(object param) => true;
        #endregion

        #region Load Recipe
        private void LoadRecipe(object param)
        {
            //RecipesPopup p = new RecipesPopup(Cnst.RecipesMode.SelectRecipe);
            //object r = p.Open();

            Dictionary<string, object> popParam = new Dictionary<string, object>()
            {
                {"Operation", Cnst.RecipesMode.SelectRecipe},
            };

            PopUp p = new PopUp(ViewNames.RecipePopup, popParam);
            if(!p.IsInitialized)
                return;

            object popResult = p.Show();

            Product r = null;
            if(popResult != null && popResult is Dictionary<string, object> par)
            {
                if (par.TryGetValue("Product", out object temp) && temp is Product prod)
                {
                    r = prod;
                }
                else return;
            }

            if (r is Product recipe)
            {
                // Apply recipe
                // Reset the application
                SetInitialConditions();
                ActualTarget = recipe.Target;
                FinalSequence = recipe.FinalSeq;
                // Set Statuses
                IsRecordingFinal = false;
                explorationActive = true;
                RaiseCommandExecutionChanged(); // Reset commands

                // Calculate sequence
                var seq = computeShortest.Compute(CurrentPos, ActualTarget, FinalSequence.ToList());
                if (seq == null)
                {
                    MessageBox.Show("Impossibile calcolare la sequenza", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ComputedSequence = new ObservableCollection<Move>(seq);
                NotifyPropertyChanged(nameof(ComputedSequence));

                // Apply the sequence to update current position
                CurrentPos += ComputedSequence.Sum(s => s.Delta);
                RaiseCommandExecutionChanged();
            }
        }
        private bool CanLoadRecipe(object param) => true;
        #endregion

        #endregion
    }
}
