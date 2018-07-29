using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token DEFG(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            int bit = 0x80;
            int result = 0;
            while (token != null)
            {
                string value = token.Value;
                if (value == "#")
                    result |= bit;
                bit >>= 1;
                token = token.Next;
                if (token == null) break;
            }
            StoreByte(result);
            return token;
        }
    }
}
