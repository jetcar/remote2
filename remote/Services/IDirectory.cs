﻿using System.Collections.Generic;

namespace remote
{
    public interface IDirectory
    {
        bool Exists(string currentDirectory);
        IList<string> GetFiles(string currentPath);
        IList<string> GetDirectories(string currentPath);
        string NextFileIsFromList(string folder, string currentFile);
    }
}