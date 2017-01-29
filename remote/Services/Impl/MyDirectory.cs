using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace remote
{
    public class MyDirectory : IDirectory
    {
        public ObservableCollection<string> OpenDirectory(string currentPath)
        {
            var files = LoadFiles(currentPath);
            Files = files;
            CURRENTDIRECTORY = currentPath;
            Properties.Settings.Default.currentDirectory = CURRENTDIRECTORY;
            Task.Run(() =>
            {
                Properties.Settings.Default.Save();

            });

            return files;
        }

        public string CURRENTDIRECTORY
        {
            get { return Properties.Settings.Default.currentDirectory; }
            set { Properties.Settings.Default.currentDirectory = value; }
        }
        public string CURRENTFILE
        {
            get { return Properties.Settings.Default.currentfile; }
            set { Properties.Settings.Default.currentfile = value; }
        }


        public string CurrentPath { get; set; }

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

        public int SelectedIndex { get; set; }


        public bool OpenSelected()
        {
            if (SelectedIndex < 0)
                return false;
            string path;
            if (SelectedIndex >= Files.Count)
                SelectedIndex = Files.Count - 1;
            if (Files[SelectedIndex] == "..")
            {
                var folders = CURRENTDIRECTORY.Split(Path.DirectorySeparatorChar);
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

        public ObservableCollection<string> Files { get; set; }

        public bool MoveOpenNextIfSameName()
        {
            if (SelectedIndex < Files.Count-1)
            {
                if (Compare(Files[SelectedIndex], Files[SelectedIndex + 1]) < 4)
                {
                    SelectedIndex++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool Open(string currentPath)
        {
            currentPath = currentPath.Replace("\\\\", "\\");
            bool openFile = false;
            bool isDirectory = false;
            try
            {

                Files = LoadFiles(currentPath);
                isDirectory = true;
                SelectedIndex = 0;
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
                   

                        openFile = true;
                        p = Process.Start(currentPath);

                        while (p != null && !p.HasExited && p.MainWindowHandle == (IntPtr)0)
                        {
                            Thread.Sleep(10);
                        }
                        //                        Actions.Player.SetFullScreen(p);
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

        private ObservableCollection<string> LoadFiles(string currentPath)
        {
            var directories = Directory.GetDirectories(currentPath);
            var files = Directory.GetFiles(currentPath);
            var Files = new ObservableCollection<string>();
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
            return Files;
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

        public bool OpenFile(string currentPath)
        {
            throw new NotImplementedException();
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
