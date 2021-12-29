using System;
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

            foreach (string e in expressions)
            {
                if (e.Equals("")) continue;
                Console.WriteLine("[Solving for " + e + "]");
                Term exp = Expression.Parse(e);
                exp.Evaluate();
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
                Console.WriteLine("Result: " + result);
            }

            //int i = (int)e.Evaluate();
            //Console.WriteLine("Result: " + i);
        }
    }
}
