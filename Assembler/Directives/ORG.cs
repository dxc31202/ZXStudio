using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token ORG(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            int nextProgramCounter = token.Number;
            if (FirstAddress == -1)
                FirstAddress = nextProgramCounter;
            if (nextProgramCounter < FirstAddress)
                    FirstAddress = nextProgramCounter;
            if (EntryPoint == -1)
                EntryPoint = nextProgramCounter;
            ProgramCounter = nextProgramCounter;
            return token;
        }
    }
}
