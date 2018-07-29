using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token DEFM(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            if (token.TokenType == TokenType.String || token.TokenType == TokenType.Symbol)
            {
                for (int i = 0; i < token.Value.Length; i++)
                {
                    if (token.TokenType == TokenType.Symbol)
                    {
                        StoreByte(Convert.ToChar(Convert.ToInt16(token.Value)));
                        i++;
                    }
                    else
                        StoreByte(token.Value[i]);
                }
            }
            return token;
        }
    }
}
