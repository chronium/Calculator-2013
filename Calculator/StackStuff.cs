using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class StackStuff
    {
        public Stack<double> stack = new Stack<double>();

        public void push(double val)
        {
            stack.Push(val);
        }

        public void add()
        {
            double op2 = stack.Pop();
            double op1 = stack.Pop();
            stack.Push(op1 + op2);
        }

        public void sub()
        {
            double op2 = stack.Pop();
            double op1 = stack.Pop();
            stack.Push(op1 - op2);
        }

        public void mul()
        {
            double op2 = stack.Pop();
            double op1 = stack.Pop();
            stack.Push(op1 * op2);
        }

        public void div()
        {
            double op2 = stack.Pop();
            double op1 = stack.Pop();
            stack.Push(op1 / op2);
        }

        public void percent()
        {
            double op2 = stack.Pop();
            double op1 = stack.Pop();
            stack.Push((op1 / 100) * op2);
        }

        public void pow()
        {
            double op2 = stack.Pop();
            double op1 = stack.Pop();
            stack.Push(Math.Pow(op1, op2));
        }

        public void fact()
        {
            stack.Push(factorial(stack.Pop()));
        }

        public double factorial(double n)
        {
            if (n == 0)
                return 1;
            else
                return n * factorial(n - 1);
        }

        public double getResult()
        {
            return stack.Count != 0 ? stack.Pop() : 0;
        }
    }
}
