using System;
using System.Threading.Tasks;
using System.Windows;

namespace remote
{
    public interface IExplorer
    {
        void MoveUp();

        bool MoveDown();

        void Close();

        void Show();

        double Left { get; set; }
        WindowState WindowState { get; set; }
        double Top { get; set; }
        int SelectedIndex { get; set; }
        string CurrentPath { get; set; }

        event EventHandler Closed;

        void Refresh();
    }
}