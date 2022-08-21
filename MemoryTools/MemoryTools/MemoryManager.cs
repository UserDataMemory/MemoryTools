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
using MemoryTools.Assembler;

namespace MemoryTools
{
    public class MemoryManager
    {
        private IntPtr handle;
        private RemoteCall remoteCall;
        private RemoteThreadManager remoteThreadManager;
        private RemoteHookerManager remoteHookerManager;
        private Process process;
        private RemoteFunction __readfsdword = null;
        private RemoteFunction LoadLibraryA = null;
        public static MemoryManager memoryManager;
        public const int PREVIOUS_STACK = 0x24;

        public static MemoryManager Current => memoryManager;

        public RemoteThreadManager ThreadManager
        {
            get => this.remoteThreadManager;
        }
        public RemoteCall CallManager
        {
            get => this.remoteCall;
        }
        public RemoteHookerManager HookerManager
        {
            get => this.remoteHookerManager;
        }
        public IntPtr Handle
        {
            get => this.handle;
        }
        public bool ProcessExiteed
        {
            get => process.HasExited;
        }
        public MemoryManager()
        {

            this.remoteCall = new RemoteCall(this);
            this.remoteThreadManager = new RemoteThreadManager(this);
            this.remoteHookerManager = new RemoteHookerManager(this);
            memoryManager = this;


        }

        public bool Attach(int pid)
        {
            __readfsdword = null;
            LoadLibraryA = null;
            return (handle = NativeFunctions.OpenProcess(ProcessAccessRight.PROCESS_ALL_ACCESS, false, pid)) != IntPtr.Zero && (this.process = Process.GetProcessById(pid)) != null;
        }

        public bool is64()
        {
            return IntPtr.Size == 8;
        }
        public Context GetThreadContext(IntPtr handle, ContextFlags flags)
        {
            Context context = new Context();
            context.ContextFlags = flags;
            return NativeFunctions.GetThreadContext(handle, ref context) ? context : default(Context);

        }

        public T RemoteReadFSDword<T>(int offset)
        {
            if (__readfsdword == null)
            {
                StringBuilder code = new StringBuilder();
                code.AppendLine($"mov edx,[esp+4]");
                code.AppendLine($"fs mov eax,[edx]");
                code.AppendLine($"ret");
                IntPtr mem = AllocMemory(IntPtr.Zero, 8, true);
                WriteBytes(mem, Assembler.Assembler.assemble(code, mem));
                RemoteThread thread = this.ThreadManager.Create();
                __readfsdword = this.CallManager.Create(mem, 1, Remote.CallingConventions.cdecl, thread);
            }
            return __readfsdword.Invoke<T>(offset);
        }
        public IntPtr RemoteLoadLibrary(string path)
        {
            IntPtr handle = IntPtr.Zero;
            if (LoadLibraryA == null)
            {
                IntPtr LoadLibraryAddress = getModuleFunction("kernel32.dll", "LoadLibraryA");
                if (LoadLibraryAddress != IntPtr.Zero)
                {
                    RemoteThread thread = this.ThreadManager.Create();
                    LoadLibraryA = memoryManager.CallManager.Create(LoadLibraryAddress, 1, Remote.CallingConventions.stdcall, thread);

                }
            }
            if(LoadLibraryA != null)
            {
                handle = LoadLibraryA.Invoke<IntPtr>(path);
            }
            return handle;
        }
        public T QueryInformationThread<T>(IntPtr ThreadHandle, ThreadInfoClass ThreadInformationClass, out NtStatus result)
        {
            Type type = typeof(T);
            int threadInformationSize = Marshal.SizeOf<T>();
            IntPtr mem = Marshal.AllocHGlobal(threadInformationSize);
            result = NativeFunctions.NtQueryInformationThread(ThreadHandle, ThreadInformationClass, mem, threadInformationSize, IntPtr.Zero);
            T value = Marshal.PtrToStructure<T>(mem);
            Marshal.FreeHGlobal(mem);
            return value;
        }

