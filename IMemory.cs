using System;
using System.Collections.Generic;
using System.Text;

namespace ZXStudio
{
    public delegate void MemoryChangedEventHandler(object o, MemoryChangedEventArgs e);
    public interface IMemory
    {
        event MemoryChangedEventHandler MemoryChanged;

        int this[int index] { get; set; }
        void Load(string filename, int startaddress);
        void Load(byte[] data, int startaddress);
        int Length { get; }
    }
    public class MemoryChangedEventArgs : EventArgs
    {
        public int Address;
        public int OldValue;
        public int NewValue;
        public MemoryChangedEventArgs(int address, int oldValue, int newValue)
        {
            Address = address;
            OldValue = oldValue;
            NewValue = newValue;
        }


    }
}
