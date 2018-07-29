using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    /*
INC (HL)
INC (IX +d)
INC (IY +d)

INC A
INC B
INC C
INC D
INC E
INC H
INC L

INC IXH
INC IXL
INC IYH
INC IYL

INC BC
INC DE
INC HL
INC IX
INC IY
INC SP
    */
    partial class Assembler
    {
        static Token INCDEC(Token token, int baseOpcode)
        {
            if ((token = NextToken(token)) == null) return token;
            if(token.IsRegisterPair)
            {
                switch (token.Value.ToUpper())
                {
                    case "BC": StoreOpcode((int)Codes.INC_BC + baseOpcode); return token;
                    case "DE": StoreOpcode((int)Codes.INC_DE + baseOpcode); return token;
                    case "HL": StoreOpcode((int)Codes.INC_HL + baseOpcode); return token;
                    case "SP": StoreOpcode((int)Codes.INC_SP + baseOpcode); return token;
                    case "IX": StoreIXOpcode((int)Codes.INC_HL + baseOpcode); return token;
                    case "IY": StoreIYOpcode((int)Codes.INC_HL + baseOpcode); return token;
                }
                return LogError(currentFileName, token,"Expected register pair BC, DE, HL, SP, IX or IY");
            }
            if (token.IsRegister)
            {
                RegisterType registerType = GetRegisterType(token);
                if (registerType > RegisterType.XH && registerType < RegisterType.YL)
                {
                    switch (registerType)
                    {
                        case RegisterType.XH: StoreIXOpcode((int)Codes.INC_H + (baseOpcode >> 3)); return token;
                        case RegisterType.XL: StoreIXOpcode((int)Codes.INC_L + (baseOpcode >> 3)); return token;
                        case RegisterType.YH: StoreIYOpcode((int)Codes.INC_H + (baseOpcode >> 3)); return token;
                        case RegisterType.YL: StoreIYOpcode((int)Codes.INC_L + (baseOpcode >> 3)); return token;
                    }
                    return LogError(currentFileName, token,"Expected register XH, XL, YH or YL");
                }
                StoreOpcode((int)Codes.INC_B + (baseOpcode >> 3) + ((int)registerType << 3));
                return token;
            }
            if (token.Value=="(")
            {

                if ((token = NextToken(token)) == null) return token;
                switch (token.Value.ToUpper())
                {
                    case "HL":
                        {
                            if ((token = NextToken(token)) == null) return token;
                            if (!CheckCloseBracket(token, true)) return token;
                            StoreOpcode((int)Codes.INC_xHL + (baseOpcode >> 3));
                        }
                        return token;
                    case "IX":
                        {
                            if ((token = NextToken(token)) == null) return token;
                            int offset = 0;
                            if (token.IsSymbol || token.IsEquation)
                            {
                                offset = GetValue(ref token);
                                if ((token = NextToken(token)) == null) return token;
                            }
                            if (!CheckCloseBracket(token, true)) return token;
                            StoreIXOpcodeDisplacement((int)Codes.INC_xHL + (baseOpcode >> 3), offset);
                        }
                        return token;
                    case "IY":
                        {
                            if ((token = NextToken(token)) == null) return token;
                            int offset = 0;
                            if (token.IsSymbol || token.IsEquation)
                            {
                                offset = GetValue(ref token);
                                if ((token = NextToken(token)) == null) return token;
                            }
                            if (!CheckCloseBracket(token, true)) return token;
                            StoreIYOpcodeDisplacement((int)Codes.INC_xHL + (baseOpcode >> 3), offset);
                        }
                        return token;
                    default:
                        return LogError(currentFileName, token,"Expecting HL, IX or IY");
                }
            }
            return LogError(currentFileName, token,"Expecting Register B, C, D, E, H, L or A");
        }
    }
}
