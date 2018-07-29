using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace ZXStudio
{

    delegate void TokenFoundEventHandler(object sender, TokenizerEventArgs e);

    class Tokenizer
    {
        public event TokenFoundEventHandler TokenFound;
        public Tokenizer()
        {
        }

        public string Source;
        public int CurrentChar;
        string lastoperation="";
        List<Token> tokens = new List<Token>();

        public char this[int index]
        {
            get {
                if (index < Source.Length)
                    return Source[index];
                else
                    return ' ';
            }
        }
        int row,col;
        int counter = 0;
        public Token Tokenize(string source)
        {
            Token.NextIndex = 0;
            row = col = 0;
            tokens = new List<Token>();
            CurrentChar = 0;

            Source = source;
            while (!EOF(CurrentChar))
            {
                IsWhitespace();             // Discard White Space
                if (IsLineFeed()) continue; // Discard Line Feeds
                if (IsComment()) continue;  // Discard Comments

                if (IsNumber()) continue;

                //if (IsFilename()) continue;
                if (IsIdentifier()) continue;

                if (IsString()) continue;
                if (IsComma()) continue;
                if (IsOperator()) continue;
                if (IsBracket()) continue;
                if (EOF(CurrentChar)) break;
                AddToken(source.Substring(CurrentChar++, 1), TokenType.Unknown);
            }
            if(CurrentToken!=null)
                return CurrentToken.First;
            return null;
        }

        
        #region Token Functions

        /// <summary>
        /// Space or Tab
        /// </summary>
        /// <returns></returns>
        bool IsWhitespace()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            while (Source[position] == ' ' || Source[position] == '\t')
            {
                if (EOF(++position)) break;
            }
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            return true;
        }

        /// <summary>
        /// \n
        /// </summary>
        /// <returns></returns>
        bool IsLineFeed()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            while (Source[position] == '\n')
            {
                if (EOF(++position)) break;
            }
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            row++;
            col = 0;
            return true;
        }


        bool IsComment()
        {
            if (IsComment1()) return true;
            if (IsComment2()) return true;
            return false;
        }
        bool IsBinary()
        {
            if (IsBinary1()) return true;
            if (IsBinary2()) return true;
            return false;
        }
        bool IsHex()
        {
            if (IsHex1()) return true;
            if (IsHex2()) return true;
            if (IsHex3()) return true;
            return false;
        }
        bool IsString()
        {
            if (IsString('\'')) return true;
            if (IsString('"')) return true;
            return false;
        }
        bool IsBracket()
        {
            if (IsChar('(', TokenType.OpenBracket)) return true;
            if (IsChar(')', TokenType.CloseBracket)) return true;
            return false;
        }
        bool IsComma()
        {
            if (IsChar(',', TokenType.Comma)) return true;
            return false;
        }

        bool IsOperator()
        {
            if (EOF(CurrentChar)) return false;
            int position = CurrentChar;
            string equation = "";
            if(EOF(CurrentChar)) return false;
            if (!IsOperator(Source[CurrentChar])) return false;
            Token last = Last;
            Token work = null;
            if (last == null) return false;
            if (last.TokenType == TokenType.Symbol && last.Resolved)
                equation += last.Value + Source[CurrentChar];
            else if (last.TokenType == TokenType.String && last.Value.Length == 1)
                equation += last.Number.ToString() + Source[CurrentChar];
            else if (last.TokenType == TokenType.Symbol)
                equation += last.Value.ToString() + Source[CurrentChar];
            else
            {
                //if (last.TokenType == TokenType.RegisterPair)                    return false;
                    last = AddToken(Source[CurrentChar].ToString(), TokenType.Operator);
                equation += Source[CurrentChar].ToString();
            }
            CurrentChar++;

            while (!EOF(CurrentChar))
            {
                IsWhitespace();          // Discard White Space
                if (IsLineFeed()) break; // Discard Line Feeds
                if (IsComment()) break;  // Discard Comments

                int x = CurrentChar;

                if (IsNumber())
                {
                    work = RemoveLast();
                    equation += work.Value;
                    continue;
                }

                if (IsIdentifier())
                {
                    work = RemoveLast();
                    switch(work.Value.ToLower())
                    {
                        case "and": equation += "&"; continue;
                        case "or": equation += "|"; continue;
                        case "xor": equation += "^"; continue;
                        default: equation += work.Value; continue;
                    }
                    //if (work.TokenType == TokenType.Symbol)
                    {
                        
                        
                    }
                    //else
                    //    break;
                }

                if (IsString())
                {
                    work = RemoveLast();
                    if (work.Value.Length==1)
                    {
                        equation += work.Value;
                        continue;
                    }
                    else
                        break;
                }
                if (EOF(CurrentChar)) break;
                if (IsOperator(Source[CurrentChar]))
                {
                    equation += Source[CurrentChar++].ToString();
                    continue;
                }
                break;
            }

            tokens[tokens.Count - 1].Value = equation;
            tokens[tokens.Count - 1].TokenType = TokenType.Symbol;
            tokens[tokens.Count - 1].IsEquation = true;
            CurrentToken = tokens[tokens.Count - 1];
            return true;
        }

        Token RemoveLast()
        {
            Token token = tokens[tokens.Count - 1];
            tokens.Remove(token);
            tokens[tokens.Count - 1].Next = null;
            return token;

        }
        /// <summary>
        /// Decimal number
        /// </summary>
        /// <returns></returns>
        bool IsNumber()
        {
            if (IsBinary()) return true;   // %dddddddd, ddddddddb
            if (IsHex()) return true;      // 0xdddd, $dddd, #dddd, ddddh

            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            while (IsDigit(Source[position]))
                if (EOF(++position)) break;
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            AddToken(Source.Substring(start, length), TokenType.Symbol, true);
            return true;
        }

        bool IsFilename()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;

            if (!IsAlpha(Source[position])) return false;
            while (IsFilename(Source[position]))
                if (EOF(++position)) break;
            int start = CurrentChar;
            int length = position - CurrentChar;
            string fileName = Source.Substring(start, length);
            if (System.IO.File.Exists(fileName))
            {
                CurrentChar += length;
                AddToken(Source.Substring(start, length), TokenType.FileName);
                return true;
            }
            //CurrentChar= position;

            return false;
        }


        bool IsIdentifier()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;

            if (Source[position] != '#' && !IsAlpha(Source[position])) return false;
            if (Source[position] == '#')
                if (EOF(++position)) return false;
            while (IsIdentifier(Source[position]))
                    if (EOF(++position)) break;


            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            TokenType tokenType = TokenType.Symbol;

            CurrentChar += length;
            AddToken(Source.Substring(start, length), tokenType);

            return true;
        }

        #region Sub Token Functions
        /// <summary>
        /// String within ''
        /// </summary>
        /// <returns></returns>
        bool IsString(char stringtype)
        {
            if (EOF(CurrentChar)) return false;
            int position = CurrentChar;
            if (Source[position] != stringtype) return false;
            if (!EOF(++position))
                while (Source[position] != stringtype)
                {
                    if (Source[position] == '\n') return false;
                    if (EOF(++position)) break;
                }
            if (EOF(position++)) return false; 
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            if (length - 2 == 1)
            {
                string val = ((byte)(char)(Source[start + 1])).ToString();

                Token token = AddToken(val, TokenType.Symbol, true);
            }
            else
                AddToken(Source.Substring(start + 1, length - 2), TokenType.String);
            return true;
        }

        /// <summary>
        /// Comment with prefix ;
        /// </summary>
        /// <returns></returns>
        bool IsComment1()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (Source[position] != ';') return false;
            while (Source[position] != '\n')
                if (EOF(++position)) break;
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            return true;
        }

        /// <summary>
        /// Comment with prefix //
        /// </summary>
        /// <returns></returns>
        bool IsComment2()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (Source[position] != '/') return false;
            if (EOF(++position)) return false;
            if (Source[position] != '/') return false;
            if (EOF(++position)) return false;
            while (Source[position] != '\n')
                if (EOF(++position)) break;
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            return true;
        }
        /// <summary>
        /// Format = dddh where d is hex digit
        /// </summary>
        /// <returns></returns>
        bool IsHex1()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (!IsHex(Source[position])) return false;
            while (IsHex(Source[position]))
                if (EOF(++position)) break;
            if (!EOF(position))
            {
                if ((Source[position] | 32) != 'h')
                    return false;
                else if (!EOF(position + 1))
                        if(IsAlpha(Source[position + 1]))
                            return false;
            }
            if (EOF(position))
                return false;
            EOF(++position);

            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            string val = Source.Substring(start, length - 1);
            val = Functions.BaseToDecimal(val, Functions.BaseType.Hex).ToString();
            AddToken(val, TokenType.Symbol, true, Source.Substring(start, length));
            return true;
        }

        /// <summary>
        /// Format = #ddd where d is hex digit
        /// Format = $ddd where d is hex digit
        /// </summary>
        /// <returns></returns>
        bool IsHex2()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (!"#$".Contains(Source[position])) return false;
            if (EOF(++position)) return false;
            while (IsHex(Source[position]))
                if (EOF(++position)) break;
            if (!EOF(position + 1))
                if (IsAlpha(Source[position]))
                    return false;

            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 1) return false;
            CurrentChar += length;
            string val = Source.Substring(start + 1, length - 1);
            val = Functions.BaseToDecimal(val, Functions.BaseType.Hex).ToString();
            AddToken(val, TokenType.Symbol, true, Source.Substring(start, length));
            return true;
        }

        /// <summary>
        /// Format = 0xddd where d is hext digit
        /// </summary>
        /// <returns></returns>
        bool IsHex3()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (Source[position] != '0') return false;
            if (EOF(++position)) return false;
            if ((Source[position] | 32) != 'x') return false;
            if (EOF(++position)) return false;
            int p = position;
            while (IsHex(Source[position]))
                if (EOF(++position)) break;
            if (p == position) return false;
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 1) return false;
            CurrentChar += length;
            string val = Source.Substring(start + 2, length - 2);
            val = Functions.BaseToDecimal(val, Functions.BaseType.Hex).ToString();
            AddToken(val, TokenType.Symbol, true, Source.Substring(start, length ));
            return true;
        }

        /// <summary>
        /// Format = ddddddddb where d is Binary digit
        /// </summary>
        /// <returns></returns>
        bool IsBinary1()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (!IsBinary(Source[position])) return false;
            while (IsBinary(Source[position]))
                if (EOF(++position)) break;
            if (EOF(position)) return false;
            if ((Source[position] | 32) != 'b')
                return false;
            EOF(++position);
            if (IsHex(Source[position]))
                return false;
            if (Source[position].ToString().ToLower() == "h")
                return false;
            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 0) return false;
            CurrentChar += length;
            string val = Source.Substring(start, length - 1);
            val = Functions.BaseToDecimal(val, Functions.BaseType.Binary).ToString();
            AddToken(val, TokenType.Symbol, true, Source.Substring(start, length - 1));
            return true;
        }


        /// <summary>
        /// Format = %dddddddd where d is binary digit
        /// </summary>
        /// <returns></returns>
        bool IsBinary2()
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (!"%".Contains(Source[position])) return false;
            if (EOF(++position)) return false;
            while (IsBinary(Source[position]))
                if (EOF(++position)) break;

            int start = CurrentChar;
            int length = position - CurrentChar;
            if (length == 1) return false;
            CurrentChar += length;
            string val = Source.Substring(start + 1, length - 1);
            val = Functions.BaseToDecimal(val, Functions.BaseType.Binary).ToString();
            AddToken(val, TokenType.Symbol,true, Source.Substring(start, length - 1));
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsChar(char charType, TokenType tokentype)
        {
            if (EOF(CurrentChar)) return false;

            int position = CurrentChar;
            if (Source[position] != charType) return false;
            Token token = AddToken(Source.Substring( position, 1), tokentype);
            token.Col = position;
            CurrentChar++;
            return true;
        }

        #endregion Sub Token Functions
        #endregion Token Functions


        #region Helper Functions
        // ------------------------------------------------------------------------
        // If ASCII value of c is Hex char "A-F" "a-f" then return true
        //   otherwise return false
        //
        bool IsHexChar(char c)
        {
            if ((c | 32) < 'a') return false;
            if ((c | 32) > 'f') return false;
            return true;
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is Alphabetic "A-Z" "a-z" then return true
        //   otherwise return false
        //
        bool IsAlpha(char c)
        {
            if ((c | 32) < 'a') return false;
            if ((c | 32) > 'z') return false;
            return true;
        }

        // ------------------------------------------------------------------------
        // If FileName value of c is a valid fileName that exists
        //   otherwise return false
        //
        bool IsFilename(char c)
        {
            if (c == ' ') return true;
            if (c == '.') return true;
            if (c == ':') return true;
            if (c == '\\') return true;
            if (IsIdentifier(c)) return true;
            return false;
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is a digit "0-9" then return true
        //   otherwise return false
        //
        bool IsDigit(char c)
        {
            if (c < '0') return false;
            if (c > '9') return false;
            return true;
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is an identifier  then return true
        //   otherwise return false
        //
        bool IsIdentifier(char c)
        {
            if (IsAlpha(c)) return true;
            if (IsDigit(c)) return true;
            if (c == '_') return true;
            //if (c == '-') return true;
            return false;
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is Hex then return true
        //   otherwise return false
        //
        bool IsHex(char c)
        {
            if (IsHexChar(c)) return true;
            if (IsDigit(c)) return true;
            return false;
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is Hex then return true
        //   otherwise return false
        //
        bool IsBinary(char c)
        {
            return "01".Contains(c);
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is punctuation then return true
        //   otherwise return false
        //
        bool IsPunctuation(char c)
        {
            return @";:$,!;?#|<=>\!".Contains(c);
        }
        // ------------------------------------------------------------------------
        // If ASCII value of c is an operator (i.e. +,-,*,/,= etc.)  then return true
        //   otherwise return false
        //
        bool IsOperator(char c)
        {
            return @"*+-/^%<=>|$&".Contains(c);
        }
        bool EOF(int address)
        {
            return address >= Source.Length;
        }
        #endregion Helper Functions

       public Token CurrentToken;
        Token AddToken(string value, TokenType tokentype)
        {
            if (value == "\r") value = "";
            CurrentToken = new Token(value, tokentype, CurrentToken);
            CurrentToken.Line = Assembler.currentLine;
            tokens.Add(CurrentToken);
            TokenizerEventArgs e = new TokenizerEventArgs(CurrentChar, CurrentToken);
            TokenFound?.Invoke(this, e);
            CurrentChar = e.CurrentChar;
            CurrentToken = e.Token;
            CurrentToken.Row = row;
            CurrentToken.Col = CurrentChar - value.Length;
            CurrentToken.OriginalValue = value;

            return CurrentToken;
        }
        Token AddToken(string value, TokenType tokentype, bool resolved,string originalvalue)
        {
            CurrentToken = new Token(value, tokentype, CurrentToken, resolved);
            CurrentToken.Line = Assembler.currentLine;
            tokens.Add(CurrentToken);
            TokenizerEventArgs e = new TokenizerEventArgs(CurrentChar, CurrentToken);
            TokenFound?.Invoke(this, e);
            CurrentChar = e.CurrentChar;
            CurrentToken.Row = row;
            CurrentToken.Col = CurrentChar - originalvalue.Length;
            CurrentToken.OriginalValue = originalvalue;
            return CurrentToken;
        }

        Token AddToken(string value, TokenType tokentype, bool resolved)
        {
            CurrentToken = new Token(value, tokentype, CurrentToken, resolved);
            CurrentToken.Line = Assembler.currentLine;
            tokens.Add(CurrentToken);
            TokenizerEventArgs e = new TokenizerEventArgs(CurrentChar, CurrentToken);
            TokenFound?.Invoke(this, e);
            CurrentChar = e.CurrentChar;
            CurrentToken.Row = row;
            CurrentToken.Col = CurrentChar - value.Length; 
            CurrentToken.OriginalValue = value;
            
            return CurrentToken;
        }


        

        Token Last
        {
            get
            {
                if (tokens.Count > 0)
                    return tokens[tokens.Count - 1];
                return null;
            }
        }


    }
}
