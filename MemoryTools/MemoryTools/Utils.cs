using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools
{
    public static class Utils
    {
        public static string Join<T>(T[] arr)
        {
            return String.Join(",", arr.Select(e => e.ToString()).ToArray());
        }
    }
}
