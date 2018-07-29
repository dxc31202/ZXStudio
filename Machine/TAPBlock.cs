using System;
using System.IO;

namespace ZXStudio
{
    /*
     * For use by LoadTapFile()
     */
    public class TAPBlock
    {
        public UInt16 Length;
        public byte[] Data;
        public int Checksum;
        public int ID;
        public bool IsValid;
        public int Position;
        public TAPBlock(BinaryReader br, int position)
        {
            Position = position;
            try
            {
                Length = (UInt16)((br.ReadByte() | (br.ReadByte() << 8)) - 2);
                ID = br.ReadByte();
                Data = br.ReadBytes(Length);

                int checksum = ID;
                foreach (byte b in Data)
                {
                    checksum ^= b;
                }
                Checksum = br.ReadByte();
                IsValid = (checksum == Checksum);
            }
            catch
            {
                IsValid = false;
            }
        }

        public override string ToString()
        {
            if (ID < 128)
                return "Header Block: [" + Length.ToString() + "]";
            else
                return "Data Block: [" + Length.ToString() + "]";
        }

    }
}
