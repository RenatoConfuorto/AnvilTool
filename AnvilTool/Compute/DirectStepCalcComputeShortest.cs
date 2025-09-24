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

                Log($"Iteration number {iter}. Starting Pos {current} - Target {target}");
                List<Move> currentSeq = new List<Move>(startSeq);

                // Apply deltas from starting seq
                current += startSeq.Sum(m => m.Delta);
                //foreach(var m in startSeq)
                //    currentSeq.Add(m);7

                //max = min = 0; // Reset max and min
                while (currentSeq.Count <  Consts.MAX_SEQ_LENGHT)
                {
                    // Calculate the distance from target
                    int diff = current - target;
                    if(diff == 0)
                    {
                        Log("SOLUTION FOUND--------------------");
                        // TODO check if there can be a better solution
                        //if (currentSeq.Count - startSeq.Count < Consts.EXIT_SEQ_LEN)
                        //    return GetFinalSeq(currentSeq, lastMoves);
                        //else finalSeq = currentSeq;

                        if (currentSeq.Count - startSeq.Count < Consts.EXIT_SEQ_LEN)
                        {
                            Log("Current solution is small enough. Exiting calculation");
                            return GetFinalSeq(currentSeq, lastMoves);      // Return directly the sequence
                        }
                        else
                        {
                            // If there is no final sequence, or the last one is longer, save the found seq
                            if(finalSeq == null || finalSeq.Count - lastMoves.Count > currentSeq.Count)
                            {
                                Log("Saving current calculated solution");
                                finalSeq = GetFinalSeq(currentSeq, lastMoves);
                            }

                            // Update starting seq
                            startSeq = GetStartingMoves(startSeq, max, min, target);
                            break;// Next iter
                        }


                        //return GetFinalSeq(currentSeq, lastMoves);
                    }
                    var m = TakeMove(diff);
                    int newPos = current + m.Delta;
                    if(newPos <= Consts.MIN_POS || newPos >= Consts.MAX_POS)
                    {
                        Log("-------------------------");
                        Log($"Selected move {m} would exceed bounds. Current position: {current}, new position: {newPos}. Skipping this path.");

                        startSeq = GetStartingMoves(startSeq, max, min, target);
                        break;
                    }


                    currentSeq.Add(m);
                    current += m.Delta;
                    SaveMaxMin(ref max, ref min, current);
                    Log($"Diff {diff} - Selected move {m} - New Pos {current} - Seq Len {currentSeq.Count}");
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
            Log($"Calculationg starting sequence. Total delta {originalStarting.Sum(m => m.Delta)} - Count {originalStarting.Count}");
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

            int currentSum = temp.Sum(m => m.Delta);

            // check if we can reduce the number of elements
            List<Move> bestComb = new List<Move>(temp);
            int bestSum = bestComb.Sum(m => m.Delta);

            // Iterate through all possible moves from the predefined list
            foreach (Move predefinedMove in Consts.Moves)
            {
                // Try replacing subsets of the originalStarting sequence with the predefinedMove
                // We'll look for a subset of at least two moves to ensure we are actually reducing the count
                for (int i = 0; i < originalStarting.Count - 1; i++) // Start with subsets of size 2
                {
                    int subsetSum = 0;
                    for (int j = i; j < originalStarting.Count; j++)
                    {
                        subsetSum += originalStarting[j].Delta;

                        // Construct a new candidate list by removing the subset and adding the predefinedMove
                        List<Move> newCandidate = new List<Move>();

                        // Add moves before the subset
                        for (int k = 0; k < i; k++)
                        {
                            newCandidate.Add(originalStarting[k]);
                        }

                        // Add moves after the subset
                        for (int k = j + 1; k < originalStarting.Count; k++)
                        {
                            newCandidate.Add(originalStarting[k]);
                        }

                        newCandidate.Add(predefinedMove);

                        // Now, check if this new, shorter sequence is a better solution
                        int candidateSum = newCandidate.Sum(m => m.Delta);

                        // A better combination is one that is shorter AND its sum is within tolerance
                        // of the current best sum.
                        if (newCandidate.Count < bestComb.Count && Math.Abs(candidateSum - bestSum) <= Consts.START_SEQ_TOL)
                        {
                            bestComb = newCandidate;
                            bestSum = candidateSum; // Update the new best sum
                                                    // A more robust search would continue, but for simplicity, we can break
                                                    // and return the first better solution found.
                        }
                    }
                }
            }

            Log($"Final solution found. Total delta {bestComb.Sum(m => m.Delta)} - Count {bestComb.Count}");
            return bestComb;
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

        private void Log(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
