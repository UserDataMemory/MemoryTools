using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Remote
{
   public abstract class CallingConvention
    {
        protected StringBuilder code;

        protected int parameters;
        protected int beginningOffset = 0;

        public abstract CallingConventions CalliingConvention
        {
            get;
        }
     
        protected CallingConvention(StringBuilder builder,int parameters)
        {
            this.code = builder;
            this.parameters = parameters;
        }
        public void SetBeginningOffset(int offset)
        {
            this.beginningOffset = offset;
        }
        public abstract void FormatArguments();
        public abstract void FormatCall(IntPtr address);

        public abstract IntPtr[] GetArguments(IntPtr address);
        public abstract int Size();
    }
}
