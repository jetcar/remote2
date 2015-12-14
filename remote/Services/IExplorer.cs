using System;
using System.Windows;

namespace remote
{
    public interface IExplorer
    {
        void OpenSelected();
        void MoveUp();
        void MoveDown();
        void Close();
        void Show();
        double Left { get; set; }
        WindowState WindowState { get; set; }
        double Top { get; set; }
        int SelectedIndex { get; set; }
        event EventHandler Closed;
    }
}