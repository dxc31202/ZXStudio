using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token EX(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            switch (token.Value.ToUpper())
            {
                case "(":
                    if ((token = NextToken(token)) == null) return token;
                    if (token.Value.ToUpper() != "SP") return LogError(currentFileName, token,"Expecting SP");
                    if ((token = NextToken(token)) == null) return token;
                    if (!CheckCloseBracket(token, true)) return token;
                    if ((token = NextToken(token)) == null) return token;
                    if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
                    switch (token.Value.ToUpper())
                    {
                        case "IX": StoreOpcode(Codes.PFX_IX); StoreOpcode(Codes.EX_HL_xSP); break;
                        case "IY": StoreOpcode(Codes.PFX_IY); StoreOpcode(Codes.EX_HL_xSP); break;
                        case "HL": StoreOpcode(Codes.EX_HL_xSP); break;
                        default:
                            return LogError(currentFileName, token,"Expecting IX, IY or HL");
                    }
                    break;
                case "AF":
                    if ((token = NextToken(token)) == null) return token;
                    if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
                    if (token.Value.ToUpper() != "AF'")
                        return LogError(currentFileName, token,"Expecting AF'");
                    StoreOpcode(Codes.EX_AF_AF);
                    break;
                case "DE":
                    if ((token = NextToken(token)) == null) return token;
                    if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
                    if (token.Value.ToUpper() != "HL")
                        return LogError(currentFileName, token,"Expecting HL");
                    StoreOpcode(Codes.EX_DE_HL);
                    break;
                default:
                    return LogError(currentFileName, token,"Expecting (SP), AF or HL");
            }

            return token;
        }
    }
}
