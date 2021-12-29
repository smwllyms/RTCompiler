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
        virtual public Term Evaluate()
        {
            return this;
        }
    }
}
