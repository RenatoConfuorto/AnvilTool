using AnvilTool;
using AnvilTool.Commands;
using AnvilTool.Compute;
using AnvilTool.Constants;
using AnvilTool.Entities;
using AnvilTool.Interfaces;
using AnvilTool.NotifyPropertyChanged;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

public class MainVM : NotifyPropertyChangedBase
{
    public ObservableCollection<Move> FinalSequence { get; } = new ObservableCollection<Move>(); // stored in input order, left is last
    public ObservableCollection<Move> ComputedSequence { get; set; } = new ObservableCollection<Move>();

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


    private int _minPossible = Consts.MIN_POS, _maxPossible = Consts.MAX_POS;
    public string IntervalDisplay => $"[{_minPossible}, {_maxPossible}]";
    public string StatusLine => $"Pos: {CurrentPos}";

    // markers: list of (position, lastMoveSign)
    //private List<(int pos, int sign)> markers = new List<(int pos, int sign)>();
    private Dictionary<int, int> markers = new Dictionary<int, int>() { { -1, Consts.MIN_POS} , { 1, Consts.MAX_POS} };
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
    #endregion

    public MainVM()
    {
        //computeShortest = new BfsComputeShortes();
        computeShortest = new DirectStepCalcComputeShortest();

        IsRecordingFinal = true;
        PressMoveCmd = new RelayCommand<Move>(PressMove, CanPressMove);
        //StartRecordFinalCmd = new RelayCommand<object>(_ => StartRecordFinal());
        ToggleExploreCmd = new RelayCommand<object>(ToggleExplore, CanToggleExplore);
        MarkDirectionChangeCmd = new RelayCommand<object>(MarkDirectionChange, CanMarkDirectionChanged);
        FoundExactCmd = new RelayCommand<object>(FoundExact, CanFoundExact);
        ComputeShortestCmd = new RelayCommand<object>(ComputeShortest, CanComputeShortest);
    }

    #region Commands methods

    #region Press Move
    public void PressMove(Move _m)
    {
        // if recording final sequence: record (we record deltas; left=last)
        if (IsRecordingFinal)
        {
            if (FinalSequence.Count >= Consts.MAX_FINAL_SEQ_NUM)
                FinalSequence.RemoveAt(0);

            FinalSequence.Add(_m);
            return;
        }

        // apply move normally
        lastMoveDelta = _m.Delta;
        CurrentPos += _m.Delta;

        // notify
        RaisePropertyChanged(nameof(CurrentPos));
    }

    private bool CanPressMove(object param) => true;
    #endregion

    #region ToggleExplore
    private void ToggleExplore(object param)
    {
        explorationActive = !explorationActive;
        if (explorationActive)
        {
            // reset for exploration session
            CurrentPos = Consts.MIN_POS;
            //markers.Clear();
            _minPossible = Consts.MIN_POS; 
            _maxPossible = Consts.MAX_POS;
            IsRecordingFinal = false;
            //FinalSequence.Clear();
        }
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
    }

    private bool CanToggleExplore(object param) => true;
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
        //markers.Add((CurrentPos, sign));

        // Save Marker
        if (sign == 1)
            markers[sign] = Math.Min(markers[sign], CurrentPos);
        else
            markers[sign] = Math.Max(markers[sign], CurrentPos);

