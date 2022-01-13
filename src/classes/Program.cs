using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RTCompiler.src.classes
{
    class Program
    {
        private Context baseContext;
        private List<Command> commands;
        public string program;
        public RTCType resultType;
        public object result;

        public Program(string program)
        {
            this.program = Regex.Replace(program, @"[\r\n]*", "");

            // Context of variables
            this.baseContext = new Context(null, RTCType.rtc_int, "afunc");

        }
        public Program(string program, string contextName, string returnType)
        {
            this.program = Regex.Replace(program, @"[\r\n]*", "");

            // Context of variables
            this.baseContext = new Context(null, Term.GetTypeFromString(returnType), contextName);
        }

        public void Compile()
        {
            // Create our Command Queue
            commands = new List<Command>();
            string[] lines = program.Split(';');
            foreach (string l in lines)
            {
                // Avoid empty lines
                if (l.Equals("")) continue;

                // Trim so we can identify first word
                string line = l.Trim();
                // See if we are evaluating expressions
                int idxOfEq = line.IndexOf('=');
                int offset;
                if (streq("return", line, out offset))
                {
                    string[] pair = { line.Substring(0, offset), line.Substring(offset).Trim() };

                    // Check if we are just calling return
                    if (pair[1].Length == 0)
                    {
                        // Ensure this is void, otherwise incorrent syntax
                        if (RTCType.rtc_void != baseContext.returnType)
                            throw new RTCParsingException("Invalid return call to non-void function '"
                                + baseContext.name + "'");
                        // Just break; We don't have to add a command
                        break;
                    }

                    // Ensure this is NOT a void, otherwise incorrent syntax
                    if (RTCType.rtc_void == baseContext.returnType)
                        throw new RTCParsingException("Invalid return call to void function '"
                            + baseContext.name + "'");

                    // Evalutation
                    commands.Add(new CmdReturn(pair[1]));
                    // We are done
                    break;
                }
                else if (streq("if", line, out offset))
                {
                    // TODO If statement
                }
                else if (streq("while", line, out offset))
                {
                    // TODO While loop
                }
                else if (streq("for", line, out offset))
                {
                    // TODO For loop
                }
                else if (idxOfEq != -1)
                {
                    // Determine if we are initiating a local variable or modifying 
                    // an existing one
                    string[] pair = { line.Substring(0, idxOfEq), line.Substring(idxOfEq + 1) };

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
                        // We are setting a variable
                        commands.Add(new CmdSetVar(lhs[0].Trim(), pair[1]));
                    }
                    else
                    {
                        // We are initializing a variable
                        // user type is lhs[0], var name is lhs[1]
                        string userType = lhs[0].Trim(), varName = lhs[1].Trim();
                        // We must assert that variable doesn't exist already
                        Term var = null;
                        baseContext.TryGetValue(lhs[1], out var);
                        if (var != null)
                            throw new RTCParsingException("Variable '" + varName
                                + "' already exists in context.");
                        // ---- For Evaluation
                        commands.Add(new CmdInitVar(varName, pair[1], Term.GetTypeFromString(userType)));
                    }
                }
                else
                {
                    // TODO We are calling a function
                }
            }

        }
        public void Execute(Argument[] args)
        {
            // Add argument
            foreach (Argument arg in args) {
                baseContext.Add(arg);
            }

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Execute(baseContext);
            }
            // Finished program, set result
            resultType = baseContext.returnType;
            result = baseContext.returnValue.result;
        }
        public void DumpContext()
        {
            baseContext.Dump();
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
                case RTCType.rtc_string:
                    set.result = from.result.ToString();
                    return;
                case RTCType.rtc_float:
                    set.result = float.Parse(from.result.ToString());
                    return;
                case RTCType.rtc_double:
                    set.result = double.Parse(from.result.ToString());
                    return;
                default:
                    // Otherwise unsafe?
                    set.result = from.result;
                    return;
            }

        }
        public static bool CompatibleTypes(RTCType existing, RTCType desired)
        {
            int eType = GetTypeIntVal(existing), dType = GetTypeIntVal(desired);
            if (eType >= dType && dType > -1)
                return true;
            return false;
        }
        public static int GetTypeIntVal(RTCType type)
        {
            switch (type)
            {
                case RTCType.rtc_string:
                    return 10;
                case RTCType.rtc_int:
                    return 1;
                case RTCType.rtc_float:
                    return 2;
                case RTCType.rtc_double:
                    return 3;
                default:
                    return -1;

            }
        }
        public static bool streq(string l, string r, out int offset)
        {
            offset = 0;
            int len = Math.Min(l.Length, r.Length);
            while (offset < len)
            {
                if (l[offset] != r[offset])
                    return false;
                offset++;
            }
            return true;
        }
        private static Term EvaluateTermString(string s, Context context)
        {
            string sExp = Regex.Replace(s, @"[\s\t]+", "");
            Term exp = Expression.TryCreate(sExp);
            exp.Evaluate(context);

            // Check for undefined result
            if (exp.type.Equals("undefined"))
                return null;
            return exp;
        }
    }
}
