using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MemoryTools.Il2cpp;
using MemoryTools.Remote;
using System.Threading.Tasks;


namespace MemoryTools.Il2cpp
{

    public class Il2cpp
    {

        private List<Il2cppAssembly> assemblies = new List<Il2cppAssembly>();
        private IntPtr domain;


        public IntPtr Domain { get => domain; }

        public Il2cppAssembly GetAssembly(string assembly)
        {
            return assemblies.FirstOrDefault(a => a.Name == assembly);
        }
        public void setHook(IntPtr address, int size, bool restore = false)
        {
            Il2cppNative.Current.setHook(address, size, restore);
        }
        
        private void Initialize()
        {
            
            this.domain = Il2cppNative.Current.domain_get();
            if (domain == IntPtr.Zero) throw new Exception("il2cpp Error Get Domain");
            AttachThread();
            Ptr<int> assemblies_size_ptr = Il2cppNative.Current.Memory.CreatePointer<int>();
            Ptr<IntPtr> assemblies_ptr = Il2cppNative.Current.domain_get_assemblies(domain, assemblies_size_ptr);
            for (int i = 0; i < assemblies_size_ptr.GetValue(); i++)
            {
                assemblies.Add(new Il2cppAssembly(assemblies_ptr.GetValue(i)));
            }
            Il2cppNative.Current.Memory.VirtualFree(assemblies_size_ptr.Pointer, 0, Native.FreeType.MEM_RELEASE);
        }

        public void AttachThread()
        {
            if (Il2cppNative.Current.thread_attach(domain) == IntPtr.Zero) throw new Exception("il2cpp Error Attach Thread");
        }

        public bool Attach(int pid)
        {
            bool Attached = Il2cppNative.Current.Attach(pid);
            if (Attached)
                Initialize();
            return Attached;
        }
    }
}
