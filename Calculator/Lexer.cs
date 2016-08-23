/*
* This file is part of GruntTheDivine's Cobalt Compiler.
*
* Cobalt is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* Cobalt is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Cobalt.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator
{
    public class Token
    {
        public int lineNumber;

    }
    public static class Tokens
    {
        public class openParenthesis : Token { public override string ToString() { return "("; } }
        public class closeParenthesis : Token { public override string ToString() { return ")"; } }
        public class openCurlyBracket : Token { public override string ToString() { return "{"; } }
        public class closeCurlyBracket : Token { public override string ToString() { return "}"; } }
        public class openBracket : Token { public override string ToString() { return "["; } }
        public class closeBracket : Token { public override string ToString() { return "]"; } }
        public class Assign : Token { public override string ToString() { return "="; } }
        public class Equal : Token { public override string ToString() { return "=="; } }
        public class Add : Token { public override string ToString() { return "+"; } }
        public class Sub : Token { public override string ToString() { return "-"; } }
        public class Div : Token { public override string ToString() { return "/"; } }
        public class Mul : Token { public override string ToString() { return "*"; } }
        public class Percent : Token { public override string ToString() { return "%"; } }
        public class Dot : Token { public override string ToString() { return "."; } }
        public class Pipe : Token { public override string ToString() { return "|"; } }
        public class And : Token { public override string ToString() { return "&"; } }
        public class At : Token { public override string ToString() { return "@"; } }
        public class Colon : Token { public override string ToString() { return ":"; } }
        public class SemiColon : Token { public override string ToString() { return ";"; } }
        public class Comma : Token { public override string ToString() { return ","; } }
        public class Hash : Token { public override string ToString() { return "#"; } }
        public class Question : Token { public override string ToString() { return "?"; } }
        public class Tilda : Token { public override string ToString() { return "~"; } }
        public class Pow : Token { public override string ToString() { return "^"; } }
        public class Underline : Token { public override string ToString() { return "_"; } }
        public class Dollah : Token { public override string ToString() { return "$"; } }
        public class Exclamation : Token { public override string ToString() { return "!"; } }
        public class StringLiteral : Token { public string Value; }
        public class IntLiteral : Token { public int Value; }
        public class FloatLiteral : Token { public double Value; }
        public class LeftBitshift : Token { public override string ToString() { return "<<"; } };
        public class RightBitshift : Token { public override string ToString() { return ">>"; } };
        public class NotEqual : Token { public override string ToString() { return "!="; } };
        public class GreaterThan : Token { public override string ToString() { return ">"; } };
        public class LessThan : Token { public override string ToString() { return "<"; } };
        public class Increment : Token { public override string ToString() { return "++"; } };
        public class Decrement : Token { public override string ToString() { return "--"; } };
        public class PlusEquals : Token { public override string ToString() { return "+="; } };
        public class BooleanAnd : Token { public override string ToString() { return "&&"; } };
        public class BooleanOr : Token { public override string ToString() { return "||"; } };
        public class PowEqual : Token { public override string ToString() { return "^="; } };
        public class EOL : Token { public override string ToString() { return ""; } };
        public class ObjectReference : Token
        {
            public override string ToString()
            {
                return "->";
            }
        }
        public class Dereference : Token
        {
            public string Name;
            public override string ToString()
            {
                return Name;
            }

        }
        public class Statement : Token
        {
            public string Name;
            public override string ToString()
            {
                return Name;
            }

        }
    }
    public class Lexer
    {
        private StringReader input;
        public List<Token> tokens = new List<Token>();
        private int lineNumber = 0;
        private Thread scannerThread;
        private bool failed = false;
        private bool encountedEOS = false;
        public bool ScanWhiteSpaces = false;
        public string Commment = "";
        public Lexer()
        {
        }
        private void addToken(Token tk)
        {
            tk.lineNumber = this.lineNumber;
            tokens.Add(tk);
        }
        private int peek()
        {
            return input.Peek();
        }
        private int read()
        {
            if (input.Peek() == -1)
                encountedEOS = true;
            else if (input.Peek() == -1 && encountedEOS)
            {
                failed = true;
                scannerThread.Abort();
            }
            return input.Read();
        }
        public void Scan(System.IO.StringReader sr)
        {
            input = new StringReader(sr.ReadToEnd().ToString() + "\n\n");
            scannerThread = new Thread(doScan);
            scannerThread.Start();
            while (scannerThread.IsAlive) ;
            if (failed)
                throw new Exception("Newline in constant");
        }
        private void doScan()
        {

            char val = (char)(byte)read();

            while (peek() != -1)
            {


                while (char.IsWhiteSpace(val) || val == '\n')
                {
                    if (val == '\n' || val == '\r' || val == Environment.NewLine[0])
                    {
                        if (ScanWhiteSpaces)
                            addToken(new Tokens.EOL());
                        this.lineNumber++;
                    }
                    val = (char)(byte)read();

                }
                if (peek() == -1)
                    break;
                if (char.IsLetter(val))
                {
                    StringBuilder sb = new StringBuilder();
                    while (char.IsLetterOrDigit(val) || val == '_')
                    {
                        sb.Append(val);
                        val = (char)(byte)read();

                    }
                    Tokens.Statement stmt = new Tokens.Statement();
                    stmt.Name = sb.ToString();

                    addToken(stmt);
                }
                else if (val == '/' && (char)(byte)peek() == '/')
                {
                    while (val != '\n' && val != '\r')
                        val = (char)(byte)read();
                }
                else if (val == '0' && (char)(byte)peek() == 'x')
                {
                    read();
                    val = (char)(byte)read();
                    StringBuilder sb = new StringBuilder();
                    while (char.IsDigit(val) || val.ToString().ToLower() == "a" || val.ToString().ToLower() == "b" || val.ToString().ToLower() == "c" || val.ToString().ToLower() == "d" || val.ToString().ToLower() == "e" || val.ToString().ToLower() == "f")
                    {
                        sb.Append(val);
                        val = (char)(byte)read();
                    }
                    Tokens.IntLiteral il = new Tokens.IntLiteral();
                    il.Value = Int32.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber);
                    addToken(il);


                }
                else if (val == '\'')
                {
                    Tokens.IntLiteral il = new Tokens.IntLiteral();
                    bool b = false;
                    if (((char)peek() == '\\'))
                    {
                        read();
                        if (((char)read()).ToString().ToLower()[0] == 'n')
                            il.Value = 10;
                        else if (((char)read()).ToString().ToLower()[0] == 'a')
                            il.Value = 7;
                        else if (((char)read()).ToString().ToLower()[0] == 'b')
                            il.Value = 8;
                        else if (((char)read()).ToString().ToLower()[0] == 'f')
                            il.Value = 12;
                        else if (((char)read()).ToString().ToLower()[0] == 'r')
                            il.Value = 13;
                        else if (((char)read()).ToString().ToLower()[0] == 't')
                            il.Value = 9;
                        else if (((char)read()).ToString().ToLower()[0] == 'v')
                            il.Value = 11;
                        else if (((char)read()).ToString().ToLower()[0] == '\'')
                            il.Value = 44;
                        else if (((char)read()).ToString().ToLower()[0] == '\"')
                            il.Value = 34;
                        b = true;
                    }
                    else
                    {
                        il.Value = read();
                        if (((char)read()) != '\'')
                            throw new Exception("Expected '");
                    }
                    if (b)
                        read();
                    b = false;
                    val = ((char)read());
                    this.tokens.Add(il);
                }
                else if (char.IsDigit(val) || (val == '-' && char.IsDigit((char)(byte)peek())))
                {
                    StringBuilder sb = new StringBuilder();
                    while (char.IsDigit(val) || val == '.' || val == '-')
                    {
                        sb.Append(val);
                        val = (char)(byte)read();

                    }
                    if (sb.ToString().Contains(".") || ((char)peek()) == 'f')
                    {
                        if (((char)peek()) == 'f')
                            val = (char)(byte)read();
                        Tokens.FloatLiteral fl = new Tokens.FloatLiteral();
                        fl.Value = float.Parse(sb.ToString());
                        addToken(fl);
                    }
                    else
                    {
                        Tokens.IntLiteral il = new Tokens.IntLiteral();
                        il.Value = Convert.ToInt32(sb.ToString());
                        addToken(il);
                    }
                }

                else if (val == '"')
                {
                    StringBuilder sb = new StringBuilder();
                    val = (char)(byte)read();
                    while (val != '"')
                    {
                        if (val == '\\')
                        {
                            char escape = (char)(byte)read();
                            if (escape == 'n')
                                sb.Append('\n');
                            else if (escape == '"')
                                sb.Append('"');
                            else if (escape == '\\')
                                sb.Append('\\');
                            else if (escape == '\'')
                                sb.Append('\'');
                            else if (escape == '0')
                                sb.Append('\0');
                            else if (escape == 'r')
                                sb.Append('\r');
                        }
                        else
                            sb.Append(val);
                        val = (char)(byte)read();
                    }
                    val = (char)(byte)read();
                    Tokens.StringLiteral sl = new Tokens.StringLiteral();
                    sl.Value = sb.ToString();
                    addToken(sl);
                }
                else
                {
                    switch (val)
                    {
                        case '+':
                            if ((char)(byte)peek() == '+')
                            {
                                read();
                                addToken(new Tokens.Increment());
                            }
                            else if ((char)(byte)peek() == '=')
                            {
                                read();
                                addToken(new Tokens.PlusEquals());
                            }
                            else
                                addToken(new Tokens.Add());
                            break;
                        case '-':
                            if ((char)(byte)peek() == '-')
                            {
                                read();
                                addToken(new Tokens.Increment());
                            }
                            else if ((char)(byte)peek() == '>')
                            {
                                read();
                                addToken(new Tokens.ObjectReference());
                            }
                            else
                                addToken(new Tokens.Sub());
                            break;
                        case '|':
                            if (((char)peek()) == '|')
                            {
                                read();
                                addToken(new Tokens.BooleanOr());
                            }
                            else
                                addToken(new Tokens.Pipe());
                            break;
                        case '^':
                            if ((char)(byte)peek() == '=')
                            {
                                read();
                                addToken(new Tokens.PowEqual());
                            }
                            else
                                addToken(new Tokens.Pow()); ;
                            break;
                        case '%':
                            addToken(new Tokens.Percent());
                            break;
                        case '?':
                            addToken(new Tokens.Question());
                            break;
                        case '/':
                            addToken(new Tokens.Div());
                            break;
                        case '*':
                            addToken(new Tokens.Mul());
                            break;
                        case ',':
                            addToken(new Tokens.Comma());
                            break;
                        case '.':
                            addToken(new Tokens.Dot());
                            break;
                        case ':':
                            addToken(new Tokens.Colon());
                            break;
                        case '#':
                            addToken(new Tokens.Hash());
                            break;
                        case '@':
                            addToken(new Tokens.At());
                            break;
                        case '>':
                            if ((char)(byte)peek() == '>')
                            {
                                read();
                                addToken(new Tokens.RightBitshift());
                            }
                            else
                                addToken(new Tokens.GreaterThan());
                            break;
                        case '<':
                            if ((char)(byte)peek() == '<')
                            {
                                read();
                                addToken(new Tokens.LeftBitshift());
                            }
                            else
                                addToken(new Tokens.LessThan());
                            break;
                        case ';':
                            addToken(new Tokens.SemiColon());
                            break;
                        case '{':
                            addToken(new Tokens.openCurlyBracket());
                            break;
                        case '}':
                            addToken(new Tokens.closeCurlyBracket());
                            break;
                        case '~':
                            addToken(new Tokens.Tilda());
                            break;
                        case '(':
                            addToken(new Tokens.openParenthesis());
                            break;
                        case ')':
                            addToken(new Tokens.closeParenthesis());
                            break;
                        case '[':
                            addToken(new Tokens.openBracket());
                            break;
                        case ']':
                            addToken(new Tokens.closeBracket());
                            break;
                        case '&':
                            if (char.IsLetter(((char)peek())))
                            {
                                string accum = "";
                                while (char.IsLetterOrDigit(((char)peek())) || ((char)peek()) == '_')
                                {
                                    accum += ((char)read()).ToString();
                                }
                                Tokens.Dereference def = new Tokens.Dereference();
                                def.Name = accum;
                                addToken(def);
                            }
                            else if ((char)(byte)peek() == '&')
                            {
                                read();
                                addToken(new Tokens.BooleanAnd());
                            }
                            else
                                addToken(new Tokens.And());
                            break;
                        case '!':
                            if ((char)(byte)peek() == '=')
                            {
                                read();
                                addToken(new Tokens.NotEqual());
                            }
                            else
                            {
                                addToken(new Tokens.Exclamation());
                            }
                            break;
                        case '=':
                            if (!((char)(byte)peek() == '='))
                                addToken(new Tokens.Assign());
                            else
                            {
                                read();
                                addToken(new Tokens.Equal());
                            }
                            break;
                        case '_':
                            addToken(new Tokens.Underline());
                            break;
                        case '$':
                            addToken(new Tokens.Dollah());
                            break;

                        default:
                            throw new Exception("Lexer encountered an unexepected character '" + val + "'");

                    }
                    if (peek() != -1)
                        val = (char)(byte)read();
                }
            }
        }
    }
}
