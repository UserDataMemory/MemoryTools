using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools.Native;
using MemoryTools;

namespace MemoryTools.Remote.Hook
{
    public class RemoteHookerManager
    {
        private MemoryManager memoryManager;
      
       
        public static uint RETURN_VALUE_MESSAGE = 0x04FF;



        public RemoteHookerManager(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;
        }

        private RemoteHook CreateCode(IntPtr address, int size)
        {
            IntPtr PostMessageA_addr = memoryManager.getModuleFunction("user32.dll", "PostThreadMessageA");
            IntPtr memcpy_addr = memoryManager.getModuleFunction("ntdll.dll", "memcpy");
            IntPtr HookAddress = memoryManager.AllocMemory(IntPtr.Zero, 45 + size);
            byte[] OriginalBytes = memoryManager.ReadBytes(address, size);
            IntPtr ThreadID = NativeFunctions.GetCurrentThreadId();
            StringBuilder code = new StringBuilder();

            code.AppendLine($"push eax");
            code.AppendLine($"push esp");
            code.AppendLine($"push {RETURN_VALUE_MESSAGE}"); 
            code.AppendLine($"push {ThreadID}");
            code.AppendLine($"call {PostMessageA_addr}");
            code.AppendLine($"push 5");
            code.AppendLine($"push {HookAddress+40}");
            code.AppendLine($"push {address}");
            code.AppendLine($"call {memcpy_addr}");
            code.AppendLine($"add esp, 0xC");
            code.AppendLine($"popad");
            code.AppendLine("jmp originalBytes");
            code.AppendLine($"db {Utils.Join(Assembler.Assembler.jmp(HookAddress+45, address))}");
            code.AppendLine("originalBytes:");
            code.AppendLine($"db {Utils.Join(OriginalBytes)}");
            code.AppendLine($"jmp {address + size}");
            memoryManager.WriteBytes(HookAddress, Assembler.Assembler.assemble(code, HookAddress));
            MemoryProtectionConstants old;
            memoryManager.VirtualProtect(address, size, MemoryProtectionConstants.PAGE_EXECUTE_READWRITE, out old);
            return new RemoteHook(HookAddress,memoryManager, address,old, OriginalBytes, ThreadID);
        }

        public RemoteHook Create(IntPtr address, int size)
        {
            return CreateCode(address, size);
        }
    }
}
