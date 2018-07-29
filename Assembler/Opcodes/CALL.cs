using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token CALL(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            int opcode = 0;
            if (token.IsCondition)
            {
                opcode = (int)Codes.CALL_NZ | ((int)GetCondition(token) << 3);
                if ((token = NextToken(token)) == null) return token;
                if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            }
            else
                opcode = (int)Codes.CALL;
            StoreOpcode(opcode);
            StoreWord(GetValue(ref token));
            return token;
        }
    }
}
