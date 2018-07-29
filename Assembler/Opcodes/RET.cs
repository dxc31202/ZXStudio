using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token RET(Token token)
        {
            if (token.Next == null)
            {
                StoreOpcode(Codes.RET);
                return token;
            }
            token = token.Next;
            if (token.IsCondition)
                StoreOpcode((int)Codes.RET_NZ + ((int)GetCondition(token) << 3));
            else
                LogError(currentFileName, token,"Expecting condition NZ, Z, NC, C, PO, PE, P or M");
            return token;
        }
    }
}
