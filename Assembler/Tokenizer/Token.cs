using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    [Serializable]
    [Flags]
    public enum TokenType : long
    {
        Symbol = 0x000000,
        Number = 0x000001,
        Unknown = 0x000002,
        Opcode = 0x000004,
        RegisterPair = 0x000008,
        Register = 0x000010,
        Condition = 0x000020,
        Directive = 0x000040,
        Preprocessor = 0x000080,
        Label = 0x000100,
        String = 0x000200,
        OpenBracket = 0x000400,
        CloseBracket = 0x000800,
        Comma = 0x001000,
        Operator = 0x002000,
        Equation = 0x004000,
        Comment = 0x008000,
        Plus = 0x010000,
        Minus = 0x020000,
        Colon = 0x040000,
        SingleQuote = 0x080000,
        Dollar = 0x100000,
        WhiteSpace = 0x200000,
        Delimiter = 0x400000,
        FileName = 0x800000,
        NewLine = 0x1000000,

    }
    public class Token
    {
        public static bool IgnoreWhitespace = true;
        public string OriginalValue;
        string value;
        TokenType tokentype;
        Token previous;
        Token next;
        public int Line { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        bool resolved;
        bool isEquation;
        public string Value { get { return value; } set { this.value = value; } }
        public TokenType TokenType
        {
            get
            {
                switch (value)
                {
                    case "+": return TokenType.Plus;
                    case "-": return TokenType.Minus;
                    case ":": return TokenType.Colon;
                    case "(": return TokenType.OpenBracket;
                    case ")": return TokenType.CloseBracket;
                    case "'": return TokenType.SingleQuote;
                    case "$": return TokenType.Dollar;
                }
                return tokentype;
            }
            set { tokentype = value; }
        }

        public bool Resolved { get { return resolved; } set { resolved = value; isEquation = !resolved; } }
        public bool IsEquation { get { return isEquation; } set { isEquation = value; } }
        public bool IsCondition { get { return tokentype == TokenType.Condition; } }
        public bool IsRegister { get { return tokentype == TokenType.Register; } }
        public bool IsRegisterPair { get { return tokentype == TokenType.RegisterPair; } }

        public Token Previous { get { return previous; } set { previous = value; } }
        public Token Next
        {
            get
            {
                if (next == null) return null;
                Token nxt = next;
                if (next.tokentype == TokenType.WhiteSpace)
                    if (IgnoreWhitespace) nxt = nxt.next;
                return nxt;
            }
            set { next = value; }
        }

        public Token(string value, TokenType tokentype, Token previous)
        {
            idx = ++NextIndex;
            this.value = value;
            this.tokentype = tokentype;
            this.isEquation = false;
            Previous = previous;
            if (Previous != null)
                Previous.Next = this;

        }
        public Token(string value, TokenType tokentype, Token previous, bool resolved)
        {
            idx = ++NextIndex;
            this.value = value;
            this.tokentype = tokentype;
            this.resolved = resolved;
            this.isEquation = false;
            Previous = previous;
            if (Previous != null)
                Previous.Next = this;
        }

        public static int NextIndex;
        int idx;
        public int Index
        { get { return idx; } }
        string pattern = "";
        public string Pattern
        {
            get
            {
                Token tmp = First;
                while (tmp.TokenType != TokenType.Opcode)
                    tmp = tmp.next;
                pattern = "";
                while (tmp.TokenType != TokenType.Symbol && tmp.TokenType != TokenType.Unknown)
                {
                    pattern += tmp.Value.ToUpper();
                    if (tmp.TokenType == TokenType.Opcode)
                        pattern += " ";
                    tmp = tmp.Next;
                    if (tmp == null) break;
                }
                pattern = pattern.Trim();
                return pattern;
            }

        }

        public int Count
        {
            get
            {
                int count = 0;
                Token token = First;
                while (token != null)
                {
                    count++;
                    token = token.next;
                }
                return count;
            }
        }
        public Token First
        {
            get
            {
                Token first = this;
                while (first.Previous != null)
                    first = first.previous;
                return first;
            }
        }
        public Token Last
        {
            get
            {
                Token last = this;
                while (last.Next != null)
                    last = last.Next;
                return last;
            }
        }
        public int Number
        {
            get
            {
                int result = 0;

                if (tokentype == TokenType.String)
                    result = Convert.ToInt16(value[0]);
                else
                    int.TryParse(value, out result);
                return result;
            }
        }
        public override string ToString()
        {
            //return AllTokens + "\t" + "{" + tokentype.ToString() + ";" + value + "}";
            return "{" + tokentype.ToString() + ", " + value + "}";
        }

        string alltokens = "";
        public string AllTokens
        {
            get
            {
                if (alltokens.Length == 0)
                {
                    string retval = "";
                    Token t = First;
                    while (t != null && t.tokentype != TokenType.Opcode)
                        t = t.next;
                    while (t != null)
                    {
                        if (t.tokentype != TokenType.WhiteSpace && t.tokentype != TokenType.Label && t.tokentype != TokenType.Comment)
                            retval += t.TokenType.ToString() + " ";
                        t = t.next;
                    }
                    alltokens = retval;
                }
                return alltokens;
            }
        }
        public string AllValues
        {
            get
            {
                string retval = "";
                Token t = First;
                while (t != null)
                {
                    if (t.tokentype != TokenType.Label)
                        retval += t.Value + " ";
                    t = t.next;
                }
                return retval;
            }
        }
        public bool LineIsIndex
        {
            get
            {
                Token t = First;
                while (t != null)
                {
                    if (t.tokentype == TokenType.RegisterPair && (t.Value.ToUpper() == "IX" || value.ToUpper() == "IY"))
                        return true;
                    t = t.next;
                }
                return false;
            }
        }

        public bool LineIsIX
        {
            get
            {
                Token t = First;
                while (t != null)
                {
                    if (t.tokentype == TokenType.RegisterPair && value.ToUpper() == "IX")
                        return true;
                    t = t.next;
                }
                return false;
            }
        }
        public bool LineIsIY
        {
            get
            {
                Token t = First;
                while (t != null)
                {
                    if (t.tokentype == TokenType.RegisterPair && value.ToUpper() == "IY")
                        return true;
                    t = t.next;
                }
                return false;
            }
        }

        public bool IsResolved { get { return resolved; } }
        public bool IsSymbol { get { return tokentype == TokenType.Symbol; } }
        public bool IsByte
        {
            get
            {
                if (tokentype == TokenType.Symbol)
                {
                    if (resolved)
                        if (Number < 0x100) return true;
                }
                return false;
            }
        }
        public bool IsWord
        {
            get
            {
                if (tokentype == TokenType.Symbol)
                {
                    if (Number < 0x10000) return true;
                }
                return false;
            }
        }
    }
}
