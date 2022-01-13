using System;
using System.Collections.Generic;
using System.Text;

namespace RTCompiler.src.classes
{
    // Can also be thought of as a scope
    class Context
    {
        Dictionary<string, Term> variables;
        public Term returnValue;
        public RTCType returnType;
        public string name;
        public Context(Argument[] arguments, RTCType returnType, string name)
        {
            variables = new Dictionary<string, Term>();

            if (arguments != null)
            {
                foreach (Argument arg in arguments)
                    variables.Add(arg.name, arg.term);
            }

            this.returnType = returnType;
            this.name = name;
            this.returnValue = new Term(null, returnType);
        }
        public void TryGetValue(string varName, out Term term)
        {
            variables.TryGetValue(varName, out term);
        }
        public void Add(string varName, Term term)
        {
            variables.Add(varName, term);
        }
        public void Dump()
        {
            Console.WriteLine("--------- Context Dump ---------");
            Console.WriteLine("Context Name:        " + name);
            Console.WriteLine("Context Return Type: " + returnType);
            Console.WriteLine("Variables:");
            foreach (KeyValuePair<string, Term> var in variables)
            {
                Console.WriteLine("\tVariable '" + var.Key + "': ");
                Console.WriteLine("\t\tType:  " + var.Value.type);
                Console.WriteLine("\t\tValue: " + var.Value.result);
            }
            Console.WriteLine("------- End Context Dump -------");
        }
    }
    class Argument
    {
        public string name;
        public Term term;
        public Argument(string name, Term term)
        {
            this.name = name;
            this.term = term;
        }
    }
}
