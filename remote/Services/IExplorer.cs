using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace remote
{
    public interface IExplorer
    {
        void Close();

        void Show();
        ObservableCollection<string> Files { get; set; }

        double Left { get; set; }
        WindowState WindowState { get; set; }
        double Top { get; set; }
        int SelectedIndex { set;  }
        string CurrentPath { get; set; }

        event EventHandler Closed;

    }
}