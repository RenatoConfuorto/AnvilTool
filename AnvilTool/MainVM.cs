using AnvilTool;
using AnvilTool.Commands;
using AnvilTool.Compute;
using AnvilTool.Constants;
using AnvilTool.Entities;
using AnvilTool.Entities.StoredData;
using AnvilTool.Interfaces;
using AnvilTool.NotifyPropertyChanged;
using AnvilTool.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

public class MainVM : NotifyPropertyChangedBase
{
    private ObservableCollection<Move> _finalSequence;
    public ObservableCollection<Move> FinalSequence 
    {
        get => _finalSequence; 
        set => SetProperty(ref _finalSequence, value);
    } 
    public ObservableCollection<Move> ComputedSequence { get; private set; } = new ObservableCollection<Move>();

    IComputeShortest computeShortest;

    #region Current Pos
    private int _currentPos = 0;
    public int CurrentPos
    {
        get => _currentPos;
        set
        {
            // Clamp value between min and max
            SetProperty(ref _currentPos, Math.Max(Consts.MIN_POS, Math.Min(Consts.MAX_POS, value)));
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
    private int _minPossible = Consts.MIN_POS;
    public int MinPossible
    {
        get => _minPossible;
        set => SetProperty(ref _minPossible, value);
    }

    private int _maxPossible = Consts.MAX_POS;
    public int MaxPossible
    {
        get => _maxPossible;
        set => SetProperty(ref _maxPossible, value);
    }

    #endregion

    // markers: list of (position, lastMoveSign)
    private Dictionary<int, int> markers;
    private int lastMoveDelta = 0;

    public bool IsRecordingFinal { get; private set; } = false;
    public bool IsTargetKnown => MinPossible == MaxPossible;
    private bool explorationActive = false;

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

    public MainVM()
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

    private void SetInitialConditions()
    {
        IsRecordingFinal = true;
        explorationActive = false;
        MinPossible = Consts.MIN_POS;
        MaxPossible = Consts.MAX_POS;
        CurrentPos = MinPossible;
        ActualTarget = MinPossible;
        lastMoveDelta = 0;

        markers = new Dictionary<int, int>() { { -1, Consts.MIN_POS }, { 1, Consts.MAX_POS } };
        FinalSequence = new ObservableCollection<Move>();
        ComputedSequence = new ObservableCollection<Move>();
        RaisePropertyChanged(nameof(ComputedSequence));

        UpdateApplicationData();
        RaiseCommandExecutionChanged();
    }

    private void UpdateApplicationData()
    {
        RaisePropertyChanged(nameof(CurrentPos));
    }
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
            if (FinalSequence.Count >= Consts.MAX_FINAL_SEQ_NUM)
                FinalSequence.RemoveAt(0);

            FinalSequence.Add(_m);
            RaisePropertyChanged(nameof(FinalSequence));
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
        CurrentPos = Consts.MIN_POS;
        MinPossible = Consts.MIN_POS; 
        MaxPossible = Consts.MAX_POS;
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

        if(ActualTarget < MinPossible) ActualTarget = MinPossible;
        if(ActualTarget > MaxPossible) ActualTarget = MaxPossible;

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

        if(seq == null)
        {
            MessageBox.Show("Impossibile calcolare la sequenza", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        ComputedSequence = new ObservableCollection<Move>(seq);
        RaisePropertyChanged(nameof(ComputedSequence));

        // Apply the sequence to update current position
        CurrentPos += ComputedSequence.Sum(s => s.Delta);
        RaiseCommandExecutionChanged();
    }
    private bool CanComputeShortest(object param) => explorationActive;
    #endregion

    #region Reset
    private void Reset(object param)
    {
        if(MessageBox.Show("Resettare tutti i dati?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            SetInitialConditions();
        }
    }
    private bool CanReset(object param) => true;
    #endregion

    #region Save
    private void Save(object param)
    {
        if(MessageBox.Show("Salvare la configurazione attuale?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            //SetInitialConditions();
        }
    }
    private bool CanSave(object param) => explorationActive;
    #endregion

    #region Save
    private void SaveRecipe(object param)
    {
        RecipesPopup p = new RecipesPopup(Consts.RecipesMode.SaveRecipe);
        Product prod = new Product()
        {
            Target = ActualTarget,
            FinalSeq = FinalSequence
        };
        p.OpenSave(prod);
    }
    private bool CanSaveRecipe(object param) => true;
    #endregion

    #region Load Recipe
    private void LoadRecipe(object param)
    {
        RecipesPopup p = new RecipesPopup(Consts.RecipesMode.SelectRecipe);
        object r = p.Open();
        if(r is Product recipe)
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
            RaisePropertyChanged(nameof(ComputedSequence));

            // Apply the sequence to update current position
            CurrentPos += ComputedSequence.Sum(s => s.Delta);
            RaiseCommandExecutionChanged();
        }
    }
    private bool CanLoadRecipe(object param) => true;
    #endregion

    #endregion
}