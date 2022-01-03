using System;
using System.Collections.Generic;
using System.Text;

namespace RTCompiler.src.classes
{
    class Term
    {
        public object result;
        public string type;
        public Term(object x, string type)
        {
            this.type = type;
            result = x;
        }
        public Term()
        {
            result = 0;
        }
        virtual public Term Evaluate(Dictionary<string, Term> context)
        {
            if (type.Equals("var"))
            {
                Term var = null;
                context.TryGetValue((string)result, out var);
                return var;
            }
            return this;
        }
    }
}
