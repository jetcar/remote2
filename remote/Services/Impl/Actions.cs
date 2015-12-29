﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using IoC;
using remote.Services;
using Timer = System.Windows.Forms.Timer;

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
        public Actions()
        {
            var thread = new Thread(timer_Tick);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
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
                        if (status == null && Explorer == null)
                        {
                            var nextFilename = Directory.NextFileIsFromList(remote.Explorer.CURRENTDIRECTORY, remote.Explorer.CURRENTFILE);
                            if (nextFilename != null)
                                NextButton();
                            else
                                ListButton();
                        }
                        else if (status != null && (status.state == PlayerStatus.States.stopped && Explorer == null))
                        {
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
                        skipRequest = !Explorer.OpenSelected();
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
        }

        public void DownButton()
        {
            if (Explorer != null)
            {
                Explorer.MoveDown();
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
            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
            }
            if (Explorer != null)
                Explorer.Close();
        }

        public void ListButton()
        {
            skipRequest = true;

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

            var files = new List<string>(Directory.GetFiles(remote.Explorer.CURRENTDIRECTORY));
            var index = new List<string>(Directory.GetFiles(remote.Explorer.CURRENTDIRECTORY)).IndexOf(remote.Explorer.CURRENTFILE) + 1 + Directory.GetDirectories(remote.Explorer.CURRENTDIRECTORY).Count;
            if (index < files.Count - 1)
            {
                index++;
            }
            remote.Explorer.CURRENTFILE = files[index];
            var extension = Path.GetExtension(remote.Explorer.CURRENTFILE);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                Process.Start(remote.Explorer.CURRENTFILE);
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

                lock (locker)
                {
                    Player.SetFullScreen();
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

            var files = new List<string>(Directory.GetFiles(remote.Explorer.CURRENTDIRECTORY));
            var index = files.IndexOf(remote.Explorer.CURRENTFILE);
            if (index > 0)
            {
                index--;
            }
            remote.Explorer.CURRENTFILE = files[index];
            var extension = Path.GetExtension(remote.Explorer.CURRENTFILE);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                Process.Start(remote.Explorer.CURRENTFILE);
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
                lock (locker)
                {
                    Player.SetFullScreen();
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
