using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token JR(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            int opcode = 0;
            if (token.IsCondition)
            {
                opcode = (int)Codes.JR_NZ | ((int)GetConditionJR(token) << 3);
                if ((token = NextToken(token)) == null) return token;
                if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            }
            else
                opcode = (int)Codes.JR;

            int val1 = GetValue(ref token);
           
            sbyte val = (sbyte)val1;
            int value = val;
            if (token.IsSymbol)
            {
                Symbol symbol = SymbolTable.Find(currentFileName, token.Value);
                if (symbol != null)
                    value = ((byte)(256 - ((sbyte)(ProgramCounter + 2 - val))));
            }

            StoreOpcode(opcode);
            StoreByte(value);
            return token;
        }
    }
}
