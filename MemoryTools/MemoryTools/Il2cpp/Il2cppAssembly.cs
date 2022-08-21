using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Il2cpp
{
    public class Il2cppAssembly
    {
        private IntPtr handle;
     //   private IntPtr domain;
        private Il2cppImage image;
        private string name;
        public Il2cppImage Image => this.image;
        public string Name => this.name;

        public Il2cppAssembly(IntPtr handle)
        {
            this.handle = handle;
            Initialize();
        }


        private void Initialize()
        {
         
            this.image = new Il2cppImage(Il2cppNative.Current.assembly_get_image(this.handle));
            this.name = image.Name;
            //Console.WriteLine($"initialize image: {name}");
        }

        public bool Open()
        {
            return false;
        }
       
    }
}
