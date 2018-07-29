
namespace ZXStudio
{

    public struct SourceLine
    {
        public int Address;
        public string Text;
        public string Disassembly;
        public int Start;
        public int Length;
        public bool Breakpoint;
        public SourceLine(int start, int length, string text, string disassembly, int address)
        {

            this.Text = text;
            this.Start = start;
            this.Length = length;
            this.Disassembly = disassembly;
            this.Breakpoint = false;
            this.Address = address;
        }
        public override string ToString()
        {
            return Start.ToString() + ", " + Length.ToString() + "'" + Text + "' [" + Breakpoint.ToString() + "]";
        }

    }
}
