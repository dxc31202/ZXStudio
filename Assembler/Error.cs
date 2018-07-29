using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    internal static class ErrorTable
    {
        public static void Clear()
        {
            Errors.Clear();
        }

        public static List<Error> Errors = new List<Error>();
        public static Error Add(string fileName, int line, Token token, string text)
        {
            Error error = null;
            if (token != null)
                error = new Error(fileName, line, token.Col, text, token);
            else
                error = new Error(fileName, line, 0, text);
            Errors.Add(error);
            return error;
        }

        public static bool HasErrors(string fileName)
        {
            return Errors.FindAll(x => x.FileName == fileName).Count > 0;
        }
        internal static Error Find(string fileName, int lineno)
        {
            return Errors.Find(x => x.FileName == fileName && x.Line == lineno);
        }

        public static void Reset()
        {
            Errors.Clear();
            Errors = new List<Error>();
        }

        public static string ErrorList(string fileName)
        {
            string retval = "";
            if (HasErrors(fileName))
            {
                retval = '\n' + "Errors" + '\n';
                retval += "------" + '\n';
                foreach (Error e in Errors)
                {
                    if (e.FileName != fileName) continue;
                    if (e.Token == null)
                        retval += Path.GetFileName(e.FileName) + '\t' + e.Line.ToString() + '\t' + "" + '\t' + e.Text + '\n';
                    else
                        retval += Path.GetFileName(e.FileName) + '\t' + e.Line.ToString() + '\t' + e.Token.ToString() + '\t' + e.Text + '\n';
                }
                retval += '\n';
            }
            return retval;

        }

    }
    //            outputWindow.Clear();

    class Error
    {
        public string FileName;
        public int Line;
        public int Column;
        public string Text;
        public Token Token;
        public Error(string fileName, int line, int column, string text)
        {
            FileName = fileName;
            Line = line;
            Column = column;
            Text = text;
        }
        public Error(string fileName, int line, int column, string text,Token token)
        {
            FileName = fileName;
            Token = token;
            Line = line;
            Column = column;
            Text = text;
        }
        public override string ToString()
        {
            return "Line: " + Line.ToString() + " " +
            "Column: " + Column.ToString() + " '" +
            Text + "'";
            
        }
    }
}
