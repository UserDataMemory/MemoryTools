using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Il2cpp
{
    [AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
    public class Il2cppAttribute : Attribute
    {
        public string Assembly;
        public string NameSpace;
        public string Class;
        public string Method;
        public string Arguments;

        public Il2cppAttribute(string Assembly,string NameSpace, string Class, string Method, string Arguments)
        {
            this.Assembly = Assembly;
            this.NameSpace = NameSpace;
            this.Class = Class;
            this.Method = Method;
            this.Arguments = Arguments;
        }
        public Il2cppAttribute()
        {
        }

    }
}
