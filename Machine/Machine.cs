using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ZXCassetteDeck;

namespace ZXStudio
{
    public delegate void LoadedFileEventHandler(string fileName);
    public delegate void ClosedFileEventHandler();
    
    public class Machine : Z80
    {
        CassetteController cassetteDeck;

        public string AutoFileName = "";
        int TotalScreenHeight;
        int TotalScreenWidth;
        int screenLength;
        Bitmap bufferedScreenImage;
        Rectangle bufferedScreenRectangle;
        Keyboard keyboard;
        int lastSoundOut;
        Sound Sound;

        public Machine() : base()
        {
            AllowDrop = true;
            DoubleBuffered = true;
            TotalScreenHeight = 296;
            TotalScreenWidth = 352;
            screenLength = TotalScreenWidth * TotalScreenHeight;
            InitialisePalette();
            InitialiseFlash();
            InitializeScreenMap();
            Initialise();
            videoBitmap = new int[screenLength];
            Sound = new Sound(Handle, 32, 2, 44100);

            ClientSize = new System.Drawing.Size(TotalScreenWidth, TotalScreenHeight + 25 + 22);

            bufferedScreenRectangle = new Rectangle(0, 0, 352, 296);
            bufferedScreenImage = new Bitmap(352, 296, PixelFormat.Format32bppPArgb);

            //ToggleCassette += Machine_ToggleCassette;
            cassetteDeck = null;
            cassetteDeck = new CassetteController();

            CassetteController.OutTone += CassetteController_OutTone;
            CassetteController.Progress += CassetteController_Progress;
            CassetteController.Loaded += CassetteController_Loaded;
            CassetteController.EndFileLoad += CassetteController_EndFileLoad;
            CassetteController.FileClosed += CassetteController_FileClosed;
            CassetteController.EndBlock += CassetteController_EndBlock;

            keyboard = new Keyboard();


            OpenFileEventHandler += Machine_OpenFileEventHandler;
            ToggleCassettDeckEventHandler += Machine_ToggleCassettDeckEventHandler;
            ResetEventHandler += Machine_ResetEventHandler;
            DragEnter += Machine_DragEnter;
            DragDrop += Machine_DragDrop;

            string[] characterset = Properties.Resources.CharacterSet.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            CommandList = new List<string>();
            foreach (string s in characterset)
            {
                if (s.StartsWith("Code")) continue;

                string[] row = s.Split('\t');
                CommandList.Add(row[1]);
            }
            LastOut= 7;

        }

        public new void DoRun()
        {
            if (AutoFileName.Length > 0)
                AutoLoad(AutoFileName);
            AutoFileName = "";
            base.DoRun();
        }
        bool isturbo = false;
        public override void ResetComplete()
        {
            if(!isturbo)
                Turbo = false;
            base.ResetComplete();
        }

