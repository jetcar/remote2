using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using IoC;
using remote.Annotations;
using Path = System.IO.Path;

namespace remote
{
    /// <summary>
    /// Interaction logic for Explorer.xaml
    /// </summary>
    public partial class Explorer : INotifyPropertyChanged, IExplorer
    {
        private string _currentPath;
        private ObservableCollection<string> _files = new ObservableCollection<string>();
        private int _selectedIndex;
        public static string CURRENTDIRECTORY = Properties.Settings.Default.currentDirectory;
        public static string CURRENTFILE = Properties.Settings.Default.currentfile;
        public Explorer()
        {

            if (!Directory.Exists(Properties.Settings.Default.currentDirectory))
                CURRENTDIRECTORY = null;
            CurrentPath = CURRENTDIRECTORY ?? ConfigurationManager.AppSettings["defaultPath"];

            if (!File.Exists(Properties.Settings.Default.currentfile))
                CURRENTFILE = null;
            if (CURRENTFILE != null)
                SelectedIndex = new List<string>(Directory.GetFiles(CurrentPath)).IndexOf(CURRENTFILE) + 1 + Directory.GetDirectories(CurrentPath).Count;


            Open(CurrentPath);

            InitializeComponent();
        }

        public IDirectory Directory { get { return IocKernel.GetInstance<IDirectory>(); } }
        public IProcess Process { get { return IocKernel.GetInstance<IProcess>(); } }
        bool Open(string currentPath)
        {
            bool openFile = false;
            bool isDirectory = false;
            try
            {
                var directories = Directory.GetDirectories(currentPath);
                var files = Directory.GetFiles(currentPath);
                isDirectory = true;
                Files = new ObservableCollection<string>();
                Files.Add("..");
                SelectedIndex = 0;
                foreach (var dir in directories)
                {
                    var folders = dir.Split(Path.DirectorySeparatorChar);
                    Files.Add(folders.Last());
                }
                foreach (var file in files)
                {
                    var folders = file.Split(Path.DirectorySeparatorChar);

                    Files.Add(folders.Last());
                }
                CURRENTDIRECTORY = currentPath;
                Properties.Settings.Default.currentDirectory = CURRENTDIRECTORY;
                Task.Run(() =>
                {
                    Properties.Settings.Default.Save();

                });

            }
            catch (Exception)
            {

            }
            if (!isDirectory)
            {
                var extension = Path.GetExtension(currentPath);
                if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
                {
                    var player = ConfigurationManager.AppSettings["playerName"];

                    Process p = Process.GetProcessesByName(player).FirstOrDefault();
                    if (p != null)
                    {
                        p.Kill();
                    }

                    Process.Start(currentPath);
                    openFile = true;
                    var playerName = ConfigurationManager.AppSettings["playerName"];
                    p = Process.GetProcessesByName(playerName).FirstOrDefault();
                    while (p == null)
                    {
                        Thread.Sleep(10);
                        p = Process.GetProcessesByName(playerName).FirstOrDefault();
                    }
                    while (!p.HasExited && p.MainWindowHandle == (IntPtr)0)
                    {
                        Thread.Sleep(10);
                    }

                    Actions.Player.SetFullScreen();

                    this.Close();
                }
                CURRENTFILE = currentPath;
                Properties.Settings.Default.currentfile = CURRENTFILE;
                Task.Run(() =>
                {
                    Properties.Settings.Default.Save();

                });
            }
            CurrentPath = CURRENTDIRECTORY;
            return openFile;
        }
        IActions Actions { get { return IoC.IocKernel.GetInstance<IActions>(); } }

        public ObservableCollection<string> Files
        {
            get { return _files; }
            set
            {
                if (Equals(value, _files)) return;
                _files = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value == _selectedIndex) return;
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }



        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                if (value == _currentPath) return;
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MoveUp()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
        }

        public void MoveDown()
        {
            if (SelectedIndex < Files.Count)
                SelectedIndex++;
        }


        public bool OpenSelected()
        {
            if (SelectedIndex < 0)
                return false;
            string path;
            if (SelectedIndex >= Files.Count)
                SelectedIndex = Files.Count - 1;
            if (Files[SelectedIndex] == "..")
            {
                var folders = CurrentPath.Split(Path.DirectorySeparatorChar);
                var list = new List<string>(folders);
                if (list.Count > 1)
                    list.RemoveAt(list.Count - 1);
                path = Path.Combine(list.ToArray());
            }
            else
            {
                path = Path.Combine(CURRENTDIRECTORY, Files[SelectedIndex]);
            }
            return Open(path);
        }

        private void ScrollIntoView(object sender, SelectionChangedEventArgs e)
        {
            if (Files.Count > SelectedIndex && SelectedIndex > -1)
                ListView.ScrollIntoView(Files[SelectedIndex]);
        }
    }
}
