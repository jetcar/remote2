using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using IoC;
using remote.Services;

namespace remote
{
    public class Actions : IActions
    {
        public IDirectory Directory { get { return IocKernel.GetInstance<IDirectory>(); } }
        public IProcess Process { get { return IocKernel.GetInstance<IProcess>(); } }
        public IExplorer Explorer { get; set; }
        public IPlayer Player { get { return IocKernel.GetInstance<IPlayer>(); } }
        public IDispatcher Dispatcher { get { return IocKernel.GetInstance<IDispatcher>(); } }
        static string playerName;
        static int screenIndex;
        public static string CurrentPath;
        private Thread timerThread;
        public Actions()
        {
            timerThread = new Thread(timer_Tick);
            timerThread.IsBackground = true;
            timerThread.Priority = ThreadPriority.Lowest;

            playerName = ConfigurationManager.AppSettings["playerName"];
            screenIndex = Convert.ToInt32(ConfigurationManager.AppSettings["defaultScreenIndex"]);
        }
        object locker = new object();
        private bool skipRequest = false;
        void timer_Tick()
        {
            while (true)
            {

                lock (locker)
                {
                    if (!skipRequest)
                    {
                        PlayerStatus status = Player.GetStatus();
                        if (status == null)
                        {
                            if (Directory.MoveOpenNextIfSameName())
                            {
                                OkButton();
                                Explorer.Close();
                            }
                            else
                            ListButton();
                            
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }


        public void OkButton()
        {
            lock (locker)
            {
                if (Explorer != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        skipRequest = !Directory.OpenSelected();
                        if(!skipRequest)
                            Explorer.Close();
                        if (!timerThread.IsAlive)
                            timerThread.Start();
                    });
                }
                else
                {
                    PlayerStatus status = Player.GetStatus();
                    if (status != null && status.state != PlayerStatus.States.stopped)
                        Player.PlayPause();
                }
            }
        }

        public void Power()
        {
            //throw new NotImplementedException();
        }

        public void UpButton()
        {
            if (Explorer != null)
            {
                Explorer.MoveUp();
            }
            else
            {
                lock (locker)
                {
                    Player.VolUp();
                }
            }
        }

        public void DownButton()
        {
            if (Explorer != null)
            {
                Explorer.MoveDown();
            }
            else
            {
                lock (locker)
                {
                    Player.VolDown();
                }
            }
        }

        public void LeftButton()
        {
            lock (locker)
            {
                Player.Backward();
            }
        }

        public void RightButton()
        {
            lock (locker)
            {
                Player.Forward();
            }
        }

        public void ExitButton()
        {
            skipRequest = true;
            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
                while (Process.GetProcessesByName(playerName).FirstOrDefault() != null)
                {
                    Thread.Sleep(1);
                }
                if (Explorer == null)
                    ListButton();
                return;
            }
            if (Explorer != null)
            {
                Dispatcher.Invoke(() =>
                {
                    Explorer.Close();
                });
            }
        }

        private bool opening = false;

        public void ListButton()
        {
            if(opening)
                return;
            opening = true;
            skipRequest = true;
            if (!timerThread.IsAlive)
                timerThread.Start();

            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);

            }
            Dispatcher.Invoke(() =>
            {
                if (Explorer != null)
                    Explorer.Close();
                Explorer = new Explorer();
                Explorer.Show();
                Explorer.Closed += explorer_Closed;
                Screen[] screens = Screen.AllScreens;
                var x = screens[screenIndex].WorkingArea.X;
                var y = screens[screenIndex].WorkingArea.Y;
                Explorer.Left = x;
                Explorer.Top = y;
                Explorer.WindowState = WindowState.Minimized;
                Explorer.WindowState = WindowState.Maximized;
            });
            opening = false;


        }

        void explorer_Closed(object sender, EventArgs e)
        {
            Explorer = null;
        }

        public void NextButton()
        {
            if (Explorer != null)
                return;
            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
            }

            var dirs = new List<string>(Directory.GetDirectories(Directory.CURRENTDIRECTORY));
            var files = new List<string>(Directory.GetFiles(Directory.CURRENTDIRECTORY));
            for (int i = dirs.Count - 1; i >= 0; i--)
            {
                files.Insert(0, dirs[i]);
            }
            var index = new List<string>(Directory.GetFiles(Directory.CURRENTDIRECTORY)).IndexOf(Directory.CURRENTFILE) + 1 + Directory.GetDirectories(Directory.CURRENTDIRECTORY).Count;
            if (index < files.Count - 1)
            {
                index++;
            }
            Directory.CURRENTFILE = files[index];
            var extension = Path.GetExtension(Directory.CURRENTFILE);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {

                p = Process.Start(Directory.CURRENTFILE);

                while (!p.HasExited && p.MainWindowHandle == (IntPtr)0)
                {
                    Thread.Sleep(10);
                }

                lock (locker)
                {
                    // Player.SetFullScreen(p);
                }
            }
        }


        public void PreviousButton()
        {
            if (Explorer != null)
                return;

            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);

            }

            var files = new List<string>(Directory.GetFiles(Directory.CURRENTDIRECTORY));
            var index = files.IndexOf(Directory.CURRENTFILE);
            if (index > 0)
            {
                index--;
            }
            Directory.CURRENTFILE = files[index];
            var extension = Path.GetExtension(Directory.CURRENTFILE);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                p = Process.Start(Directory.CURRENTFILE);

                while (!p.HasExited && p.MainWindowHandle == (IntPtr)0)
                {
                    Thread.Sleep(10);
                }
                lock (locker)
                {
                    //Player.SetFullScreen(p);
                }

            }
        }

        public void VolUpButton()
        {
            lock (locker)
            {
                Player.VolUp();
            }
        }

        public void VolDownButton()
        {
            lock (locker)
            {
                Player.VolDown();
            }
        }


    }
}
