using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RTCompiler.src.classes
{
    class ExampleUsage
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string program = File.ReadAllText(
                "../../../resources/tests/variableparsing.txt");

            Program p = new Program(program);
            // Compile p
            p.Compile();
            // Execute with an arg
            p.Execute(new Argument[] { new Argument("d", new Term(5, "int")) });

            // Return the value from context
            if (RTCType.rtc_void != p.resultType)
            {
                p.DumpContext();
                Console.WriteLine("Program returned: "
                    + p.result.ToString());
            }
        }
    }
}
