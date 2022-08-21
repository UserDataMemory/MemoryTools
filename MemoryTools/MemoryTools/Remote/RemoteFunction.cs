using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryTools.Native;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using MemoryTools.Remote.Hook;
using System.Runtime.InteropServices;

namespace MemoryTools.Remote
{
    public class RemoteFunction
    {
        public IntPtr address;
        public IntPtr ret;
        private int parameters;
        private MemoryManager memoryManager;
        private CallingConvention callingConvention;
        private int size;
        private RemoteCallType calltype;
        private double timeout = 60000;
        public double Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }


        public IntPtr Address
        {
            get { return this.address; }
        }

        public RemoteFunction(IntPtr address, int size, int parameters, RemoteCallType calltype, MemoryManager memoryManager, CallingConvention callingConvention)
        {
            this.address = address;
            this.ret = address + size;
            this.parameters = parameters;
            this.memoryManager = memoryManager;
            this.callingConvention = callingConvention;
            this.size = size;
            this.calltype = calltype;
        }

        /* public object[] InvokeSequence(object[][] args, params Type[] returns)
         {

             CallInfo methodInfo = GetMethodInfo();
             List<object> results = new List<object>();
             for (int i = 0; i < methodInfo.calls.Count(); i++)
             {
                 methodInfo.calls[i].ChangeArguments(args[i]);
             }

             MethodInfo method = GetType().GetMethod("InvokeMethod");

             method.MakeGenericMethod(returns[0]).Invoke(this,args[0]);



         }
         */

        public object[] FormatArguments(object[] args)
        {
            List<object> obj = new List<object>();
            for (int i = 0; i < args.Length; i++)
            {
                if (memoryManager.isStruct(args[i].GetType()))
                {
                    obj.AddRange(memoryManager.GetStructFields(args[i]));
                }
                else
                {
                    obj.Add(args[i]);
                }
            }
            return obj.ToArray();
        }
        public T Invoke<T>(params object[] args)
        {

            ChangeArguments(FormatArguments(args));
            return Invoke<T>();
        }


        public void ChangeArguments(params object[] args)
        {
            if (this.parameters != args.Length) throw new ArgumentOutOfRangeException($"this function does not accept {args.Length} arguments. number of arguments: {this.parameters}");
            IntPtr[] argumentsAddress = callingConvention.GetArguments(this.address);
            for (int i = 0; i < argumentsAddress.Length; i++)
            {
                memoryManager.WriteMemory(argumentsAddress[i], 0);
                if (args[i].GetType() == typeof(string))
                {
                    memoryManager.WriteMemory(argumentsAddress[i], memoryManager.CreateRemoteString((string)args[i]));
                }

                else
                {
                    memoryManager.WriteMemory(argumentsAddress[i], args[i]);
                }
            }
        }

        public T Invoke<T>()
        {
            
            if (calltype is RemoteThread)
            {
                RemoteThread thread = (RemoteThread)calltype;
                thread.changeBranch(this.address);
                thread.Resume();
                ThreadWait threadWait = thread.WaitThread(timeout);
                if (threadWait == ThreadWait.TIMEOUT) throw new TimeoutException("Thread Execution Timeout");
                return typeof(T) == typeof(Null) ? default(T) : memoryManager.ReadMemory<T>(this.ret);
            }
            if(calltype is RemoteHook)
            {
                RemoteHook hook = (RemoteHook)calltype;
               /// Debugger.Break();
                hook.changeBranch(this.address);
                return memoryManager.IntPtrToObject<T>(hook.WaitReturnValue().lParam);
            }
            return default(T);

        }
    }

}
