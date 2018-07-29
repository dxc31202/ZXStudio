using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token LastToken;
        static Token NextToken(Token token)
        {
            LastToken = token;
            if (token.Next == null)
                LogError(currentFileName, token, "Unexpected end of line");
            return token.Next;
        }

        static bool CheckComma(Token token, bool logerror)
        {
            if (token.Value == ",")
            {
                ProgramCounterOffset++;
                token = token.Next;
                return true;
            }
            return false;
            //if (logerror) LogError(currentFileName, token, "Expected Comma");
            //return false;
        }
        static bool CheckCloseBracket(Token token, bool logerror)
        {
            if (token.Value == ")") { token = token.Next; return true; }
            if (logerror) LogError(currentFileName,token, "Expected Close Bracket");
            return false;
        }
        static bool CheckOpenBracket(Token token, bool logerror)
        {
            if (token.Value == "(") { token = token.Next; return true; }
            if (logerror) LogError(currentFileName, token, "Expected Open Bracket");
            return false;
        }
        static RegisterType GetRegisterType(Token token)
        {
            switch (token.Value.ToUpper())
            {
                case "A": return RegisterType.A;
                case "F": return RegisterType.F;
                case "B": return RegisterType.B;
                case "C": return RegisterType.C;
                case "D": return RegisterType.D;
                case "E": return RegisterType.E;
                case "H": return RegisterType.H;
                case "L": return RegisterType.L;
                case "(": return RegisterType.BRACKET;
                case "I": return RegisterType.I;
                case "R": return RegisterType.R;
                case "AF": return RegisterType.AF;
                case "BC": return RegisterType.BC;
                case "DE": return RegisterType.DE;
                case "HL": return RegisterType.HL;
                case "IX": return RegisterType.IX;
                case "IY": return RegisterType.IY;
                case "SP": return RegisterType.SP;
                case "PC": return RegisterType.PC;
                case "XH": return RegisterType.XH;
                case "XL": return RegisterType.XL;
                case "YH": return RegisterType.YH;
                case "IYH": return RegisterType.YH;
                case "YL": return RegisterType.YL;
                case "IYL": return RegisterType.YL;
                case "IXH": return RegisterType.XH;
                case "IXL": return RegisterType.XL;

            }
            return RegisterType.Unknown;
        }
        static Condition GetCondition(Token token)
        {
            switch (token.Value.ToUpper())
            {
                case "NZ": return Condition.NZ;
                case "Z": return Condition.Z;
                case "NC": return Condition.NC;
                case "C": return Condition.C;
                case "PO": return Condition.PO;
                case "PE": return Condition.PE;
                case "P": return Condition.P;
                case "M": return Condition.M;
            }
            return Condition.Unknown;
        }
        static Condition GetConditionJR(Token token)
        {
            switch (token.Value.ToUpper())
            {
                case "NZ": return Condition.NZ;
                case "Z": return Condition.Z;
                case "NC": return Condition.NC;
                case "C": return Condition.C;
            }
            return Condition.Unknown;
        }
        /// <summary>
        /// Handle varions:
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="registerType"></param>
        /// <param name="baseopcode"></param>
        /// <returns></returns>
        static Token RegMemOrByte(Token token, RegisterType registerType, int baseopcode)
        {
            if (registerType != RegisterType.BRACKET && registerType != RegisterType.Unknown)
            {
                if (token.Next != null)
                    if ((token = NextToken(token)) == null) return token.Next;
                if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
                //if (registerType != RegisterType.A)
                //{
                //    LogError(currentFileName, token, token.Previous.Value + " is not allowed here");
                //    return token.Next;
                //}
                //token = NextToken(token);
                registerType = GetRegisterType(token);
            }
            switch (registerType)
            {
                case RegisterType.Unknown:  // Byte
                    StoreOpcode(baseopcode + 70);
                    int operand = GetValue(ref token);

                    if (!StoreByte(operand))
                        LogError(currentFileName, token, "Number too large");
                    return token.Next;
                case RegisterType.BRACKET:  // (HL or (IX or (IY
                    if ((token = NextToken(token)) == null) return token.Next;
                    int opcode = baseopcode + (int)registerType;
                    switch (GetRegisterType(token))
                    {
                        case RegisterType.IX:
                            {
                                int offset = 0;
                                if ((token = NextToken(token)) == null) return token.Next;
                                if (token.IsEquation)
                                {
                                    offset = (int)Parser.Evaluate(currentFileName, token.Value, currentLine, ProgramCounterOffset, token);
                                    if (currentPass == 2)
                                        if (!Parser.IsValid)
                                        {
                                            LogError(currentFileName, token, "Invalid Offset");
                                            return token.Next;
                                        }

                                }
                                else if (token.Value != ")")
                                {
                                    LogError(currentFileName, token, "Expecting +/- or close bracket");
                                    return token.Next;
                                }
                                StoreIXOpcodeDisplacement(opcode, offset);
                                if (token.Value != ")")
                                    if ((token = NextToken(token)) == null) return token.Next;
                                break;
                            }
                        case RegisterType.IY:
                            {
                                int offset = 0;
                                if ((token = NextToken(token)) == null) return token.Next;
                                if (token.IsEquation)
                                {
                                    offset = (int)Parser.Evaluate(currentFileName, token.Value, currentLine, ProgramCounterOffset, token);
                                    if (currentPass == 2)
                                        if (!Parser.IsValid)
                                        {
                                            LogError(currentFileName, token, "Invalid Offset");
                                            return token.Next;
                                        }

                                }
                                else if (token.Value != ")")
                                {
                                    LogError(currentFileName, token, "Expecting +/- or close bracket");
                                    return token.Next;
                                }
                                StoreIYOpcodeDisplacement(opcode, offset);
                                if (token.Value != ")")
                                    if ((token = NextToken(token)) == null) return token.Next;
                                break;
                            }
                        case RegisterType.HL:
                            StoreOpcode(opcode);
                            if ((token = NextToken(token)) == null) return token.Next;

                            break;
                        default:
                            LogError(currentFileName, token, token.Value + " is not allowed here");
                            return token.Next;
                    }
                    CheckCloseBracket(token, true);
                    return token.Next;
                case RegisterType.XH:
                case RegisterType.XL:
                    StoreIXOpcode(baseopcode + (int)registerType - 4);
                    return token.Next;
                case RegisterType.YH:
                    StoreIYOpcode(baseopcode + (int)registerType - 4);
                    return token.Next;
                case RegisterType.YL:
                    return token.Next;
                default:
                    if (registerType > RegisterType.A)
                    {
                        LogError(currentFileName, token, registerType .ToString()+   " is not valid here");
                        return token;
                    }
                    StoreOpcode(baseopcode + (int)registerType);
                    return token;
            }
        }

        static Token HLRegSP(Token token, RegisterType registerType, int baseopcode)
        {
            if (registerType !=RegisterType.HL)
            {
                LogError(currentFileName, token, "Expecting HL");
                return token.Next;

            }
            if ((token = NextToken(token)) == null) return token.Next;
            if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            switch (token.Value.ToUpper())
            {
                case "BC": StoreEDOpcode(baseopcode); return token.Next; 
                case "DE": StoreEDOpcode(baseopcode + 0x10); return token.Next;
                case "HL": StoreEDOpcode(baseopcode + 0x20); return token.Next;
                case "SP": StoreEDOpcode(baseopcode + 0x30); return token.Next;
            }
            LogError(currentFileName, token, "Expecting BC, DE, HL or SP");
            return token.Next;
        }

        // --------------------------------------------------------------------------------------
        // Memory write procedures
        static void StoreOpcode(Codes value)
        {
            StoreOpcode((int)value);
        }
        static bool StoreOpcode(int value)
        {
            if (ProgramCounter > 0xffff)
            {
                LogError(currentFileName, null,"Program Counter out of range");
                return false;
            }
            Memory[ProgramCounter++] = (byte)value;
            return true;
        }
        static bool StoreByte(int value)
        {
            if (value > 0xff)
            {
                LogError(currentFileName, null,"Value too big, expeced byte!");
                return false;
            }
            if (ProgramCounter > 0xffff)
            {
                LogError(currentFileName, null,"Program Counter out of range: " + ProgramCounter.ToString("X4"));
                return false;
            }
            Memory[ProgramCounter++] = (byte)value;
            return true;
        }

        static bool StoreWord(int value)
        {
            if (value > 0xffff)
            {
                LogError(currentFileName, null,"Value too big, expected word!");
                return false;
            }
            StoreByte(value & 0xff);
            StoreByte((value >> 8) & 0xff);
            return true;
        }

        static void StoreEDOpcode(CodesED value)
        {
            StoreEDOpcode((int)value);
        }
        static void StoreEDOpcode(int value)
        {
            Memory[ProgramCounter++] = 0xED;
            Memory[ProgramCounter++] = (byte)value;
        }

        static void StoreIXCBOpcode(int opcode, int displacement)
        {
            Memory[ProgramCounter++] = (int)Codes.PFX_IX;
            Memory[ProgramCounter++] = (int)Codes.PFX_CB;
            StoreByte(displacement);
            Memory[ProgramCounter++] = (byte)opcode;

        }
        static void StoreIYCBOpcode(int opcode, int displacement)
        {
            Memory[ProgramCounter++] = (int)Codes.PFX_IY;
            Memory[ProgramCounter++] = (int)Codes.PFX_CB;
            StoreByte(displacement);
            Memory[ProgramCounter++] = (byte)opcode;

        }
        static void StoreCBOpcode(CodesCB value)
        {
            StoreCBOpcode((int)value);
        }
        static void StoreCBOpcode(int value)
        {
            Memory[ProgramCounter++] = 0xCB;
            Memory[ProgramCounter++] = (byte)value;
        }

        static void StoreIXOpcode(Codes value)
        {
            StoreIXOpcode((int)value);
        }
        static void StoreIXOpcode(int value)
        {
            Memory[ProgramCounter++] = 0xDD;
            Memory[ProgramCounter++] = (byte)value;
        }
        static bool StoreIXOpcodeDisplacement(int opcode, int displacement)
        {
            Memory[ProgramCounter++] = 0xDD;
            Memory[ProgramCounter++] = (byte)opcode;
            return StoreByte(displacement);
        }

        static void StoreIYOpcode(Codes value)
        {
            StoreIYOpcode((int)value);
        }
        static void StoreIYOpcode(int value)
        {
            Memory[ProgramCounter++] = 0xFD;
            Memory[ProgramCounter++] = (byte)value;
        }
        static bool StoreIYOpcodeDisplacement(int opcode, int displacement)
        {
            Memory[ProgramCounter++] = 0xFD;
            Memory[ProgramCounter++] = (byte)opcode;
            return StoreByte(displacement);
        }

        static int GetValue(ref Token token)
        {
            int n = 0;
            if (token.IsEquation)
            {
                n = (int)Parser.Evaluate(currentFileName, token.Value, ProgramCounter, ProgramCounterOffset, token);
                if (Parser.IsValid)
                    return n;
            }
            else if (token.IsSymbol)
            {
                if (token.IsResolved) return token.Number;
                Symbol symbol = SymbolTable.Find(currentFileName, token.Value);
                if (symbol != null)
                {
                    if (symbol.Resolved)
                    {
                        symbol.IsUsedAt(currentFileName, token.Line, token);
                        //if (!symbol.UsedAtLines.Contains(new UsedAt(token.Line, token)))
                        //    symbol.UsedAtLines.Add(new UsedAt(token.Line, token));
                        return symbol.Value;
                    }
                    else
                        SymbolTable.UsedAt(currentFileName, -1, token, token.Value, token.Number, token.Line);

                }
                else
                {
                    SymbolTable.Add(currentFileName, token, token.Value, token.Line);
//                    SymbolTable.UsedAt(-1, token, token.Value, token.Number, token.Line);
                }
                if (currentPass == 2)
                {
                    LogError(currentFileName, token, "Undefined symbol");
                    return n;
                }
            }
            if (currentPass == 2)
            {
                IsTerminal = true;
                LogError(currentFileName, token, "Invalid value; expecting equation or resolved symbol, FATAL");
            }
            return n;
        }

        static bool EOL(Token token)
        {
            if (token == null) return true;
            if (token.Next != null)
            {
                if(!ErrorTable.HasErrors(currentFileName))
                LogError(currentFileName, token, "Unwanted characters at end of line! " + token.Value +" "+ token.Next.Value);
                return false;
            }
            return true;
        }


        public static event ErrorLoggedEventHandler ErrorLogged;
        public static Token LogError(string fileName, Token token, string message,int lineNo)
        {
            int col = 0;
            if (token != null)
                col = token.Col;
            else
            {
                if (LastToken != null)
                {
                    if (LastToken.Next != null)
                        col = LastToken.Col;
                    else
                        col = LastToken.Col;
                }

            }
            if (lineNo >= 0)
                ErrorTable.Add(fileName, currentLine, token, message);
//            OnErrorLogged(SourceDocument, new LogErrorEventArgs(token, message, col, lineNo));
            if (token != null)
                return token.Next;
            return token;
        }
        public static Token LogError(string fileName, Token token, string message)
        {
            return LogError(fileName,token, message, currentLine);
        }

        protected static void OnErrorLogged(object sender, LogErrorEventArgs e)
        {
            ErrorLogged?.Invoke(sender, e);
        }

    }

}

