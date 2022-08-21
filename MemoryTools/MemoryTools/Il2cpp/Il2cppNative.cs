using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools.Remote;
using MemoryTools.Remote.Hook;
using MemoryTools;
using System.Runtime.InteropServices;

namespace MemoryTools.Il2cpp
{
    public class Il2cppNative
    {
        // NAME, PARAMETER COUNT
        Dictionary<string, int> il2cppFunctionsModel = new Dictionary<string, int>()
        {
            {"il2cpp_domain_get", 0},
            {"il2cpp_domain_assembly_open", 2},
            {"il2cpp_assembly_get_image", 1},
            {"il2cpp_class_from_name", 3},
            {"il2cpp_class_get_method_from_name", 3},
            {"il2cpp_runtime_invoke", 4},
            {"il2cpp_thread_attach", 1},
            {"il2cpp_class_get_methods", 2},
            {"il2cpp_method_get_name", 1},
            {"il2cpp_image_get_class", 2},
            {"il2cpp_class_get_name", 1},
            {"il2cpp_image_get_class_count", 1},
            {"il2cpp_domain_get_assemblies", 2},
            {"il2cpp_image_get_name", 1},
            {"il2cpp_class_get_namespace", 1},
            {"il2cpp_image_get_entry_point", 1},
            {"il2cpp_image_get_filename", 1},
            {"il2cpp_method_get_param_count", 1},
            {"il2cpp_method_get_param_name", 2},
            {"il2cpp_method_get_param", 2},
            {"il2cpp_type_get_name", 1},
            {"il2cpp_method_is_instance", 1},
            {"il2cpp_method_get_flags", 2},
            {"il2cpp_string_new", 1},
            {"il2cpp_class_get_type",1},
            {"il2cpp_type_get_object",1},
            {"il2cpp_type_get_type",1},
            {"il2cpp_class_get_declaring_type",1},
            {"il2cpp_object_new",1},
            {"il2cpp_object_unbox",1},
            {"il2cpp_runtime_object_init",1},
            {"il2cpp_class_instance_size",1},
            {"il2cpp_object_get_class",1},
            {"il2cpp_array_length", 1},
            {"il2cpp_class_get_field_from_name", 2},
            {"il2cpp_field_get_value", 3},
            {"il2cpp_field_static_get_value", 2},
            {"il2cpp_field_get_value_object", 2},
            {"il2cpp_field_get_offset",1},
            {"il2cpp_class_get_fields", 2},
            {"il2cpp_field_get_name",1},
            {"il2cpp_field_get_type",1 },
            {"il2cpp_array_new", 2},
            {"il2cpp_gchandle_new", 2},
            {"il2cpp_array_element_size", 1},
            {"il2cpp_array_class_get",2 }
         };
        Dictionary<string, RemoteFunction> il2cppFunctions = new Dictionary<string, RemoteFunction>();


        private MemoryManager memoryManager;
        private RemoteThread thread;
        private RemoteHook hook;
        private RemoteFunction GetModuleHandleA;
        private RemoteFunction GetProcAddress;
        static Il2cppNative il2CppNative;

        public static Il2cppNative Current => il2CppNative == null ? (il2CppNative = new Il2cppNative()) : il2CppNative;
        public RemoteHook Hook => this.hook;

        public Il2cppNative()
        {
            memoryManager = new MemoryManager();
        }
        public void setHook(IntPtr address, int size, bool restore = false)
        {
            if (restore && RemoteHook.isHooked(address))
                this.hook = new RemoteHook(address, size);
            else
                this.hook = memoryManager.HookerManager.Create(address, size);
        }
        private IntPtr GetModule(string module)
        {
            if (GetModuleHandleA == null) GetModuleHandleA = memoryManager.CallManager.Create(memoryManager.getModuleFunction("kernel32.dll", "GetModuleHandleA"), 1, CallingConventions.stdcall, thread);
            return GetModuleHandleA.Invoke<IntPtr>(module);
        }
        private IntPtr GetModuleFunction(IntPtr module, string func)
        {
            if (GetProcAddress == null) GetProcAddress = memoryManager.CallManager.Create(memoryManager.getModuleFunction("kernel32.dll", "GetProcAddress"), 2, CallingConventions.stdcall, thread);
            return GetProcAddress.Invoke<IntPtr>(module, func);
        }

