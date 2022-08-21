using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools;
using System.Runtime.InteropServices;
using MemoryTools;


namespace MemoryTools.Il2cpp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CsharpString
    {
        IntPtr address;
        public string GetString()
        {
            int size = MemoryManager.Current.ReadMemory<int>(address + 8);
           return Encoding.Unicode.GetString(MemoryManager.Current.ReadBytes(address + 12, size * 2));
        }
    }
 
    [Memory()]
    public struct CSharpArray<T>
    {
      private uint typeinfo;
      public int length;
      [Memory(ArraySizeReference ="length")]
      public T[] items;
    };
  
}
