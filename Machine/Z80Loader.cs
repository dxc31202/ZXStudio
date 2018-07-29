using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZXStudio
{
    public class Z80Loader
    {
        byte[] memory = new byte[0x10000];
        public static Snapshot LoadZ80(string filename)
        {
            Snapshot sn = new Snapshot();
            byte[] data;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }
            /*
             * Header block, common to all versions
                Offset  Length  Description
                ---------------------------
                0       1       A register
                1       1       F register
                2       2       BC register pair (LSB, i.e. C, first)
                4       2       HL register pair
                6       2       Program counter
                8       2       Stack pointer
                10      1       Interrupt register
                11      1       Refresh register (Bit 7 is not significant!)
                12      1       Bit 0  : Bit 7 of the R-register
                                Bit 1-3: Border colour
                                Bit 4  : 1=Basic SamRom switched in
                                Bit 5  : 1=Block of data is compressed
                                Bit 6-7: No meaning
                13      2       DE register pair
                15      2       BC' register pair
                17      2       DE' register pair
                19      2       HL' register pair
                21      1       A' register
                22      1       F' register
                23      2       IY register (Again LSB first)
                25      2       IX register
                27      1       Interrupt flipflop, 0=DI, otherwise EI
                28      1       IFF2 (not particularly important...)
                29      1       Bit 0-1: Interrupt mode (0, 1 or 2)
                                Bit 2  : 1=Issue 2 emulation
                                Bit 3  : 1=Double interrupt frequency
                                Bit 4-5: 1=High video synchronisation
                                         3=Low video synchronisation
                                         0,2=Normal
                                Bit 6-7: 0=Cursor/Protek/AGF joystick
                                         1=Kempston joystick
                                         2=Sinclair 2 Left joystick (or user
                                           defined, for version 3 .z80 files)
                                         3=Sinclair 2 Right joystick
             */
            sn.A = data[0]; sn.A = data[1];
            sn.BC = data[3] << 8 | data[2];
            sn.HL = data[4] | (data[5] << 8);
            sn.PC = data[6] | (data[7] << 8);
            sn.SP = data[8] | (data[9] << 8);

            sn.I = data[10]; sn.R = data[11];
            sn.R |= (byte)(data[12] << 7);
            bool IsCompressed = (data[12] & 0x20) == 0x20;
            sn.DE = (ushort)(data[13] | (data[14] << 8));

            sn.BC_ = (ushort)(data[15] | (data[16] << 8));
            sn.DE_ = (ushort)(data[17] | (data[18] << 8));
            sn.HL_ = (ushort)(data[19] | (data[20] << 8));
            sn.AF_ = (ushort)((data[21] << 8) | data[22]);
            sn.IY = (ushort)(data[23] | (data[24] << 8));
            sn.IX = (ushort)(data[25] | (data[26] << 8));
            //Interrupt flipflop, 0 = DI, otherwise EI
            sn.IFF2 = data[27] & 0x01;
            //sn.IFF2 = data[28] & 0x01;
            sn.IM = data[29] & 0x03;

            if (sn.PC == 0)
            {
                /*
                 * The value of the word at position 30 is 23 for version 2 files, and 54 or 55 for version 3; 
                 * the fields marked '*' are the ones that are present in the version 2 header. 
                 * The final byte (marked '**') is present only if the word at position 30 is 55
                 */

                LoadZ80V23(data, sn);
            }
            else
            {

                // Version 1 is 48K only
                LoadZ80V1(IsCompressed, data, 30, 0x4000, data.Length - 34, sn);
            }

            return sn;

        }

        public static void LoadZ80V1(bool isCompressed, byte[] data, int startAt, ushort memoryStart, int length, Snapshot sn)
        {
            if (isCompressed)
            {
                /*
                 * The compression method replaces repetitions of at least five equal bytes by a four-byte code ED ED xx yy, 
                 * which stands for "byte yy repeated xx times". 
                 * Only sequences of length at least 5 are coded. 
                 * The exception is sequences consisting of ED's; if they are encountered, 
                 * even two ED's are encoded into ED ED 02 ED. 
                 * Finally, every byte directly following a single ED is not taken into a block, 
                 * for example ED 6*00 is not encoded into ED ED ED 06 00 but into ED 00 ED ED 05 00. 
                 * The block is terminated by an end marker, 00 ED ED 00.
                 */
                ushort m = memoryStart;
                // startAt is the next byte following the XX byte header
                // -4 to allow for the end marker 00 ED ED 00
                for (int i = startAt; i < startAt + length; i++)
                {
                    // is this an encoded block of data ??
                    if (data[i] == 0xed)
                    {
                        // Really ???
                        if (data[i + 1] == 0xed)
                        {
                            // this is an encoded block of data
                            i += 2;                     // move beyond the previous ED ED
                            int counter = data[i++];    // xx
                            byte value = data[i];       // yy
                            for (int j = 0; j < counter; j++)
                                sn.Memory[m++] = value;
                        }
                        else
                            // this really is an ED
                            sn.Memory[m++] = 0xed;
                    }
                    else
                        // not encoded
                        sn.Memory[m++] = data[i];

                }
            }
            else
            {
                ushort x = memoryStart;
                // No compression, straight copy
                for (int i = startAt; i < startAt + length; i++)
                    sn.Memory[x++] = data[i];
            }
        }

        public static void LoadZ80V23(byte[] data, Snapshot sn)
        {
            // Version 2 or 3
            /*
                    Offset  Length  Description
                    ---------------------------
                  * 30      2       Length of additional header block (see below)
                  * 32      2       Program counter
                  * 34      1       Hardware mode (see below)
                  * 35      1       If in SamRam mode, bitwise state of 74ls259.
                                    For example, bit 6=1 after an OUT 31,13 (=2*6+1)
                                    If in 128 mode, contains last OUT to 0x7ffd
                        If in Timex mode, contains last OUT to 0xf4
                  * 36      1       Contains 0xff if Interface I rom paged
                        If in Timex mode, contains last OUT to 0xff
                  * 37      1       Bit 0: 1 if R register emulation on
                                    Bit 1: 1 if LDIR emulation on
                        Bit 2: AY sound in use, even on 48K machines
                        Bit 6: (if bit 2 set) Fuller Audio Box emulation
                        Bit 7: Modify hardware (see below)
                  * 38      1       Last OUT to port 0xfffd (soundchip register number)
                  * 39      16      Contents of the sound chip registers
                    55      2       Low T state counter
                    57      1       Hi T state counter
                    58      1       Flag byte used by Spectator (QL spec. emulator)
                                    Ignored by Z80 when loading, zero when saving
                    59      1       0xff if MGT Rom paged
                    60      1       0xff if Multiface Rom paged. Should always be 0.
                    61      1       0xff if 0-8191 is ROM, 0 if RAM
                    62      1       0xff if 8192-16383 is ROM, 0 if RAM
                    63      10      5 x keyboard mappings for user defined joystick
                    73      10      5 x ASCII word: keys corresponding to mappings above
                    83      1       MGT type: 0=Disciple+Epson,1=Disciple+HP,16=Plus D
                    84      1       Disciple inhibit button status: 0=out, 0ff=in
                    85      1       Disciple inhibit flag: 0=rom pageable, 0ff=not
                 ** 86      1       Last OUT to port 0x1ffd
             * */
            int headerLength = (data[30] | (data[31] << 8));
            sn.PC = data[32] | (data[33] << 8);
            int hardwareMode = data[34];
            int samRamMode = data[35];
            int interfaceOnePaged = data[36];
            int rRegLDIREmulation = data[37];
            int lastOutPortFFFD = data[38];

            // Bla, Bla, Bla ... May implement later
            /*
             * Hereafter a number of memory blocks follow, each containing the compressed data of a 16K block. 
             * The compression is according to the old scheme, except for the end-marker, which is now absent. 
             * The structure of a memory block is:
                 Byte    Length  Description
                ---------------------------
                0       2       Length of compressed data (without this 3-byte header)
                                If length=0xffff, data is 16384 bytes long and not compressed
                2       1       Page number of block
                3       [0]     Data
             */
            int nextBlockStart = headerLength + 32;
            /*
             * The pages are numbered, depending on the hardware mode, in the following way:
                 Page    In '48 mode     In '128 mode    In SamRam mode
                ------------------------------------------------------
                 0      48K rom         rom (basic)     48K rom
                 1      Interface I, Disciple or Plus D rom, according to setting
                 2      -               rom (reset)     samram rom (basic)
                 3      -               page 0          samram rom (monitor,..)
                 4      8000-bfff       page 1          Normal 8000-bfff
                 5      c000-ffff       page 2          Normal c000-ffff
                 6      -               page 3          Shadow 8000-bfff
                 7      -               page 4          Shadow c000-ffff
                 8      4000-7fff       page 5          4000-7fff
                 9      -               page 6          -
                10      -               page 7          -
                11      Multiface rom   Multiface rom   -
             * In 48K mode, pages 4,5 and 8 are saved. 
             * In SamRam mode, pages 4 to 8 are saved. 
             * In 128K mode, all pages from 3 to 10 are saved. 
             * Pentagon snapshots are very similar to 128K snapshots, while Scorpion snapshots have the 16 RAM pages saved in pages 3 to 18. 
             * There is no end marker

             */
            while (nextBlockStart < data.Length)
            {

                int BlockLength;
                BlockLength = ((data[nextBlockStart + 1] << 8) | data[nextBlockStart]);
                nextBlockStart += 2;
                int dataType = data[nextBlockStart++];
                if (BlockLength == 0xffff)
                    LoadZ80V1(false, data, nextBlockStart, 0x4000, BlockLength, sn);
                else
                    switch (dataType) // See table above
                    {
                        case 0:
                            LoadZ80V1(true, data, nextBlockStart, 0x0000, BlockLength, sn); break;
                        case 4:
                            LoadZ80V1(true, data, nextBlockStart, 0x8000, BlockLength, sn); break;
                        case 5:
                            LoadZ80V1(true, data, nextBlockStart, 0xc000, BlockLength, sn); break;
                        case 8:
                            LoadZ80V1(true, data, nextBlockStart, 0x4000, BlockLength, sn); break;
                        default: break;
                    }
                nextBlockStart += BlockLength;
            }
        }

    }

}