using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token DEFS(Token token)
        {
            /*
                ds stands for "define space".
                It takes one or two arguments, num and val.
                It reserves num bytes of space and initializes them to val.
                If val is omitted, it defaults to 0.Example:
                buffer: defs 20
                sevens: defs 10, 7
           */
            {
                if ((token = NextToken(token)) == null) return token;
                int count = GetValue(ref token);
                int value = 0;
                //if(count==0&&currentPass==2    )
                //{
                //    LogError(currentFileName, token,"DEFS: Missing value!");
                //    return token;
                //}
                token = token.Next;
                if (token != null)
                {
                    if (CheckComma(token, false)) if ((token = NextToken(token)) == null)
                        {
                            return token;
                        }
                    value = GetValue(ref token);
                    token = token.Next;


                }
                //if (value > 0)
                    for (int i = 0; i < count; i++)
                        StoreByte(value);
                //else
                //    ProgramCounter += count;
            }
            return token;
        }
    }
}
