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
                "../../../resources/tests/variableparsing.txt");
            program = Regex.Replace(program, @"[\r\n]*", "");
            string[] lines = program.Split(';');

            // Context of variables
            Dictionary<string, Term> context = new Dictionary<string, Term>();

            foreach (string l in lines)
            {
                // Avoid empty lines
                if (l.Equals("")) continue;

                // Determine if we are initiating a local variable or modifying 
                // an existing one
                string[] pair = l.Split('=');

                // Begin parsing expression (pair[1])
                string sExp = Regex.Replace(pair[1], @"[\s\t]+", "");
                Console.WriteLine("[Parsing " + l + "]");
                Term exp = Expression.Parse(sExp);
                exp.Evaluate(context);

                // Check for undefined result
                if (exp.type.Equals("undefined"))
                    throw new RTCParsingException("Error parsing term '" + pair[0] + "'");
                // If initiating there will be two strings separated by whitespace
                string lhsRaw = pair[0].Replace('\t', ' ');
                int lhsRawLen = lhsRaw.Length;
                // Traverse string from left (j) and find first non-space character.
                // Then find space of end of string. If we didn't hit end of string go until
                // another non-space character or end of string. At this point, if j is
                // end of string we know there is only 1 string (a var name)
                int j = 0, k;
                while (lhsRaw[j] == ' ') j++;
                while (j < lhsRawLen && lhsRaw[j] != ' ') j++;
                k = j;
                while (j < lhsRawLen && lhsRaw[j] == ' ') j++;
                if (j < lhsRawLen)
                    lhsRaw = lhsRaw.Insert(k, "|");
                lhsRaw = lhsRaw.Replace(" ", "");

                string[] lhs = lhsRaw.Split('|');
                // Trim excess
                if (lhs.Length == 1)
                {
                    // Only 1 means we are updating a var and it MUST exist
                    Term var = null;
                    context.TryGetValue(lhs[0], out var);
                    if (var == null)
                        throw new RTCUndefinedReferenceException("Could not find variable '"
                            + lhs[0] + "' while parsing.");
                    // OK, but we need to assert types are compatible
                    if (!CompatibleTypes(var.type, exp.type))
                        throw new RTCParsingException("Evaluated type of variable '" + lhs[0]
                            + "' <" + exp.type + "> does not match existing type <" 
                            + var.type + ">.");
                    // Set the variable
                    SetTermValue(var, exp);
                }
                else
                {
                    // user type is lhs[0], var name is lhs[1]
                    string userType = lhs[0].Trim(), varName = lhs[1].Trim();
                    // We must assert that variable doesn't exist already
                    Term var = null;
                    context.TryGetValue(lhs[1], out var);
                    if (var != null)
                        throw new RTCParsingException("Variable '"+ varName
                            + "' already exists in context.");
                    // Assert the user type and evaluated types match
                    if (!CompatibleTypes(userType, exp.type))
                        throw new RTCParsingException("Evaluated type of variable '" + varName
                            + "' <" + exp.type + "> does not match declared type <"
                            + userType + ">.");
                    // Set the variable
                    var = new Term(exp.result, userType);
                    SetTermValue(var, exp);
                    // Add it to the context
                    context.Add(varName, var);
                }
            }
        }
        public static void SetTermValue(Term set, Term from)
        {
            if (set.type == from.type)
            {
                set.result = from.result;
                return;
            }

            // Otherwise me must implicitly case
            switch (set.type)
            {
                case "string":
                    set.result = from.result.ToString();
                    return;
                case "float":
                    set.result = float.Parse(from.result.ToString());
                    return;
                case "double":
                    set.result = double.Parse(from.result.ToString());
                    return;
                default:
                    // Otherwise unsafe?
                    set.result = from.result;
                    return;
            }

        }
        public static bool CompatibleTypes(string existing, string desired)
        {
            int eType = GetTypeIntVal(existing), dType = GetTypeIntVal(desired);
            if (eType >= dType && dType > -1)
                return true;
            return false;
        }
        public static int GetTypeIntVal(string type)
        {
            switch (type)
            {
                case "string":
                    return 10;
                case "int":
                    return 1;
                case "float":
                    return 2;
                case "double":
                    return 3;
                default:
                    return -1;

            }
        }
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello World!");
        //    string program = File.ReadAllText(
        //        "../../../resources/tests/expressions.txt");
        //    program = Regex.Replace(program, @"[\s\t\r\n]*", "");
        //    string[] expressions = program.Split(';');

        //    Dictionary<string, Term> context = new Dictionary<string, Term>();
        //    int a = 9;

        //    foreach (string e in expressions)
        //    {
        //        if (e.Equals("")) continue;
        //        Console.WriteLine("[Solving for " + e + "]");
        //        Term exp = Expression.Parse(e);
        //        context.Add("a", new Term(a, "int"));
        //        exp.Evaluate(context);
        //        string result;
        //        switch (exp.type)
        //        {
        //            case "int":
        //                result = ((int)exp.result).ToString();
        //                break;
        //            case "float":
        //                result = ((float)exp.result).ToString();
        //                break;
        //            case "double":
        //                result = ((double)exp.result).ToString();
        //                break;
        //            default:
        //                result = "undefined";
        //                break;
        //        }
        //        Console.WriteLine("Result 1: " + result);
        //        context.Remove("a");
        //        context.Add("a", new Term(--a, "int"));
        //        exp.Evaluate(context);
        //        switch (exp.type)
        //        {
        //            case "int":
        //                result = ((int)exp.result).ToString();
        //                break;
        //            case "float":
        //                result = ((float)exp.result).ToString();
        //                break;
        //            case "double":
        //                result = ((double)exp.result).ToString();
        //                break;
        //            default:
        //                result = "undefined";
        //                break;
        //        }
        //        context.Remove("a");
        //        Console.WriteLine("Result 2: " + result);
        //    }
        //}
    }
}
