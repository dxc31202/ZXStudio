using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token EQU(Token token)
        {
            Symbol symbol = SymbolTable.Find(currentFileName, token.First.Value.Replace(":", ""));
            if (symbol == null)
                SymbolTable.Add(currentFileName, currentLine, token, token.Previous.Value.Replace(":", ""), token.Next.Number);
            else
                SymbolTable.Resolve(currentFileName, token.First.Value.Replace(":", ""), token.Next.Number, currentLine);
            return null;
        }
    }
}
