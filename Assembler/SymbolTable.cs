using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    public static class SymbolTable
    {
        public static void Clear()
        {
            Labels.Clear();
        }
        public static void Clear(string fileName)
        {
            Labels.RemoveAll(x => x.FileName == fileName);
            
        }
        public static List<Symbol> Labels = new List<Symbol>();
        public static Symbol Add(string fileName, int line, Token token, string name, int value)
        {
            if (Labels.Exists(x => x.FileName==fileName && x.Name == name))
            {
                Symbol s1 = new Symbol(fileName,name, token, value, -1);
                s1.UsedAtLines.Add(new UsedAt(fileName, token.Line, token));
                Labels.Add(s1);
                ErrorTable.Add(fileName, line, token, "Duplicate Label");
                return null;
            }


            Symbol label = new Symbol(fileName, name, token, value, line);
            Labels.Add(label);
            return label;
        }
        public static Symbol Add(string fileName, Token token, string name, int usedAt)
        {
            Symbol label;
            if (Labels.Exists(x => x.FileName== fileName && x.Name == name))
                label = Find(fileName,name);
            else
            {
                label = new Symbol(fileName, name, token);
            }
            label.Line = -1;

            UsedAt(fileName, -1, token, name, 0, usedAt);
            return label;
        }

        public static void UsedAt(string fileName, int line, Token token, string name, int value, int usedAt)
        {
            Symbol label = Find(fileName,name);
            if (label == null)
                label = Add(fileName, line, token, name, value);
            foreach (UsedAt ua in label.UsedAtLines)
            {
                if (ua.FileName == fileName && ua.Line == usedAt) return;
            }
            label.UsedAtLines.Add(new UsedAt(fileName,usedAt, token));
        }
        public static Symbol Find(string fileName, string name)
        {
            return Labels.Find(x => x.FileName== fileName && x.Name == name);
        }
        public static Symbol Resolve(string fileName, string name, int value, int line)
        {
            Symbol label = Labels.Find(x => x.FileName == fileName && x.Name == name);
            if (label != null)
            {
                if(!label.Resolved)
                    label.Resolve(fileName, value, line);
            }
            return label;
        }
        public static void Reset(string fileName)
        {
            Labels.RemoveAll(x => x.FileName == fileName);
            //Labels = new List<Symbol>();
        }

        public static int MaxLength
        {
            get
            {
                int max = 0;
                foreach (Symbol s in Labels)
                {
                    if (s.Name.Length > max)
                        max = s.Name.Length;
                }
                return max;
            }
        }
        public static string Table(string fileName)
        {

            int max = MaxLength;
            string output = "";
            output += '\n';
            output += "Symbol Table" + '\n';
            output += '\n';
            output += Justify("Name", max) + '\t' + "Value" + '\t' + "Defined at" + '\t' + "Used at" + '\n';
            output += new String('-', max) + '\t' + "-----" + '\t' + "----------" + '\t' + "-------" + '\n';
            foreach (Symbol s in Labels)
            {
                if (s.FileName != fileName) continue;
                output += Justify(s.Name, max) + '\t';
                if (s.Line > -1)
                {
                    output += s.Value.ToString("X4") + '\t';
                    output += Justify(s.Line.ToString(), 10) + '\t';
                }
                else
                {
                    output += "Invalid" + '\t';
                    output += "Invalid   " + '\t';
                }

                foreach (UsedAt ua in s.UsedAtLines)
                    if (ua.FileName == fileName)
                        output += ua.Line.ToString() + " ";
                output += Environment.NewLine;

            }
            return output;
        }

        public static string Justify(string value, int length)
        {
            string retval = value + new string(' ', length);
            return retval.Substring(0, length);
        }

    }
    public class UsedAt
    {
        public string FileName;
        public int Line;
        public Token Token;
        public UsedAt(string fileName, int line, Token token)
        {
            FileName = fileName;
            Line = line;
            Token = token;

        }
    }
    public class Symbol
    {
        public List<UsedAt> UsedAtLines = new List<UsedAt>();
        public string FileName;
        public string Name;
        public int Value;
        public int Line;
        public bool Resolved;
        public Token Token;
        public Symbol(string fileName, string name, Token token)
        {
            FileName= fileName;
            Name = name;
            Token = token;
            Resolved = false;
        }
        public Symbol(string fileName, string name, Token token, int value, int line)
        {
            FileName = fileName;
            Name = name;
            Token = token;
            Value = value;
            Line = line;
            if (line > -1)
                Resolved = true;
        }
        public void IsUsedAt(string fileName, int line, Token token)
        {
            foreach (UsedAt ua in UsedAtLines)
            {
                if (ua.FileName== fileName && ua.Line == line)
                    return;
            }
            UsedAtLines.Add(new UsedAt(fileName,line, token));


        }
        public void Resolve(string fileName, int value, int line)
        {
            FileName= fileName;
            Value = value;
            Line = line;
            if (line > -1)
                Resolved = true;
        }

        public override string ToString()
        {
            return "{FileName: " + FileName + ", Name: " + Name + ", Value: " + Value.ToString() + "}";

        }
    }
}

