using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token PUSHPOP(Token token, int baseOpcode)
        {
            if ((token = NextToken(token)) == null) return token;
            if (token.IsRegisterPair)
            {
                switch (token.Value.ToUpper())
                {
                    case "BC": StoreOpcode((int)Codes.POP_BC + baseOpcode); return token;
                    case "DE": StoreOpcode((int)Codes.POP_DE + baseOpcode); return token;
                    case "HL": StoreOpcode((int)Codes.POP_HL + baseOpcode); return token;
                    case "AF": StoreOpcode((int)Codes.POP_AF + baseOpcode); return token;
                    case "IX": StoreIXOpcode((int)Codes.POP_HL + baseOpcode); return token;
                    case "IY": StoreIYOpcode((int)Codes.POP_HL + baseOpcode); return token;
                }
            }
            return LogError(currentFileName, token,"Expected register pair BC, DE,HL, AF, IX or IY");
        }
    }
}
