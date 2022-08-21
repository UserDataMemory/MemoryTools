using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTools.Il2cpp
{
    public class Il2cppImage
    {
        private IntPtr handle;
        private string name;
        private IntPtr entryPoint;
        private string filename;
        private List<Il2cppNameSpace> nameSpaces = new List<Il2cppNameSpace>();
        public string Name => this.name;
        public IntPtr EntryPoint => this.entryPoint;
        public string FileName => this.filename;
        public IntPtr Handle => this.handle;

        public Il2cppNameSpace GetNameSpace(string name_space)
        {
            return nameSpaces.FirstOrDefault(n => n.Name == name_space);
        }
        public Il2cppImage(IntPtr handle)
        {
            this.handle = handle;
            Initialize();
            
        }
        private void Initialize()
        {
           
            this.name = Il2cppNative.Current.image_get_name(this.handle);
       //     Console.WriteLine($"get name image {name}");
            this.entryPoint = Il2cppNative.Current.image_get_entry_point(this.handle);
            //Console.WriteLine($"get entry point");
            this.filename = Il2cppNative.Current.image_get_filename(this.handle);
           // Console.WriteLine($"get filename");
            InitializerNameSpaces();
        }

        private void InitializerNameSpaces()
        {
            List<Il2cppClass> classes = GetClasses();
            Dictionary<string, List<Il2cppClass>> NameSpaces = new Dictionary<string, List<Il2cppClass>>();

            for (int i = 0; i < classes.Count; i++)
            {
                if (!NameSpaces.ContainsKey(classes[i].NameSpace))
                {
                    NameSpaces.Add(classes[i].NameSpace, new List<Il2cppClass>());
                }
                NameSpaces[classes[i].NameSpace].Add(classes[i]);
            }
            foreach(var kv in NameSpaces)
            {
               nameSpaces.Add(new Il2cppNameSpace(this.handle, kv.Value, kv.Key));
            }

        }
        public List<Il2cppClass> GetClasses()
        {

            int count = Il2cppNative.Current.image_get_class_count(this.handle);
            List<Il2cppClass> classes = new List<Il2cppClass>(count);
            for (int i = 0; i < count; i++)
            {
                classes.Add(new Il2cppClass(Il2cppNative.Current.image_get_class(this.handle, i)));
            }
            return classes;
        }




    }
}
