using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token JP(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            if (token.Value == "(")
            {
                token = token.Next;
                if(token==null)
                    return LogError(currentFileName, token,"Expecting HL,IX or IY");
                switch (token.Value.ToUpper())
                {
                    case "HL": StoreOpcode(Codes.JP_HL); break;
                    case "IX": StoreIXOpcode(Codes.JP_HL); break;
                    case "IY": StoreIYOpcode(Codes.JP_HL); break;
                    default: return LogError(currentFileName, token,"Expecting HL,IX or IY");
                }
                if ((token = NextToken(token)) == null) return token;
                if (!CheckCloseBracket(token, true)) return token;
                return token;
            }
            int opcode = 0;
            if (token.IsCondition)
            {
                opcode = (int)Codes.JP_NZ | ((int)GetCondition(token) << 3);
                if ((token = NextToken(token)) == null) return token;
                if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            }
            else
                opcode = (int)Codes.JP;
            StoreOpcode(opcode);
            StoreWord(GetValue(ref token));
            return token;
        }
    }
}
