using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools.Native;

namespace MemoryTools.Remote.Hook
{
    public class RemoteHook : RemoteCallType
    {
       
       
        private IntPtr address;
        private IntPtr hookedMethodAddress;
        private MemoryManager memoryManager;
        private MemoryProtectionConstants oldProtection;
        private byte[] originalBytes;
        private IntPtr threadID;

        public IntPtr MethoodAddress
        {
            get => this.hookedMethodAddress;
        }
        public IntPtr Address
        {
            get => this.address;
        }
        public IntPtr ThreadID
        {
            get => this.threadID;
        }
        public RemoteHook(IntPtr address,MemoryManager memoryManager, IntPtr hookedMethodAddress, MemoryProtectionConstants oldProtection, byte[] originalBytes,IntPtr threadID)
        {
           
            this.address = address;
            this.memoryManager = memoryManager;
            this.hookedMethodAddress = hookedMethodAddress;
            this.oldProtection = oldProtection;
            this.originalBytes = originalBytes;
            this.threadID = threadID;
        }
        public RemoteHook(IntPtr hookedMethodAddress, int size)
        {
           
            int offset = new Ptr<int>(hookedMethodAddress + 1)[0];
            IntPtr originalBytesAddress = hookedMethodAddress + offset;
            this.originalBytes = memoryManager.ReadBytes(originalBytesAddress, size);
            this.hookedMethodAddress = hookedMethodAddress;
            this.address = originalBytesAddress - 40;
        }

        public static bool isHooked(IntPtr address)
        {
            return new Ptr<byte>(address)[0] == 0xE9;
        }
        public void changeBranch(IntPtr address)
        {
            if (threadID != NativeFunctions.GetCurrentThreadId())
                memoryManager.WriteMemory(this.address + 8, NativeFunctions.GetCurrentThreadId());
            memoryManager.WriteBytes(this.hookedMethodAddress, Assembler.Assembler.jmp(address, this.hookedMethodAddress));
          
        }
     

       
       
        public MSG WaitReturnValue(double dwMilliseconds= 60000)
        {
            DateTime dateTime = DateTime.Now;
            bool timeout = false;
            MSG msg = new MSG();
            while (!NativeFunctions.PeekMessageA(out msg, IntPtr.Zero, RemoteHookerManager.RETURN_VALUE_MESSAGE, RemoteHookerManager.RETURN_VALUE_MESSAGE, 0x1) && !(timeout = DateTime.Now.Subtract(dateTime).TotalMilliseconds > dwMilliseconds)) ;
            if(timeout) throw new TimeoutException("Hooked Method Execution Timeout");
            return msg;
        }
    }
}
