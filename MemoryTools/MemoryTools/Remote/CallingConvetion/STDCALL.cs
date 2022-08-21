using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Remote
{
    public class STDCALL : CallingConvention
    {
        public STDCALL(StringBuilder builder,int parameters) : base(builder, parameters) { }

        public override CallingConventions CalliingConvention => CallingConventions.stdcall;
        public override void FormatArguments()
        {
            for (int i = 0; i < this.parameters; i++)
            {
                this.code.AppendLine("push 0x10000000");
            }
        }
        public override void FormatCall(IntPtr address)
        {
            code.AppendLine($"call 0x{address.ToString("X")}");
        }
        public override IntPtr[] GetArguments(IntPtr address)
        {
            IntPtr[] parametersAddress = new IntPtr[this.parameters];
            for (int i = 0; i < this.parameters; i++)
            {
                parametersAddress[this.parameters-1-i] = address + this.beginningOffset + 1 + i * 5;
            }
            return parametersAddress;
        }

        public override int Size()
        {
            return 5 * this.parameters + 5; // push = 5 * n  bytes : call address = 5 bytes;
        }
    }
}
