using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoOrderSender
{
    class Program
    {
        static void Main(string[] args)
        {
            string dicPath = @"D:\others\亿乐豪-干果-site";
            var root = new DirectoryInfo(dicPath);
            foreach (DirectoryInfo nextFolder in root.GetDirectories())
            {
                foreach (FileInfo file in nextFolder.GetFiles())
                {
                    var path = file.DirectoryName;
                    var name = file.Name;
                    Console.WriteLine(string.Concat(path,"\\",name));
                    var opath = string.Concat(path, "\\", name);
                    var npath = string.Concat(path, "\\thumb_", name);
                    //ImageUtil2.MakeThumbnail(opath, npath, 600, 0, "W");
                    if (!name.StartsWith("thumb_"))
                    {
                        file.Delete();
                    }
                }
            }
            Console.ReadLine();
        }


    }
}
