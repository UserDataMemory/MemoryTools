using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Il2cpp
{
    public class Il2cppNameSpace
    {
        private IntPtr handle;
        private string name;
        private List<Il2cppClass> il2CppClasses;
        public string Name => this.name;


        public Il2cppNameSpace(IntPtr handle, List<Il2cppClass> classes, string name)
        {
            this.handle = handle;
            this.il2CppClasses = classes;
            this.name = name;
        }
     
        public Il2cppClass GetClass(string _class)
        {
            Il2cppClass il2CppClass = il2CppClasses.FirstOrDefault(c => c.Name == _class); 
            if(il2CppClass == default(Il2cppClass)) throw new Exception($"Class ({_class}) not found in namespace ({this.Name})");

            return il2CppClass;
        }


    }
}
