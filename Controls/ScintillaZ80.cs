using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ScintillaNET;
using ScintillaNET_FindReplaceDialog;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
public class ScintillaZ80 : Scintilla
{

    public ScintillaZ80 Buddy;
    private const int EM_GETSCROLLPOS = 0x4DD;  //you send this message and the control returns it's scroll position
    private const int EM_SETSCROLLPOS = 0x4DE;//this is used to set the control's scroll position
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref Point lParam);

    static bool scrolling = false;
    public ScintillaZ80()
    {
        SetZ80Styles();
        
        
    }


    public new int MousePosition;
    public int Row;
    public int Column;
    public string CurrentWord = "";
    struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x,int y)
        {
            X = x;
            Y = y;
        }
    }

    public Point CurrentRowCol = new Point(0,0);
    
    public event EventHandler PositionChangedEvent;
    [DllImport("user32.dll")]
    static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wp, int lp);

    const int VK_LEFT = 0x25;
    const int VK_UP = 0x26;
    const int VK_RIGHT = 0x27;
    const int VK_DOWN = 0x28;
    const int VK_HOME = 0x24;
    const int VK_END = 0x23;
    const int VK_PRIOR = 0x21;
    const int VK_NEXT = 0x22;

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if ((WM)m.Msg == WM.MOUSEMOVE)
        {
            int y = ((int)m.LParam >> 16) & 0xffff;
            int x = (int)m.LParam & 0xffff;
            CurrentRowCol.X = x;
            CurrentRowCol.Y = y;
            MousePosition = CharPositionFromPoint(x, y);
            Row = LineFromPosition(MousePosition);
            CurrentWord = GetWordFromPosition(MousePosition);
            Column = GetColumn(MousePosition);
            PositionChangedEvent?.Invoke(this, EventArgs.Empty);
        }
        if ((WM)m.Msg == WM.NCHITTEST)
        {
            int y = ((int)m.LParam >> 16) & 0xffff;
            int x = (int)m.LParam & 0xffff;
            POINT point = new POINT(x, y);
            ScreenToClient(Handle, ref point);
            CurrentRowCol.X = point.X;
            CurrentRowCol.Y = point.Y;
            MousePosition = CharPositionFromPoint(point.X, point.Y);
            Row = LineFromPosition(MousePosition);
            CurrentWord = GetWordFromPosition(MousePosition);
            Column = GetColumn(MousePosition);
            PositionChangedEvent?.Invoke(this, EventArgs.Empty);
        }
        if ((WM)m.Msg == WM.KEYDOWN)
        {
            switch ((int)m.WParam)
            {
                case VK_DOWN:
                case VK_LEFT:
                case VK_RIGHT:
                case VK_UP:
                case VK_HOME:
                case VK_END:
                case VK_PRIOR:
                case VK_NEXT:
                    {

                        MousePosition = CurrentPosition;
                        Row = LineFromPosition(MousePosition);
                        CurrentWord = GetWordFromPosition(MousePosition);
                        Column = GetColumn(MousePosition);
                        PositionChangedEvent?.Invoke(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        if (!Selectable)
            switch ((WM)m.Msg)
            {
                case WM.LBUTTONDOWN:
                case WM.RBUTTONDOWN:
                case WM.MBUTTONDOWN:
                case WM.LBUTTONUP:
                case WM.RBUTTONUP:
                case WM.MBUTTONUP:
                    return;
            }

        WM msg = (WM)m.Msg;
        if (m.Msg == 0x20a)
            msg = WM.MOUSEHWHEEL;
        if (msg == WM.MOUSEHWHEEL || msg == WM.VSCROLL || msg == WM.HSCROLL || msg == WM.KEYUP || msg == WM.MOUSEMOVE)
            if (!scrolling)
                if (Buddy != null && Buddy.IsHandleCreated)
                {
                    scrolling = true;
                    Buddy.FirstVisibleLine = FirstVisibleLine;
                    //SendMessage(Buddy.Handle, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                    scrolling = false;
                }

    }


    [Category("Appearance")]
    [DefaultValue(typeof(bool), "true")]
    [Description("Can Select the control.")]
    public bool Selectable
    {
        get
        {
            return this.GetStyle(ControlStyles.Selectable);
        }
        set
        {
            this.SetStyle(ControlStyles.Selectable, value);
            TabStop = value;
            

        }
    }

    
    private void UpdateLineNumbers(int startingAtLine)
    {
        // Starting at the specified line index, update each
        // subsequent line margin text with a hex line number.
        int maxwidth = 0;
        for (int i = startingAtLine; i < Lines.Count; i++)
        {
            Lines[i].MarginStyle = Style.LineNumber;
            Lines[i].MarginText = (i + 1).ToString() + " ";
            maxwidth = TextWidth(1, "1" + (i + 1).ToString() + " ");
        }
        Margins[BOOKMARK_MARGIN].Width = maxwidth;
    }

    public  string ForceText
    {
        get
        {
            return base.Text;
        }

        set
        {
            bool ro = ReadOnly;
            ReadOnly = false;
            base.Text = value;
            ReadOnly = ro;
        }
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {

        using (FindReplace FindReplace = new FindReplace(this))
        {

            switch (e.KeyCode)
            {

                case Keys.F3:
                    if (e.Shift)
                        FindReplace.Window.FindPrevious();
                    else
                        FindReplace.Window.FindNext();
                    //if (!result)
                    //    MessageBox.Show(this, FindReplace.Window.LastFindMessage, "ZX Studio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.SuppressKeyPress = true;
                    break;
            }
            #region CTRL+key
            if (e.Control)
                switch (e.KeyCode)
                {
                    case Keys.F:
                        FindReplace.ShowFind();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.H:
                        FindReplace.ShowReplace();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.I:
                        FindReplace.ShowIncrementalSearch();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.G:
                        GoTo MyGoTo = new GoTo(this);
                        MyGoTo.ShowGoToDialog();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.A:
                        SelectAll();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.C:
                        Clipboard.SetText(Text);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.V:
                        if (Clipboard.ContainsText())
                            Text = Clipboard.GetText();
                        e.SuppressKeyPress = true;
                        break;
                }
            #endregion CTRL+key
        }
        base.OnKeyUp(e);
    }
    protected override void OnDelete(ModificationEventArgs e)
    {
        // Only update line numbers if the number of lines changed
        //if (e.LinesAdded != 0)                UpdateLineNumbers(LineFromPosition(e.Position));
        base.OnDelete(e);
    }

    protected override void OnInsert(ModificationEventArgs e)
    {

        // Only update line numbers if the number of lines changed
        //if (e.LinesAdded != 0)                UpdateLineNumbers(LineFromPosition(e.Position));
        base.OnInsert(e);
        //Focus();
        //HighlightWord(e.Text);
        //SelectionStart = CurrentPosition + e.Text.Length;
    }

    public void HighlightWord(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        // Indicators 0-7 could be in use by a lexer
        // so we'll use indicator 8 to highlight words.
        const int NUM = 20;

        // Remove all uses of our indicator
        IndicatorCurrent = NUM;
        IndicatorClearRange(0, TextLength);

        // Update indicator appearance
        Indicators[NUM].Style = IndicatorStyle.StraightBox;
        Indicators[NUM].Under = true;
        Indicators[NUM].ForeColor = Color.Green;
        Indicators[NUM].OutlineAlpha = 50;
        Indicators[NUM].Alpha = 60;

        IndicatorFillRange(CurrentPosition, text.Length);


    }
    bool showMargin = false;
    public bool ShowMargin
    {
        get { return showMargin; }
        set
        {
            showMargin = value;
            SetZ80Styles();
        }
    }
    void SetZ80Styles()
    {
        // Extended instruction (keyword list 5) style index.
        Margin margin = Margins[LINENUMBER_MARGIN];
        margin.Type = MarginType.Number;

        margin.Width = (ShowMargin ? 48 : 0);
        margin.Cursor = MarginCursor.ReverseArrow;

        margin = Margins[BOOKMARK_MARGIN];
        margin.Width = 0;
        margin.Sensitive = true;
        margin.Type = MarginType.Symbol;
        margin.Mask = Marker.MaskAll;
        margin.Cursor = MarginCursor.Arrow;
        SetFoldMarginColor(true, Color.LightSteelBlue);

        var marker = Markers[BOOKMARK_MARKER];
        marker.Symbol = MarkerSymbol.Bookmark;
        marker.SetBackColor(Color.Red);
        marker.SetForeColor(Color.Black);

        StyleResetDefault();
        Styles[Style.Default].Font = "Courier New";
        Styles[Style.Default].Size = 9;
        StyleClearAll();

        // Line Number
        Styles[Style.LineNumber].ForeColor = Color.Teal;


        // Comment
        Styles[Style.Asm.CommentBlock].ForeColor = Color.Black;
        Styles[Style.Asm.Comment].ForeColor = Color.FromArgb(0x008000);
        Styles[Style.Asm.Comment].Font = "Courier New";
        Styles[Style.Asm.Comment].Size = 9;
        Styles[Style.Asm.Comment].FillLine = false;
        Styles[Style.Asm.Comment].Bold=false;

        Lexer = Lexer.Asm;

        /// CPU instruction (keyword list 0) style index.
        Styles[Style.Asm.CpuInstruction].ForeColor = Color.Blue;
        Styles[Style.Asm.CpuInstruction].Case = StyleCase.Upper;
        Styles[Style.Asm.CpuInstruction].Bold = false;
        SetKeywords(0, "adc add and bit ccf cp cpd cpdr cpi cpir cpl daa dec di ei ex exx halt im in inc ind indr ini inir ld ldd lddr ldi ldir neg nop or otd otdr oti otir out pop push res rl rla rlc rlc rlca rld rr rra rrc rrca rrd rst 00h rst 08h rst 10h rst 18h rst 20h rst 28h rst 30h rst 38h sbc scf set sla sll sra srl sub xor");

        /// Register (keyword list 2) style index.
        Styles[Style.Asm.Register].ForeColor = Color.Blue;
        Styles[Style.Asm.Register].Case = StyleCase.Upper;
        Styles[Style.Asm.Register].Bold = false;
        SetKeywords(2, "bc de hl ix iy sp af af' a f b c d e h l ixh ixl iyh iyl i r nz z nc c po pe p m");

        /// Directive (keyword list 3) string style index.
        Styles[Style.Asm.Directive].ForeColor = Color.FromArgb(0x33A6CD);
        Styles[Style.Asm.Directive].Case = StyleCase.Upper;
        SetKeywords(3, "equ defb db defs ds defm dm text defw dw org .equ .defb .db .defs .ds .defm .dm .text .defw .dw .org .byte .word");

        /// Directive operand (keyword list 4) style index.
        Styles[Style.Asm.DirectiveOperand].ForeColor = Color.Red;
        Styles[Style.Asm.DirectiveOperand].Case = StyleCase.Upper;
        Styles[Style.Asm.DirectiveOperand].Bold = false;
        SetKeywords(4, "call djnz jp jr ret reti retn");

        Styles[Style.Asm.Number].ForeColor = Color.Black;
        Styles[Style.Asm.String].ForeColor = Color.FromArgb(0xA31515);
        Styles[Style.Asm.Character].ForeColor = Color.FromArgb(0xA31515);
        SetSelectionBackColor(true, Color.Yellow);
        MultipleSelection = false;


    }

    const int LINENUMBER_MARGIN = 0;
    const int BOOKMARK_MARGIN = 1; // Conventionally the symbol margin
    const int BOOKMARK_MARKER = 0; // Arbitrary. Any valid index would work.
    const int STYLE_LINENUMBER = 33;

}
