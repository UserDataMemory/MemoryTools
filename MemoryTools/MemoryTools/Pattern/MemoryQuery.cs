using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools;
using MemoryTools.Native;
using System.Runtime.InteropServices;

namespace MemoryTools.Pattern
{
    public class MemoryQuery
    {
        MemoryManager memoryManager;
        public MemoryProtectionConstants allocationType = MemoryProtectionConstants.PAGE_READWRITE | MemoryProtectionConstants.PAGE_EXECUTE;
        public MemoryType memoryType = MemoryType.MEM_PRIVATE | MemoryType.MEM_IMAGE | MemoryType.MEM_MAPPED;
        public MemoryQuery(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;
        }

        public IntPtr StartAddress = IntPtr.Zero;
        public IntPtr EndAddress = new IntPtr(int.MaxValue);

        public Dictionary<IntPtr, byte[]> GetMemory()
        {
            IntPtr CurrentAddress = StartAddress;

            MEMORY_BASIC_INFORMATION memory_basic_information = new MEMORY_BASIC_INFORMATION();
            int structSize = Marshal.SizeOf<MEMORY_BASIC_INFORMATION>();

        

            Dictionary<IntPtr, byte[]> memInfo = new Dictionary<IntPtr, byte[]>();

            while (NativeFunctions.VirtualQueryEx(memoryManager.Handle, (CurrentAddress = memory_basic_information.BaseAddress + memory_basic_information.RegionSize), out memory_basic_information, structSize) != 0 && CurrentAddress.ToInt32() < EndAddress.ToInt32())
            {
               if (allocationType.HasFlag(memory_basic_information.Protect) && !MemoryProtectionConstants.PAGE_GUARD.HasFlag(memory_basic_information.Protect) && memory_basic_information.State == MemoryState.MEM_COMMIT && memoryType.HasFlag(memory_basic_information.Type))
                {
                   //Console.WriteLine($"[Memory Chunk]\nBaseAddress: {memory_basic_information.BaseAddress.ToString("X")}\nAllocationBase: {memory_basic_information.BaseAddress.ToString("X")}\nAllocationProtection: {memory_basic_information.AllocationProtect}\nRegionSize: {memory_basic_information.RegionSize.ToString("X")}\nState: {memory_basic_information.State}\nProtect: {memory_basic_information.Protect}\nType: {memory_basic_information.Type}\n\n");

                    byte[] bytes = memoryManager.ReadBytes(memory_basic_information.BaseAddress, memory_basic_information.RegionSize);
                    memInfo.Add(CurrentAddress, bytes);
                }


            }
            return memInfo;
        }
    }
    }
