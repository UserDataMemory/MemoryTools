using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryTools.Remote;
using System.Reflection;

namespace MemoryTools.Il2cpp
{
    public class Il2cppMethod
    {
        private IntPtr handle;
        private uint paramsCount;
        private string[] paramsType;
        private string name;
        private uint flags;
        private IntPtr address;
        private RemoteFunction remoteCall;
        private RemoteFunction remoteHookedCall;

        public string[] ParameterType => paramsType;
        public uint ParameterCount => paramsCount;
        public string Name => this.name;

        public bool Static => (flags & (uint)Il2CppType.METHOD_ATTRIBUTE_STATIC) != 0;

        public string DescName => $"{this.name}({String.Join(",", paramsType)})";

        public IntPtr Address => this.address;
        public RemoteFunction Function => this.remoteCall;
        public Il2cppMethod(IntPtr handle)
        {

            this.handle = handle;
            Initialize();
        }
        private void Initialize()
        {
            this.name = Il2cppNative.Current.method_get_name(this.handle);
            this.paramsCount = Il2cppNative.Current.method_get_param_count(this.handle);
            this.paramsType = new string[paramsCount];
            this.flags = Il2cppNative.Current.method_get_flags(this.handle, IntPtr.Zero);
            this.address = Il2cppNative.Current.method_get_address(this.handle);

            for (int i = 0; i < paramsCount; i++)
            {
                paramsType[i] = Il2cppNative.Current.type_get_name(Il2cppNative.Current.method_get_param(this.handle, i));
            }
           // Console.WriteLine(DescName);
            
        }
        private void CreateCall(object[] args)
        {
            int argsCount = 0;
           
            for (int i = 0; i < args.Length; i++)
            {
                Type argType = args[i].GetType();
                if (Attribute.IsDefined(argType, typeof(MemoryAttribute)) && Il2cppNative.Current.Memory.isStruct(argType) && ((MemoryAttribute)Attribute.GetCustomAttribute(argType,typeof(MemoryAttribute))).Argument)
                {
                    argsCount += Il2cppNative.Current.Memory.GetStructFields(args[i]).Count;
                    continue;
                }
                argsCount++;
            }
            remoteCall = Il2cppNative.Current.Memory.CallManager.Create(this.address, argsCount, Remote.CallingConventions.cdecl, Il2cppNative.Current.Thread);
        }
        private void CreateHookedCall(object[] args)
        {
            if (Il2cppNative.Current.Hook == null) throw new Exception("RemoteHook is null, you need to set with Il2cpp.setHook(IntPtr address,int size)");
            int argsCount = 0;

            for (int i = 0; i < args.Length; i++)
            {
                Type argType = args[i].GetType();
                if (Attribute.IsDefined(argType, typeof(MemoryAttribute)) && Il2cppNative.Current.Memory.isStruct(argType) && ((MemoryAttribute)Attribute.GetCustomAttribute(argType, typeof(MemoryAttribute))).Argument)
                {
                    argsCount += Il2cppNative.Current.Memory.GetStructFields(args[i]).Count;
                    continue;
                }
                argsCount++;
            }

            remoteHookedCall = Il2cppNative.Current.Memory.CallManager.Create(this.address, argsCount, Remote.CallingConventions.cdecl, Il2cppNative.Current.Hook);
        }

        public T InvokeinMainThread<T>(params object[] args)
        {
         
            if (remoteHookedCall == null) CreateHookedCall(args);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].GetType() == typeof(string))
                    args[i] = Il2cppNative.Current.string_new((string)args[i]);
            }
           // Console.WriteLine($"CALL: {Name} {remoteHookedCall.Address.ToString("X")}");
            return this.remoteHookedCall.Invoke<T>(args);
        }
        public T Invoke<T>(params object[] args)
        {
           // Console.WriteLine(DescName);
            if (remoteCall == null) CreateCall(args);
           // Console.WriteLine(remoteCall.address.ToString("X"));
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].GetType() == typeof(string))
                    args[i] = Il2cppNative.Current.string_new((string)args[i]);
            }
            return this.remoteCall.Invoke<T>(args);
        }
        public static object InvokeObject<T>(object instance, params object[] p)
        {
           MethodInfo info = typeof(Il2cppMethod).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
           return info.MakeGenericMethod(typeof(T)).Invoke(instance, p);
        }
    }
}
