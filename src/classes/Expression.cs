using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RTCompiler.src.classes
{
    class Expression : Term
    {
        public static char[] validOperations =
        {
            '+', '-', '*', '/'
        };
        public static int TOP_PRIORITY = 1;
        public Term left;
        public Term right;
        public char operation;
        public Expression(Term left, Term right, char operation)
        {
            result = 0;
            type = RTCType.rtc_expression;
            this.left = left;
            this.right = right;
            this.operation = operation;
        }
        override public Term Evaluate(Context context)
        {
            Term lEval = left.Evaluate(context);
            Term rEval = right.Evaluate(context);
            object l = lEval.result;
            object r = rEval.result;
            // Type will bubble
            RTCType lType = lEval.type;
            RTCType rType = rEval.type;

            bool typesEqual = (lType.Equals(rType));
            RTCType determinedType = DetermineType(lType, rType, typesEqual);
            this.type = determinedType;
            switch (determinedType)
            {
                case RTCType.rtc_int:
                    int lInt = int.Parse(l.ToString());
                    int rInt = int.Parse(r.ToString());
                    switch (operation)
                    {
                        case '+':
                            result = lInt + rInt;
                            break;
                        case '-':
                            result = lInt - rInt;
                            break;
                        case '*':
                            result = lInt * rInt;
                            break;
                        case '/':
                            result = lInt / rInt;
                            break;
                        default:
                            result = null;
                            break;
                    }
                    break;
                case RTCType.rtc_float:
                    float lFloat = float.Parse(l.ToString());
                    float rFloat = float.Parse(r.ToString());
                    switch (operation)
                    {
                        case '+':
                            result = lFloat + rFloat;
                            break;
                        case '-':
                            result = lFloat - rFloat;
                            break;
                        case '*':
                            result = lFloat * rFloat;
                            break;
                        case '/':
                            result = lFloat / rFloat;
                            break;
                        default:
                            result = null;
                            break;
                    }
                    break;
                case RTCType.rtc_double:
                    double lDouble = double.Parse(l.ToString());
                    double rDouble = double.Parse(r.ToString());
                    switch (operation)
                    {
                        case '+':
                            result = lDouble + rDouble;
                            break;
                        case '-':
                            result = lDouble - rDouble;
                            break;
                        case '*':
                            result = lDouble * rDouble;
                            break;
                        case '/':
                            result = lDouble / rDouble;
                            break;
                        default:
                            result = null;
                            break;
                    }
                    break;
                default:
                    result = null;
                    break;
            }
            return this;
        }
        public static Term ParseAndCreate(string expression)
        {
            return TryCreate(expression);
        }
        public static string Parse(string expression)
        {
            // Ensure we don't have any screwy chars
            expression = Regex.Replace(expression, @"[\s\t\r\n]*", "");
            // Ensure expression follows protocol
            int expLen = expression.Length;
            expression = expression.Insert(0, "(").Insert(expLen + 1, ")");
            // Parse operations by priority
            for (int priority = TOP_PRIORITY; priority > -1; priority--)
            {
                // Create a RegEx with all operations to identify all operations
                // Form a new expression in valid format
                string clone = expression.Replace('(', '[').Replace(')', ']');
                // With the given priority
                StringBuilder validOps = new StringBuilder();
                StringBuilder invalidOps = new StringBuilder();
                foreach (char op in validOperations)
                {
                    if (GetOperationPriority(op) == priority)
                        validOps.Append('\\' + op.ToString());
                    else
                        invalidOps.Append('\\' + op.ToString());
                }
                string regex = validOps.ToString();
                // Replace all valid ops with '|'
                clone = Regex.Replace(clone, "[" + regex + "]+", "|");
                // If no valid ops were found
                if (!clone.Contains("|"))
                    continue;
                regex = invalidOps.ToString();
                // Replace all invalid ops with 'u'
                clone = Regex.Replace(clone, "[" + regex + "]+", "u");

                //Console.WriteLine("Start");

                while (true)
                {
                    // Console.WriteLine(clone);
                    // Exit if no brackets
                    if (!clone.Contains('['))
                        break;
                    int end = 0, len = clone.Length, firstPriorityOp = -1;
                    // Find first end bracket
                    while (end < len && clone[end] != ']')
                    {
                        end++;
                    }
                    // Find corresponding start bracket
                    int start = end;
                    while (start > -1 && clone[start] != '[')
                    {
                        start--;
                    }
                    while (true)
                    {
                        int numOps = 0, numPriorityOps = 0, pos = start;
                        // Find all operations
                        while (pos < end)
                        {
                            if (isOperation(clone[pos]))
                            {
                                numOps++;
                                if (clone[pos] == '|')
                                {
                                    numPriorityOps++;
                                    if (numPriorityOps == 1)
                                        firstPriorityOp = pos;
                                }
                            }
                            pos++;
                        }
                        if (numPriorityOps == 0)
                        {
                            //Console.WriteLine("3a: " + clone);
                            //if (clone.IndexOf(']') == len - 1 && clone.LastIndexOf('[') == 0)
                            //{
                            //    clone = clone.Remove(len - 1, 1).Remove(0, 1);
                            //    expression = expression.Remove(len - 1, 1).Remove(0, 1);
                            //}
                            if (clone[start] == '[')
                            {
                                clone = clone.Remove(start, 1).Insert(start, "(");
                                clone = clone.Remove(end, 1).Insert(end, ")");
                            }
                            //Console.WriteLine("3b: " + clone);
                            break;
                        }
                        else if (numPriorityOps >= 1)
                        {
                            pos = firstPriorityOp - 1;
                            int level = 0, placeStart = start, placeEnd = end;
                            while (pos > start)
                            {
                                char c = clone[pos];
                                if (isOperation(c) && level == 0)
                                    break; // need +1
                                else if (c == ')')
                                    level++;
                                else if (c == '(')
                                    level--;
                                pos--;
                            }
                            placeStart = pos + 1;
                            pos = firstPriorityOp + 1;
                            level = 0;
                            while (pos < end)
                            {
                                char c = clone[pos];
                                if (isOperation(c) && level == 0)
                                    break; // need +1
                                else if (c == '(')
                                    level++;
                                else if (c == ')')
                                    level--;
                                pos++;
                            }
                            placeEnd = pos;
                            // Add parentheses
                            if (numOps == 1)
                            {
                                //Console.WriteLine("1a: " + clone);
                                clone = clone.Remove(firstPriorityOp, 1).Insert(firstPriorityOp, "u");
                                clone = clone.Remove(placeEnd, 1).Insert(placeEnd, ")");
                                clone = clone.Remove(placeStart - 1, 1).Insert(placeStart - 1, "(");
                                //Console.WriteLine("1b: " + clone);
                                continue;
                            }
                            //Console.WriteLine("2a: " + clone);
                            clone = clone.Remove(firstPriorityOp, 1).Insert(firstPriorityOp, "u");
                            clone = clone.Insert(placeEnd, ")");
                            clone = clone.Insert(placeStart, "(");
                            expression = expression.Insert(placeEnd, ")");
                            expression = expression.Insert(placeStart, "(");
                            //Console.WriteLine("2b: " + clone);
                            len += 2;
                            end += 2;
                        }
                    }
                }
            }
            // Remove the extra parentheses we added originally
            expression = expression.Remove(0, 1).Remove(expression.Length - 2, 1);
            return expression;
        }
        public static Term TryCreate(string expression)
        {
            if (expression.Contains("("))
            {
                int level = 0, pos = 0, len = expression.Length;
                do
                {
                    if (expression[pos] == ')')
                        level--;
                    else if (expression[pos] == '(')
                        level++;
                    pos++;
                } while (level != 0 && pos < len);

                if (pos < len)
                {
                    foreach (char operation in validOperations)
                    {
                        if (expression[pos] == operation)
                        {
                            pos++;
                            return new Expression(
                                TryCreate(expression.Substring(0, pos - 1)),
                                TryCreate(expression.Substring(pos, len - pos)), operation);
                        }
                    }
                }
                else
                {
                    expression = expression.Remove(0, 1).Remove(len - 2, 1);
                    return TryCreate(expression);
                }
            }
            foreach (char operation in validOperations)
            {
                if (expression.Contains(operation))
                {
                    string[] landr = expression.Split(operation);
                    return new Expression(
                        TryCreate(landr[0]),
                        TryCreate(landr[1]), operation);
                }
            }
            // Guess the type
            RTCType type = RTCType.rtc_var;
            object value = expression;
            bool isNumber = char.IsDigit(expression[0]);
            bool isString = (expression[0] == '\"');
            bool isChar = (expression[0] == '\'');
            bool hasDecimal = expression.Contains('.');
            bool isFloat = expression.Contains('f');
            if (isNumber)
            {
                if (isFloat)
                {
                    type = RTCType.rtc_float;
                    value = float.Parse(expression.Trim('f'));
                }
                else if (hasDecimal)
                {
                    type = RTCType.rtc_double;
                    // just incase there is a d
                    value = double.Parse(expression.Trim('d'));
                }
                else
                {
                    type = RTCType.rtc_int;
                    value = int.Parse(expression);
                }
            }
            else if (isString)
            {
                type = RTCType.rtc_string;
                value = expression.Trim('\"');
            }
            else if (isChar)
            {
                type = RTCType.rtc_char;
                value = expression.Trim('\'');
            }
            return new Term(value, type);
        }

        private static RTCType DetermineType(RTCType l, RTCType r, bool equal)
        {
            if (equal)
            {
                foreach (RTCType type in Enum.GetValues(typeof(RTCType)))
                {
                    if (type == l) return type;
                }
                return RTCType.rtc_var;
            }
            RTCType[] types = { l, r };
            // Implementation choice -> treat op with char as string
            bool hasString = hasType(types, RTCType.rtc_string) || hasType(types, RTCType.rtc_char);
            bool hasDouble = hasType(types, RTCType.rtc_double);
            bool hasInt = hasType(types, RTCType.rtc_int);
            bool hasFloat = hasType(types, RTCType.rtc_float);
            if (hasString) return RTCType.rtc_string;
            if (hasDouble) return RTCType.rtc_double;
            if (hasFloat) return RTCType.rtc_float;
            if (hasInt) return RTCType.rtc_int;
            return RTCType.rtc_var;
        }
        private static bool hasType(RTCType[] types, RTCType type)
        {
            return (types[0] == type || types[1] == type);
        }
        private static int GetOperationPriority(char op)
        {
            if (op == '+' || op == '-') return 0;
            return 1;
        }
        private static bool isOperation(char c)
        {
            return c == '|' || c == 'u';
        }
    }
}
