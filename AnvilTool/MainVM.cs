using AnvilTool;
using AnvilTool.Commands;
using AnvilTool.Compute;
using AnvilTool.Constants;
using AnvilTool.Entities;
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
    //public ObservableCollection<MoveButton> Moves { get; } = new ObservableCollection<MoveButton>();
    public ObservableCollection<Move> FinalSequence { get; } = new ObservableCollection<Move>(); // stored in input order, left is last
    public ObservableCollection<string> ComputedSequence { get; } = new ObservableCollection<string>();


    #region Current Pos
    private int _currentPos = 0;
    public int CurrentPos
    {
        get => _currentPos;
        set
        {
            SetProperty(ref _currentPos, Math.Max(Consts.MIN_POS, Math.Min(Consts.MAX_POS, value)));
            RaisePropertyChanged(nameof(StatusLine));
        }
    } 
    #endregion


    private int _minPossible = Consts.MIN_POS, _maxPossible = Consts.MAX_POS;
    public string IntervalDisplay => $"[{_minPossible}, {_maxPossible}]";
    public string StatusLine => $"Pos: {CurrentPos}";

    // markers: list of (position, lastMoveSign)
    private List<(int pos, int sign)> markers = new List<(int pos, int sign)>();
    private int lastMoveDelta = 0;

    public bool IsRecordingFinal { get; private set; } = false;
    public bool IsTargetKnown => _minPossible == _maxPossible;

    #region Commands Definitions
    public ICommand PressMoveCmd { get; }
    public ICommand StartRecordFinalCmd { get; }
    public ICommand ToggleExploreCmd { get; }
    public ICommand MarkDirectionChangeCmd { get; }
    public ICommand FoundExactCmd { get; }
    public ICommand ComputeShortestCmd { get; } 
    #endregion

    public MainVM()
    {
        PressMoveCmd = new RelayCommand<Move>(PressMove);
        StartRecordFinalCmd = new RelayCommand<object>(_ => StartRecordFinal());
        ToggleExploreCmd = new RelayCommand<object>(_ => ToggleExplore());
        MarkDirectionChangeCmd = new RelayCommand<object>(_ => MarkDirectionChange());
        FoundExactCmd = new RelayCommand<object>(_ => FoundExact());
        ComputeShortestCmd = new RelayCommand<object>(_ => ComputeShortest());
    }

    private bool explorationActive = false;
    private void ToggleExplore()
    {
        explorationActive = !explorationActive;
        if (explorationActive)
        {
            // reset for exploration session
            CurrentPos = 0;
            markers.Clear();
            _minPossible = 0; _maxPossible = 150;
            FinalSequence.Clear(); // or keep? In spec final seq recorded in phase 1
        }
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
    }

    private void StartRecordFinal()
    {
        IsRecordingFinal = true;
        FinalSequence.Clear();
        // user must press 3 moves; handle in PressMove: if IsRecordingFinal add to FinalSequence
    }

    public void PressMove(Move _m)
    {
        // if recording final sequence: record (we record deltas; left=last)
        if (IsRecordingFinal)
        {
            FinalSequence.Add(_m);
            if (FinalSequence.Count >= 3)
            {
                // stop recording when we have 3 (FinalSequence[0] is first clicked, which we'll treat as last in game)
                IsRecordingFinal = false;
            }
        }

        // apply move normally
        lastMoveDelta = _m.Delta;
        CurrentPos += _m.Delta;

        // clamp 0..150
        if (CurrentPos < Consts.MIN_POS) CurrentPos = Consts.MIN_POS;
        if (CurrentPos > Consts.MAX_POS) CurrentPos = Consts.MAX_POS;

        // notify
        RaisePropertyChanged(nameof(CurrentPos));
    }

    public void MarkDirectionChange()
    {
        if (lastMoveDelta == 0)
        {
            MessageBox.Show("Nessun movimento recente: premi un bottone prima di marcare cambio.");
            return;
        }
        int sign = Math.Sign(lastMoveDelta);
        markers.Add((CurrentPos, sign));

        // if there is an earlier marker with opposite sign, tighten interval
        for (int i = 0; i < markers.Count - 1; i++)
        {
            var a = markers[i];
            var b = markers[markers.Count - 1];
            if (a.sign != b.sign)
            {
                int low = Math.Min(a.pos, b.pos);
                int high = Math.Max(a.pos, b.pos);
                _minPossible = Math.Max(_minPossible, low);
                _maxPossible = Math.Min(_maxPossible, high);
            }
        }
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
    }

    public void FoundExact()
    {
        // user signals the exact value is CurrentPos
        _minPossible = CurrentPos;
        _maxPossible = CurrentPos;
        RaisePropertyChanged(nameof(IntervalDisplay));
        RaisePropertyChanged(nameof(IsTargetKnown));
    }

    // BFS to compute shortest sequence from 0 to target, with final suffix enforced
    public void ComputeShortest()
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

    private bool EndsWithSuffix(List<int> last, int[] suffix)
    {
        if (last.Count < 3) return false;
        for (int i = 0; i < 3; i++) if (last[last.Count - 3 + i] != suffix[i]) return false;
        return true;
    }

    private string EncodeState(int pos, List<int> last)
    {
        return pos + "|" + string.Join(",", last);
    }
}