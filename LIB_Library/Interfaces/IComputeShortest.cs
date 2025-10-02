using LIB.Constants;
using LIB.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Interfaces
{
    public interface IComputeShortest
    {
        List<Move> Compute(int startingPos, int targetPos, List<Move> lastMoves);
    }
}
