using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Remote
{
    public class THISCALL : CallingConvention
    {
        public THISCALL(StringBuilder builder, int parameters) : base(builder, parameters) { }

        public override CallingConventions CalliingConvention => CallingConventions.thiscall;
        public override void FormatArguments()
        {
           

            for (int i = 1; i < this.parameters; i++)
            {
                this.code.AppendLine("push 0x10000000");
            }
             this.code.AppendLine("mov ecx, 0x10000000");
        }

        public override void FormatCall(IntPtr address)
        {
            code.AppendLine($"call 0x{address.ToString("X")}");
           // code.AppendLine($"add esp, {(parameters-1) * 4}");
        }

        public override IntPtr[] GetArguments(IntPtr address)
        {

            IntPtr[] parametersAddress = new IntPtr[this.parameters];

            for (int i = 0; i < this.parameters; i++)
            {
                parametersAddress[this.parameters - 1 - i] = address + this.beginningOffset + 1 + i * 5;
                // Console.WriteLine($"{parametersAddress[i].ToString("X")}");

            }
        

            return parametersAddress;
        }

        public override int Size()
        {
            return 5 * this.parameters + 5; // push = 5 * n  bytes : call address = 5 bytes  : add esp,argscount * 4 = 3 bytes
        }

    }
}
