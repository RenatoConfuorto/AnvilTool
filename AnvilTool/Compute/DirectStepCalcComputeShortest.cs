using AnvilTool.Compute.Base;
using AnvilTool.Constants;
using AnvilTool.Entities;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media.TextFormatting;

namespace AnvilTool.Compute
{
    public class DirectStepCalcComputeShortest : ComputeShortesBase
    {
        public override List<Move> Compute(int startingPos, int targetPos, List<Move> lastMoves)
        {
            if (lastMoves == null || lastMoves.Count == 0)
                throw new ArgumentException("Last moves not setted");
            
            // Movement caused by the last offset
            int lastOffset = lastMoves.Sum(m => m.Delta);
            // Position to reach before doing the last movement
            int posBeforeLast = targetPos - lastOffset;

            int start = startingPos;
            int target = posBeforeLast;

            int iter = 1;

            List<Move> finalSeq = null;
            List<Move> startSeq = new List<Move>();
            while(iter < Consts.MAX_ITER)
            {
                int current, max, min;
                current = max = min = start;

                Console.WriteLine($"Iteration number {iter}. Starting Pos {current} - Target {target}");
                List<Move> currentSeq = new List<Move>();

                foreach(var m in startSeq)
                    currentSeq.Add(m);

                while(currentSeq.Count <  Consts.MAX_SEQ_LENGHT)
                {
                    // Calculate the distance from target
                    int diff = current - target;
                    if(diff == 0)
                    {
                        Console.WriteLine("SOLUTION FOUND--------------------");
                        // TODO check if there can be a better solution
                        //if (currentSeq.Count - startSeq.Count < Consts.EXIT_SEQ_LEN)
                        //    return GetFinalSeq(currentSeq, lastMoves);
                        //else finalSeq = currentSeq;

                        return GetFinalSeq(currentSeq, lastMoves);
                    }
                    var m = TakeMove(diff);
                    currentSeq.Add(m);
                    current += m.Delta;
                    SaveMaxMin(ref max, ref min, current);
                    Console.WriteLine($"Diff {diff} - Selected move {m} - New Pos {current} - Seq Len {currentSeq.Count}");
                }
                iter++;
            }

            // No solution found
            if(finalSeq != null )
                finalSeq = GetFinalSeq(finalSeq, lastMoves);

            return finalSeq;
        }

        private void SaveMaxMin(ref int max, ref int min, int current)
        {
            if(current > max) max = current;
            if(current < min) min = current;
        }

        private List<Move> GetStartingMoves(List<Move> originalStarting, int max, int min, int target)
        {
            Move _m = null;
            List<Move> temp = new List<Move>(originalStarting);

            // Check the deviations from target of max and min
            int maxDev = Math.Abs(max - target);
            int minDev = Math.Abs(min - target);

            // If the maxDev is higher, I try to reduce the starting position
            if (maxDev > minDev)
                _m = TakeSmallestNegative();
            else _m = TakeSmallestPositive();

            temp.Add(_m);
            int sum = temp.Sum(m => m.Delta);
            int ogSum = originalStarting.Sum(m => m.Delta);
            // I Check if I can reduce the number of starting Sequence

            return temp;
        }

        #region Take Move
        private Move TakeMove(int diff)
        {
            Move res = null;
            if (diff < 0)
                res = TakePositive(diff);
            else
                res = TakeNegative(diff);

            if(res == null)
                res = TakeSmallest(diff);

            return res;
        }

        private Move TakePositive(int diff)
        {
            return Consts.Moves
            .Where(m => m.Delta > 0 && m.Delta < (-1) * diff) // Take positives and with offset less than difference
            .OrderByDescending(m => m.Delta)
            .FirstOrDefault();

            //return Consts.Moves.FirstOrDefault(m => m.Delta)
        }

        private Move TakeNegative(int diff)
        {
            return Consts.Moves
                .Where(m => m.Delta < 0 && m.Delta > (-1) * diff) // Take negatives and with offset bigger than difference
                .OrderBy(m => m.Delta)
                .FirstOrDefault();
        }
        #endregion

        #region Take Smallest
        private Move TakeSmallest(int diff)
        {
            Console.WriteLine("Taking smallest");
            if (diff < 0)
                return TakeSmallestPositive();

            return TakeSmallestNegative();
        }

        private Move TakeSmallestPositive() => Consts.Moves
            .Where(m => m.Delta > 0)
            .OrderBy(m => m.Delta)
            .FirstOrDefault();

        private Move TakeSmallestNegative() => Consts.Moves
            .Where(m => m.Delta < 0)
            .OrderByDescending(m => m.Delta)
            .FirstOrDefault();
        #endregion
    }
}
