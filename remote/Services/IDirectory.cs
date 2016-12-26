using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace remote
{
    public interface IDirectory
    {
        bool FileExists(string currentDirectory);
        bool Exists(string currentDirectory);
        IList<string> GetFiles(string currentPath);
        IList<string> GetDirectories(string currentPath);
        string NextFileIsFromList(string folder, string currentFile);
        bool OpenFile(string currentPath);
        ObservableCollection<string> OpenDirectory(string currentPath);
        string CURRENTDIRECTORY { get; set; }
        string CURRENTFILE { get; set; }
        int SelectedIndex { get; set; }
        bool OpenSelected();
        bool MoveOpenNextIfSameName();
    }
}