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
using System.Windows.Input;
using IoC;
using remote.Annotations;
using Path = System.IO.Path;
using Timer = System.Windows.Forms.Timer;

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
        private string _currentTime;
        public static string CURRENTDIRECTORY = Properties.Settings.Default.currentDirectory;
        public static string CURRENTFILE = Properties.Settings.Default.currentfile;
        public IDispatcher MyDispatcher { get { return IocKernel.GetInstance<IDispatcher>(); } }
        private Timer timer;
        public Explorer()
        {
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
            if (!Directory.Exists(Properties.Settings.Default.currentDirectory))
                CURRENTDIRECTORY = null;
            CurrentPath = CURRENTDIRECTORY ?? ConfigurationManager.AppSettings["defaultPath"];

            if (!File.Exists(Properties.Settings.Default.currentfile))
                CURRENTFILE = null;
            else
            {
                CURRENTFILE = Properties.Settings.Default.currentfile;
            }
            if (CURRENTFILE != null)
                SelectedIndex = new List<string>(Directory.GetFiles(CurrentPath)).IndexOf(CURRENTFILE) + 1 + Directory.GetDirectories(CurrentPath).Count;


            Open(CurrentPath);

            InitializeComponent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToLongTimeString();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            base.OnClosing(e);
        }

        public IDirectory Directory { get { return IocKernel.GetInstance<IDirectory>(); } }
        public IProcess Process { get { return IocKernel.GetInstance<IProcess>(); } }
        bool Open(string currentPath)
        {
            currentPath = currentPath.Replace("\\\\", "\\");
            bool openFile = false;
            bool isDirectory = false;
            try
            {

                var directories = Directory.GetDirectories(currentPath);
                var files = Directory.GetFiles(currentPath);
                isDirectory = true;
                Files = new ObservableCollection<string>();
                Files.Add("..");
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
            catch (Exception e)
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
                    MyDispatcher.Invoke(() =>
                    {

                        openFile = true;
                        p = Process.Start(currentPath);

                        while (p != null && !p.HasExited && p.MainWindowHandle == (IntPtr)0)
                        {
                            Thread.Sleep(10);
                        }
                        //                        Actions.Player.SetFullScreen(p);
                    });
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


        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (value == _currentTime) return;
                _currentTime = value;
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

        public bool MoveDown()
        {
            if (SelectedIndex < Files.Count)
            {
                SelectedIndex++;
                return true;
            }
            return false;
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
                list[0] = list[0] + "\\";
                path = Path.Combine(list.ToArray());
            }
            else
            {
                path = Path.Combine(CURRENTDIRECTORY, Files[SelectedIndex]);
            }
            var result = Open(path);
            if (!result)
                SelectedIndex = 0;
            return result;
        }

        private void ScrollIntoView(object sender, SelectionChangedEventArgs e)
        {
            if (Files.Count > SelectedIndex && SelectedIndex > -1)
                ListView.ScrollIntoView(Files[SelectedIndex]);
        }

        private void ListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Actions.OkButton();
        }
    }
}
