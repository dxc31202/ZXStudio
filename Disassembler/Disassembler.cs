using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZXStudio
{
    public enum CallingType
    {
        Return,
        Djnz,
        Jp,
        JpCC,
        Call,
        CallCC,
        Jr,
        JrCC
    }
    class CallingDetail
    {
        public int DestinationAddress;
        public int SourceAddress;
        public CallingType CallType;

        public CallingDetail(int destinationAddress, int sourceAddress, CallingType callType)
        {
            DestinationAddress = destinationAddress;
            SourceAddress = sourceAddress;
            CallType = callType;
        }
    }

    class Disassembler
    {
        public static bool DisassembleROM = false;
        public static SourceLine[] SourceLines = new SourceLine[0x10000];
        static bool Code;
        static bool Data;
        static int StartAddress, LowestAddress;
        public static string DisassembleLine(byte[] memory, int address)
        {
            return DecodeOpcode(memory, address);
        }
        public static void Disassemble(byte[] memory)
        {
            if (memory == null) return;
            StartAddress = LowestAddress = 0;
            Destination = new bool[0x10000];
            Visited = new bool[0x10000];
            Disassembly = new object[0x10000];
            for (int i = 0; i < 0x10000; i++)
            {
                if (i >= memory.Length)
                    break;
                Disassembly[i] = memory[i];
            }
            Code = true;
            Data = true;
        }
        public static bool ProduceCode = true;
        static byte[] copyMemory;
        public static TreeNode Disassemble(int startaddress, List<int> addresses, int lowestaddress, int length,byte[] memory, bool restart, bool code, bool data)
        {
            copyMemory = memory;
            LowestAddress = lowestaddress;
            StartAddress = startaddress;
            TreeNode node = new TreeNode("Root");
            Code = code;
            Data = data;

            if (restart)
            {
                for (int i = 0; i < 0x10000; i++)
                    Disassembly[i] = -1;
                Destination = new bool[0x10000];
                Visited = new bool[0x10000];

                //length = 65535 - lowestaddress;
                if (lowestaddress + length > 0xffff)
                    length = 0x10000 - lowestaddress;

                for (int i = lowestaddress; i < lowestaddress+ length; i++)
                {
                    Disassembly[i] = memory[i];
                }
            }

            Stack<CallingDetail> CallStack;
            CallStack = new Stack<CallingDetail>();
            CallStack.Push(new CallingDetail(startaddress, 0,CallingType.Jp));
            foreach (int add in addresses)
                CallStack.Push(new CallingDetail(add, 0, CallingType.Jp));
            int max = lowestaddress + length;
            IEnumerable<CallingDetail> list = CallStack.Reverse();
            foreach (CallingDetail callingDetail in list)
            {

                int add = callingDetail.DestinationAddress;
                Destination[callingDetail.DestinationAddress] = true;

                Disassemble(callingDetail, memory, node);
            }
            for (int i = 0; i < 0x10000; i++)
                if (i <= max)
                    if (Destination[i])
                        if (Disassembly[i] is DOpcode)
                            ((DOpcode)Disassembly[i]).IsDestination = true;


            return node;
        }
        static ToolTip tooltip = new ToolTip();
        static string[] tooltiptext = { "Return", "DJNZ Loop", "Direct Jump", "Conditional Jump", "Direct Call", "Conditional Call", "Direct Relative Jump", "Conditional Relative Jump" };
        public static void Disassemble(CallingDetail callDetail, byte[] memory, TreeNode node)
        {
            if (callDetail.DestinationAddress > memory.Length) return ;
            
            int address = callDetail.DestinationAddress;
            int imageindex = (int)callDetail.CallType;
            node = PopulateTree(callDetail, node);
            if (Visited[callDetail.DestinationAddress]) return;
            Stack<CallingDetail> CallStack = new Stack<CallingDetail>();
            while (true)
            {
                try
                {
                    if (!DisassembleROM)
                        if (address < 0x4000)
                            break;
                    if (address > 0xffff)
                                break;
                    //if (address == 0x162c)
                    //    Console.Write("");

                    //
                    // Decode one opcode
                    //
                    DOpcode opcode;
                    List<byte> opcodebytes = new List<byte>();
                    int startaddress = address;
                    Visited[address] = true;
                    if (address + 1 > memory.Length)
                        return ;
                    byte opcodebyte = (byte)memory[address++];
                    opcodebytes.Add(opcodebyte);
                    switch (opcodebyte)
                    {
                        case 0xCB:
                            {
                                opcodebyte = (byte)memory[address++];
                                opcodebytes.Add(opcodebyte);
                                opcode = (DOpcode)Opcodes.CBPrefix[opcodebyte].Clone();
                            }
                            break;
                        case 0xED:
                            {
                                opcodebyte = (byte)memory[address++];
                                opcodebytes.Add(opcodebyte);
                                opcode = (DOpcode)Opcodes.EDPrefix[opcodebyte].Clone();
                            }
                            break;
                        case 0xDD:
                            {
                                opcodebyte = (byte)memory[address++];
                                opcodebytes.Add(opcodebyte);
                                if (opcodebyte == 0xCB)
                                {
                                    byte offset = (byte)memory[address++];
                                    opcodebytes.Add(offset);
                                    opcodebyte = (byte)memory[address++];
                                    opcodebytes.Add(opcodebyte);
                                    opcode = (DOpcode)Opcodes.DDCBPrefix[opcodebyte].Clone();
                                    opcode.Offset = offset;
                                }
                                else
                                    opcode = (DOpcode)Opcodes.DDPrefix[opcodebyte].Clone();
                            }
                            break;
                        case 0xFD:
                            {
                                opcodebyte = (byte)memory[address++];
                                opcodebytes.Add(opcodebyte);
                                if (opcodebyte == 0xCB)
                                {
                                    byte offset = (byte)memory[address++];
                                    opcodebytes.Add(offset);
                                    opcodebyte = (byte)memory[address++];
                                    opcodebytes.Add(opcodebyte);
                                    opcode = (DOpcode)Opcodes.FDCBPrefix[opcodebyte].Clone();
                                    opcode.Offset = offset;
                                }
                                else
                                    opcode = (DOpcode)Opcodes.FDPrefix[opcodebyte].Clone();
                            }
                            break;
                        default:
                            opcode = (DOpcode)Opcodes.NoPrefix[opcodebyte].Clone();
                            break;
                    }
                    
                    //
                    // Decode operands
                    //
                    if (opcode.Mnemonic != null)
                    {
                        // Index offset i.e. (IX+$o)
                        if (opcode.Mnemonic.Contains("$o"))
                        {
                            sbyte offset = (sbyte)memory[address++];
                            opcodebytes.Add((byte)offset);
                            opcode.Offset = (byte)offset;
                        }
                        // Word; i.e. LD HL,$nn
                        if (opcode.Mnemonic.Contains("$nn"))
                        {
                            byte lo = (byte)memory[address++];
                            byte hi = (byte)memory[address++];
                            opcodebytes.Add(lo);
                            opcodebytes.Add(hi);
                            int word = ((hi << 8) | lo);
                            opcode.Operand = word;
                        }
                        // Address; i.e. CALL $aa
                        CallingDetail callingDetail = null;
                        if (opcode.Mnemonic.Contains("$aa"))
                        {
                            byte lo = (byte)memory[address++];
                            byte hi = (byte)memory[address++];
                            opcodebytes.Add(lo);
                            opcodebytes.Add(hi);
                            int word = ((hi << 8) | lo);
                            Destination[word] = true;
                            opcode.Operand = word;

                            if (opcode.Mnemonic.StartsWith("JP"))
                            {
                                if ((opcode.OpcodePropertys & OpcodeProperty.Conditional) == OpcodeProperty.Conditional)
                                    callingDetail = new CallingDetail(word, startaddress, CallingType.JpCC);
                                else
                                    callingDetail = new CallingDetail(word, startaddress, CallingType.Jp);
                            }
                            else
                            {
                                if ((opcode.OpcodePropertys & OpcodeProperty.Conditional) == OpcodeProperty.Conditional)
                                    callingDetail = new CallingDetail(word, startaddress, CallingType.CallCC);
                                else
                                    callingDetail = new CallingDetail(word, startaddress, CallingType.Call);
                            }
                            if (word != 0x335E)
                            {
                                CallStack.Push(callingDetail);
                            }
                            else
                            {
                                Disassembly[startaddress] = opcode;
                                break;
                            }
                        }
                        // Displacement i.e. DJNZ $d
                        else if (opcode.Mnemonic.Contains("$d"))
                        {
                            sbyte displacement = (sbyte)memory[address++];
                            opcodebytes.Add((byte)displacement);
                            int dispaddress = address + displacement;

                            opcode.Operand = dispaddress;
                            Destination[dispaddress] = true;

                            if (opcode.Mnemonic.StartsWith("JR"))
                                if ((opcode.OpcodePropertys & OpcodeProperty.Conditional) == OpcodeProperty.Conditional)
                                    callingDetail = new CallingDetail(dispaddress, startaddress, CallingType.JrCC);
                                else
                                    callingDetail = new CallingDetail(dispaddress, startaddress, CallingType.Jr);
                            else
                                callingDetail = new CallingDetail(dispaddress, startaddress, CallingType.Djnz);

                            CallStack.Push(callingDetail);
                        }
                        // Byte; i.e. LD A,$b
                        else if (opcode.Mnemonic.Contains("$b"))
                        {
                            byte value = (byte)memory[address++];
                            opcodebytes.Add(value);
                            opcode.Operand = value;
                        }

                    }
                    opcode.Address = startaddress;
                    opcode.Bytes = opcodebytes;
                    //Visited[startaddress] = true;

                    // RST 08H; THE 'ERROR' RESTART (0xcf)
                    if (opcode.Value == 0xcf)
                    {
                        Disassembly[startaddress] = opcode;
                        Visited[address++] = true;
                        //CallingDetail callingDetail = new CallingDetail(address, address, CallingType.Jp);
                        //CallStack.Push(callingDetail);
                        break; 
                    }

                    // RST 28H; THE 'CALCULATE' RESTART (0xef)
                    if (opcode.Value == 0xef)
                    {
                        Disassembly[startaddress] = opcode;
                        while (true)
                        {
                            Visited[address++] = true;
                            // END WITH 0x38
                            if (memory[address] == 0x38)
                                if (memory[address - 1] != 0x8f)
                                    break;
                        }
                        Visited[address++] = true;
                        CallingDetail callingDetail = new CallingDetail(address, address, CallingType.Jp);
                        CallStack.Push(callingDetail);
                        break;
                    }
                    // Any other opcode
                    for (int i = startaddress; i < startaddress + opcodebytes.Count; i++)
                    {
                        Disassembly[i] = null;
                    }
                    
                    Disassembly[startaddress] = opcode;
                    if (opcode.IsTerminal) break;
                }
                catch { break; }
            }
            IEnumerable<CallingDetail> list = CallStack.Reverse();
            foreach (CallingDetail callingDetail in list)
            {
                Disassemble(callingDetail, memory, node);
            }
        }

        static string DecodeOpcode(byte[] memory, int address)
        {
            //
            // Decode one opcode
            //
            DOpcode opcode;
            List<byte> opcodebytes = new List<byte>();
            int startaddress = address;
            Visited[address] = true;
            if (address + 1 > memory.Length)
                return "";
            byte opcodebyte = memory[address++];
            opcodebytes.Add(opcodebyte);
            switch (opcodebyte)
            {
                case 0xCB:
                    {
                        opcodebyte = memory[address++];
                        opcodebytes.Add(opcodebyte);
                        opcode = (DOpcode)Opcodes.CBPrefix[opcodebyte].Clone();
                    }
                    break;
                case 0xED:
                    {
                        opcodebyte = memory[address++];
                        opcodebytes.Add(opcodebyte);
                        opcode = (DOpcode)Opcodes.EDPrefix[opcodebyte].Clone();
                    }
                    break;
                case 0xDD:
                    {
                        opcodebyte = memory[address++];
                        opcodebytes.Add(opcodebyte);
                        if (opcodebyte == 0xCB)
                        {
                            byte offset = memory[address++];
                            opcodebytes.Add(offset);
                            opcodebyte = memory[address++];
                            opcodebytes.Add(opcodebyte);
                            opcode = (DOpcode)Opcodes.DDCBPrefix[opcodebyte].Clone();
                            opcode.Offset = offset;
                        }
                        else
                            opcode = (DOpcode)Opcodes.DDPrefix[opcodebyte].Clone();
                    }
                    break;
                case 0xFD:
                    {
                        opcodebyte = memory[address++];
                        opcodebytes.Add(opcodebyte);
                        if (opcodebyte == 0xCB)
                        {
                            byte offset = memory[address++];
                            opcodebytes.Add(offset);
                            opcodebyte = memory[address++];
                            opcodebytes.Add(opcodebyte);
                            opcode = (DOpcode)Opcodes.FDCBPrefix[opcodebyte].Clone();
                            opcode.Offset = offset;
                        }
                        else
                            opcode = (DOpcode)Opcodes.FDPrefix[opcodebyte].Clone();
                    }
                    break;
                default:
                    opcode = (DOpcode)Opcodes.NoPrefix[opcodebyte].Clone();
                    break;
            }

            //
            // Decode operands
            //
            if (opcode.Mnemonic != null)
            {
                // Index offset i.e. (IX+$o)
                if (opcode.Mnemonic.Contains("$o"))
                {
                    sbyte offset = (sbyte)memory[address++];
                    opcodebytes.Add((byte)offset);
                    opcode.Offset = (byte)offset;
                }
                // Word; i.e. LD HL,$nn
                if (opcode.Mnemonic.Contains("$nn"))
                {
                    byte lo = memory[address++];
                    byte hi = memory[address++];
                    opcodebytes.Add(lo);
                    opcodebytes.Add(hi);
                    int word = ((hi << 8) | lo);
                    opcode.Operand = word;
                }
                // Address; i.e. CALL $aa
                CallingDetail callingDetail = null;
                if (opcode.Mnemonic.Contains("$aa"))
                {
                    byte lo = memory[address++];
                    byte hi = memory[address++];
                    opcodebytes.Add(lo);
                    opcodebytes.Add(hi);
                    int word = ((hi << 8) | lo);
                    Destination[word] = true;
                    opcode.Operand = word;

                    if (opcode.Mnemonic.StartsWith("JP"))
                    {
                        if ((opcode.OpcodePropertys & OpcodeProperty.Conditional) == OpcodeProperty.Conditional)
                            callingDetail = new CallingDetail(word, startaddress, CallingType.JpCC);
                        else
                            callingDetail = new CallingDetail(word, startaddress, CallingType.Jp);
                    }
                    else
                    {
                        if ((opcode.OpcodePropertys & OpcodeProperty.Conditional) == OpcodeProperty.Conditional)
                            callingDetail = new CallingDetail(word, startaddress, CallingType.CallCC);
                        else
                            callingDetail = new CallingDetail(word, startaddress, CallingType.Call);
                    }
                }
                // Displacement i.e. DJNZ $d
                else if (opcode.Mnemonic.Contains("$d"))
                {
                    sbyte displacement = (sbyte)memory[address++];
                    opcodebytes.Add((byte)displacement);
                    int dispaddress = address + displacement;

                    opcode.Operand = dispaddress;
                    Destination[dispaddress] = true;

                    if (opcode.Mnemonic.StartsWith("JR"))
                        if ((opcode.OpcodePropertys & OpcodeProperty.Conditional) == OpcodeProperty.Conditional)
                            callingDetail = new CallingDetail(dispaddress, startaddress, CallingType.JrCC);
                        else
                            callingDetail = new CallingDetail(dispaddress, startaddress, CallingType.Jr);
                    else
                        callingDetail = new CallingDetail(dispaddress, startaddress, CallingType.Djnz);

                }
                // Byte; i.e. LD A,$b
                else if (opcode.Mnemonic.Contains("$b"))
                {
                    byte value = memory[address++];
                    opcodebytes.Add(value);
                    opcode.Operand = value;
                }

            }
            return opcode.Disassembly;
        }
        public static StringBuilder Listing
        {
            get
            {
                if (Disassembly == null) return new StringBuilder();
                string currentline = "";
                SourceLines = new SourceLine[0x10000];
                string saddress = "";
                string sbytes = "";
                StringBuilder sb = new StringBuilder();

                string filler = new string(' ', 6 + (ProduceCode ? 30 : 0));
                sb.AppendLine(filler + ";-----------------------------------------------");
                int count = 0;
                int firstNoneData;
                for (int address = LowestAddress; address < Disassembly.Length; address++)
                {
                    if (!DisassembleROM)
                        if (address < 0x4000)
                            continue;
                    object o = Disassembly[address];

                    if (o == null) continue;
                    if (o is int)
                    {
                        if ((int)o == -1)
                            continue;
                    }

                    SourceLines[address].Address = address;

                    if (o is DOpcode)
                    {
                        if (Code)
                        {
                            DOpcode opcode = o as DOpcode;
                            if (opcode.Value == 0xcf)
                            {


                                if (!IsNumber(Disassembly[address])) continue;
                                sb.AppendLine(filler + "; Error Restart");
                                sb.AppendLine(opcode.ToString());
                                address++;
                                if ((int)Disassembly[address] == -1)
                                    Disassembly[address] = copyMemory[address];

                                // Build the line
                                saddress = sbytes = "";
                                if (ProduceCode)
                                {
                                    saddress = address.ToString("X4") + " ";
                                    sbytes = (Convert.ToByte(Disassembly[address])).ToString("X2") + new string(' ', 29);
                                }
                                SourceLines[address].Disassembly = (Convert.ToByte(Disassembly[address])).ToString("X2");

                                currentline = saddress + sbytes + "DEFB " + "#" + (Convert.ToByte(Disassembly[address])).ToString("X2") + new string(' ', 24) + " ; ";

                                SourceLines[address].Start = sb.Length;
                                SourceLines[address].Text = currentline;
                                SourceLines[address].Length = currentline.Length;
                                sb.AppendLine(currentline);

                                continue;
                            }
                            if (opcode.Value == 0xef || (opcode.IsBranch && opcode.Operand == 0x335e))
                            {
                                if (opcode.Operand == 0x335e)
                                {
                                    address++;
                                    address++;
                                }
                                sb.AppendLine(filler + "; Calculator Restart");
                                sb.AppendLine(opcode.ToString());
                                address++;
                                while (Disassembly[address] is int)
                                {
                                    if ((int)Disassembly[address] == -1)
                                        Disassembly[address] = copyMemory[address];

                                    // Build the line
                                    string comment = getfpname(((int)Disassembly[address]).ToString("X2"));
                                    saddress = sbytes = "";
                                    if (ProduceCode)
                                    {
                                        saddress = address.ToString("X4") + " ";
                                        sbytes = ((int)Disassembly[address]).ToString("X2") + new string(' ', 29);
                                    }
                                    currentline = saddress + sbytes + "DEFB " + "#" + ((int)Disassembly[address]).ToString("X2") + new string(' ', 24) + " ; " + comment + " ";
                                    SourceLines[address].Start = sb.Length;
                                    SourceLines[address].Text = currentline;
                                    SourceLines[address].Length = currentline.Length;
                                    sb.AppendLine(currentline);
                                    if(address<0xffff)
                                        if ((int)Disassembly[address] == 0x38 || (Disassembly[address + 1] is int && (int)Disassembly[address + 1] == 0xef))
                                            break;
                                    address++;
                                }
                                sb.AppendLine(filler + "; End Calculator");
                                continue;
                            }
                            else
                            {
                                if (count == 0)
                                {
                                    sb.AppendLine(filler + "; Procedure: " + opcode.Label);
                                    sb.AppendLine(filler + ";-----------------------------------------------");
                                }
                                count++;
                                currentline = opcode.ToString();
                                SourceLines[address].Start = sb.Length;
                                SourceLines[address].Text = currentline;
                                SourceLines[address].Length = currentline.Length;
                                sb.AppendLine(currentline);
                                if (opcode.IsTerminal)
                                {
                                    count = 0;
                                    sb.AppendLine();
                                    sb.AppendLine(filler + ";-----------------------------------------------");
                                }
                                address += opcode.Bytes.Count - 1;
                            }
                            continue;
                        }
                    }

                    if (o is int)
                    {

                        if (Data)
                        {
                            if (count == 0)
                            {
                                sb.AppendLine(filler + "; Data");
                                sb.AppendLine(filler + ";-----------------------------------------------");
                            }
                            int c = 0;
                            int add = address;
                            string bytes1 = "";
                            string bytes2 = "";
                            string chars = "";
                            string label = "L" + add.ToString("X4") + " ";
                            while (Disassembly[address] is int)
                            {
                                if ((int)Disassembly[address] == -1)
                                    Disassembly[address] = copyMemory[address];
                                //if ((int)Disassembly[address] == -1)
                                //{
                                //    address++;
                                //    if (address > 0xffff) break;
                                //    continue;
                                //}
                                byte b = Convert.ToByte(Disassembly[address]);

                                if (b > 31 && b < 128)
                                    chars += Convert.ToChar(b);
                                else
                                    chars += ".";
                                bytes1 += b.ToString("X2") + " ";
                                bytes2 += "#" + b.ToString("X2") + " ";
                                address++;
                                c++;
                                if (c == 8)
                                {
                                    bytes1 = bytes1 + new string(' ', 24);
                                    saddress = sbytes = "";
                                    if (ProduceCode)
                                    {
                                        saddress = add.ToString("X4") + " ";
                                        sbytes = bytes1.Substring(0, 24) + " ";
                                    }
                                    // Build the line
                                    sb.AppendLine(
                                        saddress + sbytes +
                                         label + "DEFB " + bytes2 + " " +
                                         "; " + chars + " "
                                    );
                                    label = "      ";
                                    add = address;
                                    bytes1 = bytes2 = chars = "";
                                    c = 0;
                                }
                                if (address > 0xffff) break;

                            }
                            if (bytes1.Length > 0)
                            {
                                bytes1 = bytes1 + new string(' ', 24);
                                // Build the line
                                saddress = sbytes = "";
                                if (ProduceCode)
                                {
                                    saddress = add.ToString("X4") + " ";
                                    sbytes = bytes1.Substring(0, 24) + " ";
                                }
                                sb.AppendLine(
                                        saddress + sbytes +
                                     label + "DEFB " + bytes2 + " " +
                                     "; " + chars + " "
                                    );
                            }
                            sb.AppendLine();
                            sb.AppendLine(filler + ";-----------------------------------------------");

                            address--;
                        }
                    }

                }
                return sb;
            }
        }
        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
        public static void Reset()
        {
            Disassembly = new object[0x10000];
        }
        static bool[] Destination = new bool[0x10000];
        static bool[] Visited = new bool[0x10000];
        public static object[] Disassembly;

        static string[] fpxref = null;
        static string getfpname(string value)
        {
            if (fpxref == null) fpxref = Properties.Resources.XRefFPRoutines.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string row in fpxref)
            {
                string[] item = row.Split('\t');
                if (item[0].ToUpper() == value)
                    return " [" + item[1] + "] ";
            }
            return " [" + value + "] ";
        }

        static TreeNode PopulateTree(CallingDetail CallingDetail, TreeNode node)
        {
            int sourceaddress = CallingDetail.SourceAddress;
            int address = CallingDetail.DestinationAddress;
            int imageindex = (int)CallingDetail.CallType;
            if (!DisassembleROM)
                if (address < 0x4000)
                    return node;
                //TreeNode[] tn = node.Nodes.Find(address.ToString("X4"), false);
                //if (tn.Length == 0)
                {
                string text = sourceaddress.ToString("X4") ;
                node = node.Nodes.Add(text, text, imageindex, imageindex);
                node.ToolTipText = tooltiptext[imageindex];
                    node.Tag += CallingDetail.DestinationAddress.ToString("X4");
                node.ToolTipText += " To " + node.Tag.ToString();

                text = address.ToString("X4");
                node = node.Nodes.Add(text, text, 0, 0);
                node.ToolTipText = "Procedure entry point: "+ tooltiptext[imageindex];
                node.ImageIndex = node.SelectedImageIndex= 0;
                if (node.Tag == null)
                    node.Tag += CallingDetail.SourceAddress.ToString("X4");
                else
                    node.Tag += ", " + CallingDetail.SourceAddress.ToString("X4");
                node.ToolTipText += " From " + node.Tag.ToString();
            }
            //else
            {
            //    node = tn[0];
            //    node.Text += ">";
            }
            return node;
        }

    }
}
