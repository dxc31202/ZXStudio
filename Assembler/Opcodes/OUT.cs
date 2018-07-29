using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token OUT(Token token)
        {
            //OUT (C), 0
            if ((token = NextToken(token)) == null) return token;
            if (!CheckOpenBracket(token, true)) return token;
            if ((token = NextToken(token)) == null) return token;
            if (token.Value.ToUpper() == "C")
            {
                if ((token = NextToken(token)) == null) return token;
                if (!CheckCloseBracket(token, true)) return token;

                if ((token = NextToken(token)) == null) return token;
                if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
                RegisterType registerType = GetRegisterType(token);

                if (token.Value == "0")
                {
                    StoreEDOpcode(CodesED.OUT_xC_0);
                    return token;
                }
                else if (registerType == RegisterType.Unknown)
                    return LogError(currentFileName, token,"Expecting a register, B, C, D, E, H, L or A");
                StoreEDOpcode((int)CodesED.OUT_xC_B | ((int)registerType << 3));
                return token;
            }
            // OUT (n), A
            int value = GetValue(ref token);

            if ((token = NextToken(token)) == null) return token;
            if (!CheckCloseBracket(token, true)) return token;

            if ((token = NextToken(token)) == null) return token;
            if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            if (token.Value.ToUpper() != "A")
                return LogError(currentFileName, token,"Expecting Register A");
            StoreOpcode(Codes.OUTA);
            StoreByte(value);

            return token;
        }
    }
}
