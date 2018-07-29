using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace ZXStudio
{
    public partial class Monitor : Form
    {

        bool[] breakpoints = new bool[0x10000];
        bool[] steppoints = new bool[0x10000];
        Z80 z80;
        public Monitor(Z80 z80) :base()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            scintillaZ80Monitor1.ShowBreakpointsMargin = true;
            scintillaZ80Monitor1.BreakpointsChanged += ScintillaZ80Monitor1_BreakpointsChanged;
            scintillaZ80Monitor1.MouseOverEvent += ScintillaZ80Monitor1_MouseOverEvent;
            this.z80 = z80;
            KeyPreview = true;
        }

        ToolTip tooltip = new ToolTip();
        string tooltiptext;
        string lasttooltiptext;
        private void ScintillaZ80Monitor1_MouseOverEvent(object sender, MouseOverEventArgs e)
        {
            scintillaZ80Monitor1.SetupPositionDetails(e.Position,e.Line);
            

            switch(scintillaZ80Monitor1.WordUnderMouse.ToUpper())
            {
                case "A":
                    tooltiptext = "Hex = #" + z80.A.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.A.ToString();
                    break;
                case "F":
                    tooltiptext = "Hex = #" + z80.F.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.F.ToString() + Environment.NewLine +
                                  "Flags = " + z80.FlagFS.ToString() + " "
                                             + z80.FlagFZ.ToString() + " "
                                             + z80.FlagF5.ToString() + " "
                                             + z80.FlagFH.ToString() + " "
                                             + z80.FlagF3.ToString() + " "
                                             + z80.FlagFP.ToString() + " "
                                             + z80.FlagFN.ToString() + " "
                                             + z80.FlagFC.ToString();
                    break;
                case "B":
                    tooltiptext = "Hex = #" + z80.B.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.B.ToString();
                    break;
                case "C":
                    tooltiptext = "Hex = #" + z80.C.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.C.ToString();
                    break;
                case "D":
                    tooltiptext = "Hex = #" + z80.D.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.D.ToString();
                    break;
                case "E":
                    tooltiptext = "Hex = #" + z80.E.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.E.ToString();
                    break;
                case "H":
                    tooltiptext = "Hex = #" + z80.H.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.H.ToString();
                    break;
                case "L":
                    tooltiptext = "Hex = #" + z80.L.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.L.ToString();
                    break;
                case "AF":
                    tooltiptext = "Hex = #" + z80.AF.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.AF.ToString() + Environment.NewLine +
                                  "Memory = #" + z80[z80.AF].ToString("X2") + " (" + z80[z80.AF].ToString() + ")"+Environment.NewLine+
                                  "S = " + z80.FlagFS.ToString() + Environment.NewLine +
                                  "Z = " + z80.FlagFZ.ToString() + Environment.NewLine +
                                  "Y = " + z80.FlagF5.ToString() + Environment.NewLine +
                                  "H = " + z80.FlagFH.ToString() + Environment.NewLine +
                                  "X = " + z80.FlagF3.ToString() + Environment.NewLine +
                                  "P = " + z80.FlagFP.ToString() + Environment.NewLine +
                                  "N = " + z80.FlagFN.ToString() + Environment.NewLine +
                                  "C = " + z80.FlagFC.ToString();
                    break;
                case "BC":
                    tooltiptext = "Hex = #" + z80.BC.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.BC.ToString() + Environment.NewLine +
                                  "Memory = #" + z80[z80.BC].ToString("X2") + " (" + z80[z80.BC].ToString() + ")";
                    break;
                case "DE":
                    tooltiptext = "Hex = #" + z80.DE.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.DE.ToString() + Environment.NewLine +
                                  "Memory = #" + z80[z80.DE].ToString("X2") + " (" + z80[z80.DE].ToString() + ")";
                    break;
                case "HL":
                    tooltiptext = "Hex = #" + z80.HL.ToString("X4") + Environment.NewLine +
                                  "Decimal = " + z80.HL.ToString() + Environment.NewLine +
                                  "Memory = #" + z80[z80.HL].ToString("X2") + " (" + z80[z80.HL].ToString() + ")";
                    break;
                default:
                    tooltiptext = "";
                    break;
            }
            if (tooltiptext != "")
            {
                if (lasttooltiptext != tooltiptext)
                {
                    tooltip.ShowAlways = true;
                    tooltip.ToolTipTitle = scintillaZ80Monitor1.WordUnderMouse;
                    tooltip.Show(tooltiptext, this, e.Location);
                }
            }
            else
                tooltip.Hide(this);
            lasttooltiptext = tooltiptext;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        //protected override void OnShown(EventArgs e)
        //{
        //    z80.ResetEventHandler -= Z80_ResetEventHandler;
        //    z80.BeforeExecuteEventHandler -= Z80_BeforeExecuteEventHandler;
        //    z80.AfterExecuteEventHandler -= Z80_AfterExecuteEventHandler;

        //    z80.ResetEventHandler += Z80_ResetEventHandler;
        //    z80.BeforeExecuteEventHandler += Z80_BeforeExecuteEventHandler;
        //    z80.AfterExecuteEventHandler += Z80_AfterExecuteEventHandler;
        //    ShowListing();
        //    base.OnShown(e);
        //}
        private void ScintillaZ80Monitor1_BreakpointsChanged(object sender, BreakpointsChangedEventArgs e)
        {
            ScintillaZ80Monitor MonitorWindow = sender as ScintillaZ80Monitor;
            int pc = MonitorWindow.LastBeakpointAddress;
            if (Disassembler.SourceLines[pc].Text == null)
            {
                e.SourceChanged = true;
                if (addresses == null)
                    addresses = new List<int>();
                addresses.Add(pc);
                Disassembler.Disassemble(z80.Memory);
                Disassembler.Disassemble(0, addresses, 0, 65535, z80.Memory, true, true, true);
                //MonitorWindow.ReadOnly = false;
                MonitorWindow.ForceText = Disassembler.Listing.ToString();
                //MonitorWindow.ReadOnly = true;

            }
            else if (MonitorWindow.Text.Length == 0)
                MonitorWindow.ForceText = Disassembler.Listing.ToString();
            breakpoints = MonitorWindow.Breakpoints;
            toolStripButton6.Text = "Clear Breakpoints";
            toolStripButton6.Enabled = scintillaZ80Monitor1.CheckBreakpoints();
            toolStripButton6.Visible = false; toolStripButton6.Visible = true;
        }

        private void Z80_ResetEventHandler(object sender, EventArgs e)
        {
            
            UpdateDisplay(sender);
        }
        Stopwatch stopwatch;
        long lastStopwatch;

        protected override void OnVisibleChanged(EventArgs e)
        {
            z80.ResetEventHandler -= Z80_ResetEventHandler;
            z80.BeforeExecuteEventHandler -= Z80_BeforeExecuteEventHandler;
            z80.AfterExecuteEventHandler -= Z80_AfterExecuteEventHandler;
            if (Visible)
            {
                ShowListing();
                Globals.EnsureVisible(Handle);
                BringToFront();
                Focus();

                z80.ResetEventHandler += Z80_ResetEventHandler;
                z80.BeforeExecuteEventHandler += Z80_BeforeExecuteEventHandler;
                z80.AfterExecuteEventHandler += Z80_AfterExecuteEventHandler;

                z80.DoPause();
                PClabel.Text = "";
                UpdateDisplay(z80);

            }

            base.OnVisibleChanged(e);
        }
        
        public void Reset()
        {
            if (!Visible) return;

            statusMessage.Text = "Paused";
            //z80.DoPause();
        }
        bool closing = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
            
            steppingOver = false;
            z80.ResetEventHandler -= Z80_ResetEventHandler;
            z80.BeforeExecuteEventHandler -= Z80_BeforeExecuteEventHandler;
            z80.AfterExecuteEventHandler -= Z80_AfterExecuteEventHandler;

            scintillaZ80Monitor1.ClearBreakpoints();
            breakpoints = new bool[0x10000];
            z80.Stopped = false;
            z80.Running = true;
            z80.Stepping = false;
            if (!steppingOver) z80.Run(true);

        }

        public string StatusMessage
        {
            set
            {
                statusMessage.Text = value;
                //z80.infoMessage.Text = "";
            }
        }
        private void Z80_AfterExecuteEventHandler(object sender, EventArgs e)
        {
            if (breakpoints[z80.PC])
                Breakpoint(true);
            else
            if (steppoints[z80.PC])
            {
                
                steppoints[z80.PC] = false;
                Pause();
                fast = false;
                steppingOver = false;
            }
            if (steppingOver)
                return;
            if (running) return;
            running = false;
            Show();
            UpdateDisplay(sender);
            if (fast)
            {
                while (stopwatch.ElapsedMilliseconds < lastStopwatch + (ContinuousSpeed * 100))
                {
                    System.Threading.Thread.SpinWait(10);
                }
                lastStopwatch = stopwatch.ElapsedMilliseconds;
            }
        }

        private void Z80_BeforeExecuteEventHandler(object sender, EventArgs e)
        {
        }

        public void Pause()
        {
            running = false;
            z80.DoPause();
        }
        public void Breakpoint(bool breakpoint)
        {
            running = false;
            z80.DoPause();
            fast = false;
            steppingOver = false;
            //if (breakpoint)
            //    StatusMessage = "Breakpoint Reached";
            //else
            //{
            //    if (z80.StepOut)
            //        StatusMessage = "Stepped out";
            //    else
            //        StatusMessage = "Stepped over";
            //}

        }
        void UpdateDisplay(object sender)
        {
            if (sender == null) return;
            Z80 z80 = sender as Z80;

            if (scintillaZ80Monitor1.Visible)
                UpdateListing(z80);
            else
                PClabel.Text = "PC  " + z80.ToHexWord(z80.PC) + " " + Disassembler.DisassembleLine(z80.Memory, z80.PC);

            AFastLabel.Text = z80.ToHexByte(z80.A);
            FFastLabel.Text = z80.ToHexByte(z80.F);
            fSFastLabel.Text = z80.FlagFS.ToString();
            fZFastLabel.Text = z80.FlagFZ.ToString();
            f5FastLabel.Text = z80.FlagF5.ToString();
            fHFastLabel.Text = z80.FlagFH.ToString();
            f3FastLabel.Text = z80.FlagF3.ToString();
            fPFastLabel.Text = z80.FlagFP.ToString();
            fNFastLabel.Text = z80.FlagFN.ToString();
            fCFastLabel.Text = z80.FlagFC.ToString();

            BFastLabel.Text = z80.ToHexByte(z80.B);
            CFastLabel.Text = z80.ToHexByte(z80.C);

            DFastLabel.Text = z80.ToHexByte(z80.D);
            EFastLabel.Text = z80.ToHexByte(z80.E);

            HFastLabel.Text = z80.ToHexByte(z80.H);
            LFastLabel.Text = z80.ToHexByte(z80.L);

            _AFFastLabel.Text = z80.ToHexWord(z80.AF_);
            _BCFastLabel.Text = z80.ToHexWord(z80.BC_);
            _DEFastLabel.Text = z80.ToHexWord(z80.DE_);
            _HLFastLabel.Text = z80.ToHexWord(z80.HL_);

            IXFastLabel.Text = z80.ToHexWord(z80.IX);
            IYFastLabel.Text = z80.ToHexWord(z80.IY);

            cBCFastLabel.Text = "(" + z80.ToHexWord(z80.Memory[z80.BC]) + ")";
            cDEFastLabel.Text = "(" + z80.ToHexWord(z80.Memory[z80.DE]) + ")";
            cHLFastLabel.Text = "(" + z80.ToHexWord(z80.Memory[z80.HL]) + ")";

            StringBuilder ss = new StringBuilder();
            ushort n = (ushort)z80.SP;
            for (int i = 0; i < 4; i++)
            {
                ss.Append(n.ToString("X4") + " (" + (z80.ReadByte((ushort)(n + 1))).ToString("X2") + z80.ReadByte(n).ToString("X2") + ") " + Environment.NewLine);
                n += 2;
            }
            SPFastLabel.Text = ss.ToString(); ;

            IFastLabel.Text = z80.ToHexByte(z80.I);
            RFastLabel.Text = z80.ToHexByte(z80.R);
            IMFastLabel.Text = z80.IM.ToString();

            IFF1FastLabel.Text = z80.IFF1.ToString();
            IFF2FastLabel.Text = z80.IFF1.ToString();
            CyclesFastLabel.Text = z80.TotalCycles.ToString();
            int ii = z80.ExecutedCycles;
            string s = ii.ToString();
            lastFastLabel.Text = s;

            //MouseMoveHandler(this, null);
            Globals.DoEvents();
        }

        static List<int> addresses;
        public void AddAddress(int address)
        {
            if (addresses == null)
                addresses = new List<int>();
            if (addresses.IndexOf(address) == -1) addresses.Add(address);
        }
        void UpdateListing(Z80 z80)
        {
            if (z80 == null) return;
            Disassembler.DisassembleROM = true;
            if (Disassembler.SourceLines[z80.PC].Text == null)
            {
                if (addresses == null)
                    addresses = new List<int>();
                addresses.Add(z80.PC);
                Disassembler.Disassemble(z80.Memory);
                Disassembler.Disassemble(0, addresses, 0, 65535, z80.Memory, true, true, true);
                //MonitorWindow.ReadOnly = false;
                scintillaZ80Monitor1.ForceText = Disassembler.Listing.ToString();
                //MonitorWindow.ReadOnly = true;

            }
            else if (scintillaZ80Monitor1.Text.Length == 0)
                scintillaZ80Monitor1.ForceText = Disassembler.Listing.ToString();


            int line = scintillaZ80Monitor1.LineFromPosition(Disassembler.SourceLines[z80.PC].Start);
            scintillaZ80Monitor1.GotoPosition(Disassembler.SourceLines[z80.PC].Start);
            scintillaZ80Monitor1.SelectionStart = Disassembler.SourceLines[z80.PC].Start;
            scintillaZ80Monitor1.SelectionEnd = Disassembler.SourceLines[z80.PC].Start + Disassembler.SourceLines[z80.PC].Length;
            Globals.DoEvents();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Visible = false;
            toolStripButton1.Visible = true;

            Slower();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            toolStripButton9.Visible = false;
            toolStripButton9.Visible = true;
            Faster();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toolStripButton3.Visible = false;
            toolStripButton3.Visible = true;

            Step();
        }

        void Step()
        {
            running = false;
            fast = false;
            StatusMessage = "Step";
            z80.DoStep();
        }
        bool steppingOver = false;
        void StepOver()
        {
            running = false;
            steppingOver = false;
            switch (z80.Memory[z80.PC])
            {
                case 0xED:  
                    switch (z80.Memory[z80.PC + 1])
                    {
                        case 0xB9:  //	CPDR			
                        case 0xB1:  //	CPIR			
                        case 0xBA:  //	INDR			
                        case 0xB2:  //	INIR			
                        case 0xB8:  //	LDDR			
                        case 0xB0:  //	LDIR			
                        case 0xBB:  //	OTDR			
                        case 0xB3:  //	OTIR			
                            StatusMessage = "Step Over";
                            steppoints[z80.PC + 2] = true;
                            Hide();
                            steppingOver = true;
                            z80.DoRun(true);
                            return;
                    }
                    break;
                case 0xCD:  //  CALL
                case 0xC4:  //	CALL NZ,(nn)
                case 0xCC:  //	CALL Z,(nn)
                case 0xD4:  //	CALL NC,(nn)
                case 0xDC:  //	CALL C,(nn)
                case 0xE4:  //	CALL PO,(nn)
                case 0xEC:  //	CALL PE,(nn)
                case 0xF4:  //	CALL P,(nn)
                case 0xFC:  //	CALL M,(nn)
                    StatusMessage = "Step Over";
                    steppoints[z80.PC + 3] = true;
                    Hide();
                    steppingOver = true;
                    z80.DoRun(true);
                    return;
                default:
                    StatusMessage = "Step";
                    z80.DoStep();
                    break;

            }
        }
        void StepOut()
        {
            running = false;
            steppingOver = false;
            if (z80.CallStack.Count > 0)
            {
                int x = z80.CallStack.Peek();
                steppoints[x] = true;
                steppingOver = true;
                Hide();
                StatusMessage = "Step Out (1)";
                z80.DoRun(true);
                return;
            }

            switch (z80.Memory[z80.PC])
            {
                case 0xED:  
                    switch (z80.Memory[z80.PC + 1])
                    {
                        case 0xB9:  //	CPDR			
                        case 0xB1:  //	CPIR			
                        case 0xBA:  //	INDR			
                        case 0xB2:  //	INIR			
                        case 0xB8:  //	LDDR			
                        case 0xB0:  //	LDIR			
                        case 0xBB:  //	OTDR			
                        case 0xB3:  //	OTIR			
                            steppoints[z80.PC + 2] = true;
                            Hide();
                            steppingOver = true;
                            StatusMessage = "Step Out";
                            z80.DoRun(true);
                            break;
                    }
                    break;
                default:
                    StatusMessage = "Step";
                    z80.DoStep();
                    break;

            }
        }

        void DoRun()
        {
            StatusMessage = "Run";
            z80.DoRun();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toolStripButton4.Visible = false;
            toolStripButton4.Visible = true;

            StepOver();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            toolStripButton5.Visible = false;
            toolStripButton5.Visible = true;
            StepOut();
        }
        public void ListingVisible()
        {
            Show();
            if (scintillaZ80Monitor1.Visible) return;
            toolStripButton7.Visible = false;
            toolStripButton8.Visible = true;
            ShowListing();
            UpdateDisplay(z80);
            StatusMessage = "Show Listing";
        }
        void ListingHidden()
        {
            scintillaZ80Monitor1.Visible = false;
            toolStripButton7.Visible = !scintillaZ80Monitor1.Visible;
            toolStripButton8.Visible = scintillaZ80Monitor1.Visible;
            Width -= 640;
            scintillaZ80Monitor1.Width = 640;
            ListingIsVisible = scintillaZ80Monitor1.Visible;
            UpdateDisplay(z80);
            StatusMessage = "Hide Listing";
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            scintillaZ80Monitor1.ClearBreakpoints();
            breakpoints = new bool[0x10000];
            StatusMessage = "Clear Breakpoints";
            toolStripButton6.Text = "No Breakpoints";
            toolStripButton6.Visible = false;
            toolStripButton6.Visible = true;
            toolStripButton6.Enabled = false;
        }


        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            ListingVisible();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ListingHidden();
        }
        void ToggleListing()
        {
            if(scintillaZ80Monitor1.Visible)
                ListingHidden();
            else
                ListingVisible();
        }
        public void ShowListing()
        {
            PClabel.Text = "";
            if (addresses == null)
                addresses = new List<int>();
            if (addresses.IndexOf(z80.PC) == -1)
                addresses.Add(z80.PC);
            Disassembler.Disassemble(z80.Memory);
            Disassembler.Disassemble(0, addresses, 0, 65535, z80.Memory, true, true, true);
            //MonitorWindow.ReadOnly = false;
            scintillaZ80Monitor1.ForceText = Disassembler.Listing.ToString();
            //MonitorWindow.ReadOnly = true;
            scintillaZ80Monitor1.Visible = true;
            toolStripButton7.Visible = !scintillaZ80Monitor1.Visible;
            toolStripButton8.Visible = scintillaZ80Monitor1.Visible;
            Width += 640;
            if (Width >= 867)
                Width = 867;
            ListingIsVisible = scintillaZ80Monitor1.Visible;
            UpdateDisplay(z80);

        }
        bool ListingIsVisible = true;

        public int ContinuousSpeed = 5;
        public void Slower()
        {
            running = false;
            fast = true;
            ContinuousSpeed += 1;
            if (CtlKey)
                ContinuousSpeed = 10;
            CtlKey = false;
            if (ContinuousSpeed > 10) ContinuousSpeed = 10;
            StatusMessage = ((ContinuousSpeed == 10) ? "Slowest" : "Slower [" + (10 - ContinuousSpeed).ToString() + "]");
            z80.DoRun(false);

        }
        bool fast = false;
        public void Faster()
        {
            running = false;
            fast = true;
            ContinuousSpeed -= 1;
            if (CtlKey)
                ContinuousSpeed = 0;
            CtlKey = false;
            if (ContinuousSpeed < 0) ContinuousSpeed = 0;
            StatusMessage = ((ContinuousSpeed == 0) ? "Fastest" : "Faster [" + (10 - ContinuousSpeed).ToString() + "]");
            z80.DoRun(false);
        }

        bool CtlKey = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            CtlKey = false;
            if (e.KeyCode == Keys.ControlKey)
            {
                CtlKey = true;
                e.Handled = true;
            }

            switch(e.KeyCode)
            {
                case Keys.F5:Close(); e.Handled = true; break;
                case Keys.Oemplus: Faster(); e.Handled = true; break;
                case Keys.OemMinus: Slower(); e.Handled = true; break;
                case Keys.F9: toolStripButton5_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.F10: toolStripButton4_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.F11: Step(); e.Handled = true; break;
                case Keys.L: if (CtlKey) ToggleListing(); e.Handled = true; break;
            }
            base.OnKeyDown(e);
        }
        bool running = false;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripButton2.Visible = false;
            toolStripButton2.Visible = true;
            Hide();
            running = true;
            z80.DoRun();
        }

        private void Monitor_Load(object sender, EventArgs e)
        {
        }
    }
}
