using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using RTCompiler.src.classes;

namespace RTCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string program = File.ReadAllText(
                "../../../resources/test.txt");
            program = Regex.Replace(program, @"[\s\t\r\n]*", "");
            string[] expressions = program.Split(';');

            Dictionary<string, Term> context = new Dictionary<string, Term>();
            int a = 9;

            foreach (string e in expressions)
            {
                if (e.Equals("")) continue;
                Console.WriteLine("[Solving for " + e + "]");
                Term exp = Expression.Parse(e);
                context.Add("a", new Term(a, "int"));
                exp.Evaluate(context);
                string result;
                switch (exp.type)
                {
                    case "int":
                        result = ((int)exp.result).ToString();
                        break;
                    case "float":
                        result = ((float)exp.result).ToString();
                        break;
                    case "double":
                        result = ((double)exp.result).ToString();
                        break;
                    default:
                        result = "undefined";
                        break;
                }
                Console.WriteLine("Result 1: " + result);
                context.Remove("a");
                context.Add("a", new Term(--a, "int"));
                exp.Evaluate(context);
                switch (exp.type)
                {
                    case "int":
                        result = ((int)exp.result).ToString();
                        break;
                    case "float":
                        result = ((float)exp.result).ToString();
                        break;
                    case "double":
                        result = ((double)exp.result).ToString();
                        break;
                    default:
                        result = "undefined";
                        break;
                }
                context.Remove("a");
                Console.WriteLine("Result 2: " + result);
            }
        }
    }
}