        _minPossible = markers[-1];
        _maxPossible = markers[1];
            //// if there is an earlier marker with opposite sign, tighten interval
            //for (int i = 0; i < markers.Count - 1; i++)
            //{
            //    var a = markers[i];
            //    var b = markers[markers.Count - 1];
            //    if (a.sign != b.sign)
            //    {
            //        int low = Math.Min(a.pos, b.pos);
            //        int high = Math.Max(a.pos, b.pos);
            //        _minPossible = Math.Max(_minPossible, low);
            //        _maxPossible = Math.Min(_maxPossible, high);
            //    }
            //}
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
        lastMoveDelta = 0;
    }

    private bool CanMarkDirectionChanged(object param) => true;
    #endregion

    #region FoundExact
    public void FoundExact(object param)
    {
        // user signals the exact value is CurrentPos
        _minPossible = CurrentPos;
        _maxPossible = CurrentPos;
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
    }
    private bool CanFoundExact(object param) => true;
    #endregion

    #region ComputeShortest
    private void ComputeShortest(object param)
    {
        if(_minPossible != _maxPossible)
        {
            MessageBox.Show("Il target non è stato identificato con precisione. Il minimo verrà utilizzato per il calcolo"
                , "ATTENZIONE"
                , MessageBoxButton.OK
                , MessageBoxImage.Warning);

            _minPossible++;
            markers[-1]++;
            RaisePropertyChanged(nameof(IntervalDisplay));
            RaisePropertyChanged(nameof(IsTargetKnown));
            ComputedSequence = new ObservableCollection<Move>
            (
                computeShortest.Compute(CurrentPos, _minPossible - 1, FinalSequence.ToList())
            );
        }
        else
            ComputedSequence = new ObservableCollection<Move>
                (
                    computeShortest.Compute(CurrentPos, _minPossible, FinalSequence.ToList())
                );
        RaisePropertyChanged(nameof(ComputedSequence));

        // Apply the sequence to update current position
        CurrentPos += ComputedSequence.Sum(s => s.Delta);
    }
    private bool CanComputeShortest(object param) => true;
    #endregion

    #endregion

    // BFS to compute shortest sequence from 0 to target, with final suffix enforced
    public void ComputeShortestOld()
    {
        //if (_minPossible != _maxPossible)
        //{
        //    MessageBox.Show("Target non ancora identificato univocamente.");
        //    return;
        //}
        //int target = _minPossible;
        //// allowed moves: extract unique deltas from Moves
        //var allowed = Consts.Moves.Select(m => m.Delta).Distinct().ToArray();
        //var suffix = FinalSequence.Count == 3 ? FinalSequence.ToArray() : throw new InvalidOperationException("Final sequence non impostata di 3 elementi");

        //// BFS state: pos, lastK (queue or string). We'll store last up to 3 presses
        //var start = (pos: 0, last: new List<int>()); // last is from oldest->newest
        //var visited = new HashSet<string>();
        //var q = new Queue<(int pos, List<int> last, List<int> history)>(); // history holds full pressed deltas sequence
        //q.Enqueue((0, new List<int>(), new List<int>()));
        //visited.Add(EncodeState(0, new List<int>()));

        //List<int> solution = null;
        //while (q.Count > 0)
        //{
        //    var cur = q.Dequeue();
        //    if (cur.pos == target && EndsWithSuffix(cur.last, suffix))
        //    {
        //        solution = cur.history.ToList();
        //        break;
        //    }

        //    foreach (var d in allowed)
        //    {
        //        int nxt = cur.pos + d;
        //        if (nxt < 0 || nxt > 150) continue;
        //        var nxtLast = new List<int>(cur.last);
        //        nxtLast.Add(d);
        //        if (nxtLast.Count > 3) nxtLast.RemoveAt(0);
        //        string code = EncodeState(nxt, nxtLast);
        //        if (visited.Contains(code)) continue;
        //        visited.Add(code);
        //        var nxtHist = new List<int>(cur.history) { d };
        //        q.Enqueue((nxt, nxtLast, nxtHist));
        //    }
        //}

        //ComputedSequence.Clear();
        //if (solution == null)
        //{
        //    ComputedSequence.Add("NESSUNA SEQUENZA TROVATA");
        //}
        //else
        //{
        //    foreach (var s in solution) ComputedSequence.Add((s >= 0 ? "+" : "") + s.ToString());
        //}

        //ComputeShortes.ComputeShortest(CurrentPos, _minPossible, lastMoveDelta);
    }

    //private bool EndsWithSuffix(List<int> last, int[] suffix)
    //{
    //    if (last.Count < 3) return false;
    //    for (int i = 0; i < 3; i++) if (last[last.Count - 3 + i] != suffix[i]) return false;
    //    return true;
    //}

    //private string EncodeState(int pos, List<int> last)
    //{
    //    return pos + "|" + string.Join(",", last);
    //}
}