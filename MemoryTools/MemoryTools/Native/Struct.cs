using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MemoryTools.Native
{

    [StructLayout(LayoutKind.Sequential)]
    public struct CLIENT_ID
    {
        public IntPtr UniqueProcess;
        public IntPtr UniqueThread;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct THREAD_BASIC_INFORMATION
    {
        public readonly uint ExitStatus;
        public readonly IntPtr TebBaseAdress;
        [MarshalAs(UnmanagedType.Struct)]
        public readonly CLIENT_ID ClientId;
        public readonly IntPtr AffinityMask;
        public readonly uint Priority;
        public readonly uint BasePriority;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

    }
    [StructLayout(LayoutKind.Sequential)]

    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public POINT pt;
        public int lPrivate;
    }

   
    public struct ENTRY_LIST
    {
        public IntPtr Flink;
        public IntPtr Blink;
    }
  
    public struct LDR_MODULE
    {
        public ENTRY_LIST InLoadOrderModuleList;
        public ENTRY_LIST InMemoryOrderModuleList;
        public ENTRY_LIST InInitializationOrderModuleList;
        public IntPtr BaseAddress;
        public IntPtr EntryPoint;
        public IntPtr SizeOfImage;
        public IntPtr FullDllName;
        public IntPtr BaseDllName;
    }



    /*0x00	0x00	
LARGE_INTEGER KernelTime;
0x08	0x08	
LARGE_INTEGER UserTime;
0x10	0x10	
LARGE_INTEGER CreateTime;
0x18	0x18	
ULONG WaitTime;
0x1C	0x20	
PVOID StartAddress;
0x20	0x28	
CLIENT_ID ClientId;
0x28	0x38	
LONG Priority;
0x2C	0x3C	
LONG BasePriority;
0x30	0x40	
ULONG ContextSwitches;
0x34	0x44	
ULONG ThreadState;
0x38	0x48	
ULONG WaitReason;
    */
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LARGE_INTEGER
    {
        [FieldOffset(0)] public long QuadPart;
        [FieldOffset(0)] public uint LowPart;
        [FieldOffset(4)] public int HighPart;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_THREAD_INFORMATION
    {
        public long KernelTime; //0
        public long UserTime; //8
        public long CreateTime; // 16
        public uint WaitTime; // 
        public IntPtr StartAddress;
        public IntPtr ClientIdUniqueProcess;
        public IntPtr ClientIdUniqueThread;
        public int Priority;
        public int BasePriority;
        public uint ContextSwitches;
        public uint ThreadState;
        public uint WaitReason;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FLOATING_SAVE_AREA
    {
        public uint ControlWord;
        public uint StatusWord;
        public uint TagWord;
        public uint ErrorOffset;
        public uint ErrorSelector;
        public uint DataOffset;
        public uint DataSelector;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] RegisterArea;
        public uint Cr0NpxState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Context
    {

        public ContextFlags ContextFlags;

        public uint Dr0;
        public uint Dr1;
        public uint Dr2;
        public uint Dr3;
        public uint Dr6;
        public uint Dr7;
        [MarshalAs(UnmanagedType.Struct)]
        public FLOATING_SAVE_AREA FloatingSave;

        public uint SegGs;
        public uint SegFs;
        public uint SegEs;
        public uint SegDs;

        public uint Edi;
        public uint Esi;
        public uint Ebx;
        public uint Edx;
        public uint Ecx;
        public uint Eax;

        public uint Ebp;
        public uint Eip;
        public uint SegCs;
        public uint EFlags;
        public uint Esp;
        public uint SegSs;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] ExtendedRegisters;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtectionConstants AllocationProtect;
        public int RegionSize;
        public MemoryState State;
        public MemoryProtectionConstants Protect;
        public MemoryType Type;


    }
}
