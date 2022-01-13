using System;
using System.Collections.Generic;
using System.Text;

namespace RTCompiler.src.classes
{
    public enum RTCType
    {
        rtc_void = 0, rtc_var = 1, rtc_int = 2, rtc_float = 3, rtc_double = 4, 
        rtc_string = 5, rtc_char = 6, rtc_expression = 7
    };
    class Term
    {
        public object result;
        public RTCType type;
        public Term(object x, RTCType type)
        {
            this.type = type;
            result = x;
        }
        public Term(object x, string type)
        {
            this.type = GetTypeFromString(type);
            result = x;
        }
        public Term()
        {
            type = RTCType.rtc_int;
            result = 0;
        }
        virtual public Term Evaluate(Context context)
        {
            if (type.Equals(RTCType.rtc_var))
            {
                Term var = null;
                context.TryGetValue((string)result, out var);
                if (var == null)
                    throw new RTCUndefinedReferenceException("variable '"
                        + result + "' does not exist in the current context.");
                return var;
            }
            return this;
        }
        public static RTCType GetTypeFromString(string type)
        {
            switch (type)
            {
                case "void":
                    return RTCType.rtc_void;
                case "int":
                    return RTCType.rtc_int;
                case "float":
                    return RTCType.rtc_float;
                case "double":
                    return RTCType.rtc_double;
                case "string":
                    return RTCType.rtc_string;
                case "char":
                    return RTCType.rtc_char;
                default:
                    return RTCType.rtc_var;
            }
        }
    }
}
