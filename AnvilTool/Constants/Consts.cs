using AnvilTool.Commands;
using AnvilTool.Entities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilTool.Constants
{
    public static class Consts
    {
        public const int MIN_POS = 0;
        public const int MAX_POS = 150;

        public const int MAX_ITER = 100;

        public static readonly ObservableCollection<Move> Moves = new ObservableCollection<Move>()
        {
            { new Move { Label = "Light Hit", Delta = -3 } },
            { new Move { Label = "Madium Hit", Delta = -6 } },
            { new Move { Label = "Punch", Delta = +2 } },
            { new Move { Label = "Bend", Delta = +7 } },
            { new Move { Label = "Hard hit", Delta = -9 } },
            { new Move { Label = "Draw", Delta = -15 } },
            { new Move { Label = "Upset", Delta = +13 } },
            { new Move { Label = "Shrink", Delta = +16 } },
        };
    }
}
