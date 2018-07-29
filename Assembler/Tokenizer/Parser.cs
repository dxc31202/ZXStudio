using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    public class Parser
    {
        public const char START_ARG = '(';
        public const char END_ARG = ')';
        public const char END_LINE = '\n';
        public static int LineNo;
        class Cell
        {
            internal Cell(double value, char action)
            {
                Value = value;
                Action = action;
            }

            internal double Value { get; set; }
            internal char Action { get; set; }
        }
        internal static bool IsValid = true;
        static Token Token;
        static string currentFileName;
        internal static double Evaluate(string fileName, string input, int addr,int offset,Token token)
        {
            currentFileName = fileName;
            Token = token;
            LineNo = token.Line;
            return process(Split(input, addr,offset ));
        }

        internal static double process(string data)
        {
            IsValid = true;
            // Get rid of spaces and check parenthesis
            string expression = preprocess(data);
            int from = 0;

            return loadAndCalculate(data, ref from, END_LINE);
        }

        static string preprocess(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                IsValid = false;
                //throw new ArgumentException("Loaded empty data");
            }

            int parentheses = 0;
            StringBuilder result = new StringBuilder(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                char ch = data[i];
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\n': continue;
                    case END_ARG:
                        parentheses--;
                        break;
                    case START_ARG:
                        parentheses++;
                        break;
                }
                result.Append(ch);
            }

            if (parentheses != 0)
            {
                IsValid = false;
                throw new ArgumentException("Uneven parenthesis");
            }

            return result.ToString();
        }

        internal static double loadAndCalculate(string data, ref int from, char to = END_LINE)
        {
            if (from >= data.Length || data[from] == to)
            {
                IsValid = false;
                throw new ArgumentException("Loaded invalid data: " + data);
            }

            List<Cell> listToMerge = new List<Cell>(16);
            StringBuilder item = new StringBuilder();

            do
            { // Main processing cycle of the first part.
                char ch = data[from++];
                if (stillCollecting(item.ToString(), ch, to))
                { // The char still belongs to the previous operand.
                    item.Append(ch);
                    if (from < data.Length && data[from] != to)
                    {
                        continue;
                    }
                }

                // We are done getting the next token. The getValue() call below may
                // recursively call loadAndCalculate(). This will happen if extracted
                // item is a function or if the next item is starting with a START_ARG '('.
                ParserFunction func = new ParserFunction(data, ref from, item.ToString(), ch);
                double value = func.getValue(data, ref from);

                char action = validAction(ch) ? ch
                                              : updateAction(data, ref from, ch, to);

                listToMerge.Add(new Cell(value, action));
                item.Clear();

            } while (from < data.Length && data[from] != to);

            if (from < data.Length &&
               (data[from] == END_ARG || data[from] == to))
            { // This happens when called recursively: move one char forward.
                from++;
            }

            Cell baseCell = listToMerge[0];
            int index = 1;

            return merge(baseCell, ref index, listToMerge);
        }

        static bool stillCollecting(string item, char ch, char to)
        {
            // Stop collecting if either got END_ARG ')' or to char, e.g. ','.
            char stopCollecting = (to == END_ARG || to == END_LINE) ?
                                   END_ARG : to;
            return (item.Length == 0 && (ch == '-' || ch == END_ARG)) ||
                  !(validAction(ch) || ch == START_ARG || ch == stopCollecting);
        }

        static bool validAction(char ch)
        {
            return ch == '|' || ch == '&' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '^' || ch == '%';
        }

        static char updateAction(string item, ref int from, char ch, char to)
        {
            if (from >= item.Length || item[from] == END_ARG || item[from] == to)
            {
                return END_ARG;
            }

            int index = from;
            char res = ch;
            while (!validAction(res) && index < item.Length)
            { // Look for the next character in string until a valid action is found.
                res = item[index++];
            }

            from = validAction(res) ? index
                                    : index > from ? index - 1
                                                   : from;
            return res;
        }

        // From outside this function is called with mergeOneOnly = false.
        // It also calls itself recursively with mergeOneOnly = true, meaning
        // that it will return after only one merge.
        static double merge(Cell current, ref int index, List<Cell> listToMerge,
                     bool mergeOneOnly = false)
        {
            while (index < listToMerge.Count)
            {
                Cell next = listToMerge[index++];

                while (!canMergeCells(current, next))
                { // If we cannot merge cells yet, go to the next cell and merge
                  // next cells first. E.g. if we have 1+2*3, we first merge next
                  // cells, i.e. 2*3, getting 6, and then we can merge 1+6.
                    merge(next, ref index, listToMerge, true /* mergeOneOnly */);
                }
                mergeCells(current, next);
                if (mergeOneOnly)
                {
                    return current.Value;
                }
            }

            return current.Value;
        }

        static void mergeCells(Cell leftCell, Cell rightCell)
        {
            switch (leftCell.Action)
            {
                case '^':
                    //leftCell.Value = Math.Pow(leftCell.Value, rightCell.Value);
                    leftCell.Value = (int)leftCell.Value ^ (int)rightCell.Value;
                    break;
                case '|':
                    leftCell.Value = (int)leftCell.Value | (int)rightCell.Value;
                    break;
                case '&':
                    leftCell.Value = (int)leftCell.Value & (int)rightCell.Value;
                    break;
                case '*':
                    leftCell.Value *= rightCell.Value;
                    break;
                case '/':
                    if (rightCell.Value == 0)
                    {
                        IsValid = false;
                        throw new ArgumentException("Division by zero");
                    }
                    leftCell.Value /= rightCell.Value;
                    break;
                case '+':
                    leftCell.Value += rightCell.Value;
                    break;
                case '-':
                    leftCell.Value -= rightCell.Value;
                    break;
                case '%':
                    leftCell.Value %= rightCell.Value;
                    break;
            }
            leftCell.Action = rightCell.Action;
        }

        static bool canMergeCells(Cell leftCell, Cell rightCell)
        {
            return getPriority(leftCell.Action) >= getPriority(rightCell.Action);
        }

        static int getPriority(char action)
        {
            switch (action)
            {
                case '&':
                case '^': return 4;
                case '|':
                case '*':
                case '/': return 3;
                case '+':
                case '-': return 2;
                case '%': return 2;
            }
            return 0;
        }
        static string Split(string input, int addr, int offset)
        {
            string result = "";
            string current = "";
            int value;
            Symbol symbol;
            string lastsign = "";
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '&':
                    case '|':
                    case '^':
                    case '-':
                    case '+':
                    case '*':
                    case '/':
                    case '%':
                        {
                            
                            lastsign = input[i].ToString();
                            if (current.Length > 0)
                            {
                                value=0;

                                if (!int.TryParse(current, out value))
                                {
                                    symbol = SymbolTable.Find(currentFileName, current);
                                    if (symbol != null && symbol.Resolved)
                                    {
                                        //if (!symbol.UsedAtLines.Contains(new UsedAt(Token.Line, Token)))
                                        //    symbol.UsedAtLines.Add(new UsedAt(Token.Line, Token));
                                        current = symbol.Value.ToString();
                                    }
                                    if(symbol!=null)
                                        symbol.IsUsedAt(currentFileName, Token.Line, Token);
                                }
                                result += current;
                                current = "";
                            }

                            if (i == 0)
                                current += "0";
                            symbol = SymbolTable.Find(currentFileName, current);
                            if (symbol != null && symbol.Resolved)
                            {
                                symbol.IsUsedAt(currentFileName, Token.Line, Token);
                                //if (!symbol.UsedAtLines.Contains(new UsedAt(Token.Line, Token)))
                                //    symbol.UsedAtLines.Add(new UsedAt(Token.Line, Token));
                                result += symbol.Value.ToString();
                            }
                            else
                                result += current;
                            current = "";

                            result += input[i].ToString();
                        }
                        break;
                    case '$':
                        // TODO
                        result += (addr-  offset).ToString();
                        //result += input[i].ToString();
                        break;
                    default:
                        {
                            current += input[i].ToString();
                        }
                        break;
                }
            }
            value = 0;
            if (!int.TryParse(current, out value))
            {
                symbol = SymbolTable.Find(currentFileName, current);
                if (symbol != null && symbol.Resolved)
                {
                    symbol.IsUsedAt(currentFileName, Token.Line, Token);
                    //if (!symbol.UsedAtLines.Contains(new UsedAt(Token.Line, Token)))
                    //    symbol.UsedAtLines.Add(new UsedAt(Token.Line, Token));
                    current = symbol.Value.ToString();
                }
            }
            result += current;
            return result;
        }
    }

    internal class ParserFunction
    {
        internal ParserFunction()
        {
            m_impl = this;
        }

        // A "virtual" Constructor
        internal ParserFunction(string data, ref int from, string item, char ch)
        {
            if (item.Length == 0 && ch == Parser.START_ARG)
            {
                // There is no function, just an expression in parentheses
                m_impl = s_idFunction;
                return;
            }

            if (m_functions.TryGetValue(item, out m_impl))
            {
                // Function exists and is registered (e.g. pi, exp, etc.)
                return;
            }

            // Function not found, will try to parse this as a number.
            s_strtodFunction.Item = item;
            m_impl = s_strtodFunction;
        }

        internal static void addFunction(string name, ParserFunction function)
        {
            m_functions[name] = function;
        }

        internal double getValue(string data, ref int from)
        {
            return m_impl.evaluate(data, ref from);
        }

        protected virtual double evaluate(string data, ref int from)
        {
            // The real implementation will be in the derived classes.
            return 0;
        }

        private ParserFunction m_impl;
        private static Dictionary<string, ParserFunction> m_functions = new Dictionary<string, ParserFunction>();

        private static StrtodFunction s_strtodFunction = new StrtodFunction();
        private static IdentityFunction s_idFunction = new IdentityFunction();
    }

    class StrtodFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double num;
            if (!Double.TryParse(Item, out num))
            {
                Parser.IsValid = false;
                //throw new ArgumentException("Could not parse token [" + Item + "]");
            }
            return num;
        }
        internal string Item { private get; set; }
    }

    class IdentityFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            return Parser.loadAndCalculate(data, ref from, Parser.END_ARG);
        }
    }

    class PiFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            return 3.141592653589793;
        }
    }
    class ExpFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);
            return Math.Exp(arg);
        }
    }
    class PowFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg1 = Parser.loadAndCalculate(data, ref from, ',');
            double arg2 = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);

            return Math.Pow(arg1, arg2);
        }
    }
    class SinFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);
            return Math.Sin(arg);
        }
    }
    class SqrtFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);
            return Math.Sqrt(arg);
        }
    }
    class AbsFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);
            return Math.Abs(arg);
        }
    }


}
