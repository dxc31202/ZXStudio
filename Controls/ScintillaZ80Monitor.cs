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
public class ScintillaZ80Monitor : Scintilla
{

    public ScintillaZ80Monitor()
    {
        SetZ80Styles();

    }
    
    public string FileName;
    public string WordUnderMouse;
    public char CharUnderMouse;
    public int MouseLocation;
    public int LineNumber;
    public int LinePosition;
    public int Column;
    public Point CurrentMouse;
    protected override void OnMouseUp(MouseEventArgs e)
    {
        Focus();
        SetupPositionDetails(CurrentPosition, LinePosition);
        base.OnMouseUp(e);
    }

    public event MouseOverEventHandler MouseOverEvent;
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Focus();
        int position = CharPositionFromPoint(e.X, e.Y);
        int line = LineFromPosition(position);
        MouseOverEvent?.Invoke(this, new MouseOverEventArgs(position,line,e.Location));
    }
    protected override void OnKeyUp(KeyEventArgs e)
    {
        SetupPositionDetails(CurrentPosition, LinePosition);
        base.OnKeyUp(e);
    }

    public new int SelectionStart
    {
        get { return base.SelectionStart; }
        set
        {
            base.SelectionStart = value;
            SetupPositionDetails(CurrentPosition, LinePosition);
        }
    }


    protected override void OnGotFocus(EventArgs e)
    {
        SetupPositionDetails(CurrentPosition, LinePosition);
        base.OnGotFocus(e);
    }
    public event EventHandler UpdatePosition;

    public void SetupPositionDetails(int position, int linePosition)
    {
        WordUnderMouse = base.GetWordFromPosition(position);
        CharUnderMouse = (char)GetCharAt(position);
        LineNumber = LineFromPosition(position);
        LinePosition = Lines[LineNumber].Position;
        Column = CurrentPosition - linePosition;
        UpdatePosition?.Invoke(this, EventArgs.Empty);
    }
    protected override void OnDoubleClick(DoubleClickEventArgs e)
    {
        ToggleBreakpoint(e.Line);
        base.OnDoubleClick(e);
    }
    protected override void OnMarginClick(MarginClickEventArgs e)
    {
        int line = LineFromPosition(e.Position);
        ToggleBreakpoint(line);
        base.OnMarginClick(e);
    }
    public event BreakpointsChangedEventHandler BreakpointsChanged;
    public bool[] Breakpoints = new bool[0x10000];
    public bool[] Steppoints = new bool[0x10000];
    public int LastBeakpointAddress;
    public void SetBreakpoint(int PC)
    {
        if (!ShowBreakpointsMargin) return;
        Line sline;
        int lineNo = 0;
        string saddress = PC.ToString("X4");
        while (true)
        {
            sline = Lines[lineNo];
            if (sline.Text.Length < 4 || sline.Text.Substring(0, 4) == "    ")
            {
                lineNo++;
                continue;
            }
            if (sline.Text.Substring(0, 4).Trim() == saddress)
            {
                // Add bookmark
                //if (Breakpoint != null) Breakpoint(address, true);
                Breakpoints[lineNo] = true;
                return;
            }
            lineNo++;
            sline = Lines[lineNo];
            if (lineNo > Lines.Count) return;

        }
    }
    void ToggleBreakpoint(int lineNo)
    {
        if (!ShowBreakpointsMargin) return;
        BreakpointsChangedEventArgs e;
        Line sline = Lines[lineNo];
        const uint mask = (1 << BOOKMARK_MARKER);
        while (sline.Text.Length < 4 || sline.Text.Substring(0, 4) == "    ")
        {
            lineNo++;
            sline = Lines[lineNo];
            if (lineNo > Lines.Count) return;
        }
        string saddress = sline.Text.Substring(0, 4).Trim();
        if (saddress.Length > 0)
        {

            try
            {
                LastBeakpointAddress = Convert.ToInt32(saddress, 16);
            }
            catch { return; }
            Breakpoints[LastBeakpointAddress] = !Breakpoints[LastBeakpointAddress];
            if (!Breakpoints[LastBeakpointAddress])
            {
                // Remove existing bookmark
                sline.MarkerDelete(BOOKMARK_MARKER);
                //if (Breakpoint != null) Breakpoint(address, false);
                Breakpoints[LastBeakpointAddress] = false;
                BreakpointsChanged?.Invoke(this, new BreakpointsChangedEventArgs(Breakpoints));
                Refresh();
                return;
            }
            else
            {
                // Add bookmark
                sline.MarkerAdd(BOOKMARK_MARKER);
                //if (Breakpoint != null) Breakpoint(address, true);
                Breakpoints[LastBeakpointAddress] = true;
            }
            e = new BreakpointsChangedEventArgs(Breakpoints);
            BreakpointsChanged?.Invoke(this, e);
            if (!e.SourceChanged) return;
            lineNo = 0;

            while (true)
            {
                sline = Lines[lineNo];
                if (sline.Text.Length < 4 || sline.Text.Substring(0, 4) == "    ")
                {
                    lineNo++;
                    continue;
                }
                if (sline.Text.Substring(0, 4).Trim() == saddress)
                {
                    if ((sline.MarkerGet() & mask) > 0)
                    {
                        // Remove existing bookmark
                        sline.MarkerDelete(BOOKMARK_MARKER);
                        //if (Breakpoint != null) Breakpoint(address, false);
                        Breakpoints[LastBeakpointAddress] = false;
                    }
                    else
                    {
                        // Add bookmark
                        sline.MarkerAdd(BOOKMARK_MARKER);
                        //if (Breakpoint != null) Breakpoint(address, true);
                        Breakpoints[LastBeakpointAddress] = true;
                    }

                    BreakpointsChanged?.Invoke(this, new BreakpointsChangedEventArgs(Breakpoints));
                    break;
                }
                lineNo++;
                sline = Lines[lineNo];
                if (lineNo > Lines.Count) return;

            }
        }
    }


    public bool CheckBreakpoints()
    {
        for (int i = 0; i < 0x10000; i++)
            if (Breakpoints[i])
                return true;
        return false;
    }

    public void ClearBreakpoints()
    {
        Breakpoints = new bool[0x10000];
        for (int i = 0; i < Lines.Count; i++)
            Lines[i].MarkerDelete(BOOKMARK_MARKER);
        BreakpointsChanged?.Invoke(this, new BreakpointsChangedEventArgs(Breakpoints));
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

    public string ForceText
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

        base.OnKeyDown(e);
    }

    protected override void OnDelete(ModificationEventArgs e)
    {
        // Only update line numbers if the number of lines changed
        //if (e.LinesAdded != 0) UpdateLineNumbers(LineFromPosition(e.Position));
        base.OnDelete(e);
    }

    protected override void OnInsert(ModificationEventArgs e)
    {
        // Only update line numbers if the number of lines changed
        //if (e.LinesAdded != 0) UpdateLineNumbers(LineFromPosition(e.Position));
        base.OnInsert(e);
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
    bool showBreakpointsMargin = false;
    public bool ShowBreakpointsMargin
    {
        get { return showBreakpointsMargin; }
        set
        {
            showBreakpointsMargin = value;
            SetZ80Styles();
        }
    }
    public int bb;
    public void SetZ80Styles()
    {
        /// Extended instruction (keyword list 5) style index.
        /// 
        Margin margin = Margins[LINENUMBER_MARGIN];
        margin.Type = MarginType.Number;
        if (ShowMargin)
        {
            margin.Width = 48;
            margin.Cursor = MarginCursor.ReverseArrow;
        }
        else
            margin.Width = 0;

        margin = Margins[BOOKMARK_MARGIN];
        margin.Type = MarginType.Symbol;
        if (showBreakpointsMargin)
        {
            margin.Width = 16;
            margin.Sensitive = true;
            margin.Mask = Marker.MaskAll;
            margin.Cursor = MarginCursor.Arrow;
            SetFoldMarginColor(true, Color.LightSteelBlue);
            var marker = Markers[bb];  //BOOKMARK_MARKER
            marker.Symbol = MarkerSymbol.Bookmark;
            marker.SetBackColor(Color.Red);
            marker.SetForeColor(Color.Black);

            string saddress;
            foreach (Line sline in Lines)
            {
                sline.MarkerDelete(BOOKMARK_MARKER);
                string text = sline.Text + "    ";
                saddress = text.Substring(0, 4).Trim();
                try
                {
                    int add = Convert.ToInt32(saddress, 16);
                    if (Breakpoints[add])
                        sline.MarkerAdd(BOOKMARK_MARKER);
                }
                catch { }
            }
        }
        else
        {

            foreach (Line sline in Lines)
                sline.MarkerDelete(BOOKMARK_MARKER);
            margin.Width = 0;
        }



        StyleResetDefault();
        Styles[Style.Default].Font = "Courier New";
        Styles[Style.Default].Size = 9;
        StyleClearAll();

        // Line Number
        Styles[Style.LineNumber].ForeColor = Color.Teal;
        Styles[Style.LineNumber].Visible = true;



        // Comment
        Styles[Style.Asm.CommentBlock].ForeColor = Color.Black;
        Styles[Style.Asm.Comment].ForeColor = Color.FromArgb(0x008000);
        Styles[Style.Asm.Comment].Font = "Courier New";
        Styles[Style.Asm.Comment].Size = 9;
        Styles[Style.Asm.Comment].FillLine = false;
        Styles[Style.Asm.Comment].Bold = false;

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
        Invalidate();

    }

    const int LINENUMBER_MARGIN = 0;
    const int BOOKMARK_MARGIN = 1; // Conventionally the symbol margin
    const int BOOKMARK_MARKER = 0; // Arbitrary. Any valid index would work.
    const int STYLE_LINENUMBER = 33;

}
public delegate void MouseOverEventHandler(object sender, MouseOverEventArgs e);
public class MouseOverEventArgs : EventArgs
{
    public int Position;
    public int Line;
    public Point Location;
    public MouseOverEventArgs(int position,int line,Point location)
    {
        Position = position;
        Line = line;
        Location = location;

    }
}
public delegate void BreakpointsChangedEventHandler(object sender, BreakpointsChangedEventArgs e);
public class BreakpointsChangedEventArgs : EventArgs
{
    public bool[] Breakpoints = new bool[0x10000];
    public bool SourceChanged;
    public BreakpointsChangedEventArgs(bool[] breakpoints)
    {
        Breakpoints = breakpoints;
        SourceChanged = false;

    }
}

