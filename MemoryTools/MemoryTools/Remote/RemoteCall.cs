using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools;
using MemoryTools.Assembler;
using MemoryTools.Remote.Hook;


namespace MemoryTools.Remote
{
    public class RemoteCall
    {
        public MemoryManager memoryManager;

        public RemoteCall(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;

        }

        public RemoteFunction Create(IntPtr address, int parameters, CallingConventions conventions, RemoteThread thread)
        {
            if (conventions == CallingConventions.thiscall) parameters += 1;
            StringBuilder code = new StringBuilder();
            CallingConvention callingConvention = GetConvention(conventions, code, parameters);
            int size = callingConvention.Size() + 5 + 5;
            IntPtr baseAddress = memoryManager.AllocMemory(IntPtr.Zero, size);
            callingConvention.FormatArguments();
            callingConvention.FormatCall(address);
            code.AppendLine($"mov [{baseAddress + size}], eax");
            code.AppendLine($"jmp {thread.Address}");
            memoryManager.WriteBytes(baseAddress, Assembler.Assembler.assemble(code, baseAddress));
            return new RemoteFunction(baseAddress, size, parameters, thread, this.memoryManager, callingConvention);
        }

        public RemoteFunction Create(IntPtr address, int parameters, CallingConventions conventions, RemoteHook hook)
        {
            if (conventions == CallingConventions.thiscall) parameters += 1;
            StringBuilder code = new StringBuilder();
            CallingConvention callingConvention = GetConvention(conventions, code, parameters);
            callingConvention.SetBeginningOffset(1);
            int size = callingConvention.Size() + 6;
            IntPtr baseAddress = memoryManager.AllocMemory(IntPtr.Zero, size);
            code.AppendLine($"pushad");
            callingConvention.FormatArguments();
            callingConvention.FormatCall(address);
            code.AppendLine($"jmp {hook.Address}");
            memoryManager.WriteBytes(baseAddress, Assembler.Assembler.assemble(code, baseAddress));
            return new RemoteFunction(baseAddress, size, parameters, hook, this.memoryManager, callingConvention);
        }

        private CallingConvention GetConvention(CallingConventions conventions, StringBuilder builder, int parameters)
        {
            switch (conventions)
            {
                case CallingConventions.stdcall:
                    return new STDCALL(builder, parameters);
                case CallingConventions.cdecl:
                    return new CDECL(builder, parameters);
                case CallingConventions.thiscall:
                    return new THISCALL(builder, parameters);
                default:
                    return new CDECL(builder, parameters);
            }
        }
    }
}
