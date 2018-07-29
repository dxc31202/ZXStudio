using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token ENTRY(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            int nextProgramCounter;
            if (token.Value == "$")
                EntryPoint = ProgramCounter;
            else
                EntryPoint = token.Number;
            return token;
        }
    }
}
