using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools.Native;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using MemoryTools.Remote;
using System.Diagnostics;
using MemoryTools.Remote.Hook;
using System.Reflection;
using System.Reflection.Emit;


namespace MemoryTools
{

    [StructLayout(LayoutKind.Sequential)]
    [MemoryAttribute()]
    public struct Ptr<T>
    {
        private IntPtr ptr;

        public Ptr(IntPtr ptr)
        {
            this.ptr = ptr;
        }
        public Ptr(int size = 4)
        {
            this.ptr = MemoryManager.Current.AllocMemory(IntPtr.Zero, size,true);
        }
        public T GetValue(int index = 0)
        {
            int size = 0;
            if (Attribute.IsDefined(typeof(T), typeof(MemoryAttribute))) size = MemoryManager.Current.SizeOf<T>();
            else if(index != 0) size = Marshal.SizeOf<T>();
            return MemoryManager.Current.ReadMemory<T>(ptr + (index == 0 ? index : size * index));

        }
        public IntPtr GetAddress(int index = 0)
        {
            int size = 0;
            if (Attribute.IsDefined(typeof(T), typeof(MemoryAttribute))) size = MemoryManager.Current.SizeOf<T>();
            else if (index != 0) size = Marshal.SizeOf<T>();
            return ptr + (index == 0 ? index : size * index);

        }

        public bool Free()
        {
            return MemoryManager.Current.VirtualFree(this.Pointer, 0, Native.FreeType.MEM_RELEASE);
        }

        public IntPtr Pointer
        {
            get => ptr;
        }
        public T this[int i]
        {
            get
            {
                int size = 0;
                if (Attribute.IsDefined(typeof(T), typeof(MemoryAttribute))) size = MemoryManager.Current.SizeOf<T>();
                else if (i != 0) size = Marshal.SizeOf<T>();
                return MemoryManager.Current.ReadMemory<T>(ptr + (i == 0 ? i : size * i));
            }
            set
            {
                int size = 0;
                if (Attribute.IsDefined(typeof(T), typeof(MemoryAttribute))) size = MemoryManager.Current.SizeOf<T>();
                else if (i != 0) size = Marshal.SizeOf<T>();
                MemoryManager.Current.WriteMemory(ptr + (i == 0 ? i : size * i), value);
            }
        }

    }



}
