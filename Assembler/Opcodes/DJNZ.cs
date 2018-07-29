using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token DJNZ(Token token)
        {
            if ((token = NextToken(token)) == null) return token;

            int val1 = GetValue(ref token);

            sbyte val = (sbyte)val1;
            int value = val;
            if (token.IsSymbol)
            {
                Symbol symbol = SymbolTable.Find(currentFileName, token.Value);
                if (symbol != null)
                    value = ((byte)(256 - ((sbyte)(ProgramCounter + 2 - val))));
            }

            StoreOpcode(Codes.DJNZ);
            StoreByte(value);
            return token;

        }
    }
}
