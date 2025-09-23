using AnvilTool.Commands;
using AnvilTool.Compute.Base;
using AnvilTool.Constants;
using AnvilTool.Entities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.Compute
{
    public class BfsComputeShortes : ComputeShortesBase
    {
        public override List<Move> Compute(int startingPos, int targetPos, List<Move> lastMoves)
        {
            if (lastMoves == null || lastMoves.Count == 0)
                throw new ArgumentException("Last moves not setted");

            List<Move> sequence = new List<Move>();

            // Calculate the total offset of the last moves
            int lastOffset = lastMoves.Sum(m => m.Delta);
            // Final position before doing last movements
            int posBeforeLast = startingPos - lastOffset; // TODO check that we don't go below 0

            ////////////////////////
            var deltas = Consts.Moves.Select(m => m.Delta).ToList();
            int start = startingPos;
            int target = posBeforeLast;

            // La coda per la BFS, memorizza il percorso attuale
            var queue = new Queue<List<int>>();
            // HashSet per tenere traccia dei valori già visitati
            var visited = new HashSet<int>();

            // Inizializza con il valore di partenza
            queue.Enqueue(new List<int> { start });
            visited.Add(start);

            while (queue.Count > 0)
            {
                var currentPath = queue.Dequeue();
                var currentValue = currentPath[currentPath.Count - 1];

                // Se il valore corrente è il target, abbiamo trovato la soluzione
                if (currentValue == target)
                {
                    var list = GetFinalSeq(currentPath, lastMoves);
                    if(VerifySequence(startingPos, targetPos, list))
                        return list;
                }

                // Applica i delta e aggiungi i nuovi valori alla coda
                foreach (var delta in deltas)
                {
                    var newValue = currentValue + delta;

                    // Evita di esplorare valori già visitati
                    if (!visited.Contains(newValue))
                    {
                        visited.Add(newValue);
                        var newPath = new List<int>(currentPath);
                        newPath.Add(newValue);
                        queue.Enqueue(newPath);
                    }
                }
            }

            // Se la coda si svuota e non troviamo il target, non c'è soluzione
            return null;
        }
    }
}
