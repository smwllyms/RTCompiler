# RTCompiler
A naive real-time compiler for C# that will *eventually* read C#-esque code as a string, parse it, and run the code.

## 1/3/2022
Please see Program.cs and test.txt for example usage. 

Expressions are parsed in a "binary" manner where 1 + 2 + 3 is evaluated as (1 + 2) + 3 [ always with a LEFT and RIGHT operand ]. Additionally, 1 + 2 * 3 is evaluated as 1 + (2 * 3) [basic pemdas]. 1 + (2 + 3) will evaluate to itself (it will still be checked but this is proper format). Another example, 1 + 2 + (3 + 4) will evaluate to (1 + 2) + (3 + 4) [ valid user defined parentheses are accepted ].
