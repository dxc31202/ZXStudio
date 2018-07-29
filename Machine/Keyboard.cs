
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ZXStudio
{
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
    public class Keyboard
    {
        // Requires installation of MouseKeyboardActivityMonitor
        // To install MouseKeyHook, run the following command in the Package Manager Console 
        // Install-Package MouseKeyHook 
        //
        // DLL Location
        // D:\Users\Dave\Documents\Visual Studio 2015\Projects\WindowsFormsApplication8\packages\MouseKeyboardActivityMonitor.4.0.5150.10665\lib\net20\MouseKeyboardActivityMonitor.dll


        //  | 00 Fl0| 01 Fl1| 02 Br0| 03 Br1| 04 In0| 05 In1| 06 CAP| 07 EDT|
        //  | 08 LFT| 09 RIG| 0A DWN| 0B UP | 0C DEL| 0D ENT| 0E SYM| 0F GRA|
        //  | 10 Ik0| 11 Ik1| 12 Ik2| 13 Ik3| 14 Ik4| 15 Ik5| 16 Ik6| 17 Ik7|
        //  | 18 Pa0| 19 Pa1| 1A Pa2| 1B Pa3| 1C Pa4| 1D Pa5| 1E Pa6| 1F Pa7|
        //  | 20 SP | 21  ! | 22  " | 23  # | 24  $ | 25  % | 26  & | 27  ' |
        //  | 28  ( | 29  ) | 2A  * | 2B  + | 2C  , | 2D  - | 2E  . | 2F  / |
        //  | 30  0 | 31  1 | 32  2 | 33  3 | 34  4 | 35  5 | 36  6 | 37  7 |
        //  | 38  8 | 39  9 | 3A  : | 3B  //| 3C  < | 3D  = | 3E  > | 3F  ? |
        //  | 40  @ | 41  A | 42  B | 43  C | 44  D | 45  E | 46  F | 47  G |
        //  | 48  H | 49  I | 4A  J | 4B  K | 4C  L | 4D  M | 4E  N | 4F  O |
        //  | 50  P | 51  Q | 52  R | 53  S | 54  T | 55  U | 56  V | 57  W |
        //  | 58  X | 59  Y | 5A  Z | 5B  [ | 5C  \ | 5D  ] | 5E  ^ | 5F  _ |
        //  | 60  £ | 61  a | 62  b | 63  c | 64  d | 65  e | 66  f | 67  g |
        //  | 68  h | 69  i | 6A  j | 6B  k | 6C  l | 6D  m | 6E  n | 6F  o |
        //  | 70  p | 71  q | 72  r | 73  s | 74  t | 75  u | 76  v | 77  w |
        //  | 78  x | 79  y | 7A  z | 7B  { | 7C  | | 7D  } | 7E  ~ | 7F  © |
        //  | 80 128| 81 129| 82 130| 83 131| 84 132| 85 133| 86 134| 87 135|
        //  | 88 136| 89 137| 8A 138| 8B 139| 8C 140| 8D 141| 8E 142| 8F 143|
        //  | 90 [A]| 91 [B]| 92 [C]| 93 [D]| 94 [E]| 95 [F]| 96 [G]| 97 [H]|
        //  | 98 [I]| 99 [J]| 9A [K]| 9B [L]| 9C [M]| 9D [N]| 9E [O]| 9F [P]|
        //  | A0 [Q]| A1 [R]| A2 [S]| A3 [T]| A4 [U]| A5 RND| A6 IK$| A7 PI |
        //  | A8 FN | A9 PNT| AA SC$| AB ATT| AC AT | AD TAB| AE VL$| AF COD|
        //  | B0 VAL| B1 LEN| B2 SIN| B3 COS| B4 TAN| B5 ASN| B6 ACS| B7 ATN|
        //  | B8 LN | B9 EXP| BA INT| BB SQR| BC SGN| BD ABS| BE PEK| BF IN |
        //  | C0 USR| C1 ST$| C2 CH$| C3 NOT| C4 BIN| C5 OR | C6 AND| C7 <= |
        //  | C8 >= | C9 <> | CA LIN| CB THN| CC TO | CD STP| CE DEF| CF CAT|
        //  | D0 FMT| D1 MOV| D2 ERS| D3 OPN| D4 CLO| D5 MRG| D6 VFY| D7 BEP|
        //  | D8 CIR| D9 INK| DA PAP| DB FLA| DC BRI| DD INV| DE OVR| DF OUT|
        //  | E0 LPR| E1 LLI| E2 STP| E3 REA| E4 DAT| E5 RES| E6 NEW| E7 BDR|
        //  | E8 CON| E9 DIM| EA REM| EB FOR| EC GTO| ED GSB| EE INP| EF LOA|
        //  | F0 LIS| F1 LET| F2 PAU| F3 NXT| F4 POK| F5 PRI| F6 PLO| F7 RUN|
        //  | F8 SAV| F9 RAN| FA IF | FB CLS| FC DRW| FD CLR| FE RET| FF CPY|



        public static bool isFocused = false;
        public static bool IsFocused
        {
            get { return isFocused; }
            set { isFocused = value; }
        }

        internal int[,] keyboard = new int[4, 2];
        /// <summary>
        /// Spectrum Keyboard Matrix
        /// 
        ///  
        /// port    ------------  BIT -------------  ------------  BIT  ------------ 
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        ///         |  0  |  1  |  2  |  3  |  4  |  |  4  |  3  |  2  |  1  |   0 |
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        /// 0xF7FE  |  1  |  2  |  3  |  4  |  5  |  |  6  |  7  |  8  |  9  |  0  |    0xEFFE
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        /// 0xFBFE  |  Q  |  W  |  E  |  R  |  T  |  |  Y  |  U  |  I  |  O  |  P  |    0xDFFE
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        /// 0xFDFE  |  A  |  S  |  D  |  F  |  G  |  |  H  |  J  |  K  |  L  |ENTER|    0xBFFE
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        /// 0xFEFE  |SHIFT|  Z  |  X  |  C  |  V  |  |  B  |  N  |  M  | SYM |SPACE|    0x7FFE
        ///         +-----+-----+-----+-----+-----+  +-----+-----+-----+-----+-----+
        /// 
        /// Initialize all key values to 0xFF
        /// </summary>
        public Keyboard()
        {
            keyboard[0, 0] = 0xFF; keyboard[0, 1] = 0xFF;
            keyboard[1, 0] = 0xFF; keyboard[1, 1] = 0xFF;
            keyboard[2, 0] = 0xFF; keyboard[2, 1] = 0xFF;
            keyboard[3, 0] = 0xFF; keyboard[3, 1] = 0xFF;

        }
        ~Keyboard()
        {
        }

        public void Reset()
        {
            keyboard = new int[4, 2];
            keyboard[0, 0] = 0xFF; keyboard[0, 1] = 0xFF;
            keyboard[1, 0] = 0xFF; keyboard[1, 1] = 0xFF;
            keyboard[2, 0] = 0xFF; keyboard[2, 1] = 0xFF;
            keyboard[3, 0] = 0xFF; keyboard[3, 1] = 0xFF;

        }
        public event KeyPressEventHandler KeyPressed;
        public event KeyEventHandler KeyDownEvent;
        public event KeyEventHandler KeyUpEvent;

        private void KeyboardHookListener_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsFocused)
                KeyPressed?.Invoke(sender, e);
        }

        private void KeyboardHookListener_KeyUp(object sender, KeyEventArgs e)
        {

            if (IsFocused)
            {
                KeyUpEvent?.Invoke(sender, e);
                ProcessKey(false, (int)e.KeyCode, (int)e.Modifiers);
            }
        }

        private void KeyboardHookListener_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsFocused)
            {
                KeyDownEvent?.Invoke(sender, e);
                ProcessKey(true, (int)e.KeyCode, (int)e.Modifiers);
            }
        }

        /// <summary>
        /// Returns the value of the keyboard matrix at the given port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public int ReadKeyboard(int port)
        {
            int key = 0xff;

            if ((port & 0xff) == 0xfe)
            {
                int hi = port >> 8;
                //int a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0;
                switch (hi)
                {
                    case 0xf7: key = keyboard[0, 0]; break;     //     1 2 3 4 5
                    case 0xef: key = keyboard[0, 1]; break;     //     6 7 8 9 0

                    case 0xfb: key = keyboard[1, 0]; break;     //     Q W E R T
                    case 0xdf: key = keyboard[1, 1]; break;     //     Y U I O P

                    case 0xfd: key = keyboard[2, 0]; break;     //     A S D F G
                    case 0xbf: key = keyboard[2, 1]; break;     //     H J K L ENTER

                    case 0xfe: key = keyboard[3, 0]; break;     // SHIFT Z X C V
                    case 0x7f: key = keyboard[3, 1]; break;     //     B N M SYM SPACE

                    default:    // reading more than one row
                        hi = ~hi & 0xff;
                        for (int row = 0, mask = 0x01; row < 4; row++, mask <<= 1)
                            for (int col = 0; col < 2; col++)
                                if ((port & mask) != 0)
                                    key &= keyboard[row, col];
                        break;
                }
            }
            key = key & 0x1f | 0xa0;

            return (key);
        }

        bool CAPS;
        bool SYMB;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="down"></param>
        /// <param name="ascii"></param>
        /// <param name="modifiers"></param>
        public void ProcessKey(bool down, int ascii, int modifiers)
        {
            CAPS = ((modifiers >> 16) & 1) == 1;
            SYMB = ((modifiers >> 16) & 2) == 2;

            if (CAPS) keyboard[3, 0] &= ~0x01; else keyboard[3, 0] |= 0x01; // CAPS 
            if (SYMB) keyboard[3, 1] &= ~0x02; else keyboard[3, 1] |= 0x02; // SYMB

            // Change control versions of keys to lower case 
            if ((ascii >= 1) && (ascii <= 0x27) && SYMB)
            {
                ascii += 0x60;
            }

            if (down)
                keyDown(ascii);
            else
                KeyUp(ascii);

        }
        public void keyDown(int ascii)
        {
            switch (ascii)
            {
                case 20: keyboard[0, 0] &= ~0x02; keyboard[3, 0] &= ~0x01; break; // Caps Lock + Caps Shift
                case 8: keyboard[0, 1] &= ~0x01; keyboard[3, 0] &= ~0x01; break; // Backspace + Caps Shift

                case 49: keyboard[0, 0] &= ~0x01; break;  // 1
                case 50: keyboard[0, 0] &= ~0x02; break;  // 2
                case 51: keyboard[0, 0] &= ~0x04; break;  // 3
                case 52: keyboard[0, 0] &= ~0x08; break;  // 4
                case 53: keyboard[0, 0] &= ~0x10; break;  // 5

                case 54: keyboard[0, 1] &= ~0x10; break;  // 6
                case 55: keyboard[0, 1] &= ~0x08; break;  // 7
                case 56: keyboard[0, 1] &= ~0x04; break;  // 8
                case 57: keyboard[0, 1] &= ~0x02; break;  // 9
                case 48: keyboard[0, 1] &= ~0x01; break;  // 0

                case 81: keyboard[1, 0] &= ~0x01; break;  // q
                case 87: keyboard[1, 0] &= ~0x02; break;  // w
                case 69: keyboard[1, 0] &= ~0x04; break;  // e
                case 82: keyboard[1, 0] &= ~0x08; break;  // r
                case 84: keyboard[1, 0] &= ~0x10; break;  // t

                case 89: keyboard[1, 1] &= ~0x10; break;  // y
                case 85: keyboard[1, 1] &= ~0x08; break;  // u
                case 73: keyboard[1, 1] &= ~0x04; break;  // i
                case 79: keyboard[1, 1] &= ~0x02; break;  // o
                case 80: keyboard[1, 1] &= ~0x01; break;  // p

                case 65: keyboard[2, 0] &= ~0x01; break;  // a
                case 83: keyboard[2, 0] &= ~0x02; break;  // s
                case 68: keyboard[2, 0] &= ~0x04; break;  // d
                case 70: keyboard[2, 0] &= ~0x08; break;  // f
                case 71: keyboard[2, 0] &= ~0x10; break;  // g

                case 72: keyboard[2, 1] &= ~0x10; break;  // h
                case 74: keyboard[2, 1] &= ~0x08; break;  // j
                case 75: keyboard[2, 1] &= ~0x04; break;  // k
                case 76: keyboard[2, 1] &= ~0x02; break;  // l
                case 13: keyboard[2, 1] &= ~0x01; break;  // Enter

                //case 16: keyboard[3, 0] &= ~0x01; break;  // Caps Shift
                case 90: keyboard[3, 0] &= ~0x02; break;  // s
                case 88: keyboard[3, 0] &= ~0x04; break;  // d
                case 67: keyboard[3, 0] &= ~0x08; break;  // f
                case 86: keyboard[3, 0] &= ~0x10; break;  // g

                case 66: keyboard[3, 1] &= ~0x10; break;  // b
                case 78: keyboard[3, 1] &= ~0x08; break;  // n
                case 77: keyboard[3, 1] &= ~0x04; break;  // m
                                                          //case 17: keyboard[3, 1] &= ~0x02; break;  // Symbol Shift
                case 32: keyboard[3, 1] &= ~0x01; break;  // Space

                // Arrow keys
                case 37: keyboard[0, 0] &= ~0x10; break;  // Left  + Caps Shift
                case 38: keyboard[0, 1] &= ~0x08; break;  // Up    + Caps Shift
                case 39: keyboard[0, 1] &= ~0x04; break;  // Right + Caps Shift
                case 40: keyboard[0, 1] &= ~0x10; break;  // Down  + Caps Shift

                case 188: keyboard[3, 1] &= ~0x08; keyboard[3, 1] &= ~0x02; break;  // , + Symbol Shift
                case 190: keyboard[3, 1] &= ~0x04; keyboard[3, 1] &= ~0x02; break;  // . + Symbol Shift

            }
        }

        public void KeyUp(int ascii)
        {
            switch (ascii)
            {
                case 20: keyboard[0, 0] |= 0x02; if (keyboard[3, 0] == 0xff) keyboard[3, 0] |= 1; break; // Caps Lock + Caps Shift
                case 8: keyboard[0, 1] |= 0x01; if (keyboard[3, 0] == 0xff) keyboard[3, 0] |= 1; break; // Backspace

                case 49: keyboard[0, 0] |= 0x01; break;  // 1
                case 50: keyboard[0, 0] |= 0x02; break;  // 2
                case 51: keyboard[0, 0] |= 0x04; break;  // 3
                case 52: keyboard[0, 0] |= 0x08; break;  // 4
                case 53: keyboard[0, 0] |= 0x10; break;  // 5

                case 54: keyboard[0, 1] |= 0x10; break;  // 6
                case 55: keyboard[0, 1] |= 0x08; break;  // 7
                case 56: keyboard[0, 1] |= 0x04; break;  // 8
                case 57: keyboard[0, 1] |= 0x02; break;  // 9
                case 48: keyboard[0, 1] |= 0x01; break;  // 0

                case 81: keyboard[1, 0] |= 0x01; break;  // q
                case 87: keyboard[1, 0] |= 0x02; break;  // w
                case 69: keyboard[1, 0] |= 0x04; break;  // e
                case 82: keyboard[1, 0] |= 0x08; break;  // r
                case 84: keyboard[1, 0] |= 0x10; break;  // t

                case 89: keyboard[1, 1] |= 0x10; break;  // y
                case 85: keyboard[1, 1] |= 0x08; break;  // u
                case 73: keyboard[1, 1] |= 0x04; break;  // i
                case 79: keyboard[1, 1] |= 0x02; break;  // o
                case 80: keyboard[1, 1] |= 0x01; break;  // p

                case 65: keyboard[2, 0] |= 0x01; break;  // a
                case 83: keyboard[2, 0] |= 0x02; break;  // s
                case 68: keyboard[2, 0] |= 0x04; break;  // d
                case 70: keyboard[2, 0] |= 0x08; break;  // f
                case 71: keyboard[2, 0] |= 0x10; break;  // g

                case 72: keyboard[2, 1] |= 0x10; break;  // h
                case 74: keyboard[2, 1] |= 0x08; break;  // j
                case 75: keyboard[2, 1] |= 0x04; break;  // k
                case 76: keyboard[2, 1] |= 0x02; break;  // l
                case 13: keyboard[2, 1] |= 0x01; break;  // Enter

                //case 16: keyboard[3, 0] |= 0x01; break;  // Caps Shift
                case 90: keyboard[3, 0] |= 0x02; break;  // s
                case 88: keyboard[3, 0] |= 0x04; break;  // d
                case 67: keyboard[3, 0] |= 0x08; break;  // f
                case 86: keyboard[3, 0] |= 0x10; break;  // g

                case 66: keyboard[3, 1] |= 0x10; break;  // b
                case 78: keyboard[3, 1] |= 0x08; break;  // n
                case 77: keyboard[3, 1] |= 0x04; break;  // m
                                                         //case 17: keyboard[3, 1] |= 0x02; break;  // Symbol Shift

                case 32: keyboard[3, 1] |= 0x01; break;  // Space


                // Arrow keys
                case 37: keyboard[0, 0] |= 0x10; break;  // Left
                case 38: keyboard[0, 1] |= 0x08; break;  // Up
                case 39: keyboard[0, 1] |= 0x04; break;  // Right
                case 40: keyboard[0, 1] |= 0x10; break;  // Down

                case 188: keyboard[3, 1] |= 0x08; keyboard[3, 1] |= 0x02; break;  // ,
                case 190: keyboard[3, 1] |= 0x04; keyboard[3, 1] |= 0x02; break;  // .

            }
        }
    }
}