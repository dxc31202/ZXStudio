using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token SUB(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            RegisterType registerType = GetRegisterType(token);
            return RegMemOrByte(token, registerType, (int)Codes.SUB_B);

        }
    }
}