        private void Initialize()
        {
            thread = memoryManager.ThreadManager.Create();
            il2cppFunctions.Clear();
            IntPtr GameAssembly = GetModule("GameAssembly.dll");
            foreach (var kv in il2cppFunctionsModel)
            {

                il2cppFunctions.Add(kv.Key, memoryManager.CallManager.Create(GetModuleFunction(GameAssembly, kv.Key), kv.Value, CallingConventions.stdcall, thread));
            }
        }
        public IntPtr domain_get()
        {
            return il2cppFunctions["il2cpp_domain_get"].Invoke<IntPtr>();
        }
        public IntPtr domain_assembly_open(IntPtr domain, string name)
        {
            return il2cppFunctions["il2cpp_domain_assembly_open"].Invoke<IntPtr>(domain, name);
        }
        public IntPtr assembly_get_image(IntPtr assembly)
        {
            return il2cppFunctions["il2cpp_assembly_get_image"].Invoke<IntPtr>(assembly);
        }

        public IntPtr gchandle_new(IntPtr obj, bool pinned)
        {
            return il2cppFunctions["il2cpp_gchandle_new"].Invoke<IntPtr>(obj, pinned);
        }
        public int array_element_size(IntPtr type)
        {
            return il2cppFunctions["il2cpp_array_element_size"].Invoke<int>(type);
        }

        public IntPtr array_class_get(IntPtr elementType, int rank)
        {
            return il2cppFunctions["il2cpp_array_class_get"].Invoke<IntPtr>(elementType, rank);
        }
        public IntPtr array_new(IntPtr type, int length)
        {

            return il2cppFunctions["il2cpp_array_new"].Invoke<IntPtr>(type, length);
        }
        public IntPtr class_get_method_from_name(IntPtr _class, string name, int paramCount)
        {
            return il2cppFunctions["il2cpp_class_get_method_from_name"].Invoke<IntPtr>(_class, name, paramCount);
        }


        public string field_get_name(IntPtr field)
        {
            return il2cppFunctions["il2cpp_field_get_name"].Invoke<Ptr<string>>(field).GetValue();
        }
        public IntPtr field_get_type(IntPtr field)
        {
            return il2cppFunctions["il2cpp_field_get_type"].Invoke<IntPtr>(field);
        }
        public int class_instance_size(IntPtr _class)
        {
            return il2cppFunctions["il2cpp_class_instance_size"].Invoke<int>(_class);
        }
        public IntPtr class_from_name(IntPtr image, string name_space, string name)
        {
            return il2cppFunctions["il2cpp_class_from_name"].Invoke<IntPtr>(image, name_space, name);
        }


        public IntPtr class_get_field_from_name(IntPtr _class, string name)
        {
            return il2cppFunctions["il2cpp_class_get_field_from_name"].Invoke<IntPtr>(_class, name);
        }
        public int field_get_offset(IntPtr field)
        {
            return il2cppFunctions["il2cpp_field_get_offset"].Invoke<int>(field);
        }
        public int field_get_offset_nocall(IntPtr field)
        {
            return memoryManager.ReadMemory<int>(field + 0xC);
        }
        public IntPtr field_get_class(IntPtr field)
        {
            return memoryManager.ReadMemory<IntPtr>(field + 0x8);
        }
        public IntPtr class_get_static_fields_pointer(IntPtr _class)
        {
            return memoryManager.ReadMemory<IntPtr>(_class + 0x5C);
        }
        public IntPtr field_get_value(IntPtr instance, IntPtr field, int size)
        {

            IntPtr ptr = memoryManager.AllocMemory(IntPtr.Zero, size);
            il2cppFunctions["il2cpp_field_get_value"].Invoke<Null>(instance, field, ptr);


            return ptr;
        }
        public T field_static_get_value<T>(IntPtr field)
        {

            Ptr<T> ptr = new Ptr<T>(Marshal.SizeOf<T>());
            il2cppFunctions["il2cpp_field_static_get_value"].Invoke<Null>(field, ptr.Pointer);
            T value = ptr.GetValue();
            ptr.Free();
            return value;
        }
        public IntPtr field_get_value_object(IntPtr instance, IntPtr field)
        {
            return il2cppFunctions["il2cpp_field_get_value_object"].Invoke<IntPtr>(field, instance);
        }
        public int array_length(IntPtr array)
        {
            return il2cppFunctions["il2cpp_array_length"].Invoke<int>(array);
        }
        public IntPtr thread_attach(IntPtr domain)
        {
            return il2cppFunctions["il2cpp_thread_attach"].Invoke<IntPtr>(domain);
        }
        public IntPtr method_get_address(IntPtr il2cppMethod)
        {
            return memoryManager.ReadMemory<IntPtr>(il2cppMethod);
        }

