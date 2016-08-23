using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class Node
    {
    }

    public class FloatLiteral : Node
    {
        public double value;

        public FloatLiteral(double val)
        {
            this.value = val;
        }
    }

    public class Expression : Node
    {
        public List<Node> value = new List<Node>();
    }

    public class Declaration : Node
    {
        public string name;
        public Expression e;

        public Declaration(string name, Expression e = null)
        {
            this.name = name;
            this.e = e;
        }
    }

    public class VarPlaceholder : Node
    {
        public string name;

        public VarPlaceholder(string name)
        {
            this.name = name;
        }
    }

    public class Function : Node
    {
        public string name;
        public Expression value;
        public List<Declaration> args;
        public string exp;

        public Function(string name, Expression value, List<Declaration> args, string exp)
        {
            this.name = name;
            this.value = value;
            this.args = args;
            this.exp = exp;
        }
    }

    public class Trig : Node
    {
        public string func;
        public Expression exp;

        public Trig(string func, Expression exp)
        {
            this.func = func;
            this.exp = exp;
        }
    }

    public class Call : Node
    {
        public string name;
        public List<Expression> args;

        public Call(string name, List<Expression> args)
        {
            this.name = name;
            this.args = args;
        }
    }

    public class Operator : Node
    {
        public char op;

        public Operator(char op)
        {
            this.op = op;
        }

        public override string ToString()
        {
            return op.ToString();
        }
    }
}
