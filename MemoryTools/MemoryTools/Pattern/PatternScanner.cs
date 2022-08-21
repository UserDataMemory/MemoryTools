using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Pattern
{
    public class PatternScanner
    {
        private MemoryManager memoryManager;
        private MemoryQuery memoryQuery;
        private int alignment = 4;


        public int Alignment
        {
            get => this.alignment;
            set => this.alignment = value;
        }

        public MemoryQuery Query
        {
            get => this.memoryQuery;
        }
        public PatternScanner(int pid)
        {
            memoryManager = new MemoryManager();
            memoryQuery = new MemoryQuery(memoryManager);
            memoryManager.Attach(pid);
        }


        public List<IntPtr> Scan(string aob)
        {
            string[] subs = aob.Split(' ');
            bool[] mask = new bool[subs.Length];
            byte[] pattern = subs.Select((c, i) => c == "??" && (mask[i] = true) ? (byte)0 : Convert.ToByte(c, 16)).ToArray();

            return Scan(pattern, mask);
        }

        public List<IntPtr> Scan(byte[] pattern, bool[] mask)
        {
            Dictionary<IntPtr, byte[]> mem = memoryQuery.GetMemory();
            List<IntPtr> results = new List<IntPtr>();
            foreach (KeyValuePair<IntPtr, byte[]> v in mem)
            {
              
                    byte[] bytes = v.Value;
                    for (int i = 0; i < bytes.Length; i += alignment)
                    {
                        if (bytes[i] == pattern[0])
                        {
                            int e = 1;
                            while (i++ < bytes.Length - 1 && (bytes[i] == pattern[e] || mask[e]))
                            {
                                if (++e == pattern.Length)
                                {
                                    results.Add(v.Key + i - e + 1);
                                    i += alignment == 1 ? 1 : (i % alignment);
                                    break;
                                }
                            }
                            i -= e;
                        }
                    }
                
            }
            return results;
        }
        public void Write(List<IntPtr> list, byte[] pattern)
        {
            for(int i = 0; i < list.Count; i++)
            {
                memoryManager.WriteBytes(list[i], pattern); 
            }
        }
        public void Write(List<IntPtr> list,string aob)
        {
            string[] subs = aob.Split(' ');
            byte[] pattern = subs.Select((c, i) =>  Convert.ToByte(c, 16)).ToArray();
            Write(list, pattern);
        }
    }
}
