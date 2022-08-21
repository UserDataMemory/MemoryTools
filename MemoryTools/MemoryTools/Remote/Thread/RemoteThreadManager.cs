using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools.Native;
using MemoryTools.Assembler;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MemoryTools.Remote
{
    public class RemoteThreadManager
    {
        private MemoryManager memoryManager;
        private IntPtr ThreadPauseAddress = IntPtr.Zero;
        public RemoteThreadManager(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;  
        }

        private void CreateThreadCode()
        {
            IntPtr GetCurrentThread_addr = memoryManager.getModuleFunction("kernel32.dll", "GetCurrentThread");
            IntPtr SuspendThread_addr = memoryManager.getModuleFunction("kernel32.dll", "SuspendThread");
            this.ThreadPauseAddress = memoryManager.AllocMemory(IntPtr.Zero, 16);
            StringBuilder code = new StringBuilder();
            code.AppendLine($"call {GetCurrentThread_addr}");
            code.AppendLine($"push eax");
            code.AppendLine($"call {SuspendThread_addr}");
            code.AppendLine($"jmp {SuspendThread_addr}");
            memoryManager.WriteBytes(ThreadPauseAddress, Assembler.Assembler.assemble(code, ThreadPauseAddress));
        }


        public RemoteThread Create()
        {
            if (ThreadPauseAddress == IntPtr.Zero) CreateThreadCode();
            int ThreadID;
            IntPtr handle = memoryManager.CreateRemoteThread(IntPtr.Zero, 0, ThreadPauseAddress, IntPtr.Zero, ThreadCreationFlags.CREATE_SUSPENDED, IntPtr.Zero, out ThreadID);
            return new RemoteThread((IntPtr)ThreadID, handle, ThreadPauseAddress, memoryManager);
        }


    }
}
