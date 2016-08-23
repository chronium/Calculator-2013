using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator
{
    class Program
    {
        private static bool running = true;

        static void Main(string[] args)
        {
            List<Variable> variables = new List<Variable>();
            Dictionary<string, Function> func = new Dictionary<string, Function>();
            while (running)
            {
                Console.Write('>');
                string input = Console.ReadLine(); if (input.StartsWith("rem"))
                {
                    string s = input.Substring(3);
                    s = s.Trim();
                    Console.WriteLine(">> " + s);
                }
                else
                {
                    Lexer l = new Lexer();
                    l.ScanWhiteSpaces = true;
                    l.Scan(new StringReader(input));
                    Parser p = new Parser(l.tokens);
                    if (p.Parse())

                        foreach (Node n in p.tree)
                        {
                            if (n is Expression)
                            {
                                StackStuff stack = new StackStuff();
                                if (interpretExpression(stack, (Expression)n, variables, func))
                                    Console.WriteLine(">> " + string.Format("{0:N2}", stack.getResult()));
                            }
                            else if (n is Declaration)
                            {
                                StackStuff stack = new StackStuff();
                                if (interpretExpression(stack, ((Declaration)n).e, variables, func))
                                {
                                    if (!isVariable(variables, ((Declaration)n).name))
                                    {
                                        Variable v = new Variable();
                                        v.name = ((Declaration)n).name;
                                        v.value = stack.getResult();
                                        variables.Add(v);
                                        Console.WriteLine(">> " + v.name + " = [" + v.value + "]");
                                    }
                                    else
                                    {
                                        Variable v = getVariable(variables, (((Declaration)n).name));
                                        v.value = stack.getResult();
                                        Console.WriteLine(">> " + v.name + " = [" + v.value + "]");
                                    }
                                }
                            }
                            else if (n is Function)
                            {
                                StackStuff stack = new StackStuff();
                                List<Variable> vars = new List<Variable>();

                                foreach (Declaration arg in ((Function)n).args)
                                {
                                    Variable v = new Variable();
                                    v.name = arg.name;
                                    vars.Add(v);
                                }

                                if (getFunction(func, ((Function)n).name) != null)
                                    func[((Function)n).name] = (Function)n;
                                else
                                    func.Add(((Function)n).name, (Function)n);

                                Console.WriteLine(">> " + ((Function)n).name + " => " + ((Function)n).exp);
                            }
                        }
                }
            }
        }

        public static bool isVariable(List<Variable> vars, string name)
        {
            foreach (Variable v in vars)
                if (v.name == name)
                    return true;
            return false;
        }

        public static Variable getVariable(List<Variable> vars, string name)
        {
            foreach (Variable v in vars)
                if (v.name == name)
                    return v;
            return null;
        }

        public static Function getFunction(Dictionary<string, Function> funcs, string name)
        {
            foreach (var v in funcs)
                if (v.Key == name)
                    return v.Value;
            return null;
        }

        private static bool interpretExpression(StackStuff stack, Expression n, List<Variable> vars, Dictionary<string, Function> func)
        {
            foreach (Node e in n.value)
            {
                if (e is FloatLiteral)
                {
                    stack.push(((FloatLiteral)e).value);
                }
                else if (e is Operator)
                {
                    switch (((Operator)e).op)
                    {
                        case '+':
                            stack.add();
                            break;
                        case '-':
                            stack.sub();
                            break;
                        case '*':
                            stack.mul();
                            break;
                        case '/':
                            stack.div();
                            break;
                        case '^':
                            stack.pow();
                            break;
                        case '!':
                            stack.fact();
                            break;
                        case '%':
                            stack.percent();
                            break;
                    }
                }
                else if (e is VarPlaceholder)
                {
                    if (isVariable(vars, ((VarPlaceholder)e).name))
                    {
                        stack.push(getVarValue(vars, ((VarPlaceholder)e).name));
                    }
                    else
                    {
                        Console.WriteLine("Error: " + ((VarPlaceholder)e).name + " is not defined!!!");
                        return false;
                    }
                }
                else if (e is Trig)
                {
                    switch (((Trig)e).func)
                    {
                        case "sin":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(Math.Sin(stack2.getResult()));
                            }
                            break;
                        case "cos":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(Math.Cos(stack2.getResult()));
                            }
                            break;
                        case "tan":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(Math.Tan(stack2.getResult()));
                            }
                            break;
                        case "cot":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(1 / Math.Tan(stack2.getResult()));
                            }
                            break;
                        case "sec":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(1 / Math.Cos(stack2.getResult()));
                            }
                            break;
                        case "csc":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(1 / Math.Sin(stack2.getResult()));
                            }
                            break;
                        case "log":
                            {
                                StackStuff stack2 = new StackStuff();

                                interpretExpression(stack2, ((Trig)e).exp, vars, func);

                                stack.push(Math.Log10(stack2.getResult()));
                            }
                            break;
                    }
                }
                else if (e is Call)
                {
                    StackStuff stack2 = new StackStuff();
                    List<Variable> vars2 = new List<Variable>();
                    Function f = getFunction(func, ((Call)e).name);

                    if (((Call)e).args.Count == f.args.Count)
                    {
                        for (int i = 0; i < ((Call)e).args.Count; i++)
                        {
                            Expression exp = ((Call)e).args[i];

                            Variable v = new Variable();
                            v.name = f.args[i].name;
                            if (interpretExpression(stack2, exp, vars, func))
                            {
                                v.value = stack2.getResult();
                                vars2.Add(v);
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine(">> " + ((Call)e).name + " with " + ((Call)e).args.Count + " arguments is not declared!");
                        return false;
                    }

                    interpretExpression(stack2, f.value, vars2, func);
                    stack.push(stack2.getResult());
                }
            }
            return true;
        }

        public static double getVarValue(List<Variable> vars, string name)
        {
            foreach (Variable v in vars)
                if (v.name == name)
                    return v.value;
            return 0;
        }
    }

    public class Variable
    {
        public string name;
        public double value;
    }
}