        public byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] bytes = new byte[size];
            int read;
            if (!NativeFunctions.ReadProcessMemory(this.handle, address, bytes, size, out read) || read != size) throw new ArgumentException(String.Format("Failed to read memory. address: {0}", address.ToInt32().ToString("X")));
            return bytes;
        }

        private bool isAllowedType(Type type)
        {

            return !type.IsGenericType && !type.IsArray && !type.IsEnum && !isStruct(type);
        }
        public List<object> GetStructFields(object obj)
        {
            List<object> fields = new List<object>();
            Type type = obj.GetType();
            foreach (var v in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                Type fieldType = v.FieldType;
                object value = v.GetValue(obj);
                if (Attribute.IsDefined(v, typeof(MemoryAttribute)) && ((MemoryAttribute)v.GetCustomAttribute(typeof(MemoryAttribute))).Ignore) continue;
                if (isStruct(fieldType))
                {
                    fields.AddRange(GetStructFields(value));

                }
                else if (fieldType.IsArray)
                {

                    Array arr = (Array)value;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        object arrayValue = arr.GetValue(i);

                        if (!isAllowedType(fieldType.GetElementType()))
                        {
                            fields.AddRange(GetStructFields(arrayValue));
                            continue;
                        }
                        fields.Add(arrayValue);
                    }
                }
                else if (fieldType.IsEnum)
                {
                    Type enumType = fieldType.GetEnumUnderlyingType();
                    fields.Add(Convert.ChangeType(value, enumType));
                }
                else
                {
                    fields.Add(value);
                }
            }
            return fields;
        }
        public int GetStructFieldSize(object obj)
        {
            int size = 0;
            List<object> fields = GetStructFields(obj);
            for (int i = 0; i < fields.Count; i++)
            {
                size += Marshal.SizeOf(fields[i]);
            }
            return size;
        }

        public int SizeOf<T>()
        {
            return SizeOf(typeof(T));
        }

        public int SizeOf(Type type)
        {
            var dynamicMethod = new DynamicMethod("SizeOf", typeof(int), Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();

            generator.Emit(OpCodes.Sizeof, type);
            generator.Emit(OpCodes.Ret);

            var function = (Func<int>)dynamicMethod.CreateDelegate(typeof(Func<int>));
            return function();
        }

        public object ParseStructFromMemory(IntPtr address, Type type, out int structSize)
        {
            object instance = Activator.CreateInstance(type);
            Type instanceType = instance.GetType();
            int offset = 0;
            structSize = 0;
            foreach (var v in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                Type fieldType = v.FieldType;
                if (fieldType.IsGenericType)
                {
                    int size = 0;
                    object obj = ParseStructFromMemory(address + offset, fieldType, out size);
                    v.SetValue(instance, obj);
                    structSize += size;
                    offset += size;
                }
                else if (fieldType.IsArray)
                {

                    Type elementType = fieldType.GetElementType();


                    MemoryAttribute generic = (MemoryAttribute)v.GetCustomAttribute(typeof(MemoryAttribute));
                    int arraySize = generic.ArraySize != 0 ? generic.ArraySize : generic.ArraySizeReference != "" ? (int)instanceType.GetField(generic.ArraySizeReference).GetValue(instance) : 0;
                    Array arr = Array.CreateInstance(elementType, arraySize);
                    int elementSize = isStruct(elementType) ? -1 : Marshal.SizeOf(elementType);
                    for (int i = 0; i < arraySize; i++)
                    {
                        object obj = ReadMemory(address + offset, elementType, out _);
                        arr.SetValue(obj, i);
                        offset += elementSize == -1 ? GetStructFieldSize(obj) : elementSize;
                    }
                    v.SetValue(instance, arr);
                }
                else if (fieldType.IsEnum)
                {
                    Type enumType = fieldType.GetEnumUnderlyingType();
                    v.SetValue(instance, ReadMemory(address + offset, enumType, out _));
                    structSize += Marshal.SizeOf(enumType);
                    offset += Marshal.SizeOf(enumType);
                }
                else
                {
                    v.SetValue(instance, ReadMemory(address + offset, fieldType, out _));
                    structSize += Marshal.SizeOf(fieldType);
                    offset += Marshal.SizeOf(fieldType);
                }
            }
            return instance;
        }
        public object ParseStructFromMemory(IntPtr address, Type type)
        {
            int offset;
            return ParseStructFromMemory(address, type, out offset);
        }
        public T ParseStruct<T>(IntPtr address)
        {
            return (T)ParseStructFromMemory(address, typeof(T));
        }
        public T IntPtrToObject<T>(IntPtr value)
        {
            Type type = typeof(T);
            IntPtr mem = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(mem, 0, value);
            object ptr = Marshal.PtrToStructure(mem, type);
            Marshal.FreeHGlobal(mem);
            return (T)ptr;
        }

        public object ReadMemory(IntPtr address, Type type, out int readSize)
        {

            readSize = 0;

            if (type.IsAssignableFrom(typeof(string))) return ReadString(address);
            if (Attribute.IsDefined(type, typeof(MemoryAttribute))) return ParseStructFromMemory(address, type, out readSize);
            //   if (!type.IsPrimitive) throw new ArgumentException(String.Format("Invalid Type<{0}>", type.Name));
            int size = Marshal.SizeOf(type);
            readSize = size;
            // IntPtr alloc = Marshal.
            byte[] bytes = ReadBytes(address, size);
            return BytesToStruct(bytes, type);        }


        public object BytesToStruct(byte[] bytes, Type type)
        {
            return Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), type);
        }
        /*
        public object ReadMemory(IntPtr address, Type type, out int readSize)
        {
            readSize = 0;
            if (type.IsAssignableFrom(typeof(string))) return ReadString(address);
            if (Attribute.IsDefined(type, typeof(MemoryAttribute))) return ParseStructFromMemory(address, type,out readSize);
            //   if (!type.IsPrimitive) throw new ArgumentException(String.Format("Invalid Type<{0}>", type.Name));
            int size = Marshal.SizeOf(type);
            readSize = size;
            IntPtr alloc = Marshal.AllocHGlobal(size);
            byte[] bytes = ReadBytes(address, size);
            Marshal.Copy(bytes, 0, alloc, size);
            object value = Marshal.PtrToStructure(alloc, type);
            Marshal.FreeHGlobal(alloc);
            return value;
        }
        */
        public T ReadMemory<T>(IntPtr address)
        {
            Type type = typeof(T);
            return (T)ReadMemory(address, type, out _);
        }
        public Ptr<T> CreatePointer<T>()
        {
            return new Ptr<T>(AllocMemory(IntPtr.Zero, Marshal.SizeOf<T>()));
        }
        public IntPtr CreatePointer()
        {
            return AllocMemory(IntPtr.Zero, Marshal.SizeOf<IntPtr>());
        }
        public bool isStruct(Type type)
        {

            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        public ProcessThread GetProcessThread(int id)
        {

            ProcessThreadCollection processThreadCollection = this.process.Threads;
            ProcessThread processThread = default(ProcessThread);
            for (int i = 0; i < processThreadCollection.Count; i++)
                if (processThreadCollection[i].Id == id)
                    processThread = processThreadCollection[i];
            return processThread;
        }
        public bool WriteMemory(IntPtr address, object value)
        {
            Type type = value.GetType();
            if (type.IsAssignableFrom(typeof(string))) return WriteString(address, (string)value);
            if (!type.IsPrimitive && !type.IsEnum) throw new ArgumentException(String.Format("Invalid Type<{0}>", type.Name));
            if (type.IsEnum)
            {
                Type enumType = type.GetEnumUnderlyingType();
                value = Convert.ChangeType(value, enumType);
                type = enumType;
            }
            int size = Marshal.SizeOf(type);
            byte[] bytes = new byte[size];
            //   Console.WriteLine($"addr: {Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0).ToInt32().ToString("X")}");
            Marshal.StructureToPtr(value, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), false);
            return WriteBytes(address, bytes);
        }
        public bool WriteBytes(IntPtr address, byte[] bytes)
        {
            MemoryProtectionConstants oldProtection;
            MemoryProtectionConstants olddProtection;


            int written;
            return NativeFunctions.VirtualProtectEx(this.Handle, address, bytes.Length, MemoryProtectionConstants.PAGE_EXECUTE_READWRITE, out oldProtection) && NativeFunctions.WriteProcessMemory(this.handle, address, bytes, bytes.Length, out written) && written == bytes.Length && NativeFunctions.VirtualProtectEx(this.Handle, address, bytes.Length, oldProtection, out olddProtection);
        }
        public bool ZeroMemory(IntPtr lpAddress, int dwSize)
        {
            return WriteBytes(lpAddress, new byte[dwSize]);
        }
        public IntPtr AllocMemory(IntPtr lpAddress, int dwSize, bool zeroMem = false, AllocationType flAllocationType = AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtectionConstants flProtect = MemoryProtectionConstants.PAGE_EXECUTE_READWRITE)
        {
            IntPtr mem = NativeFunctions.VirtualAllocEx(this.handle, lpAddress, dwSize, flAllocationType, flProtect);
            if (mem == null) new Exception(String.Format("Failed to allocate memory. address: {0}", lpAddress.ToInt32().ToString("X")));
            if (zeroMem) ZeroMemory(mem, dwSize);
            return mem;
        }

        public string ReadString(IntPtr address)
        {

            List<byte> bytes = new List<byte>();
            byte chr;
            while ((chr = ReadMemory<byte>(address)) != 0)
            {
                bytes.Add(chr);
                address = address + 1;
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        public string ReadWideString(IntPtr address)
        {

            List<byte> bytes = new List<byte>();
            byte[] chr;
            while ((chr = ReadBytes(address, 2))[0] != 0)
            {
                bytes.AddRange(chr);
                address = address + 2;
            }
            return Encoding.Unicode.GetString(bytes.ToArray());
        }
        public bool WriteString(IntPtr address, string text)
        {
            return WriteBytes(address, Encoding.UTF8.GetBytes(text));
        }
        public IntPtr CreateRemoteString(string text)
        {
            IntPtr mem = AllocMemory(IntPtr.Zero, text.Length == 0 ? 1 : text.Length);
            WriteString(mem, text);
            return mem;
        }

        public ProcessModule getProcessModule(string module)
        {
            foreach (ProcessModule v in process.Modules)
            {
                if (v.ModuleName == module)
                {
                    return v;
                }
            }
            return default(ProcessModule);
        }
        public Dictionary<string, IntPtr> getModules()
        {
            Dictionary<string, IntPtr> modules = new Dictionary<string, IntPtr>();
            IntPtr PEB = RemoteReadFSDword<IntPtr>(0x30);
            IntPtr PEB_LDR_DATA = ReadMemory<IntPtr>(PEB + 0xC);
            Ptr<LDR_MODULE> InLoadOrderModuleList = new Ptr<LDR_MODULE>(PEB_LDR_DATA + 0xC);
            ENTRY_LIST current = InLoadOrderModuleList.GetValue().InLoadOrderModuleList;
            while ((current = new Ptr<ENTRY_LIST>(current.Flink).GetValue()).Flink != IntPtr.Zero && current.Flink != InLoadOrderModuleList.Pointer)
            {

                LDR_MODULE module = new Ptr<LDR_MODULE>(current.Flink).GetValue();
                IntPtr baseAddress = module.BaseAddress;
                string name = ReadWideString(module.BaseDllName).Split('\\').Last();
                modules.Add(name, baseAddress);
            }
            return modules;
        }

        public IntPtr getModuleFS(string module)
        {
            foreach (var kv in getModules())
            {
                if (kv.Key.ToLower() == module.ToLower())
                {
                    return kv.Value;
                }
            }
            return IntPtr.Zero;
        }
        public IntPtr getModule(string module)
        {
            return NativeFunctions.GetModuleHandleA(module);
        }

        public IntPtr getModuleFunction(string module, string func)
        {
            return NativeFunctions.GetProcAddress(NativeFunctions.GetModuleHandleA(module), func);
        }
        public IntPtr getModuleFunction(IntPtr module, string func)
        {
            return NativeFunctions.GetProcAddress(module, func);
        }

        public bool VirtualProtect(IntPtr lpAddress, int dwSize, MemoryProtectionConstants flNewProtect, out MemoryProtectionConstants lpflOldProtect)
        {
            return NativeFunctions.VirtualProtectEx(handle, lpAddress, dwSize, flNewProtect, out lpflOldProtect);
        }
        public IntPtr CreateRemoteThread(IntPtr lpThreadAttributes, int dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, IntPtr lpAttributeList, out int ThreadID)
        {

            IntPtr ThreadHandler = NativeFunctions.CreateRemoteThread(this.handle, lpThreadAttributes, 0, lpStartAddress, lpStartAddress, dwCreationFlags, lpAttributeList, out ThreadID);
            if (ThreadHandler == IntPtr.Zero) new Exception("Failed to create thread");

            return ThreadHandler;
        }
        public WaitForSingleObjectReponse WaitForSingleObject(IntPtr handle, int dwMilliseconds)
        {
            return NativeFunctions.WaitForSingleObjectEx(handle, dwMilliseconds, true);
        }

        public bool VirtualFree(IntPtr lpAddress, int dwSize, FreeType dwFreeTyp)
        {

            return NativeFunctions.VirtualFreeEx(this.handle, lpAddress, dwSize, dwFreeTyp);
        }
    }
}
