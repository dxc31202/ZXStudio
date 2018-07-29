using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token IN(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            RegisterType registerType = GetRegisterType(token);
            if (!token.IsRegister)
            {
                return LogError(currentFileName, token,"Expecting a register, B, C, D, E, H, L or A");
            }
            if ((token = NextToken(token)) == null) return token;
            if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;

            if (!CheckOpenBracket(token, true)) return token;

            if ((token = NextToken(token)) == null) return token;
            if (token.Value.ToUpper()=="C")
            {

                if ((token = NextToken(token)) == null) return token;
                if (!CheckCloseBracket(token, true)) return token;
                if (registerType==RegisterType.F)
                    StoreEDOpcode((int)CodesED.IN_B_xC | 0x30);
                else
                    StoreEDOpcode((int)CodesED.IN_B_xC | ((int)registerType << 3));
                return token;
            }
            int value =GetValue(ref token);
            if ((token = NextToken(token)) == null) return token;
            if (!CheckCloseBracket(token, true)) return token;
            StoreOpcode((int)Codes.INA);
            StoreByte(value);

            return token;
        }
    }
}
