using AnvilTool;
using AnvilTool.Commands;
using AnvilTool.Compute;
using AnvilTool.Constants;
using AnvilTool.Entities;
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
    public ObservableCollection<Move> FinalSequence { get; private set; } = new ObservableCollection<Move>(); // stored in input order, left is last
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
            RaisePropertyChanged(nameof(StatusLine));
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

    private int _minPossible = Consts.MIN_POS, _maxPossible = Consts.MAX_POS;
    public string IntervalDisplay => $"[{_minPossible}, {_maxPossible}]";
    public string StatusLine => $"Pos: {CurrentPos}";

    // markers: list of (position, lastMoveSign)
    //private List<(int pos, int sign)> markers = new List<(int pos, int sign)>();
    private Dictionary<int, int> markers;
    private int lastMoveDelta = 0;

    public bool IsRecordingFinal { get; private set; } = false;
    public bool IsTargetKnown => _minPossible == _maxPossible;
    private bool explorationActive = false;

    #region Commands Definitions
    public ICommand PressMoveCmd { get; }
    //public ICommand StartRecordFinalCmd { get; }
    public ICommand ToggleExploreCmd { get; }
    public ICommand MarkDirectionChangeCmd { get; }
    public ICommand FoundExactCmd { get; }
    public ICommand ComputeShortestCmd { get; } 
    public ICommand ResetCmd { get; } 
    public ICommand SaveCmd { get; } 
    public ICommand OpenRecepiesCmd { get; } 
    #endregion

    public MainVM()
    {
        //computeShortest = new BfsComputeShortes();
        computeShortest = new DirectStepCalcComputeShortest();

        PressMoveCmd = new RelayCommand(PressMove, CanPressMove);
        //StartRecordFinalCmd = new RelayCommand(_ => StartRecordFinal());
        ToggleExploreCmd = new RelayCommand(ToggleExplore, CanToggleExplore);
        MarkDirectionChangeCmd = new RelayCommand(MarkDirectionChange, CanMarkDirectionChanged);
        FoundExactCmd = new RelayCommand(FoundExact, CanFoundExact);
        ComputeShortestCmd = new RelayCommand(ComputeShortest, CanComputeShortest);
        ResetCmd = new RelayCommand(Reset, CanReset);
        SaveCmd = new RelayCommand(Save, CanSave);
        OpenRecepiesCmd = new RelayCommand(OpenRecepies, CanOpenRecepies);

        SetInitialConditions();
    }

    private void SetInitialConditions()
    {
        IsRecordingFinal = true;
        _minPossible = Consts.MIN_POS;
        _maxPossible = Consts.MAX_POS;
        CurrentPos = _minPossible;
        ActualTarget = _minPossible;
        lastMoveDelta = 0;

        markers = new Dictionary<int, int>() { { -1, Consts.MIN_POS }, { 1, Consts.MAX_POS } };
        FinalSequence = new ObservableCollection<Move>();
        ComputedSequence = new ObservableCollection<Move>();
        RaisePropertyChanged(nameof(FinalSequence));
        RaisePropertyChanged(nameof(ComputedSequence));


        UpdateApplicationData();
        RaiseCommandExecutionChanged();
    }

    private void UpdateApplicationData()
    {
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
        RaisePropertyChanged(nameof(CurrentPos));
    }
    #region Commands methods

    private void RaiseCommandExecutionChanged()
    {
        this.GetType()
            .GetProperties()
            .Where(p => p.PropertyType == typeof(ICommand))
            .ToList()
            .ForEach(_c => (_c.GetValue(this) as RelayCommand).RaiseCanExecuteChanged());
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
            return;
        }
        else
        {
            // apply move normally
            lastMoveDelta = _m.Delta;
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
        // reset for exploration session
        CurrentPos = Consts.MIN_POS;
        //markers.Clear();
        _minPossible = Consts.MIN_POS; 
        _maxPossible = Consts.MAX_POS;
        IsRecordingFinal = false;
        //FinalSequence.Clear();
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

        _minPossible = markers[-1];
        _maxPossible = markers[1];

        if(ActualTarget < _minPossible) ActualTarget = _minPossible;
        if(ActualTarget > _maxPossible) ActualTarget = _maxPossible;

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
        _minPossible = CurrentPos;
        _maxPossible = CurrentPos;
        UpdateApplicationData();
        RaiseCommandExecutionChanged();
    }
    private bool CanFoundExact(object param) => explorationActive;
    #endregion

    #region ComputeShortest
    private void ComputeShortest(object param)
    {
        //if(_minPossible != _maxPossible)
        //{
        //    MessageBox.Show("Il target non è stato identificato con precisione. Il minimo verrà utilizzato per il calcolo"
        //        , "ATTENZIONE"
        //        , MessageBoxButton.OK
        //        , MessageBoxImage.Warning);

        //    _minPossible++;
        //    markers[-1]++;
        //    RaisePropertyChanged(nameof(IntervalDisplay));
        //    RaisePropertyChanged(nameof(IsTargetKnown));
        //    ComputedSequence = new ObservableCollection<Move>
        //    (
        //        computeShortest.Compute(CurrentPos, _minPossible - 1, FinalSequence.ToList())
        //    );
        //}
        //else
        //    ComputedSequence = new ObservableCollection<Move>
        //        (
        //            computeShortest.Compute(CurrentPos, _minPossible, FinalSequence.ToList())
        //        );

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
    private void OpenRecepies(object param)
    {
        RecepiesPopup p = new RecepiesPopup();
        p.Open();
    }
    private bool CanOpenRecepies(object param) => true;
    #endregion

    #endregion
}