        private void Machine_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (files.Length > 0) {
                    string ext = Path.GetExtension(files[0]);
                    if (ext.ToUpper() == ".ZIP")
                    {
                        ZipArchive archive = ZipFile.OpenRead(files[0]);
                        ext = Path.GetExtension(archive.Entries[0].Name);
                    }

                    switch (ext.ToUpper())
                    {
                        case ".Z80":
                        case ".SNA":
                        case ".TAP":
                        case ".TZX":
                            e.Effect = DragDropEffects.Copy;
                            break;
                    }
                }
            }
        }

        private void Machine_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                AutoLoad(files[0]);
            }
        }

        private void Machine_ResetEventHandler(object sender, EventArgs e)
        {
            Reset();
        }

        private void Machine_ToggleCassettDeckEventHandler(object sender, EventArgs e)
        {
            DoToggleCassette();
        }

        private void Machine_OpenFileEventHandler(object sender, EventArgs e)
        {
            OpenFile();
        }
        #region Cassette Responses
        private void Machine_ToggleCassette(object sender, EventArgs e)
        {
            DoToggleCassette();
        }

        void DoToggleCassette()
        {
            cassetteDeck.deck.Visible = !cassetteDeck.deck.Visible;
            //toggleCassetteToolStripMenuItem.Checked = cassetteDeck.deck.Visible;

        }

        bool TAPFileLoaded;
        string FileName;
        public string TapeBlockProgress { get; set; }
        public string TapeOverallProgress { get; set; }
        public string TapeCounter { get; set; }
        public string TapeBlock { get; set; }
        public bool IsTZXPlaying { get; set; }

        private void CassetteController_Progress(TZXFile tzxFile, int blocklength, int tapeCounter, int blockCounter, float overallProgress, float blockProgress, string block, ITZXDataBlock tZXDataBlock)
        {
            string op = overallProgress.ToString("##0.00") + "%";
            string bp = blockProgress.ToString("##0.00") + "%";
            TapeOverallProgress = "Tape Progress " + op.PadLeft(7) + "/ " + bp.PadLeft(7).PadLeft(7);
            TapeCounter = tapeCounter.ToString("#,##0").PadLeft(6) + " / " + blockCounter.ToString("#,##0").PadLeft(6);
            TapeBlock = block;
        }
        private void CassetteController_OutTone(int tone)
        {
            EarBit = tone;
        }

        private void CassetteController_Loaded()
        {
            Loaded = true;
        }

        private void CassetteController_EndBlock(ITZXBlock block, int blockIndex)
        {
            LoadedFile?.Invoke(FileName);
            Loaded = true;
        }

        private void CassetteController_FileClosed()
        {
            FileClosed();
        }

        private void CassetteController_EndFileLoad(string fileName, TZXFile tzxFile)
        {
            FileLoaded(fileName);
        }

        private void FileClosed()
        {
            TAPFileLoaded = false;
            //FileName = "";

            ClosedFile?.Invoke();
        }
        void FileLoaded(string fileName)
        {
            LoadedFile?.Invoke(fileName);

        }
        public event LoadedFileEventHandler LoadedFile;
        public event ClosedFileEventHandler ClosedFile;
        #endregion Cassette Responses



        protected override void OnVisibleChanged(EventArgs e)
        {
            Parent.ClientSize = new Size((int)(352 * base.Multiplier), (int)(296 * base.Multiplier));
            base.OnVisibleChanged(e);

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            keyboard.ProcessKey(true, (int)e.KeyCode, (int)e.Modifiers);
            // if (e.KeyCode == Keys.D2) { Logging = true; e.Handled = true; return; }
            if (e.Alt)
            switch (e.KeyCode)
                {
                    // Add functions to Reset
                    // Supress CTRL+R
                    case Keys.R:
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    // Supress CTRL+M
                    case Keys.M:
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    // Supress CTRL+A
                    case Keys.A:
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    // Supress CTRL+C
                    case Keys.C:
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    // Supress CTRL+L
                    case Keys.L:
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    case Keys.T:
                        Turbo = !Turbo;
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    case Keys.PageUp:
                        Bigger();
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    case Keys.PageDown:
                        Smaller();
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    case Keys.Insert:
                        frameratecounter++;
                        if (frameratecounter > 5)
                            frameratecounter = 5;
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                    case Keys.Delete:
                        frameratecounter--;
                        if (frameratecounter < 0)
                            frameratecounter = 0;
                        e.Handled = true;
                        keyboard.Reset();
                        break;
                }
            base.OnKeyDown(e);
        }

        int p = 0;
       List< string> Program;
        void ViewProgram()
        {
            Program = new List<string>();
            p = 0;

            string program = @"10 FOR F = 0 TO 10 : " +
                @"PRINT ""HELLO"" : " +
                @"NEXT F";
            LoadProgram(program);

            Console.WriteLine("Program Area");
            int prog = Memory[SystemVariables.PROG] | (Memory[SystemVariables.PROG + 1] << 8);
            int linenumber;
            int length;
            while (true)
            {
                if (Memory[prog] == 0x80)
                {
                    Console.WriteLine("End");
                    break;
                }
                linenumber = Memory[prog++] << 256 | Memory[prog++];
                length = Memory[prog++] << 256 | Memory[prog++];
                Console.Write(linenumber+ ">"+length);
                for (int i = 0; i < length-1; i++)
                    Console.Write(" (" + Memory[prog].ToString("X2") + ", " + CommandList[Memory[prog++]]+") ");
                // Skip CR
                prog++;
                Console.WriteLine();
            }
            
        }
        new void Reset()
        {
            bool v = cassetteDeck.deck.Visible;
            CassetteController.Pause();
            CassetteController.Rewind();
            cassetteDeck.deck.Visible = v;
            TapeOverallProgress = "";
            base.Reset();

        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            keyboard.ProcessKey(false, (int)e.KeyCode, (int)e.Modifiers);
            base.OnKeyUp(e);
        }
        bool paint = false;
        public override void LoadBlockBegin()
        {
            Console.WriteLine("Load Block Begin");
            base.LoadBlockBegin();
        }
        public override void LoadBlockComplete()
        {
            Console.WriteLine("Load Block Complete");
            base.LoadBlockComplete();
        }
        public override void FrameComplete()
        {
            Flash++;
            LastCycle = TotalCycles;
            if (Turbo)
            {
                if (framecounter == 0)
                    Invalidate();
                framecounter++;
                if (framecounter > frameratecounter * 10)
                    framecounter = 0;
            }
            else
            {
                Sound.Play();
                Sound.Reset();
                Invalidate();
            }
            if (!Turbo)
                while (stopwatch.ElapsedMilliseconds - frametime < TimerInterval)
                    System.Threading.Thread.SpinWait(10);
            frametime = stopwatch.ElapsedMilliseconds;
            base.FrameComplete();
        }
        int framecounter = 0;
        bool autoload = false;
        public void AutoLoad(string filename)
        {
            autoload = true;
            isturbo = Turbo == true;
            Turbo = true;
            Reset();
            string type = Path.GetExtension(filename);
            Properties.Settings.Default.Save();
            cassetteDeck.deck.Visible = false;
            cassetteDeck.FileClose();
            OpenFile(filename);
            LoadCommand(@"LOAD "" """);
        }

        public override void ProcessInterrupt()
        {
            if (!PerformingReset)
            {
                if (Command != null)
                {
                    if (NextCommand < Command.Length)
                    {
                        int kcur = Memory[SystemVariables.KCUR] | (Memory[SystemVariables.KCUR + 1] << 8);
                        if (NextCommand > 0 && (Memory[SystemVariables.KCUR] | (Memory[SystemVariables.KCUR + 1] << 8)) == 0x5CCC)
                            Command = null;
                        else
                        {

                            if ((this[SystemVariables.FLAGS] & 0x20) == 0)
                            {
                                this[SystemVariables.LASTK] = Command[NextCommand++];
                                this[SystemVariables.FLAGS] = this[SystemVariables.FLAGS] | 0x20;
                            }
                        }
                    }
                    else
                    {
                        Command = null;
                    }
                

                }
            }

            base.ProcessInterrupt();
        }
        public override void AfterExecute()
        {
            CurrentTapeState = null;
            if (CassetteController.IsPlaying && CassetteController.FileLoaded)
            {
                if (!CassetteController.LoadComplete)
                {
                    CurrentTapeState = cassetteDeck.deck.ProgressText;
                    cassetteDeck.NextPulse(ExecutedCycles);
                }
            }

            if (Turbo)
            {
                if (framecounter == 0)
                    DrawBytes(TotalCycles);
            }
            else
            {
                DrawBytes(TotalCycles);
                Sound.UpdateAudio(ExecutedCycles);

            }
            base.AfterExecute();
        }

        #region Screen Handling
        public int Pixel = 0;
        public int LastCycle;
        int scr = 0;
        int att = 0;
        int ink;
        int paper;
        int screenByte = 0;
        int attributeByte = 0;
        bool Flashing;
        int lastpaper;
        int lastOut;
        public int LastOut
        {
            get { return lastOut; }
            set
            {
                if (value == lastOut) return;
                lastOut = value;
                LastBorder = Palette[lastOut];
            }
        }

        int borderstart = 0;
        public ScreenMap[] ScreenMap = new ScreenMap[69888];
        int[] videoBitmap;
        int[] inkColours = new int[256];
        int[] paperColours = new int[256];
        int[] Palette =            {
                            //              RGB
                            // Normal               
            0x000000,       // Black        000    
            0x0000CC,       // Blue         001
            0xCC0000,       // Red          100
            0xCC00CC,       // Magenta      101
            0x00CC00,       // Green        010
            0x00CCCC,       // Cyan         011
            0xCCCC00,       // Yellow       110
            0xCCCCCC,       // White        111

                            // Bright
            0x000000,       // Black        000    
            0x0000FF,       // Blue         001
            0xFF0000,       // Red          100
            0xFF00FF,       // Magenta      101
            0x00FF00,       // Green        010
            0x00FFFF,       // Cyan         011
            0xFFFF00,       // Yellow       110
            0xFFFFFF        // White        111
        };

        void InitialiseFlash()
        {
            for (int idx = 0; idx < 256; idx++)
            {
                int inkindex = (idx & 0x07) | ((idx & 0x40) != 0 ? 0x08 : 0x00);
                int paperindex = ((idx >> 3) & 0x07) | ((idx & 0x40) != 0 ? 0x08 : 0x00);

                if (idx < 128)
                {
                    inkColours[idx] = Palette[inkindex];
                    paperColours[idx] = Palette[paperindex];
                }
                else
                {
                    inkColours[idx] = Palette[paperindex];
                    paperColours[idx] = Palette[inkindex];
                }
            }
        }

        void InitialisePalette()
        {
            for (int idx = 0; idx < 256; idx++)
            {
                int inkindex = (idx & 0x07) | ((idx & 0x40) != 0 ? 0x08 : 0x00);
                int paperindex = ((idx >> 3) & 0x07) | ((idx & 0x40) != 0 ? 0x08 : 0x00);
                if (idx < 128)
                {
                    inkColours[idx] = Palette[inkindex];
                    paperColours[idx] = Palette[paperindex];
                }
                else
                {
                    inkColours[idx] = Palette[paperindex];
                    paperColours[idx] = Palette[inkindex];
                }
            }
        }
        void InitializeScreenMap()
        {
            int pixel = 0;
            for (int i = 0; i < 69888; i++)
            {
                ScreenMap[i] = new ScreenMap
                {
                    Pixel = -9,
                    Screen = -9
                };
            }
            /*
            After an interrupt occurs, 64 line times (14336 T states; see below for exact timings) pass before the first byte
            of the screen (16384) is displayed.
            At least the last 48 of these are actual border - lines;
            the others may be either border or vertical retrace.
            */
            // Find the Cycle where the border starts to draw
            // 14336 = Cycle before first screen byte, therefore 14340 = first screen byte
            // No go back 24 Cycles to left border start
            // Then go back number of Cycles per line (224) * 48 
            // 
            borderstart = 14336 + 4 - 24 - (224 * 48);

            for (int i = borderstart; i < 14340 - 24; i++)
            {
                // Left border = 24 + screen = 128 + right border = 24 (total 176)
                for (int j = 0; j < 176; j += 4)
                {
                    ScreenMap[i].Cycle = i;
                    ScreenMap[i + 1].Cycle = i + 1;
                    ScreenMap[i + 2].Cycle = i + 2;
                    ScreenMap[i + 3].Cycle = i + 3;

                    ScreenMap[i].Pixel = pixel;
                    ScreenMap[i + 1].Pixel = pixel;
                    ScreenMap[i + 2].Pixel = pixel;
                    ScreenMap[i + 3].Pixel = pixel;
                    pixel += 8;
                    ScreenMap[i].Screen = -1;
                    ScreenMap[i + 1].Screen = -1;
                    ScreenMap[i + 2].Screen = -1;
                    ScreenMap[i + 3].Screen = -1;
                    i += 4;

                }
                i += 47;    // 224 - 176 Horizontal retrace
            }
            int mainscreenend = 14316 + (192 * 224);
            int row = 0;
            int col = 0;
            int Col = 0;
            attributeByte = 0;
            for (int i = 14316; i < mainscreenend; i++)
            {
                // Left border = 24 + screen = 128 + right border = 24 (total 176)
                // Left border = 24 
                for (int j = 0; j < 24; j += 4)
                {
                    ScreenMap[i].Cycle = i;
                    ScreenMap[i + 1].Cycle = i + 1;
                    ScreenMap[i + 2].Cycle = i + 2;
                    ScreenMap[i + 3].Cycle = i + 3;

                    ScreenMap[i].Pixel = pixel;
                    ScreenMap[i + 1].Pixel = pixel;
                    ScreenMap[i + 2].Pixel = pixel;
                    ScreenMap[i + 3].Pixel = pixel;
                    pixel += 8;
                    ScreenMap[i].Screen = -2;
                    ScreenMap[i + 1].Screen = -2;
                    ScreenMap[i + 2].Screen = -2;
                    ScreenMap[i + 3].Screen = -2;

                    i += 4;
                }
                // + screen = 128 
                for (int j = 0; j < 128; j += 4)
                {
                    if (j % 4 == 0)
                    {
                        /*
                                           |         |              |              |          
                            +----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
                            | 15 | 14 | 13 | 12 | 11 | 10 |  9 |  8 |  7 |  6 |  5 |  4 |  3 |  2 |  1 |  0 |
                            +----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
                            |  0 |  1 |  0 | Y7 | Y6 | Y2 | Y1 | Y0 | Y5 | Y4 | Y3 | X4 | X3 | X2 | X1 | X0 |
                            +----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
                                           |         |              |              |          

                        y7,y6 and y5,y4,y3 bits represent the row number from 0-23 inclusive 
                        y2,y1,y0 bits represent the character line (0-7 inclusive).

                        x4,x3,x2,x1,x0 bits represents the column number from 0-31 

                            ; Get screen address
                            ; B = Y pixel position
                            ; C = X pixel position
                            ; Returns address in HL
                            LD A,B                  ; Calculate Y2,Y1,Y0
                            AND %00000111		    ; Mask out unwanted bits
                            OR %01000000		    ; Set base address of screen
                            LD H,A                  ; Store in H

                            LD A,B                  ; Calculate Y7,Y6
                            RRA                     ; Shift to position
                            RRA
                            RRA
                            AND %00011000		    ; Mask out unwanted bits
                            OR H                    ; OR with Y2,Y1,Y0
                            LD H,A                  ; Store in H

                            LD A,B                  ; Calculate Y5,Y4,Y3
                            RLA	                    ; Shift to position
                            RLA
                            AND %11100000		    ; Mask out unwanted bits
                            LD L,A                  ; Store in L

                            LD A,C                  ; Calculate X4,X3,X2,X1,X0
                            RRA                     ; Shift into position
                            RRA
                            RRA
                            AND %00011111		    ; Mask out unwanted bits
                            OR L                    ; OR with Y5,Y4,Y3
                            LD L,A                  ; Store in L
                            RET
                         */

                        // Calculate Y2,Y1,Y0
                        int H = row & 0x07;
                        // Calculate Y7,Y6
                        H |= ((row >> 3) & 0x18);

                        // Calculate Y5,Y4,Y3
                        int L = (row << 2) & 0xe0;

                        // Calculate X4, X3, X2, X1, X0
                        L |= (col >> 3) & 0x1f;

                        screenByte = 0x4000 | ((H << 8) | L);

                        col += 8;
                    }
                    /*
                        Get Attribute address
                        Input: HL = address of screen byte
                        ld A,H
                        rrca
                        rrca
                        rrca
                        and #03
                        or #58
                        ld H,A
                        Output; HL = address of attribute
                      */
                    attributeByte = ((((screenByte >> 11) & 0x03) | 0x58) << 8) | (screenByte & 0xff);
                    ScreenMap[i].Cycle = i;
                    ScreenMap[i + 1].Cycle = i + 1;
                    ScreenMap[i + 2].Cycle = i + 2;
                    ScreenMap[i + 3].Cycle = i + 3;

                    ScreenMap[i].Pixel = pixel;
                    ScreenMap[i + 1].Pixel = pixel;
                    ScreenMap[i + 2].Pixel = pixel;
                    ScreenMap[i + 3].Pixel = pixel;
                    pixel += 8;
                    ScreenMap[i].Attribute = attributeByte;
                    ScreenMap[i + 1].Attribute = attributeByte;
                    ScreenMap[i + 2].Attribute = attributeByte;
                    ScreenMap[i + 3].Attribute = attributeByte;

                    ScreenMap[i].Screen = screenByte;
                    ScreenMap[i + 1].Screen = screenByte;
                    ScreenMap[i + 2].Screen = screenByte;
                    ScreenMap[i + 3].Screen = screenByte;

                    ScreenMap[i].Row = row;
                    ScreenMap[i + 1].Row = row;
                    ScreenMap[i + 2].Row = row;
                    ScreenMap[i + 3].Row = row;
                    ScreenMap[i].Col = Col;
                    ScreenMap[i + 1].Col = Col;
                    ScreenMap[i + 2].Col = Col;
                    ScreenMap[i + 3].Col = Col;
                    i += 4;
                    Col++;
                }
                Col = 0;
                row++;
                // + right border = 24 (total 176)
                for (int j = 0; j < 24; j += 4)
                {
                    ScreenMap[i].Cycle = i;
                    ScreenMap[i + 1].Cycle = i + 1;
                    ScreenMap[i + 2].Cycle = i + 2;
                    ScreenMap[i + 3].Cycle = i + 3;

                    ScreenMap[i].Pixel = pixel;
                    ScreenMap[i + 1].Pixel = pixel;
                    ScreenMap[i + 2].Pixel = pixel;
                    ScreenMap[i + 3].Pixel = pixel;
                    pixel += 8;
                    ScreenMap[i].Screen = -3;
                    ScreenMap[i + 1].Screen = -3;
                    ScreenMap[i + 2].Screen = -3;
                    ScreenMap[i + 3].Screen = -3;
                    i += 4;
                }

                i += 47;    // 224 - 176 Horizontal retrace
            }
            for (int i = mainscreenend; i < 69888 - 24; i++)
            {
                // Left border = 24 + screen = 128 + right border = 24 (total 176)
                for (int j = 0; j < 176; j += 4)
                {
                    ScreenMap[i].Cycle = i;
                    ScreenMap[i + 1].Cycle = i + 1;
                    ScreenMap[i + 2].Cycle = i + 2;
                    ScreenMap[i + 3].Cycle = i + 3;

                    ScreenMap[i].Pixel = pixel;
                    ScreenMap[i + 1].Pixel = pixel;
                    ScreenMap[i + 2].Pixel = pixel;
                    ScreenMap[i + 3].Pixel = pixel;
                    pixel += 8;
                    ScreenMap[i].Screen = -4;
                    ScreenMap[i + 1].Screen = -4;
                    ScreenMap[i + 2].Screen = -4;
                    ScreenMap[i + 3].Screen = -4;
                    i += 4;

                }
                i += 47;    // 224 - 176 Horizontal retrace
            }

        }

        public void DrawBytes(int totalCycles)
        {

            int outputbytes = (totalCycles - LastCycle) / 4;
            for (int i = 0; i < outputbytes; i++)
            {
                ScreenMap sm = ScreenMap[LastCycle];
                Pixel = sm.Pixel;
                scr = sm.Screen;
                att = sm.Attribute;
                switch (scr)
                {
                    case -9:
                        break;
                    case -1:
                    case -2:
                    case -3:
                    case -4:
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        videoBitmap[Pixel++] = LastBorder;
                        break;
                    default:
                        screenByte = Memory[scr];
                        attributeByte = Memory[att];
                        if (Flashing)
                            attributeByte &= 0xff;
                        else
                            attributeByte &= 0x7f;

                        ink = inkColours[attributeByte];
                        paper = paperColours[attributeByte];
                        lastpaper = paper;
                        if ((screenByte & 0x80) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x40) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x20) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x10) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x08) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x04) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x02) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        if ((screenByte & 0x01) != 0) videoBitmap[Pixel++] = ink; else videoBitmap[Pixel++] = paper;
                        //Pixel1 += 8;
                        break;

                }
                LastCycle += 4;
            }
        }
        float Multiplier
        {
            get
            {
                return (base.Multiplier + base.Multiplier) / 2;
            }
        }

        public int Flash;
        //static string fontname = "OCR A Extended";
        static string fontname = "Consolas";
        static float fontsize = 8f;
        static float fontsize1 = 10f;
        static float fontsize2 = 8f;
        static FontStyle fontbold = FontStyle.Bold;
        Brush TimedMessageBrush = Brushes.Green;
        Brush InfoBrush = Brushes.Navy;
        Font Infofont = new Font(fontname, 11f, fontbold);


        public string CurrentTapeState;
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Flash > 16)
            {
                Flashing = !Flashing;
                Flash = 0;
            }

            BitmapData bitmapData = bufferedScreenImage.LockBits(bufferedScreenRectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            Marshal.Copy(videoBitmap, 0, bitmapData.Scan0, screenLength);
            bufferedScreenImage.UnlockBits(bitmapData);
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            e.Graphics.DrawImage(bufferedScreenImage, 0, 0, Width, Height);

            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            if (CurrentTapeState != null && !cassetteDeck.deck.Visible)
            {
                SizeF sizef = e.Graphics.MeasureString("X", Consolas);                
                e.Graphics.DrawString(TapeBlock, Consolas, OSDBrush, 0, Height - (1 * sizef.Height));
                e.Graphics.DrawString(TapeOverallProgress, Consolas, OSDBrush, 0, Height - (2 * sizef.Height));
            }

            base.OnPaint(e);
        }

        bool resizing;
        void Bigger()
        {
            base.Multiplier += .5f;
            if (base.Multiplier > 3f)
                base.Multiplier = 3f;

            resizing = true;
            resizing = false;
            //TimedMessage = "Size: " + Multiplier.ToString();
            Parent.ClientSize = new Size((int)(352 * base.Multiplier), (int)(296 * base.Multiplier));
        }
        void Smaller()
        {
            base.Multiplier -= .5f;
            if (base.Multiplier < 1f)
                base.Multiplier = 1f;
            //if (Multiplier == 1f)                smallerToolStripMenuItem.Enabled = false;

            resizing = true;
            //TimedMessage = "Size: " + Multiplier.ToString();
            Parent.ClientSize = new Size((int)(352 * base.Multiplier), (int)(296 * base.Multiplier));
        }


        #endregion Screen Handling

        #region ULA Implementation
        public override void PokeByte(int address, int value)
        {
            if ((address & 0xc000) == 0x4000) TotalCycles += contendeMemory[TotalCycles];
            TotalCycles += 3;
            if (address >= 0x4000)
                Memory[address] = (byte)value;

            return;
        }
        public override int PeekByte(int address)
        {
            if ((address & 0xc000) == 0x4000) TotalCycles += contendeMemory[TotalCycles];
            TotalCycles += 3;
            return Memory[address];
        }
        public override int ReadByte(int address)
        {
            return Memory[address];
        }
        public override int FetchOpcode()
        {
            if ((PC & 0xc000) == 0x4000) TotalCycles += contendeMemory[TotalCycles];
            TotalCycles += 4;
            R++;
            return Memory[PC++];
        }
        int EarBit;
        public override int ReadPort(int port)
        {
            int result = keyboard.ReadKeyboard(port);
            AddCycles(port, 1);
            if ((port & 0x01) == 0)
            {
                TotalCycles += contendeMemory[TotalCycles];
                TotalCycles += 2;
                if (CassetteController.IsPlaying)
                {
                    if (EarBit == 0)
                        result &= ~(TAPE_BIT);    //reset is EAR ON
                    else
                        result |= (TAPE_BIT); //set is EAR Off
                }
            }
            else
            {
                AddCycles(port, 2);
                if ((port & 0xc000) == 0x4000)
                    TotalCycles += contendeMemory[TotalCycles];
            }
            if ((port & 0xFF) != 0xFE)
            {
                int index = TotalCycles;
                if (index < startcontendedMemory || index > endcontendedMemory)
                    result = 0xFF;
                else
                {
                    if (floatingbus[index] > 0)
                        result = this[floatingbus[index]];
                    else
                        result = 0xFF;
                }
            }



            ++TotalCycles;
            return result;


        }
        public override void WritePort(int port, int value)
        {
            if ((port & 1) == 0)
            {
                //z80.TStates += z80.DelayTstates[cpu.tstates];
                LastOut = value & 0x07;

                int num = value & 0x10 | EarBit;
                if (num != lastSoundOut)
                {
                    if (num == 0)
                    {
                        Sound.soundOut = 0f;
                    }
                    else
                    {
                        Sound.soundOut = 0.5f;
                    }
                    if ((value & 8) == 0)
                    {
                        Sound.soundOut += 0.2f;
                    }
                    lastSoundOut = num;
                }
                //if ((value & 0x10) == 0x10)
                //    sound1.BeeperVal = 0x9f;
                //else
                //    sound.BeeperVal = 0x80;

            }
            AddCycles(port, 1);
            if ((port & 1) == 0)
            {
                //this.lastFEOut = value;
                TotalCycles += contendeMemory[TotalCycles];
                //this.UpdateScreenBuffer(this.cpu.TotalCycles);
                BorderColour = value & 7;
                TotalCycles += 3;
            }
            else
            {
                AddCycles(port, 3);
            }
            if (port == 48955)
            {
            }
            if (port != 65339)
                return;
            //if ((port & 1) == 0)
            //{
            //    //z80.TStates += z80.DelayTstates[cpu.tstates];
            //    Video.LastOut = value & 0x07;
            //    if (!Mute)
            //    {
            //        int beep = value & (EAR);
            //        if (EarBit == 0)
            //        {
            //            beep &= ~(TAPE_BIT);    //reset is EAR ON
            //        }
            //        else
            //        {
            //            beep |= (TAPE_BIT); //set is EAR Off
            //        }

            //        if (beep != lastSoundOut)
            //        {
            //            if (beep == 0)
            //            {
            //                sound.soundOut = 0f;
            //            }
            //            else
            //            {
            //                sound.soundOut = 0.5f;
            //            }
            //            if ((value & 8) == 0)
            //            {
            //                sound.soundOut += 0.2f;
            //            }
            //            lastSoundOut = beep;
            //        }
            //    }

            //}

        }
        public override void AddCycles(int address, int count)
        {
            if ((address & 0xc000) == 0x4000)
            {
                for (int i = 0; i < count; i++)
                {
                    TotalCycles += contendeMemory[TotalCycles];
                    TotalCycles += 1;
                }
            }
            else
                TotalCycles += count;
        }
        const int TAPE_BIT = 0x40;
        #endregion ULA Implementation

        #region Initialisation
        protected int[] floatingbus = new int[0x11512];
        protected int[] contendeMemory = new int[0x11512];
        protected int startcontendedMemory;
        protected int endcontendedMemory;
        int screenheight = 0xc0;
        int TStatesPerScanline = 0xe0;
        int framelength = 69888;

        public void Initialise()
        {
            Load(Properties.Resources._48, 0);
            startcontendedMemory = 14336;
            endcontendedMemory = startcontendedMemory + (screenheight * TStatesPerScanline);
            //tstatetodisplay = new int[framelength];
            InitializeFloatingBus();
            InitialiseContendedMemory();
            CurrentFrameRate = 50;
        }
        void InitialiseContendedMemory()
        {
            for (int x = 14335; x < 57343; x += 224)
            {
                for (int y = 0; y < 128; y += 8)
                {
                    int offset = x + y;
                    contendeMemory[offset++] = 6;
                    contendeMemory[offset++] = 5;
                    contendeMemory[offset++] = 4;
                    contendeMemory[offset++] = 3;
                    contendeMemory[offset++] = 2;
                    contendeMemory[offset++] = 1;
                    contendeMemory[offset++] = 0;
                    contendeMemory[offset++] = 0;
                }
            }
        }

        void InitializeFloatingBus()
        {
            int row = 0;
            for (int x = 14338; x < 57346; x++)
            {
                ushort screen = ScreenTables.ScreenLine[row];
                ushort attr = ScreenTables.AttributeLine[row];
                for (int y = 0; y < 128; y += 8)
                {
                    floatingbus[x] = screen++;
                    floatingbus[x + 1] = attr++;
                    floatingbus[x + 2] = screen++;
                    floatingbus[x + 3] = attr++; ;
                    x += 8;
                }
                row++;
                x += 95;
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 14338; i < 57346; i++)
            {
                sb.Append(i.ToString().PadRight(8) + floatingbus[i].ToString() + Environment.NewLine);
            }
            System.IO.File.WriteAllText(@"D:\FloatingBus2.txt", sb.ToString());
        }
        #endregion Initialisation

        #region TAP File handler
        bool Loaded;
        public override void CheckLoading()
        {
            if (CassetteController.FileLoaded)
            {
                CassetteController.LoadRequest();
                return;
            }
            if (!TAPFileLoaded) return;
            if (LoadTapFile()) AF_ |= 0x40; else AF_ |= 0xBE;
            // Simulate EX AF,AF’
            int af = AF;
            AF = AF_;
            AF_ = af;
            // Execute a return using the ROM
            // First RET is found at ROM address 82 (end of THE 'MASKABLE INTERRUPT' ROUTINE)
            PC = 0x52;

            Loaded = true;
            base.CheckLoading();
        }
        bool LoadTapFile()
        {
            TAPBlock block = currentTAPFile.NextBlock;
            if (!block.IsValid)
                return false;
            // Valid block type is in A'
            if (block.ID != (AF_ >> 8)) return true;
            foreach (byte b in block.Data)
            {
                Memory[IX++] = b;
            }
            DE = 0;
            return true;
        }

        string Filename;
        TAPFile currentTAPFile;
        private void OpenFile()
        {
            OpenFile(this, EventArgs.Empty);
        }
        private void OpenFile(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "TAP Files (*.tap)|*.tap| Z80 files (*.z80)|*.z80|All files (*.*)|*.*";
                ofd.FilterIndex = 3;
                ofd.RestoreDirectory = true;

                ofd.FileOk += Ofd_FileOk;
                ofd.ShowDialog(this);
            }
        }

        private void Ofd_FileOk(object sender, CancelEventArgs e)
        {
            using (OpenFileDialog fd = sender as OpenFileDialog)
            {
                OpenFile(fd.FileName);
            }
        }
        public void OpenFile(string filename)
        {
            try
            {
                if (filename.ToLower().EndsWith("zip"))
                {
                    ZipArchive archive = ZipFile.OpenRead(filename);
                    string ext = Path.GetExtension(archive.Entries[0].Name);
                    filename = "temp" + ext;
                    archive.Entries[0].ExtractToFile(filename, true);
                }

                //monitor.InitializeRom(Resources._48kRom, Resources._48kPointers);
                Running = false;
                TAPFileLoaded = false;
                //TZXFileLoaded = false;
                Running = false;
                Filename = Path.GetFileName(filename);
                string filetype = Path.GetExtension(filename).ToLower();
                Snapshot s = null;
                switch (filetype)
                {
                    case ".z80":
                        s = Z80Loader.LoadZ80(filename);
                        break;
                    case ".sna":
                        s = SNALoader.LoadSnapshot(filename);
                        break;
                    case ".tap":
                        currentTAPFile = new TAPFile();
                        currentTAPFile.Load(filename);
                        TAPFileLoaded = true;
                        keyboard.Reset();
                        //TapeView.Show();
                        return;
                    case ".tzx":
                        bool v = cassetteDeck.deck.Visible;
                        if (cassetteDeck == null)
                        {
                            cassetteDeck = new CassetteController();
                        }
                        CassetteController.LoadFile(filename);
                        cassetteDeck.deck.Visible = v;
                        keyboard.Reset();
                        return;
                }
                AF_ = s.AF_;
                BC_ = s.BC_;
                DE_ = s.DE_;
                HL_ = s.HL_;

                A = s.A;
                F = s.F;
                BC = s.BC;
                DE = s.DE;
                HL = s.HL;

                LastOut = s.Border;
                I = s.I;
                R = s.R;


                IM = s.IM;
                IX = s.IX;
                IY = s.IY;
                SP = s.SP;
                for (int i = 0x4000; i < 0x10000; i++)
                {
                    Memory[i] = (byte)s.Memory[i];
                }
                Loaded = true;
                IFF1 = IFF2 = s.IFF2;
                switch (filetype)
                {
                    case ".z80":
                        WZ = PC = s.PC;
                        break;
                    case ".sna":
                        PC = WZ = (PeekByte(SP++) + (PeekByte(SP++) << 8));
                        break;
                }

                TotalCycles = framelength;
                LastCycle = 0;
                //sound.Reset();

            }
            catch { }
            finally
            {
                keyboard.Reset();
                Running = true;
            }
        }

        #endregion TAP File handler

        #region TAP File
        bool IsFileLoaded;
        string LoadedFileName;
        public override bool TAPFileFastLoader()
        {
            if (CassetteController.FileLoaded)
            {
                Logging = true;
                if (!CassetteController.IsPlaying)
                    CassetteController.LoadRequest();
                return false;
            }
            if (IsFileLoaded)
            {
                TapFilename = LoadedFileName;
                TapFileReady = true;
                currentTAPFile = new TAPFile();
                currentTAPFile.Load(TapFilename);

            }
            else
            {
                if (TapFileReady)
                {
                    if (currentTAPFile.CurrentBlock == currentTAPFile.Blocks.Count)
                        TapFileReady = false;
                }

                if (!TapFileReady)
                    return false;
                //    using (OpenFileDialog ofd = new OpenFileDialog())
                //    {
                //        ofd.InitialDirectory = Properties.Settings.Default.DefaultDirectory;
                //        ofd.Filter = "Tape files (*.tap)|*.tap";
                //        ofd.FilterIndex = 0;
                //        ofd.RestoreDirectory = true;
                //        if (ofd.ShowDialog(this) == DialogResult.OK)
                //        {
                //            TapFilename = ofd.FileName;
                //            TapFileReady = true;
                //            currentTAPFile = new TAPFile();
                //            currentTAPFile.Load(TapFilename);

                //        }
                //        else
                //            return false;
                //    }
            }
            AF = AF_;
            if (LoadTapFile()) AF_ |= 0x40; else AF_ |= 0xBE;
            //// EX AF,AF’
            //int af = AF;
            //AF = AF_;
            //AF_ = af;
            // Execute a return using the ROM
            

            Loaded = true;

            base.TAPFileFastLoader();
            return true;
        }
        int block = -1;
        string TapFilename;
        bool TapFileReady;
        public override void TAPFileSave()
        {
            if (TapFilename + "" == "")
                TapFileReady = false;
            keyboard.Reset();
            if (!TapFileReady)
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.InitialDirectory = Properties.Settings.Default.DefaultDirectory;
                    sfd.Filter = "Tape files (*.tap)|*.tap";
                    sfd.FilterIndex = 0;
                    sfd.RestoreDirectory = true;
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        TapFilename = sfd.FileName;
                        File.Create(TapFilename).Close();
                        TapFileReady = true;
                    }
                    else
                    {
                        PC = 0x0806;
                        return;
                    }
                    block = 0;
                }
            SaveTapFile(TapFilename);

            base.TAPFileSave();

        }
        void SaveTapFile(string fileName)
        {
            using (FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Append))
            {
                using (BinaryWriter bw = new System.IO.BinaryWriter(fs))
                {
                    int checksum = A;
                    if (A == 255) TapFilename = "";
                    //                    Console.WriteLine("IX=" + IX.ToString("X4") + ", DE=" + DE.ToString("X4") + " A=" + checksum.ToString("X4"));
                    //                    Console.WriteLine();
                    int len = DE + 2;
                    //                    Console.WriteLine(((byte)len).ToString("X2"));
                    bw.Write((byte)len);
                    //                    Console.WriteLine(((byte)(len>>8)).ToString("X2"));
                    bw.Write((byte)(len >> 8));
                    bw.Write((byte)checksum);
                    int length = IX + DE;
                    while (IX<length)
                    //for (int i = IX; i < length; i++)
                    {
                        byte data = Memory[IX++];
                        //                        Console.WriteLine(data.ToString("X2"));
                        bw.Write(data);
                        checksum = checksum ^ data;
                    }
                    //IX += DE;
                    //                    Console.WriteLine(((byte)(checksum)).ToString("X2"));
                    bw.Write((byte)(checksum));
                    DE = 0;
                }
            }
            PC = 1342;
            //            Console.WriteLine();
            currentTAPFile = new TAPFile();
            currentTAPFile.Load(fileName);
            TAPFileLoaded = true;
            //TapeView = new TapeView();
            //TapeView.LoadTAPFile(currentTAPFile);
        }

        #endregion TAP File

        #region Command Entry
        public static int[] Command = null;
        public static int NextCommand = 0;
        List<string> CommandList; //= {"Not Used", "Not Used", "Not Used", "Not Used", "TRUE VIDEO", "INV VIDEO", "CAPS", "EDIT", "LEFT", "RIGHT", "DOWN", "UP", "DELETE", "ENTER", "SYMBOL", "GRAPHICS", "INK", "PAPER", "FLASH", "BRIGHT", "INVERSE", "OVER", "AT", "TAB", "Not Used", "Not Used", "Not Used", "Not Used", "Not Used", "Not Used", "Not Used", "Not Used", "SP", "!", @"""", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?", "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", @"\\", "]", "^", "_", "£", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~", "Copyright", "Graphics 1", "Graphics 2", "Graphics 3", "Graphics 4", "Graphics 5", "Graphics 6", "Graphics 7", "Graphics 8", "Graphics 9", "Graphics 10", "Graphics 11", "Graphics 12", "Graphics 13", "Graphics 14", "Graphics 15", "Graphics 16", "[A]", "[B]", "[C]", "[D]", "[E]", "[F]", "[G]", "[H]", "[I]", "[J]", "[K]", "[L]", "[M]", "[N]", "[O]", "[P]", "[Q]", "[R]", "[S]", "[T]", "[U]", "RND", "INKEY$", "PI", "FN", "POINT", "SCREEN$", "ATTR", "AT", "TAB", "VAL$", "CODE", "VAL", "LEN", "SIN", "COS", "TAN", "ASN", "ACS", "ATN", "LN", "EXP", "INT", "SQR", "SGN", "ABS", "PEEK", "IN", "USR", "STR$", "CHR$", "NOT", "BIN", "OR", "AND", "<=", ">=", "<>", "LINE", "THEN", "TO", "STOP", "DEF FN", "CAT", "FORMAT", "MOVE", "ERASE", "OPEN #", "CLOSE #", "MERGE", "VERIFY", "BEEP", "CIRCLE", "INK", "PAPER", "FLASH", "BRIGHT", "INVERSE", "OVER", "OUT", "LPRINT", "LLIST", "STOP", "READ", "DATA", "RESTORE", "NEW", "BORDER", "CONTINUE", "DIM", "REM", "FOR", "GO TO", "GO SUB", "INPUT", "LOAD", "LIST", "LET", "PAUSE", "NEXT", "POKE", "PRINT", "PLOT", "RUN", "SAVE", "RANDOMIZE", "IF", "CLS", "DRAW", "CLEAR", "RETURN", "COPY"};
       
        public void LoadCommand(string commands)
        {

            List<int> command = new List<int>();
            string[] commandlist = commands.Split(' ');
            int result;
            foreach (string s in commandlist)
            {
                int i = CommandList.IndexOf(s);
                // Invalid Command
                if (i == -1) return;
                command.Add(i);

            }

            command.Add(0x0d);  // End of line
            Command = command.ToArray();
            NextCommand = 0;
        }

        public void LoadProgram(string commands)
        {

            List<int> command = new List<int>();
            string[] commandlist = commands.Split(' ');
            int result;
            foreach (string s in commandlist)
            {
                if (int.TryParse(s, out result))
                {
                    for (int j = 0; j < s.Length; j++)
                    {
                        int r = CommandList.IndexOf(s[j].ToString());
                        command.Add(r);
                    }
                    continue;
                }
                if (s.StartsWith("\""))
                {
                    for (int j = 0; j < s.Length; j++)
                    {
                        int r = CommandList.IndexOf(s[j].ToString());
                        command.Add(r);
                    }
                    continue;
                }
                int i = CommandList.IndexOf( s);
                // Invalid Command
                if (i == -1)
                    return;
                command.Add(i);

            }

            command.Add(0x0d);  // End of line
            Command = command.ToArray();
            NextCommand = 0;
        }
        #endregion Command Entry

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public new void Dispose()
        {
            Dispose(true);
            base.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual new void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Sound.Dispose();
            }

            disposed = true;
        }

        ~Machine()
        {
            Dispose(false);
        }


    }
}
