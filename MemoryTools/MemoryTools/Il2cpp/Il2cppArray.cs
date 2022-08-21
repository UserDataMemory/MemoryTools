using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Il2cpp
{
    public class Il2cppArray<T>
    {
        IntPtr domain, type, arrayType, array;
        Ptr<int> ptr;
        int length, elementSize;
        public Il2cppArray(IntPtr type, int length)
        {
            this.type = type;
            this.length = length;
           
            this.arrayType = new Ptr<IntPtr>(Il2cppNative.Current.array_new(type, 1)).GetValue();
            this.elementSize = Il2cppNative.Current.array_element_size(arrayType);
           
            this.ptr = new Ptr<int>(MemoryManager.Current.AllocMemory(IntPtr.Zero, 12 + elementSize * length,true));
            ptr[0] = arrayType.ToInt32();
            ptr[3] = length;
            array = ptr.Pointer;
        }
        public bool Free()
        {
            return MemoryManager.Current.VirtualFree(this.Pointer, 0, Native.FreeType.MEM_RELEASE);
        }
        public Il2cppArray(IntPtr address)
        {
           

            this.arrayType = new Ptr<IntPtr>(address).GetValue();
            this.elementSize = Il2cppNative.Current.array_element_size(arrayType);
            this.ptr = new Ptr<int>(address);
            this.type = new Ptr<IntPtr>(arrayType)[8];
            this.length = ptr[3];
            array = ptr.Pointer;
        }


        public bool setValue(int offset, object obj)
        {
            return MemoryManager.Current.WriteMemory(Pointer + 0x10+ offset, obj);
        }

        public IntPtr Pointer { get => array; }
        public IntPtr ArrayType { get => arrayType; }
        public IntPtr Type { get => type; }
        public int Length { get => ptr[3]; set => ptr[3] = value; }
        public int ElementSize { get => elementSize; }

        public T this[int i]
        {
            get
            {
                return MemoryManager.Current.ReadMemory<T>(ptr.Pointer + 0x10 + (i == 0 ? i : elementSize * i));
            }
            set
            {
                MemoryManager.Current.WriteMemory(ptr.Pointer + 0x10 + (i == 0 ? i : elementSize * i), value);
            }
        }

    }
}
