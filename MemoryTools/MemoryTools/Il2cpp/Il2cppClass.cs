using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MemoryTools.Il2cpp
{
     public class Il2cppClass
    {
        private IntPtr handle;
        
        private string name;
        private string nameSpace;
        private bool LoadedMethods = false;
        private IntPtr type;

        public string Name => this.name;
        public string NameSpace => this.nameSpace;
        public IntPtr Type => this.type;
        public IntPtr Handle => this.handle;


        private List<Il2cppMethod> methods = new List<Il2cppMethod>();
        private Dictionary<string, IntPtr> fields = new Dictionary<string, IntPtr>();


        public Il2cppClass(IntPtr handle)
        {
            
            this.handle = handle;
            Initialize();
        }
        public IntPtr NewObject(params object[] obj)
        {
            Ptr<long> typeinfo = new Ptr<long>(Il2cppNative.Current.object_new(this.handle));
            Il2cppNative.Current.runtime_object_init(typeinfo.Pointer);
            IntPtr Object = MemoryManager.Current.AllocMemory(IntPtr.Zero, Il2cppNative.Current.class_instance_size(this.handle) + 8);
            MemoryManager.Current.WriteMemory(Object, typeinfo.GetValue());
            int size = 0;
            for(int i = 0; i < obj.Length; i++)
            {
                MemoryManager.Current.WriteMemory(Object + 0x8 + size, obj[i]);
                size += Marshal.SizeOf(obj[i].GetType());
            }

            return Object;
        }
        public IntPtr GetField(string name, IntPtr instance)
        {
            if (fields.ContainsKey(name)) return fields[name];
            IntPtr address;
            IntPtr field = Il2cppNative.Current.class_get_field_from_name(this.Handle, name);
            if (field == IntPtr.Zero) throw new Exception($"Field ({name}) not found in class ({this.Name})");
            if (instance == IntPtr.Zero)
            {
                IntPtr staticFields = Il2cppNative.Current.class_get_static_fields_pointer(this.Handle);
                address = staticFields + Il2cppNative.Current.field_get_offset_nocall(field);
                fields.Add(name, address);
            }
            else
            {
                address = instance + Il2cppNative.Current.field_get_offset_nocall(field);
            }
            
            return address;
        }
        private void Initialize()
        {
            this.name = Il2cppNative.Current.class_get_name(this.handle);
            this.nameSpace = Il2cppNative.Current.class_get_namespace(this.handle);
            this.type = Il2cppNative.Current.class_get_type(this.handle);
        }
        public Il2cppMethod GetMethod(string method)
        {
           
            if (!LoadedMethods)
            {
                InitializeMethods();
                LoadedMethods = true;
            }
            return methods.FirstOrDefault(m => m.DescName == method);
        }
       
        private void InitializeMethods()
        {
            IntPtr iterator = Il2cppNative.Current.Memory.CreatePointer();
            IntPtr method;
            while ((method = Il2cppNative.Current.class_get_methods(this.handle, iterator)) != IntPtr.Zero)
            {
                methods.Add(new Il2cppMethod(method));
             //   Console.WriteLine(methods[methods.Count - 1].Name);
            }
            Il2cppNative.Current.Memory.VirtualFree(iterator, 0, Native.FreeType.MEM_RELEASE);
        }

    }
}
