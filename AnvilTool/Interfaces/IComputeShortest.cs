using AnvilTool.Constants;
using AnvilTool.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.Interfaces
{
    internal interface IComputeShortest
    {
        List<Move> Compute(int startingPos, int targetPos, List<Move> lastMoves);
    }
}
