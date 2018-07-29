using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    
    internal partial class Assembler
    {
        public static int EntryPoint = -1;
        public static bool Cancelled = false;
        public static event AssembledLineEventHandler AssembledLine;
        public static event AsseblyCompleteEventHandler AssemblyComplete;
        public static event AsseblyStartEventHandler AssemblyStart;
        static int currentPass = 0;
        public static int[] Memory = new int[0x10000];
        public static int ProgramCounter;
        public static int ProgramCounterOffset;
        public static int currentLine;
        public static string currentFileName;

        public static string currenttext = "";
        static int startaddress;

        public static int FirstAddress=-1;
        public static void Cancel()
        {
            Cancelled = true;
        }
        public static byte[] Bytes
        {
            get
            {

                List<byte> bytes = new List<byte>();
                for (int i = startaddress; i < ProgramCounter; i++)
                {

                    bytes.Add((byte)Memory[i]);

                }

                return bytes.ToArray();
            }
        }

        public static int PassNo;
        internal static bool Assemble(ScintillaZ80Monitor sourceDocument, string source)
        {
            currentFileName = sourceDocument.FileName;
            AssemblyStart?.Invoke(null, new AsseblyStartEventArgs());
            EntryPoint = -1;
            SymbolTable.Reset(currentFileName);
            //LogError(currentFileName, null,"Begin compilation",-1);
            PassNo = 1;
            Cancelled = false;

            bool success = Pass(PassNo, source);
            if (success)
            {
                PassNo = 2;
                success=Pass(PassNo, source);
            }
            if (EntryPoint == -1)
            {
                success = false;
                LogError(currentFileName, null,"No Entry point set ", -1);
            }
            if (success)
                LogError(currentFileName, null,"Compilation successful", -1);
            else
                LogError(currentFileName, null,"Compilation failed, Pass " + PassNo.ToString(), -1);
            OnAssemblyComplete(null, new AsseblyCompleteEventArgs(success));
            return success;


        }

        static StringBuilder sb;
        static List<string> bytes;
        static List<string> sourcelines;
        static int maxtext = 0;
        static bool IsTerminal = false;
        static bool Pass(int pass,string source)
        {
            FirstAddress = -1;
            //LogError(currentFileName, null,"Pass" + PassNo.ToString(), -1);
            maxtext = 0;
            sb = new StringBuilder();
            bytes = new List<string>();
            sourcelines = new List<string>();
            IsTerminal = false;
            Cancelled = false;
            currentPass = pass;
            terminate = false;
            currentLine = 0;
            ProgramCounter = 0;
            ErrorTable.Reset();
            Memory = new int[0x10000];
            
            for (int i = 0; i < 0x10000; i++)
                Memory[i] = -1;
            string[] lines;
            if (source.Contains("\r\n")) source = source.Replace("\r", "");
            //lines = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines = source.Split('\n');
            foreach (string line in lines)
            {
                if (ProgramCounter > 0xffff)
                    break;
                startaddress = ProgramCounter;
                currenttext = line;
                currentLine++;
                AssembleLine(line);
                if (pass == 2)
                {
                    sourcelines.Add(currenttext);
                    int length = ProgramCounter - startaddress;
                    StringBuilder textsb = new StringBuilder();
                    if (length > 0)
                    {
                        switch (lastTokenType)
                        {
                            case TokenType.Directive:
                            case TokenType.Preprocessor:
                                textsb.Append("    " + " ");
                                break;
                            case TokenType.Label:
                            case TokenType.Symbol:
                            case TokenType.Opcode:
                                textsb.Append(startaddress.ToString("X4") + " ");
                                break;
                            default:
                                textsb.Append(startaddress.ToString("X4") + " ");
                                break;

                        }
                        int newline = 0;
                        for (int i = 0; i < length; i++)
                        {
                            if (newline > 15)
                            {
                                sourcelines.Add("{''}");
                                currentLine++;
                                if (textsb.Length > maxtext)
                                    maxtext = textsb.Length + 1;
                                bytes.Add(textsb.ToString());
                                textsb = new StringBuilder();
                                textsb.Append((startaddress+i).ToString("X4") + " ");
                                newline = 0;
                            }
                            if (Memory[startaddress + i] != -1)
                            {
                                textsb.Append(Memory[startaddress + i].ToString("X2") + " ");
                                newline++;
                            }
                            if (textsb.Length > maxtext)
                                maxtext = textsb.Length + 1;

                        }
                    }
                    bytes.Add(textsb.ToString());
                    OnAssembledLine(null, new AssembledLineEventArgs(ProgramCounter - length, length));
                }
                //if (ErrorTable.HasErrors)                    break;
                if (IsTerminal) break;
                if (terminate) break;
                if (Cancelled) break;
            }
            return !ErrorTable.HasErrors(currentFileName);
        }

        static string justify(string text,int length)
        {
            return (text + new String(' ', length)).Substring(0, length);
        }
        static TokenType lastTokenType;
        static void AssembleLine(string line)
        {
            ProgramCounterOffset = 0;
            Z80Lexer lexer = new Z80Lexer();
            Token token = lexer.Lex(line);
            //if (token == null) return;
            if (ProgramCounter > 0xffff)
            {
                LogError(currentFileName, null,"Program Counter out of range: " + ProgramCounter.ToString("X4"));
                return;
            }
            lastTokenType = TokenType.Directive;
            if ((token != null))
                lastTokenType = token.TokenType;
            while (token != null)
            {

                switch (token.TokenType)
                {
                    case TokenType.Directive:
                        token = ProcessDirective(token);
                        break;

                    case TokenType.Preprocessor:
                        token = ProcessPreprocessor(token);
                        break;

                    case TokenType.Label:
                        token = AddLabel(token);
                        break;
                    case TokenType.Symbol:
                        //if (token.Previous == null)
                        token = AddLabel(token);
                        //else
                        //    LogError(currentFileName, token,"Invalid instruction");
                        break;

                    case TokenType.Opcode:
                        token = ProcessOpcode(token);
                        break;
                }
                //EOL(token);
                if (token!=null)
                    token = token.Next;
            }
        }
        static bool terminate = false;
        static Token ProcessPreprocessor(Token token)
        {
            return null;
        }
        static Token AddLabel(Token token)
        {
            if (token.IsResolved) return token;
            if (currentPass == 1)
            {
                Symbol symbol = SymbolTable.Find(currentFileName,token.Value.Replace(":", ""));
                if (symbol == null)
                {
                    if (token.Next == null)
                        SymbolTable.Add(currentFileName, token.Line, token, token.Value.Replace(":", ""), ProgramCounter);
                    else
                    {
                        if (token.Next.TokenType == TokenType.Opcode)
                            SymbolTable.Add(currentFileName, token.Line, token, token.Value.Replace(":", ""), ProgramCounter);
                        else
                        {
                            if (!token.Next.Value.ToUpper().Contains("EQU"))

                                SymbolTable.Add(currentFileName, token.Line, token, token.Value.Replace(":", ""), ProgramCounter);
                            else
                                SymbolTable.Add(currentFileName, - 1, token, token.Value.Replace(":", ""), token.Number);
                        }
                    }
                }
                else
                {
                    if (!symbol.Resolved)
                    {
                        if (token.Next == null)
                            symbol.Resolve(currentFileName, token.Number, token.Line);
                        else
                        {
                            if (token.Next.TokenType == TokenType.Opcode)
                                symbol.Resolve(currentFileName, ProgramCounter, token.Line);
                            else
                            {
                                if (!token.Next.Value.ToUpper().Contains("EQU"))
                                    symbol.Resolve(currentFileName, ProgramCounter, token.Line);
                            }
                        }
                            return token;
                    }
                    SymbolTable.Add(currentFileName ,- 1, token, token.Value.Replace(":", ""), token.Number);
                    LogError(currentFileName,token, "Duplicate Symbol");
                }
                
            }
            return token;
        }
        public static int ErrorStartLocation;
        public static int SymbolStartLocation;
        public static string Listing
        {
            get
            {

                int line = 0;
                sb = new StringBuilder();
                if (FirstAddress == -1) FirstAddress = 0;
                int index = 0;
                foreach (string s in bytes)
                {
                    string output = s + new string(' ', maxtext);
                    output = output.Substring(0, maxtext) + '\t';
                    output += sourcelines[index];
                    sb.AppendLine(output.TrimEnd());
                    index++;
                }
                sb.AppendLine();
                sb.Append((ProgramCounter - FirstAddress).ToString()+" bytes");
                sb.AppendLine();
                ErrorStartLocation = index;
                SymbolStartLocation = index;
                if (ErrorTable.HasErrors(currentFileName))
                {
                    string errors = ErrorTable.ErrorList(currentFileName);
                    sb.AppendLine(errors);
                    SymbolStartLocation += errors.Split('\n').Length;
                }
                sb.AppendLine(SymbolTable.Table(currentFileName));

                return sb.ToString(); ;
            }
        }
        public static string BytesList
        {
            get
            {
                sb = new StringBuilder();
                if (FirstAddress == -1) FirstAddress = 0;
                foreach (string s in bytes)
                {
                    
                    sb.AppendLine(s);
                }
                return sb.ToString(); ;
            }
        }

        protected static void OnAssemblyComplete(object sender, AsseblyCompleteEventArgs e)
        {
            AssemblyComplete?.Invoke(sender, e);
        }

        protected static void OnAssembledLine(object sender, AssembledLineEventArgs e)
        {
            AssembledLine?.Invoke(sender, e);
        }


    }
}
