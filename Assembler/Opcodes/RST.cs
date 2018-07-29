using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token RST(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            int value = GetValue(ref token);
            if (value % 8 == 0) value >>= 3;
            if (value >= 0 && value <= 7)
                StoreOpcode((int)Codes.RST00 + (value << 3));
            else
                LogError(currentFileName, token,"Invalid restart vector");
            return token;
        }
    }
}
