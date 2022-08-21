using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools
{



    [System.AttributeUsage(System.AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false)]
    public class MemoryAttribute : Attribute
    {
        public int ArraySize;
        public bool Ignore;
        public bool Argument;
        public string ArraySizeReference;

        public MemoryAttribute(int ArraySize = 0, bool Ignore = false, bool Argument = false, string ArraySizeReference = "")
        {
            this.ArraySize = ArraySize;
            this.Ignore = Ignore;
            this.Argument = Argument;
            this.ArraySizeReference = ArraySizeReference;
        }
    }
}
