using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    public struct ScreenMap
    {
        public int Screen;
        public int Attribute;
        public int Pixel;
        public int Cycle;
        public int Row;
        public int Col;
        public ScreenMap(int a)
        {
            Screen = 0;
            Attribute = 0;
            Pixel = 0;
            Cycle = 0;
            Row = 0;
            Col = 0;
        }
        public override string ToString()
        {
            switch (Screen)
            {
                case -1: return "Top Border, Cycle: " + Cycle.ToString();
                case -2: return "Left Border, Cycle: " + Cycle.ToString();
                case -3: return "Right Border, Cycle: " + Cycle.ToString();
                case -4: return "Bottom Border, Cycle: " + Cycle.ToString();
            }
            return "(" + Row.ToString() + "," + Col.ToString() + ":" + "#" + ") " + " Screen: " + Screen.ToString() + ", Attribute: " + Attribute.ToString() + ", Cycle: " + Cycle.ToString();
        }

    }
}
