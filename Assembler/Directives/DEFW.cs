using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token DEFW(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            while (token != null)
            {
                int value = GetValue(ref token);
                StoreWord(value);
                token = token.Next;
                if (token == null) return token;
                if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            }
            return token;
        }
    }
}