        /* il2cpp_class_get_fields
       */
        public IntPtr class_get_fields(IntPtr _class, IntPtr iterator)
        {
            return il2cppFunctions["il2cpp_class_get_fields"].Invoke<IntPtr>(_class, iterator);
        }
        public IntPtr class_get_methods(IntPtr _class, IntPtr iterator)
        {
            return il2cppFunctions["il2cpp_class_get_methods"].Invoke<IntPtr>(_class, iterator);
        }
        public IntPtr object_get_class(IntPtr obj)
        {

            return il2cppFunctions["il2cpp_object_get_class"].Invoke<IntPtr>(obj);
        }
        public string method_get_name(IntPtr method)
        {
            return il2cppFunctions["il2cpp_method_get_name"].Invoke<Ptr<string>>(method).GetValue();
        }
        public IntPtr image_get_class(IntPtr image, int index)
        {
            return il2cppFunctions["il2cpp_image_get_class"].Invoke<IntPtr>(image, index);
        }
        public string class_get_name(IntPtr _class)
        {
            return il2cppFunctions["il2cpp_class_get_name"].Invoke<Ptr<string>>(_class).GetValue();
        }

        public int image_get_class_count(IntPtr image)
        {
            return il2cppFunctions["il2cpp_image_get_class_count"].Invoke<int>(image);
        }
        public Ptr<IntPtr> domain_get_assemblies(IntPtr domain, Ptr<int> size)
        {
            return il2cppFunctions["il2cpp_domain_get_assemblies"].Invoke<Ptr<IntPtr>>(domain, size.Pointer);
        }
        public string image_get_name(IntPtr domain)
        {
            return il2cppFunctions["il2cpp_image_get_name"].Invoke<Ptr<string>>(domain).GetValue();
        }
        public string class_get_namespace(IntPtr _class)
        {
            return il2cppFunctions["il2cpp_class_get_namespace"].Invoke<Ptr<string>>(_class).GetValue();
        }

        public IntPtr image_get_entry_point(IntPtr image)
        {
            return il2cppFunctions["il2cpp_image_get_entry_point"].Invoke<IntPtr>(image);
        }
        public string image_get_filename(IntPtr image)
        {
            return il2cppFunctions["il2cpp_image_get_filename"].Invoke<Ptr<string>>(image).GetValue();
        }
        public uint method_get_param_count(IntPtr method)
        {
            return il2cppFunctions["il2cpp_method_get_param_count"].Invoke<uint>(method);
        }
        public string method_get_param_name(IntPtr method, uint index)
        {
            return il2cppFunctions["il2cpp_method_get_param_name"].Invoke<Ptr<string>>(method, index).GetValue();
        }
        public IntPtr method_get_param(IntPtr method, int index)
        {
            return il2cppFunctions["il2cpp_method_get_param"].Invoke<IntPtr>(method, index);
        }
        public string type_get_name(IntPtr type)
        {
            return il2cppFunctions["il2cpp_type_get_name"].Invoke<Ptr<string>>(type).GetValue();
        }
        public bool method_is_instance(IntPtr method)
        {
            return il2cppFunctions["il2cpp_method_is_instance"].Invoke<bool>(method);
        }
        public uint method_get_flags(IntPtr method, IntPtr flags)
        {
            return il2cppFunctions["il2cpp_method_get_flags"].Invoke<uint>(method, flags);
        }
        public IntPtr class_get_type(IntPtr _class)
        {
            return il2cppFunctions["il2cpp_class_get_type"].Invoke<IntPtr>(_class);
        }

        public IntPtr type_get_object(IntPtr type)
        {
            return il2cppFunctions["il2cpp_type_get_object"].Invoke<IntPtr>(type);
        }

        public IntPtr type_get_type(IntPtr type)
        {
            return il2cppFunctions["il2cpp_type_get_type"].Invoke<IntPtr>(type);
        }
        //il2cpp_runtime_object_init

        public void runtime_object_init(IntPtr _object)
        {
            il2cppFunctions["il2cpp_runtime_object_init"].Invoke<Null>(_object);
        }

        public IntPtr object_unbox(IntPtr _object)
        {
            return il2cppFunctions["il2cpp_object_unbox"].Invoke<IntPtr>(_object);
        }
        public IntPtr object_new(IntPtr _class)
        {
            return il2cppFunctions["il2cpp_object_new"].Invoke<IntPtr>(_class);
        }
        public IntPtr class_get_declaring_type(IntPtr _class)
        {
            return il2cppFunctions["il2cpp_class_get_declaring_type"].Invoke<IntPtr>(_class);
        }
        public IntPtr string_new(string text)
        {
            return il2cppFunctions["il2cpp_string_new"].Invoke<IntPtr>(text);
        }




        public MemoryManager Memory => this.memoryManager;
        public RemoteThread Thread
        {
            get => this.thread;
        }
        public bool Attach(int pid)
        {
            if (memoryManager.Attach(pid))
            {
                Initialize();
                return true;
            }
            return false;
        }


    }
}
