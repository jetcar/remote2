using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remote
{
    public class MyDirectory : IDirectory
    {
        public bool Exists(string currentDirectory)
        {
            return Directory.Exists(currentDirectory);
        }

        public IList<string> GetFiles(string currentPath)
        {
            return Directory.GetFiles(currentPath);

        }

        public IList<string> GetDirectories(string currentPath)
        {
            return Directory.GetDirectories(currentPath);

        }
    }
}
