using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token ROTATESHIFT(Token token,int baseOpcode)
        {
            if ((token = NextToken(token)) == null) return token;
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
                            StoreCBOpcode(baseOpcode + 6);
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
                            StoreIXCBOpcode(baseOpcode + 6, offset);
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
                            StoreIYCBOpcode(baseOpcode + 6, offset);
                        }
                        return token;
                    default:
                        return LogError(currentFileName, token,"Expecting HL, IX or IY");
                }

            }
            RegisterType registerType = GetRegisterType(token);
            if(!token.IsRegister)
                return LogError(currentFileName, token,"Expecting a register, B, C, D, E, H, L or A");
            StoreCBOpcode(baseOpcode + (int)registerType);

            return token;




        }
    }
}
