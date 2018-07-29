using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token IM(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            if (token.IsSymbol&&token.IsResolved)
            {
                switch (token.Number)
                {
                    case 0: StoreEDOpcode(CodesED.IM_0); break;
                    case 1: StoreEDOpcode(CodesED.IM_1); break;
                    case 2: StoreEDOpcode(CodesED.IM_2); break;
                    default:
                        return LogError(currentFileName, token,"Expecting Imterrupt Vector 0, 1 or 2");
                }
            }
            else
                return LogError(currentFileName, token,"Expecting Imterrupt Vector 0, 1 or 2");
            return token;
        }
    }
}
