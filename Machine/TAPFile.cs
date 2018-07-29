using System.Collections.Generic;
using System.IO;
/*
    •TAP Format

    The .tap files contain blocks of tape-saved data. 
    All blocks start with two bytes specifying how many bytes will follow (not counting the two length bytes). 
    Then raw tape data follows, including the flag and checksum bytes. 
    Note that it is possible to join .tap files by simply stringing them together, for example COPY /B FILE1.TAP + FILE2.TAP ALL.TAP
*/
namespace ZXStudio
{
    public class TAPFile
    {
        public delegate void FoundBlockEvent(TAPBlock block);
        public event FoundBlockEvent FoundBlock;

        void OnFoundBlock(TAPBlock block)
        {
            Blocks.Add(block);
            if (FoundBlock != null)
                FoundBlock(block);

        }
        public TAPFile()
        {
        }

        public int CurrentBlock;
        public int BlockCount { get { return Blocks.Count; } }
        public TAPBlock NextBlock
        {
            get
            {
                if (CurrentBlock >= Blocks.Count)
                    CurrentBlock = 0;
                return Blocks[CurrentBlock++];
            }
        }
        public List<TAPBlock> Blocks = new List<TAPBlock>();
        public void Load(string filename)
        {
            CurrentBlock = 0;
            int n = 0;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        TAPBlock block = new TAPBlock(br, n++);
                        OnFoundBlock(block);
                    }
                }
            }
        }
    }
}

