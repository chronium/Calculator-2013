using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class Parser
    {
        public List<Token> tokens;
        public List<Node> tree;

        int i = 0;

        private Token peek(int offset = 0)
        {
            if (i <= tokens.Count)
            {
                try
                {
                    return tokens[i + offset];
                }
                catch { }
                Tokens.Statement stat = new Tokens.Statement();
                stat.Name = "";
                return stat;
            }
            else
            {
                Tokens.Statement stat = new Tokens.Statement();
                stat.Name = "";
                return stat;
            }
        }

        private Token read()
        {
            if (i <= tokens.Count)
            {
                try
                {
                    return tokens[i++];
                }
                catch { }
                Tokens.Statement stat = new Tokens.Statement();
                stat.Name = "";
                return stat;
            }
            else
            {
                Tokens.Statement stat = new Tokens.Statement();
                stat.Name = "";
                return stat;
            }
        }

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            tree = new List<Node>();
        }

        public bool Parse()
        {
            while (i < tokens.Count)
            {
                if (peek() is Tokens.Statement && peek(1) is Tokens.Assign)
                {
                    string name = read().ToString();
                    read();
                    List<Token> tok = new List<Token>();
                    while (!(peek() is Tokens.EOL))
                        tok.Add(read());
                    tree.Add(new Declaration(name, parseExpression(tok)));

                }
                else if (peek().ToString() == "def" && peek(1) is Tokens.Statement && peek(2) is Tokens.openParenthesis)
                {
                    read();
                    string name = read().ToString();
                    read();

                    List<Declaration> args = new List<Declaration>();

                    while (!(peek() is Tokens.closeParenthesis))
                    {
                        args.Add(new Declaration(read().ToString()));
                        if (peek() is Tokens.Comma)
                            read();
                    }
                    read();
                    if (peek() is Tokens.Assign)
                    {
                        read();
                        string s = "";
                        List<Token> tok = new List<Token>();
                        while (!(peek() is Tokens.EOL))
                            tok.Add(read());

                        foreach (Token t in tok)
                            if (t is Tokens.IntLiteral)
                                s += ((Tokens.IntLiteral)t).Value + " ";
                            else if (t is Tokens.FloatLiteral)
                                s += ((Tokens.FloatLiteral)t).Value + " ";
                            else
                                s += t + " ";

                        tree.Add(new Function(name, parseExpression(tok), args, s));
                    }
                }
                else
                {
                    List<Token> tok = new List<Token>();
                    while (!(peek() is Tokens.EOL))
                        tok.Add(read());
                    tree.Add(parseExpression(tok));
                }
                if (peek() is Tokens.EOL)
                    break;
            }
            read();
            read();
            return true;
        }

        public Expression parseExpression(List<Token> tokens)
        {
            Expression result = new Expression();
            Stack<Operator> operators = new Stack<Operator>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (isOperator(tokens[i]))
                {
                    while (operators.Count != 0 && operators.Peek().op != '(' && hasHigherPrecedence(operators.Peek(), new Operator(tokens[i].ToString()[0])))
                    {
                        result.value.Add(operators.Pop());
                    }
                    operators.Push(new Operator(tokens[i].ToString()[0]));
                }
                else if (tokens[i] is Tokens.IntLiteral)
                {
                    result.value.Add(new FloatLiteral((double)((Tokens.IntLiteral)tokens[i]).Value));
                    if (i < tokens.Count - 1)
                        if (tokens[i + 1] is Tokens.Statement)
                        {
                            result.value.Add(new VarPlaceholder(tokens[++i].ToString()));
                            operators.Push(new Operator('*'));
                        }
                }
                else if (tokens[i] is Tokens.FloatLiteral)
                {
                    result.value.Add(new FloatLiteral((double)((Tokens.FloatLiteral)tokens[i]).Value));
                    if (i < tokens.Count - 1)
                        if (tokens[i + 1] is Tokens.Statement)
                        {
                            result.value.Add(new VarPlaceholder(tokens[++i].ToString()));
                            operators.Push(new Operator('*'));
                        }
                }
                else if (tokens[i] is Tokens.openParenthesis)
                {
                    operators.Push(new Operator(tokens[i].ToString()[0]));
                }
                else if (tokens[i] is Tokens.closeParenthesis)
                {
                    while (operators.Count != 0 && operators.Peek().op != '(')
                    {
                        result.value.Add(operators.Pop());
                    }
                    operators.Pop();
                }
                else if (tokens[i].ToString() == "sin")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "sin"));
                }
                else if (tokens[i].ToString() == "cos")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "cos"));
                }
                else if (tokens[i].ToString() == "tan")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "tan"));
                }
                else if (tokens[i].ToString() == "cot")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "cot"));
                }
                else if (tokens[i].ToString() == "sec")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "sec"));
                }
                else if (tokens[i].ToString() == "csc")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "csc"));
                }
                else if (tokens[i].ToString() == "log")
                {
                    i++;
                    result.value.Add(parseTrig(tokens, operators, ref i, "log"));
                }
                else if (tokens[i] is Tokens.Statement)
                {
                    if (i < tokens.Count - 1)
                    {
                        if (tokens[i + 1] is Tokens.openParenthesis)
                        {
                            string name = tokens[i++].ToString();
                            int parentheses = 1;
                            i++;

                            List<Expression> args = new List<Expression>();
                            while (parentheses > 0)
                            {
                                List<Token> tok = new List<Token>();
                                int startParenth = parentheses - 1;
                                while (!(tokens[i] is Tokens.Comma) && parentheses != startParenth)
                                {
                                    if (tokens[i] is Tokens.openParenthesis)
                                        parentheses++;
                                    else if (tokens[i] is Tokens.closeParenthesis)
                                        parentheses--;
                                    if (startParenth == parentheses)
                                        break;
                                    tok.Add(tokens[i++]);
                                }
                                args.Add(parseExpression(tok));
                                if (parentheses == 0) break;
                                if (tokens[i] is Tokens.Comma)
                                    i++;
                                else if (tokens[i] is Tokens.openParenthesis)
                                {
                                    parentheses++;
                                    i++;
                                }
                                else if (tokens[i] is Tokens.closeParenthesis)
                                {
                                    parentheses--;
                                    i++;
                                }
                            }
                            i++;
                            result.value.Add(new Call(name, args));
                        }
                        else
                        {
                            result.value.Add(new VarPlaceholder(tokens[i].ToString()));
                        }
                    }
                    else
                    {
                        result.value.Add(new VarPlaceholder(tokens[i].ToString()));
                    }
                }
            }
            while (operators.Count != 0)
            {
                result.value.Add(operators.Pop());
            }
            return result;
        }

        private Trig parseTrig(List<Token> tokens, Stack<Operator> operators, ref int i, string func)
        {
            Expression e = new Expression();
            if (!(tokens[i] is Tokens.openParenthesis))
            {
                if (tokens[i] is Tokens.IntLiteral)
                {
                    e.value.Add(new FloatLiteral((double)((Tokens.IntLiteral)tokens[i]).Value));
                    if (i < tokens.Count - 1)
                        if (tokens[i + 1] is Tokens.Statement)
                        {
                            e.value.Add(new VarPlaceholder(tokens[++i].ToString()));
                            operators.Push(new Operator('*'));
                        }
                }
                else if (tokens[i] is Tokens.FloatLiteral)
                {
                    e.value.Add(new FloatLiteral((double)((Tokens.FloatLiteral)tokens[i]).Value));
                    if (i < tokens.Count - 1)
                        if (tokens[i + 1] is Tokens.Statement)
                        {
                            e.value.Add(new VarPlaceholder(tokens[++i].ToString()));
                            operators.Push(new Operator('*'));
                        }
                }
            }
            else
            {
                List<Token> tok = new List<Token>();
                int parentheses = 1;
                i++;
                int startParenth = parentheses - 1;
                while (!(tokens[i] is Tokens.Comma))
                {
                    if (tokens[i] is Tokens.openParenthesis)
                        parentheses++;
                    else if (tokens[i] is Tokens.closeParenthesis)
                        parentheses--;
                    if (startParenth == parentheses)
                        break;
                    tok.Add(tokens[i++]);
                }
                i++;
                e = parseExpression(tok);
            }
            return new Trig(func, e);
        }

        public static int getWeight(Operator op)
        {
            int weight = -1;
            switch (op.op)
            {
                case '+':
                case '-':
                    weight = 1;
                    break;
                case '*':
                case '/':
                    weight = 2;
                    break;
                case '^':
                    weight = 3;
                    break;
                case '!':
                    weight = 5;
                    break;
                case '%':
                    weight = 4;
                    break;
            }

            return weight;
        }

        public static bool isOperator(Token t)
        {
            return t is Tokens.Add || t is Tokens.Sub || t is Tokens.Mul || t is Tokens.Div || t is Tokens.Percent || t is Tokens.Pow || t is Tokens.Exclamation;
        }

        public static bool hasHigherPrecedence(Operator op1, Operator op2)
        {
            return getWeight(op1) >= getWeight(op2);
        }
    }
}
