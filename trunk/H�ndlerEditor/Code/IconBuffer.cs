using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HändlerEditor.Code
{
    public static class IconBuffer
    {
        public static void Initialize(string iconPath)
        {
            DateTime start = DateTime.Now;
            Icons = new ConcurrentDictionary<string, FiestaIconFile>();

            string[] icons = Directory.GetFiles(iconPath);
            Task last = null;
            foreach(string path in icons)
            {

                string[] folders = path.Split('\\');
                string name = folders[folders.Length - 1].Split('.')[0];
                name = name.ToLower();

                var file = new FiestaIconFile(path);
                Icons.TryAdd(name, file);
            }
            
            Trace.WriteLine(string.Format("Icon buffer initialized in {0}ms",
                                          (DateTime.Now.Subtract(start)).TotalMilliseconds));
        }
        public static ConcurrentDictionary<string, FiestaIconFile> Icons;
    }
}
