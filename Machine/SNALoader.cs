using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZXStudio
{
    public class SNALoader
    {
        public static Snapshot LoadSnapshot(byte[] data)
        {

            /*
               Offset   Size   Description
               ------------------------------------------------------------------------
               0        1      byte   I
               1        8      word   HL',DE',BC',AF'
               9        10     word   HL,DE,BC,IY,IX
               19       1      byte   Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
               20       1      byte   R
               21       4      words  AF,SP
               25       1      byte   IntMode (0=IM0/1=IM1/2=IM2)
               26       1      byte   BorderColor (0..7, not used by Spectrum 1.7)
               27       49152  bytes  RAM dump 16384..65535
               ------------------------------------------------------------------------
               Total: 49179 bytes
             */

            Snapshot snapshot = new Snapshot();

            snapshot.I = (byte)data[0];

            snapshot.HL_ = data[1] | (data[2] << 8);
            snapshot.DE_ = data[3] | (data[4] << 8);
            snapshot.BC_ = data[5] | (data[6] << 8);
            snapshot.AF_ = data[7] | (data[8] << 8);

            snapshot.HL = data[9] | (data[10] << 8);
            snapshot.DE = data[11] | (data[12] << 8);
            snapshot.BC = data[13] | (data[14] << 8);
            snapshot.IY = data[15] | (data[16] << 8);
            snapshot.IX = data[17] | (data[18] << 8);

            snapshot.IFF2 = data[19] << 1;

            snapshot.R = data[20];

            snapshot.F = data[21];
            snapshot.A = data[22];

            snapshot.SP = data[23] | (data[24] << 8);

            snapshot.IM = data[25];
            snapshot.Border = data[26] & 0x07;

            ushort p = 0x4000;
            for (int i = 27; i < data.Length; i++)
                snapshot.Memory[p++] = data[i];

            return snapshot;
        }

        public static byte[] SaveSnapshot(Machine cpu,byte lastout)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                /*
                   Offset   Size   Description
                   ------------------------------------------------------------------------
                   0        1      byte   I
                   1        8      word   HL',DE',BC',AF'
                   9        10     word   HL,DE,BC,IY,IX
                   19       1      byte   Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
                   20       1      byte   R
                   21       4      words  AF,SP
                   25       1      byte   IntMode (0=IM0/1=IM1/2=IM2)
                   26       1      byte   BorderColor (0..7, not used by Spectrum 1.7)
                   27       49152  bytes  RAM dump 16384..65535
                   ------------------------------------------------------------------------
                   Total: 49179 bytes
                 */

                memoryStream.WriteByte((byte)cpu.I);

                memoryStream.WriteByte((byte)(cpu.HL_));
                memoryStream.WriteByte((byte)(cpu.HL_ >> 8));
                memoryStream.WriteByte((byte)cpu.DE_);
                memoryStream.WriteByte((byte)(cpu.DE_ >> 8));
                memoryStream.WriteByte((byte)cpu.BC_);
                memoryStream.WriteByte((byte)(cpu.BC_ >> 8));
                memoryStream.WriteByte((byte)cpu.AF_);
                memoryStream.WriteByte((byte)(cpu.AF_ >> 8));

                memoryStream.WriteByte((byte)cpu.L);
                memoryStream.WriteByte((byte)cpu.H);
                memoryStream.WriteByte((byte)cpu.E);
                memoryStream.WriteByte((byte)cpu.D);
                memoryStream.WriteByte((byte)cpu.C);
                memoryStream.WriteByte((byte)cpu.B);
                memoryStream.WriteByte((byte)cpu.IYL);
                memoryStream.WriteByte((byte)cpu.IYH);
                memoryStream.WriteByte((byte)cpu.IXL);
                memoryStream.WriteByte((byte)cpu.IXH);

                memoryStream.WriteByte((byte)(cpu.IFF2 << 1));
                memoryStream.WriteByte((byte)cpu.R);

                memoryStream.WriteByte((byte)cpu.F);
                memoryStream.WriteByte((byte)cpu.A);

                cpu.PokeByte(--cpu.SP, cpu.PC >> 8);
                cpu.PokeByte(--cpu.SP, cpu.PC & 0xff);

                //cpu.PUSH(cpu.PC);
                memoryStream.WriteByte((byte)cpu.SP);
                memoryStream.WriteByte((byte)(cpu.SP >> 8));


                memoryStream.WriteByte((byte)cpu.IM);
                memoryStream.WriteByte((byte)cpu.LastOut);


                for (int i = 0x4000; i < 0x10000; i++)
                    memoryStream.WriteByte((byte)cpu.ReadByte(i));

                memoryStream.Flush();

                return memoryStream.ToArray();

            }
        }

        public static void SaveSnapshot(Machine cpu, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                /*
                   Offset   Size   Description
                   ------------------------------------------------------------------------
                   0        1      byte   I
                   1        8      word   HL',DE',BC',AF'
                   9        10     word   HL,DE,BC,IY,IX
                   19       1      byte   Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
                   20       1      byte   R
                   21       4      words  AF,SP
                   25       1      byte   IntMode (0=IM0/1=IM1/2=IM2)
                   26       1      byte   BorderColor (0..7, not used by Spectrum 1.7)
                   27       49152  bytes  RAM dump 16384..65535
                   ------------------------------------------------------------------------
                   Total: 49179 bytes
                 */
                fs.WriteByte((byte)cpu.I);

                fs.WriteByte((byte)(cpu.HL_));
                fs.WriteByte((byte)(cpu.HL_ >> 8));
                fs.WriteByte((byte)cpu.DE_);
                fs.WriteByte((byte)(cpu.DE_ >> 8));
                fs.WriteByte((byte)cpu.BC_);
                fs.WriteByte((byte)(cpu.BC_ >> 8));
                fs.WriteByte((byte)cpu.AF_);
                fs.WriteByte((byte)(cpu.AF_ >> 8));

                fs.WriteByte((byte)cpu.L);
                fs.WriteByte((byte)cpu.H);
                fs.WriteByte((byte)cpu.E);
                fs.WriteByte((byte)cpu.D);
                fs.WriteByte((byte)cpu.C);
                fs.WriteByte((byte)cpu.B);
                fs.WriteByte((byte)cpu.IYL);
                fs.WriteByte((byte)cpu.IYH);
                fs.WriteByte((byte)cpu.IXL);
                fs.WriteByte((byte)cpu.IXH);

                fs.WriteByte((byte)(cpu.IFF2 << 1));
                fs.WriteByte((byte)cpu.R);

                fs.WriteByte((byte)cpu.F);
                fs.WriteByte((byte)cpu.A);

                cpu.PokeByte(--cpu.SP, cpu.PC >> 8);
                cpu.PokeByte(--cpu.SP, cpu.PC & 0xff);

                //cpu.PUSH(cpu.PC);
                fs.WriteByte((byte)cpu.SP);
                fs.WriteByte((byte)(cpu.SP >> 8));


                fs.WriteByte((byte)cpu.IM);
                fs.WriteByte((byte)cpu.LastOut);


                for (int i = 0x4000; i < 0x10000; i++)
                    fs.WriteByte((byte)cpu.ReadByte(i));

                fs.Flush();

            }
        }

        public static Snapshot LoadSnapshot(string filename)
        {

            /*
               Offset   Size   Description
               ------------------------------------------------------------------------
               0        1      byte   I
               1        8      word   HL',DE',BC',AF'
               9        10     word   HL,DE,BC,IY,IX
               19       1      byte   Interrupt (bit 2 contains IFF2, 1=EI/0=DI)
               20       1      byte   R
               21       4      words  AF,SP
               25       1      byte   IntMode (0=IM0/1=IM1/2=IM2)
               26       1      byte   BorderColor (0..7, not used by Spectrum 1.7)
               27       49152  bytes  RAM dump 16384..65535
               ------------------------------------------------------------------------
               Total: 49179 bytes
             */

            Snapshot snapshot = new Snapshot();

            byte[] data;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }
            snapshot.I = (byte)data[0];

            snapshot.HL_ = data[1] | (data[2] << 8);
            snapshot.DE_ = data[3] | (data[4] << 8);
            snapshot.BC_ = data[5] | (data[6] << 8);
            snapshot.AF_ = data[7] | (data[8] << 8);

            snapshot.HL = data[9] | (data[10] << 8);
            snapshot.DE = data[11] | (data[12] << 8);
            snapshot.BC = data[13] | (data[14] << 8);
            snapshot.IY = data[15] | (data[16] << 8);
            snapshot.IX = data[17] | (data[18] << 8);

            snapshot.IFF2 = (data[19] >> 2) & 0x01;

            snapshot.R = data[20];

            snapshot.F = data[21];
            snapshot.A = data[22];

            snapshot.SP = data[23] | (data[24] << 8);

            snapshot.IM = data[25];
            snapshot.Border = data[26] & 0x07;

            ushort p = 0x4000;
            for (int i = 27; i < data.Length; i++)
                snapshot.Memory[p++] = data[i];

            return snapshot;
        }
    }

    public class Snapshot
    {
        public int I;
        public int R;

        public int A;
        public int F;
        public int BC;
        public int DE;
        public int HL;

        public int AF_;
        public int BC_;
        public int DE_;
        public int HL_;

        public int IX;
        public int IY;

        public int IFF1;
        public int IFF2;
        public int SP;
        public int IM;
        public int Border;

        public int PC;

        public int[] Memory = new int[0x10000];
    }
}
