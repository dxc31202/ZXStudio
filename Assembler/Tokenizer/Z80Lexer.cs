using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZXStudio
{
    class TokenizerEventArgs : EventArgs
    {
        public int CurrentChar;
        public Token Token;

        public TokenizerEventArgs(int currentchar, Token token)
        {
            CurrentChar = currentchar;
            Token = token;
        }
    }

    public class Z80Lexer
    {
        public Token Lex(string source)
        {
            Tokenizer tokenizer = new Tokenizer();
            tokenizer.TokenFound += Tokenizer_TokenFound;
            tokenizer.Tokenize(source);
            if (tokenizer.CurrentToken == null)
                return null;
            return tokenizer.CurrentToken.First;

        }

        private void Tokenizer_TokenFound(object sender, TokenizerEventArgs e)
        {

            Tokenizer tokenizer = sender as Tokenizer;
            // ====================================================================
            //  If we have a symbol
            // ====================================================================
            if (e.Token.TokenType == TokenType.Symbol)
            {
                // ====================================================================
                //  if the current char is a colon then we have a label
                //  append a colon to the token 
                //  and advance the current char pointer
                // ====================================================================
                if (e.CurrentChar < tokenizer.Source.Length)
                {
                    if (tokenizer[e.CurrentChar] == ':')
                    {
                        e.Token.TokenType = TokenType.Label;
                        e.Token.Value += ':';
                        e.CurrentChar++;
                    }
                }
                // ====================================================================
                //  OK, so we still have a symbol, now determine the type of symbol
                //  but ignore Equations
                // ====================================================================
                if (e.Token.TokenType == TokenType.Symbol)
                {
                    e.Token.TokenType = DecodeIdentifier(e.Token.Value);
                    if (e.Token.TokenType == TokenType.Directive)
                        if (e.Token.Previous != null)
                            // ====================================================================
                            //  if the previous char is a period 
                            //  prepend a period to the token 
                            //  update the previous token to be the current token
                            // ====================================================================
                            if (e.Token.Previous.Value == ".")
                            {
                                e.Token.Previous.Value = "." + e.Token.Value;
                                e.Token.Previous.TokenType = e.Token.TokenType;
                                e.Token.Previous.Next = null;
                                e.Token = e.Token.Previous;
                            }


                }
            }
            // ====================================================================
            // Now we need to consider special cases for C and AF'
            // ====================================================================
            switch (e.Token.Value.ToUpper())
            {
                case "C":
                    // ====================================================================
                    //  Special case for C (can be Register or Condition)
                    //  Check the context of the last instruction
                    //  For a CALL, JR, JP or RET; C is Condition; 
                    //  anything else C is a Register
                    // ====================================================================
                    if (e.Token.TokenType == TokenType.String)
                        break;
                    try
                    {
                        if ("|RET|JR|JP|CALL|".Contains("|" + e.Token.Previous.Value.ToUpper() + "|"))
                            e.Token.TokenType = TokenType.Condition;
                        else
                            e.Token.TokenType = TokenType.Register;
                    }
                    catch { }
                    break;

                case "AF":
                    // ====================================================================
                    //  Special case for AF'
                    //  if the current char is a tilde append a tilde to the token 
                    //  and advance the current char pointer
                    // ====================================================================
                    if (tokenizer[e.CurrentChar] == '\'')
                    {
                        e.CurrentChar++;
                        e.Token.Value += "'";
                    }
                    break;
            }
        }

        TokenType DecodeIdentifier(string value)
        {
            TokenType identifierType = TokenType.Symbol;
            if (Instructions.Contains("|" + value.ToUpper() + "|")) identifierType |= TokenType.Opcode;
            if (RegisterPairs.Contains("|" + value.ToUpper() + "|")) identifierType |= TokenType.RegisterPair;
            if (Registers.Contains("|" + value.ToUpper() + "|")) identifierType |= TokenType.Register;
            if (Conditions.Contains("|" + value.ToUpper() + "|")) identifierType |= TokenType.Condition;
            if (Directives.Contains("|" + value.ToUpper() + "|")) identifierType |= TokenType.Directive;
            if (Preprocessor.Contains("|" + value.ToUpper() + "|")) identifierType |= TokenType.Preprocessor;
            return identifierType;
        }

        static string Instructions = "|ADC|ADD|AND|BIT|CALL|CCF|CP|CPD|CPDR|CPI|CPIR|CPL|DAA|DEC|DI|DJNZ|EI|EX|EXX|HALT|IM|IN|INC|IND|INDR|INI|INIR|JP|JR|LD|LDD|LDDR|LDI|LDIR|NEG|NOP|OR|OTD|OUTD|OTDR|OUTI|OTI|OTIR|OUT|POP|PUSH|RES|RET|RETI|RETN|RL|RLA|RLC|RLC|RLCA|RLD|RR|RRA|RRC|RRCA|RRD|RST|SBC|SCF|SET|SLA|SLL|SRA|SRL|SUB|XOR|";
        static string RegisterPairs = "|BC|DE|HL|IX|IY|SP|AF|AF'|";
        static string Registers = "|A|F|B|C|D|E|H|L|IXH|IXL|IYH|IYL|I|R|";
        static string Conditions = "|NZ|Z|NC|C|PO|PE|P|M|";
        static string Directives = "HIGH|ENTRY|ENT|EQU|BYTE|DEFB|DB|DEFG|DG|DEFS|DS|DEFM|DM|TEXT|DEFW|DW|ORG|IF|ELSE|ENDIF|EVAL|$|END|TEXT|.ENT|.ENTRY|.EQU|.BYTE|.DEFB|.DB|.DEFG|.DG|.DEFS|.DS|.DEFM|.DM|.TEXT|.DEFW|.DW|.ORG|.END|.TEXT|";
        static string Preprocessor = "|#INCLUDE|#DEFINE|#END|";



    }
}
