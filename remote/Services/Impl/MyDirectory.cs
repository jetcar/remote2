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
        public bool FileExists(string currentDirectory)
        {
            return File.Exists(currentDirectory);
        }

        public IList<string> GetFiles(string currentPath)
        {
            return Directory.GetFiles(currentPath);

        }

        public IList<string> GetDirectories(string currentPath)
        {
            return Directory.GetDirectories(currentPath);

        }

        public string NextFileIsFromList(string folder, string currentFile)
        {
            if (folder == null)
                return null;
            if (currentFile == null)
                return null;
            var files = new List<string>(Directory.GetFiles(folder));
            int currentIndex = files.IndexOf(currentFile);
            if (currentIndex >= files.Count - 1)
                return null;
            var nextFilename = files[currentIndex + 1];
            if (currentFile.CompareTo(nextFilename) == -1 || currentFile.CompareTo(nextFilename) == 1)
                return nextFilename;
            return null;
        }
    }
}
