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
    /*
    [Memory()]
    public struct Dicionary<T>
    {
        private uint typeinfo;
        public int length;
        [Memory(ArraySizeReference = "length")]
        public T[] items;
    };
    */
    /*
    [Memory()]
    struct CSharpDictionary<K,V>
    {
        ulong typeinfo;
        uint bounds;
        int length;
        CSharpArray<T> items;
    };
    */
    /*
    [StructLayout(LayoutKind.Sequential)]
    [GenericPtr()]
    public struct Il2CppGenericInst
    {
        public uint type_argc;
        public Ptr<IntPtr> type_argv;
    }
    [StructLayout(LayoutKind.Sequential)]
    [GenericPtr()]
    public struct Il2CppGenericContext
    {
        public Ptr<IntPtr> class_inst;
        public Ptr<IntPtr> method_inst;
    }

    ;
    [StructLayout(LayoutKind.Sequential)]
    [GenericPtr()]
    public struct Il2CppGenericClass
    {

        public Il2CppGenericContext context;

        public Ptr<IntPtr> cached_class;
    }
    [StructLayout(LayoutKind.Sequential)]
    [GenericPtr()]
    public struct Il2CppAssemblyName
    {
        public Ptr<string> name;
        public Ptr<string> culture;
        public Ptr<string> hash_value;
        public Ptr<string> public_key;
        public uint hash_alg;
        public int hash_len;
        public uint flags;
        public int major;
        public int minor;
        public int build;
        public int revision;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [GenericPtr(8)]
        public byte[] public_key_token;
    }


    [StructLayout(LayoutKind.Sequential)]
    [GenericPtr()]
    public struct Il2CppAssembly
    {
        public Ptr<Il2CppImage> image;
        public uint token;
        public int referencedAssemblyStart;
        public int referencedAssemblyCount;
        public Il2CppAssemblyName aname;
    }

    [StructLayout(LayoutKind.Sequential)]
    [GenericPtr()]
    public struct Il2CppImage
    {
        public Ptr<string> name;
        public Ptr<string> nameNoExt;
        public Ptr<string> assembly;
        public int typeStart;
        public uint typeCount;
        public int exportedTypeStart;
        public uint exportedTypeCount;
        public int customAttributeStart;
        public uint customAttributeCount;
        public int entryPointIndex;
        public Ptr<IntPtr> nameToClassHashTable;
        public uint token;
        public byte dynamic;
    }
    public struct FieldInfo
    {
        Ptr<string> name;
        Ptr<Il2CppType> type;
        Ptr<Il2CppClass> parent;
        int offset; // If offset is -1, then it's thread static
        uint token;
    }
    struct MethodInfo
    {
        Ptr<IntPtr> methodPointer;
        Ptr<IntPtr> invoker_method;
        Ptr<string> name;
        Ptr<Il2CppClass> klass;
        Ptr<Il2CppType> return_type;
        const ParameterInfo* parameters;

        Ptr<IntPtr> methodMetadataHandle;
        IntPtr<IntPtr> nativeFunction;

        uint token;
        ushort flags;
        ushort iflags;
        ushort slot;
        byte parameters_count;
       }
        public struct Il2CppClass
        {
            // The following fields are always valid for a Il2CppClass structure
            public Ptr<Il2CppImage> image;
            Ptr<IntPtr> gc_desc;
            Ptr<string> name;
            Ptr<string> namespaze;
            Il2CppType byval_arg;
            Il2CppType this_arg;
            Ptr<Il2CppClass> element_class;
            Ptr<Il2CppClass> castClass;
            Ptr<Il2CppClass> declaringType;
            Ptr<Il2CppClass> parent;
            Ptr<IntPtr> generic_class;
            Ptr<IntPtr> typeDefinition; // non-NULL for Il2CppClass's constructed from type defintions
            Ptr<IntPtr> interopData;
            Ptr<Il2CppClass> klass; // hack to pretend we are a MonoVTable. Points to ourself
                                    // End always valid fields

            // The following fields need initialized before access. This can be done per field or as an aggregate via a call to Class::Init
            Ptr<FieldInfo> fields; // Initialized in SetupFields
            Ptr<IntPtr> events; // Initialized in SetupEvents
            const PropertyInfo* properties; // Initialized in SetupProperties
            const MethodInfo** methods; // Initialized in SetupMethods
            Ptr<IntPtr> nestedTypes; // Initialized in SetupNestedTypes
            Ptr<IntPtr> implementedInterfaces; // Initialized in SetupInterfaces
            Ptr<IntPtr> interfaceOffsets; // Initialized in Init
            Ptr<IntPtr> static_fields; // Initialized in Init
            Ptr<IntPtr> rgctx_data; // Initialized in Init
                                    // used for fast parent checks
            Ptr<IntPtr> typeHierarchy; // Initialized in SetupTypeHierachy
                                       // End initialization required fields

            Ptr<IntPtr> unity_user_data;

            uint initializationExceptionGCHandle;

            uint cctor_started;
            uint cctor_finished;
            long cctor_thread;

            // Remaining fields are always valid except where noted
            int genericContainerIndex;
            uint instance_size;
            uint actualSize;
            uint element_size;
            int native_size;
            uint static_fields_size;
            uint thread_static_fields_size;
            int thread_static_fields_offset;
            uint flags;
            uint token;

            ushort method_count; // lazily calculated for arrays, i.e. when rank > 0
            ushort property_count;
            ushort field_count;
            ushort event_count;
            ushort nested_type_count;
            ushort vtable_count; // lazily calculated for arrays, i.e. when rank > 0
            ushort interfaces_count;
            ushort interface_offsets_count; // lazily calculated for arrays, i.e. when rank > 0

            byte typeHierarchyDepth; // Initialized in SetupTypeHierachy
            byte genericRecursionDepth;
            byte rank;
            byte minimumAlignment; // Alignment of this type
            byte naturalAligment; // Alignment of this type without accounting for packing
            byte packingSize;

      
    */
}