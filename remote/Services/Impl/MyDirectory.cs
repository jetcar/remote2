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
            if (Compare(nextFilename, currentFile) < 4)
                return nextFilename;
            return null;
        }

        public static int Compare(string nextFilename, string currentFile)
        {
            int counter = 0;
            if (Math.Abs(nextFilename.Length - currentFile.Length) > 4)
                return nextFilename.Length - currentFile.Length;
            for (int i = 0; i < Math.Min(nextFilename.Length,currentFile.Length); i++)
            {
                if(nextFilename[i] == currentFile[i])
                {
                    counter++;
                }
            }
            return Math.Min(nextFilename.Length, currentFile.Length) - counter;
        }
    }
}
