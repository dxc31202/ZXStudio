using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    internal class DOpcode:ICloneable
    {
        public int Address;
        public int Operand;
        public int Offset;
        public List<byte> Bytes = new List<byte>();
        public bool IsDestination;
        public string Label
        {
            get
            {
                if (IsDestination)
                    return "L" + Address.ToString("X4") + " ";
                return "      ";
            }
        }



        public int Value;
        public string mnemonic;
        public string Mnemonic
        {
            get { return mnemonic; }
            set { mnemonic = value; }
        }
        public int Cycles;
        public int CyclesMet;
        public string Properties;
        public bool Documented;
        public OpcodeProperty OpcodePropertys = OpcodeProperty.None;
        DOpcode()
        {

        }
        internal DOpcode(int value, string mnemonic, int Cycles, int CyclesMet, string properties, bool documented)
        {
            Value = value;
            Mnemonic = mnemonic;
            Cycles = Cycles;
            CyclesMet = CyclesMet;
            Properties = properties;
            Documented = documented;
            string[] propertyitems = properties.Split(' ');
            foreach(string property in propertyitems)
            {

                if(property.Trim().Length>0)
                {
                    switch(property.Trim())
                    {

                        case "Branch": OpcodePropertys |= OpcodeProperty.Branch; break;
                        case "Call": OpcodePropertys |= OpcodeProperty.Call; break;
                        case "Conditional": OpcodePropertys |= OpcodeProperty.Conditional; break;
                        case "Port": OpcodePropertys |= OpcodeProperty.Port; break;
                        case "Indirect": OpcodePropertys |= OpcodeProperty.Indirect; break;
                        case "Restart": OpcodePropertys |= OpcodeProperty.Restart; break;
                        case "Return": OpcodePropertys |= OpcodeProperty.Return; break;
                        case "Terminal": OpcodePropertys |= OpcodeProperty.Terminal; break;
                        case "Indexed": OpcodePropertys |= OpcodeProperty.Indexed; break;
                    }
                }
            }
        }
        internal bool IsTerminal { get { return ((OpcodePropertys & OpcodeProperty.Terminal) == OpcodeProperty.Terminal); } }
        internal bool IsBranch{ get { return ((OpcodePropertys & OpcodeProperty.Branch) == OpcodeProperty.Branch); } }

        public string Disassembly
        {
            get
            {
                string sbytes = "";
                string chars = "";
                // Build bytes string - 25 bytes long
                for (int i = 0; i < Bytes.Count; i++)
                {
                    if (Bytes[i] < 32 || Bytes[i] > 127)
                        chars += ".";
                    else
                        chars += Convert.ToChar(Bytes[i]);
                    sbytes += Bytes[i].ToString("X2") + " ";
                }
                sbytes += new string(' ', 25);
                sbytes = sbytes.Substring(0, 25);

                // Build mnemonics string - 32 bytes long
                string properties = "";
                string mnemonic = "";
                if (Mnemonic == null)
                    // Belts and braces; shouldn't really get here.
                    mnemonic = "DEFB #" + sbytes.Trim().Replace(" ", " #");
                else
                {
                    mnemonic = Mnemonic;
                    properties = OpcodePropertys.ToString();
                    // Perform value subsistution
                    // Address
                    if (Mnemonic.Contains("$aa"))
                        mnemonic = mnemonic.Replace("$aa", "L" + Operand.ToString("X4"));
                    // Word
                    else if (Mnemonic.Contains("$nn"))
                        mnemonic = mnemonic.Replace("$nn", "#" + Operand.ToString("X4"));
                    // Displacement
                    else if (Mnemonic.Contains("$d"))
                        mnemonic = mnemonic.Replace("$d", "L" + Operand.ToString("X4"));
                    // Byte
                    else if (Mnemonic.Contains("$b"))
                        mnemonic = mnemonic.Replace("$b", "#" + Operand.ToString("X2"));
                    // Index offset 1
                    if (Mnemonic.Contains("$o"))
                        mnemonic = mnemonic.Replace("$o", ((sbyte)Offset).ToString("+0;-#"));
                    // Index offset 2
                    if (Mnemonic.Contains("$x"))
                        mnemonic = mnemonic.Replace("$x", ((sbyte)Offset).ToString("+0;-#"));
                }
                mnemonic += new string(' ', 32);
                mnemonic = mnemonic.Substring(0, 32);

                // Build the output string 
                return mnemonic ;
            }
        }
        public override string ToString()
        {
            string sbytes = "";
            string chars = "";
            // Build bytes string - 25 bytes long
            for (int i = 0; i < Bytes.Count; i++)
            {
                if (Bytes[i] < 32 || Bytes[i] > 127)
                    chars += ".";
                else
                    chars += Convert.ToChar(Bytes[i]);
                sbytes += Bytes[i].ToString("X2") + " ";
            }
            sbytes += new string(' ', 25);
            sbytes = sbytes.Substring(0, 25);

            // Build mnemonics string - 32 bytes long
            string properties = "";
            string mnemonic = "";
            if (Mnemonic == null)
                // Belts and braces; shouldn't really get here.
                mnemonic = "DEFB #" + sbytes.Trim().Replace(" ", " #");
            else
            {
                mnemonic = Mnemonic;
                properties = OpcodePropertys.ToString();
                // Perform value subsistution
                // Address
                if (Mnemonic.Contains("$aa"))
                    mnemonic = mnemonic.Replace("$aa", "L" + Operand.ToString("X4"));
                // Word
                else if (Mnemonic.Contains("$nn"))
                    mnemonic = mnemonic.Replace("$nn", "#" + Operand.ToString("X4"));
                // Displacement
                else if (Mnemonic.Contains("$d"))
                    mnemonic = mnemonic.Replace("$d", "L" + Operand.ToString("X4"));
                // Byte
                else if (Mnemonic.Contains("$b"))
                    mnemonic = mnemonic.Replace("$b", "#" + Operand.ToString("X2"));
                // Index offset 1
                if (Mnemonic.Contains("$o"))
                    mnemonic = mnemonic.Replace("$o", ((sbyte)Offset).ToString("+0;-#"));
                // Index offset 2
                if (Mnemonic.Contains("$x"))
                    mnemonic = mnemonic.Replace("$x", ((sbyte)Offset).ToString("+0;-#")); 
            }
            mnemonic += new string(' ', 32);
            mnemonic = mnemonic.Substring(0, 32);

            // Build the output string 
            if(ProduceCodes)
                return Address.ToString("X4") + " " + sbytes + Label + mnemonic + "; " + chars + " ";
            return Label + mnemonic + "; " + chars + " ";
        }
        public static bool ProduceCodes = true;
        public object Clone()
        {
            DOpcode opcode = new DOpcode();
            opcode.Value = this.Value;
            opcode.Mnemonic = this.Mnemonic;
            opcode.Cycles = this.Cycles;
            opcode.CyclesMet = this.CyclesMet;
            opcode.Properties = this.Properties;
            opcode.Documented = this.Documented;
            opcode.OpcodePropertys = this.OpcodePropertys;
            return opcode;
        }
    }

}
