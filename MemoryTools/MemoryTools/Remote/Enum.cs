using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Remote 
{
    public enum CallingConventions {
        stdcall,
        thiscall,
        fastcall,
        cdecl
    }
    public enum ThreadWait
    {
        OK,
        TIMEOUT
    }

}
