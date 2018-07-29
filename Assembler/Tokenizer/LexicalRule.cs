using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZXStudio
{
    public class LexicalRule
    {
        public TokenType Type { get; set; }

        public Regex RegExpression { get; set; }

        public static Regex WordRegex(params string[] words)
        {
            return new Regex("^((" + string.Join(")|(", words) + "))\\b");
        }
    }
}
