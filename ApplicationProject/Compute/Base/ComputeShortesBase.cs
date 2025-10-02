using LIB.Constants;
using LIB.Entities;
using LIB.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnvilTool.Compute.Base
{
    public abstract class ComputeShortesBase : IComputeShortest
    {
        public abstract List<Move> Compute(int startingPos, int targetPos, List<Move> lastMoves);
        protected List<Move> GetFinalSeq(List<int> seq, List<Move> finalSeq)
        {
            List<Move> computedSeq = new List<Move>();
            for (int i = 1; i < seq.Count; i++)
            {
                int delta = seq[i] - seq[i - 1];
                Move move = Cnst.Moves.FirstOrDefault(m => m.Delta == delta);
                if (move == null)
                    throw new Exception($"Cannot find move for delta {delta}");

                computedSeq.Add(move);
            }

            if (finalSeq.Count == 0) return computedSeq;

            for (int i = finalSeq.Count - 1; i >= 0; i--)
            {
                computedSeq.Add(finalSeq[i]);
            }

            return computedSeq;
        }
        protected List<Move> GetFinalSeq(List<Move> seq, List<Move> finalSeq)
        {
            List<Move> computedSeq = new List<Move>(seq);

            for (int i = finalSeq.Count - 1; i >= 0; i--)
            {
                computedSeq.Add(finalSeq[i]);
            }

            return computedSeq;
        }

        protected bool VerifySequence(int startPos, int targetPos, List<Move> moves)
        {
            foreach (Move move in moves)
                startPos += move.Delta;

            return startPos == targetPos;
        }
    }
}