using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token BITSETRES(Token token, int baseOpcode)
        {
            if ((token = NextToken(token)) == null) return token;
            int bit = GetValue(ref token);
            if (bit < 0 || bit > 7)
                return LogError(currentFileName, token,"Expecting bit value 0-7");
            if ((token = NextToken(token)) == null) return token;
            if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            RegisterType registerType = GetRegisterType(token);
            if (token.Value == "(")
            {
                if ((token = NextToken(token)) == null) return token;
                switch (token.Value.ToUpper())
                {
                    // RRC (IX +5)

                    case "HL":
                        {
                            if ((token = NextToken(token)) == null) return token;
                            if (!CheckCloseBracket(token, true)) return token;
                            StoreCBOpcode(baseOpcode + 6 | (bit << 3));
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
                            StoreIXCBOpcode(baseOpcode + 6 | (bit << 3), offset);
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
                            StoreIYCBOpcode(baseOpcode + 6 | (bit << 3), offset);
                        }
                        return token;
                    default:
                        return LogError(currentFileName, token,"Expecting HL, IX or IY");
                }
            }
            if (token.IsRegister)
            {
                StoreCBOpcode((baseOpcode + (int)registerType) | (bit << 3));
                return token;
            }
            return LogError(currentFileName, token,"Expecting Register B, C, D, E, H, L or A");
        }
    }
}
