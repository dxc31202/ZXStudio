using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token DEFB(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            while (token != null)
            {
                {
                    if (token.TokenType == TokenType.String)
                    {
                        for(int i=0;i< token.Value.Length;i++)
                        {
                            if (!StoreByte(token.Value[i])) break;
                        }

                    }
                    else
                    {
                        int value = GetValue(ref token);
                        if (!StoreByte(value)) break;
                    }
                    token = token.Next;
                    if (token == null) return token;
                    if (CheckComma(token, false))
                    {
                        if ((token = NextToken(token)) == null)
                            return token;
                    }
                    
                }
            }
            return token;
        }
    }
}
