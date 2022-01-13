using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RTCompiler.src.classes
{
    abstract class Command
    {
        public string type;
        public abstract void Execute(Context context);
        protected static Term ParseExpression(string s)
        {
            string sExp = Regex.Replace(s, @"[\s\t]+", "");
            return Expression.ParseAndCreate(sExp);
        }
        protected static Term EvaluateExpression(Term exp, Context context)
        {
            Term result = exp.Evaluate(context);

            // Check for undefined result
            if (result.type.Equals("undefined"))
                return null;
            return result;
        }
    }
    class CmdSetVar : Command
    {
        public string varName;
        public Term exp;
        public CmdSetVar(string varName, string varExpression)
        {
            type = "setvar";
            this.varName = varName;
            this.exp = ParseExpression(varExpression);
        }
        override public void Execute(Context context)
        {
            Term var = null;
            context.TryGetValue(varName, out var);
            if (var == null)
                throw new RTCUndefinedReferenceException("Could not find variable '"
                    + varName + "' while parsing.");
            Term term = EvaluateExpression(exp, context);
            if (term == null)
                throw new RTCRuntimeException("Error parsing term '" + varName + "'");
            // OK, but we need to assert types are compatible
            if (!Program.CompatibleTypes(var.type, term.type))
                throw new RTCRuntimeException("Evaluated type of variable '" + varName
                    + "' <" + term.type + "> does not match existing type <"
                    + var.type + ">.");
            // Set the variable
            Program.SetTermValue(var, term);
        }
    }
    class CmdInitVar : Command
    {
        public string varName;
        private RTCType userType;
        public Term exp;
        public CmdInitVar(string varName, string varExpression, RTCType userType)
        {
            type = "initvar";
            this.varName = varName;
            this.exp = ParseExpression(varExpression);
            this.userType = userType;
        }
        override public void Execute(Context context)
        {
            Term term = EvaluateExpression(exp, context);
            if (term == null)
                throw new RTCRuntimeException("Error parsing term '" + varName + "'");
            // Assert the user type and evaluated types match
            if (!Program.CompatibleTypes(userType, term.type))
                throw new RTCRuntimeException("Evaluated type of variable '" + varName
                    + "' <" + term.type + "> does not match declared type <"
                    + userType + ">.");

            Term t;
            context.TryGetValue(varName, out t);

            // Set the variable
            Term var = new Term(term.result, userType);
            Program.SetTermValue(var, term);
            // Add it to the context
            context.Add(varName, var);
        }
    }
    class CmdReturn : Command
    {
        public Term exp;
        public CmdReturn(string expression)
        {
            type = "return";
            if (expression != null)
                this.exp = ParseExpression(expression);
        }
        override public void Execute(Context context)
        {
            Term term = EvaluateExpression(exp, context);
            if (term == null)
                throw new RTCParsingException("Error parsing return expression.");

            // Ensure the types match
            if (!Program.CompatibleTypes(context.returnType, term.type))
                throw new RTCParsingException("Evaluated type of return value of function '"
                    + context.name + "' <" + term.type + "> does not match provided "
                    + "type of function <" + context.returnType + ">.");
            // Set the variable
            Program.SetTermValue(context.returnValue, term);
        }
    }
}
