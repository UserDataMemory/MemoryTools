using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MemoryTools.Native;
using MemoryTools.Remote;
using System.Runtime.InteropServices;

namespace MemoryTools.Remote
{
    public class RemoteThread : RemoteCallType
    {
        private IntPtr threadID;
        private IntPtr handle;
        private IntPtr address;
        private MemoryManager memoryManager;
        public RemoteThread(IntPtr id, IntPtr handle, IntPtr address, MemoryManager memoryManager)
        {
            this.threadID = id;
            this.handle = handle;
            this.address = address;
            this.memoryManager = memoryManager;
            Resume();
            WaitThread(60000);
        }
        public void changeBranch(IntPtr address)
        {
            IntPtr jmpAddress = this.Address + 11;
         
            memoryManager.WriteBytes(jmpAddress, Assembler.Assembler.assemble($"jmp {address}", jmpAddress));
        }

        public bool isRunning
        {
            get  {
                NtStatus result;
                return (memoryManager.QueryInformationThread<int>(this.handle, ThreadInfoClass.ThreadSuspendCount, out result) == 0 && result == NtStatus.Success);
            }
        }
        public IntPtr Address
        {
            get => this.address;
        }
        public IntPtr Handle
        {
            get =>  this.handle; 
        }
        public IntPtr Pid
        {
            get => this.threadID;
        }
        public void Suspend()
        {
            NativeFunctions.SuspendThread(this.handle);
        }
        public void Resume()
        {
            NativeFunctions.ResumeThread(this.handle);
        }
        public ThreadWait WaitThread(double dwMilliseconds = 60000)
        {
            DateTime dateTime = DateTime.Now;
            bool timeout = false;
            while (isRunning && !(timeout = DateTime.Now.Subtract(dateTime).TotalMilliseconds > dwMilliseconds)) ;
            return timeout ? ThreadWait.TIMEOUT : ThreadWait.OK;
        }
    }
}
