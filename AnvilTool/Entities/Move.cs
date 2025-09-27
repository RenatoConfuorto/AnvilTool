using AnvilTool.Properties;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;

using static AnvilTool.Constants.Consts;

namespace AnvilTool.Entities
{
    public class Move
    { 
        public string Label { get; set; } 
        public int Delta { get; set; }
        public MovesEnum MoveEn { get; set; }
        public Bitmap MoveImage { get => GetMoveImage(); }
        public Bitmap MoveBackground { get => GetMoveBackground(); }

        private Bitmap GetMoveBackground()
        {
            if(Delta > 0)
                return Resources.actionButtonGreen;
            else return Resources.actionButtonRed;
        }

        private Bitmap GetMoveImage()
        {
            switch(MoveEn)
            {
                case MovesEnum.LightHit:
                    return Resources.lightHit;
                case MovesEnum.MediumHit:
                    return Resources.mediumHit;
                case MovesEnum.HardHit:
                    return Resources.hardHit;
                case MovesEnum.Draw:
                    return Resources.draw;
                case MovesEnum.Punch:
                    return Resources.punch;
                case MovesEnum.Bend:
                    return Resources.bend;
                case MovesEnum.Upset:
                    return Resources.upset;
                case MovesEnum.Shrink:
                    return Resources.shrink;
                default: return null;
            }
        }

        public Move Copy()
        {
            return new Move()
            {
                Label = this.Label,
                Delta = this.Delta,
                MoveEn = this.MoveEn,
            };
        }

        public override string ToString() => $"{Label} {Delta}";
    }

}
