using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ZXStudio
{
    public partial class AssemblerEditor : Form
    {
        public event EventHandler BeginCompileEventHandler;
        public event RunInEmulatorEventHandler RunInEmulatorHandler;
        public event LoadInEmulatorEventHandler LoadInEmulatorHandler;
        public event InfoEventHandler InfoEvent;
        public event LogSourceErrorEventHandler LogErrorHandler;

        bool symbolTableVisible;
        string originalText = "";
        int selectedSymbol = 0;
        int selectedLine = 0;
        string[] selectedLines;

        public AssemblerEditor()
        {
            InitializeComponent();
            toolStrip2.BackColor = ColorTranslator.FromHtml("#FF4D6082");
            toolStrip3.BackColor = ColorTranslator.FromHtml("#FFFFFFFF");
            //AssertColor(ColorPalette.TabSelectedActive.Button, ColorTranslator.FromHtml("#FF75633D"));
            //AssertColor(ColorPalette.TabSelectedActive.Text, ColorTranslator.FromHtml("#FF000000"));

            //AssertColor(ColorPalette.TabSelectedInactive.Background, ColorTranslator.FromHtml("#FF4D6082"));
            //AssertColor(ColorPalette.TabSelectedInactive.Button, ColorTranslator.FromHtml("#FFCED4DD"));
            //AssertColor(ColorPalette.TabSelectedInactive.Text, ColorTranslator.FromHtml("#FFFFFFFF"));

            //vsToolStripExtender = new VisualStudioToolStripExtender();
            //vsToolStripExtender.SetStyle(toolStrip2, version, vS2015BlueTheme);
            //vsToolStripExtender.SetStyle(toolStrip3, version, vS2015BlueTheme);
            symbolTableListView.UseCompatibleStateImageBehavior = true;
            Resize += SourceDocumentWindow_Resize;
            scintillaZ80Monitor1.UpdatePosition += ScintillaZ80Monitor1_UpdatePosition;
            scintillaZ80Monitor1.TextChanged += ScintillaZ80Monitor1_TextChanged;
            ToggleSymbolTable = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //listViewEx2.StopThread();
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        public ToolStrip ChildToolStrip
        {
            get
            {
                return toolStrip1;
            }

            set
            {
                toolStrip1 = value;
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!ToggleSymbolTable)
                ToggleSymbolTable = false;

            base.OnVisibleChanged(e);
        }
        private void ScintillaZ80Monitor1_TextChanged(object sender, EventArgs e)
        {
            Text = Path.GetFileName(FileName);
            if (scintillaZ80Monitor1.Text != originalText)
                Text = Path.GetFileName(FileName) + "*";

        }
        private void SourceDocumentWindow_Resize(object sender, EventArgs e)
        {
            try
            {
                ToggleSymbolTable = ToggleSymbolTable;
                symbolTableListView.Columns[0].Width = 65;
                symbolTableListView.Columns[1].Width = 60;
                symbolTableListView.Columns[2].Width = 65;
                symbolTableListView.Columns[3].Width = -2;
            }
            catch { }
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            ToggleSymbolTable = false;
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ToggleSymbolTable = true;
        }
        private void ScintillaZ80Monitor1_UpdatePosition(object sender, EventArgs e)
        {
            char mouseChar = scintillaZ80Monitor1.CharUnderMouse;
            string current = (scintillaZ80Monitor1.WordUnderMouse.Length > 0 ? scintillaZ80Monitor1.WordUnderMouse : mouseChar.ToString());
            //if (scintillaZ80Monitor1.CharUnderMouse != '\r')                toolStripStatusLabel1.Text = "'" + current + "'";
            int mouseLocation = scintillaZ80Monitor1.CurrentPosition;
            //toolStripStatusLabel2.Text = mouseLocation.ToString();
            int column = scintillaZ80Monitor1.Column;
            //toolStripStatusLabel4.Text = "{Line=" + (scintillaZ80Monitor1.LineNumber + 1).ToString() + ", Column=" + column.ToString() + "}";
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            scintillaZ80Monitor1.ShowMargin = !scintillaZ80Monitor1.ShowMargin;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            scintillaZ80Monitor1.ShowBreakpointsMargin = !scintillaZ80Monitor1.ShowBreakpointsMargin;
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            BeginCompileEventHandler?.Invoke(this, EventArgs.Empty);
            Compile();
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            int pos = 0;
            Symbol symbol = symbolTableListView.SelectedItems[0].Tag as Symbol;
            //if (listView1.SelectedItems[0].Index != selectedSymbol)
            {
                selectedSymbol = symbolTableListView.SelectedItems[0].Index;
                selectedLines = symbolTableListView.SelectedItems[0].SubItems[3].Text.Split(',');

            }
            if (selectedLines == null)
            {
                selectedLine = 0;
                return;
            }
            if (selectedLines.Length == 1)
                if (selectedLines[0] == "")
                {
                    selectedLine = 0;
                    return;
                }
            if (selectedLines != null)
            {
                if (symbolTableListView.SelectedItems[0].SubItems[1].Text == "Invalid")
                    scintillaZ80Monitor1.SetSelectionBackColor(true, Color.Salmon);
                else
                    scintillaZ80Monitor1.SetSelectionBackColor(true, Color.Yellow);
                int.TryParse(selectedLines[selectedLine], out pos);
                selectedLine++;
                if (selectedLine >= selectedLines.Length)
                    selectedLine = 0;


                foreach (UsedAt ua in symbol.UsedAtLines)
                {
                    if (ua.Line == pos)
                    {
                        scintillaZ80Monitor1.GotoPosition(scintillaZ80Monitor1.Lines[pos - 1].Position);
                        scintillaZ80Monitor1.SelectionStart = scintillaZ80Monitor1.Lines[pos - 1].Position + ua.Token.Col;
                        scintillaZ80Monitor1.SelectionEnd = scintillaZ80Monitor1.SelectionStart + ua.Token.Value.Length;
                        break;
                    }
                }
            }

        }
        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            RunInEmulator(true);
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            RunInEmulator(false);
        }
        private void debugInEmulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunInEmulator(true);
        }
        private void runInEmulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunInEmulator(false);
        }
        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Compile();
        }
        private void toggleLineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintillaZ80Monitor1.ShowMargin = !scintillaZ80Monitor1.ShowMargin;
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = 0;
            if (symbolTableListView.SelectedItems.Count != 1) return;
            scintillaZ80Monitor1.SetSelectionBackColor(true, Color.LightGreen);
            Symbol symbol = symbolTableListView.SelectedItems[0].Tag as Symbol;
            Token token = symbol.Token;
            selectedSymbol = symbolTableListView.SelectedItems[0].Index;
            string s = symbolTableListView.SelectedItems[0].SubItems[2].Text;
            bool result = int.TryParse(s, out pos);

            if (result)
            {
                scintillaZ80Monitor1.GotoPosition(scintillaZ80Monitor1.Lines[pos - 1].Position);
                scintillaZ80Monitor1.SelectionStart = scintillaZ80Monitor1.Lines[pos - 1].Position;// + symbol.Token.Col;
                scintillaZ80Monitor1.SelectionEnd = scintillaZ80Monitor1.SelectionStart + scintillaZ80Monitor1.Lines[pos - 1].Length;
                selectedLine = 0;
                return;
            }
            s = symbolTableListView.SelectedItems[0].SubItems[3].Text;
            string[] s1 = s.Split(',');
            if (selectedLine >= s1.Length)
                selectedLine = 0;

            result = int.TryParse(s1[selectedLine], out pos);
            selectedLine++;
            if (selectedLine >= s1.Length)
                selectedLine = 0;
            if (result)
            {
                if (symbolTableListView.SelectedItems[0].SubItems[1].Text == "Invalid")
                    scintillaZ80Monitor1.SetSelectionBackColor(true, Color.Salmon);
                scintillaZ80Monitor1.GotoPosition(scintillaZ80Monitor1.Lines[pos - 1].Position);
                scintillaZ80Monitor1.SelectionStart = scintillaZ80Monitor1.Lines[pos - 1].Position + symbol.Token.Col;
                scintillaZ80Monitor1.SelectionEnd = scintillaZ80Monitor1.SelectionStart + symbol.Token.Value.Length;
                return;
            }

        }
        private void Sfd_FileOk(object sender, CancelEventArgs e)
        {
            using (SaveFileDialog sfd = sender as SaveFileDialog)
            {
                Rename(sfd.FileName);
            }
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        string FileName;
        public void Save()
        {
            File.WriteAllText(FileName, scintillaZ80Monitor1.Text);
            InfoEvent?.Invoke(this, new InfoEventArgs("Saved " + FileName));
            originalText = scintillaZ80Monitor1.Text;
            Text = Path.GetFileName(FileName);
        }
        public void SaveAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = Path.GetDirectoryName(FileName);
                sfd.FileName = Path.GetFileName(FileName);
                sfd.FileOk += Sfd_FileOk;
                sfd.ShowDialog(this);
            }
        }

        public bool ToggleSymbolTable
        {
            get => symbolTableVisible;
            set
            {
                symbolTableVisible = value;
                if (value)
                {
                    int w = splitContainer4.Width;
                    float p2w = w * .6f;
                    splitContainer4.SplitterDistance = (int)p2w;
                    panel3.BorderStyle = BorderStyle.FixedSingle;
                    toolStrip3.Visible = false;
                    toolStrip2.Visible = true;
                }
                else
                {
                    if (splitContainer4.Width > 9)
                    {
                        splitContainer4.SplitterDistance = splitContainer4.Width - 9;
                        panel3.BorderStyle = BorderStyle.None;
                        toolStrip3.Visible = true;
                        toolStrip2.Visible = false;
                    }
                }
                symbolTableListView.Columns[3].Width = -2;

            }
        }
        public void ViewSymbolTable()
        {
            symbolTableListView.BeginUpdate();
            symbolTableListView.Items.Clear();
            List<ListViewItem> items = new List<ListViewItem>();
            foreach (Symbol symbol in SymbolTable.Labels)
            {
                if (symbol.FileName != FileName) continue;
                ListViewItem item = new ListViewItem(symbol.Name);
                if (symbol.Line > -1)
                {
                    item.SubItems.Add(symbol.Value.ToString("X4"));
                    item.SubItems.Add(symbol.Line.ToString());
                }
                else
                {
                    item.SubItems.Add("Invalid");
                    item.SubItems.Add("Invalid");
                }
                string usedatlines = "";
                string sep = "";
                foreach (UsedAt ua in symbol.UsedAtLines)
                {
                    usedatlines += sep + ua.Line.ToString();
                    sep = ", ";
                }
                item.SubItems.Add(usedatlines);
                items.Add(item);
                item.ImageIndex = symbol.Resolved ? 0 : 1;
                item.Tag = symbol;
            }
            symbolTableListView.Items.AddRange(items.ToArray());
            symbolTableListView.Columns[0].Width = 65;
            symbolTableListView.Columns[1].Width = 60;
            symbolTableListView.Columns[2].Width = 65;
            symbolTableListView.Columns[3].Width = -2;
            symbolTableListView.EndUpdate();
            selectedLine = 0;
        }
        public void Open(string fileName)
        {
            toolStripButton3.Text = "Compile " + Path.GetFileName(fileName);
            saveToolStripMenuItem.Text = "Save " + Path.GetFileName(fileName);
            saveAsToolStripMenuItem.Text = "Save " + Path.GetFileName(fileName) + " As";
            FileName = fileName;
            if (File.Exists(fileName))
                scintillaZ80Monitor1.ForceText = originalText = File.ReadAllText(fileName);
        }
        public void HighlightWord(int line, int column)
        {
            scintillaZ80Monitor1.SetSelectionBackColor(true, Color.LightSalmon);
            int pos = scintillaZ80Monitor1.Lines[line - 1].Position + column;
            string word = scintillaZ80Monitor1.GetWordFromPosition(pos);
            scintillaZ80Monitor1.GotoPosition(pos);
            if (column > 0)
            {
                scintillaZ80Monitor1.SelectionStart = pos;
                scintillaZ80Monitor1.SelectionEnd = scintillaZ80Monitor1.SelectionStart + word.Length;
            }

        }
        public void Rename(string fileName)
        {
            File.Move(FileName, fileName);
            FileName = fileName;
            File.WriteAllText(FileName, scintillaZ80Monitor1.Text);
            InfoEvent?.Invoke(this, new InfoEventArgs("Saved As" + FileName));

            Text = Path.GetFileName(FileName);
            originalText = scintillaZ80Monitor1.Text;


        }
        public bool Compile()
        {
            hexView1.Clear();

            bool result = Assembler.Assemble(scintillaZ80Monitor1, scintillaZ80Monitor1.Text);

            scintillaZ801.Text = Assembler.Listing;
            if (result)
                hexView1.Load(Assembler.Memory, Assembler.FirstAddress, Assembler.ProgramCounter);
            ViewSymbolTable();
            ToggleSymbolTable = true;
            if (result)
                LoadInEmulatorHandler?.Invoke(this, new LoadInEmulatorEventArgs(Assembler.Memory, (ushort)Assembler.EntryPoint));

            return result;

        }
        void RunInEmulator(bool debug)
        {
            BeginCompileEventHandler?.Invoke(this, EventArgs.Empty);
            if (!Compile()) return;
            RunInEmulatorHandler?.Invoke(this, new RunInEmulatorEventArgs(Assembler.Memory, (ushort)Assembler.EntryPoint, debug));

        }

        private void Editor_Load(object sender, EventArgs e)
        {
            #region Create Event Handlers
            Assembler.ErrorLogged -= Assembler_ErrorLogged;
            Assembler.ErrorLogged += Assembler_ErrorLogged;

            Assembler.AssemblyStart += Assembler_AssemblyStart;
            Assembler.AssemblyComplete -= Assembler_AssemblyComplete;
            Assembler.AssemblyComplete += Assembler_AssemblyComplete;
            LogErrorHandler += Window_LogErrorHandler;
            Z80.ReturnFromRunInIDE += Z80_ReturnFromRunInIDE;
            #endregion Create Event Handlers

            outputListView.GridLines = true;
            outputListView.HeaderColor = SystemColors.ButtonFace;
            outputListView.Columns[0].Width = 120;
            outputListView.Columns[1].Width = 150;
            outputListView.Columns[2].Width = 100;
            outputListView.Columns[3].Width = 100;
            outputListView.Columns[4].Width = -2;

        }

        private void Z80_ReturnFromRunInIDE(object sender, EventArgs e)
        {
            infoLabel.Text = "Run in IDE Complete " + Z80.RunInEmulatorTSTates.ToString("#,##0") + " tstates in " + (((float)Z80.RunInIDEStopwatch.ElapsedMicroseconds / 1000000f)).ToString("#,##0.000000") + " seconds";
            System.Media.SystemSounds.Exclamation.Play();
            //Show();
            //Focus();
            //BringToFront();
            //SendToBack();
            //Globals.EnsureVisible(Handle);
        }

        private void Window_LogErrorHandler(object sender, LogErrorEventArgs e)
        {
            LogOutput(sender, e.Message, e.Column, e.Line);
        }


        private void Assembler_AssemblyStart(object sender, AsseblyStartEventArgs e)
        {
            Clear();
            LogErrorHandler?.Invoke(scintillaZ80Monitor1, new LogErrorEventArgs(null, "Assembly Started", 0, 0));
            infoLabel.Text = "Assembly Started";
        }
        private void Assembler_ErrorLogged(object sender, LogErrorEventArgs e)
        {

            LogErrorHandler?.Invoke(sender, e);
            infoLabel.Text = e.Message;
        }
        private void Assembler_AssemblyComplete(object sender, AsseblyCompleteEventArgs e)
        {
            infoLabel.Text = (e.Success ? "Assembly Complete" : "Assembly Failed");
            LogErrorHandler?.Invoke(scintillaZ80Monitor1, new LogErrorEventArgs(null, (e.Success ? "Assembly Complete" : "Assembly Failed"), 0, 0));
        }

        public void LogOutput(object sender, string message, int column, int line, Token token = null)
        {
            string file = "N/A";
            ListViewItem lvi = outputListView.Items.Add(DateTime.Now.ToString());
            switch (sender.GetType().Name)
            {
                case "ScintillaZ80Monitor":
                    ScintillaZ80Monitor sourceDocumentWindow = sender as ScintillaZ80Monitor;
                    file = Path.GetFileName(sourceDocumentWindow.FileName);
                    lvi.Tag = sourceDocumentWindow.FileName;
                    break;
            }
            lvi.SubItems.Add(file);
            if (line > 0)
            {
                lvi.SubItems.Add(line.ToString());
                lvi.SubItems.Add(column.ToString());
            }
            else
            {
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }
            lvi.SubItems.Add(message);

            outputListView.Columns[0].Width = 120;
            outputListView.Columns[1].Width = 150;
            outputListView.Columns[2].Width = 100;
            outputListView.Columns[3].Width = 100;
            outputListView.Columns[4].Width = -2;
        }

        public void Clear()
        {
            outputListView.Items.Clear();
        }
        public event SelectErrorEventHandler SelectError;
        private void listViewEx1_DoubleClick(object sender, EventArgs e)
        {

            if (outputListView.SelectedItems.Count != 1) return;
            //if (listViewEx1.SelectedItems[0].SubItems[2].Text.Trim().Length == 0) return;
            string fileName = outputListView.SelectedItems[0].Tag as string;
            int line = 0;
            int column = 0;
            int.TryParse(outputListView.SelectedItems[0].SubItems[2].Text, out line);
            int.TryParse(outputListView.SelectedItems[0].SubItems[3].Text, out column);
            SelectError?.Invoke(this, new SelectErrorEventArgs(fileName, line, column));

        }


        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            using (OpenDialog ofd = new OpenDialog())
            {
                ofd.ShowDialog(this);

            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.ShowDialog(this);

            }
        }
    }
}

