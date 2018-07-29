using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token ProcessDirective(Token token)
        {
            switch (token.Value.ToUpper())
            {
                case "ENT":
                case ".ENT":
                case "ENTRY":
                case ".ENTRY":
                    return ENTRY(token);

                case "ORG":
                case ".ORG":
                    return ORG(token);

                case "EQU":
                case ".EQU":
                    return EQU(token);

                case "END":
                case ".END":
                    lastTokenType = TokenType.Operator;
                    return END(token);

                case "DEFB":
                case "DB":
                case ".DEFB":
                case ".DB":
                    lastTokenType = TokenType.Operator;
                    return DEFB(token);

                case "DEFG":
                case "DG":
                case ".DEFG":
                case ".DG":
                    lastTokenType = TokenType.Operator;
                    return DEFG(token);

                case "DEFS":
                case "DS":
                case ".DEFS":
                case ".DS":
                    lastTokenType = TokenType.Operator;
                    return DEFS(token);

                case "DEFW":
                case "DW":
                case ".DEFW":
                case ".DW":
                    lastTokenType = TokenType.Operator;
                    return DEFW(token);

                case "TEXT":
                case ".TEXT":
                case "BYTE":
                case ".BYTE":
                case "DEFM":
                case ".DEFM":
                case "DM":
                    lastTokenType = TokenType.Operator;
                    return DEFM(token);
            }
            return token;
        }
    }
}
