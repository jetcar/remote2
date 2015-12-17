using System;
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
        private Timer timer;
        static string playerName;
        static int screenIndex;
        public static string CurrentPath;
        public Actions()
        {
            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += timer_Tick;
            timer.Start();
            playerName = ConfigurationManager.AppSettings["playerName"];
            screenIndex = Convert.ToInt32(ConfigurationManager.AppSettings["defaultScreenIndex"]);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            PlayerStatus status = Player.GetStatus();
            if (status == null)
                timer.Stop();
        }


        public void OkButton()
        {
            if (Explorer != null)
                Explorer.OpenSelected();
            else
            {
                PlayerStatus status = Player.GetStatus();
                if (status != null && status.state != PlayerStatus.States.stopped)
                    Player.PlayPause();
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
                timer.Stop();
                Explorer.MoveUp();
                timer.Start();
            }
        }

        public void DownButton()
        {
            if (Explorer != null)
            {
                timer.Stop();
                Explorer.MoveDown();
                timer.Start();
            }
        }

        public void LeftButton()
        {
            timer.Stop();
            Player.Backward();
            timer.Start();
        }

        public void RightButton()
        {
            timer.Stop();
            Player.Forward();
            timer.Start();
        }

        public void ExitButton()
        {
            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
                timer.Stop();

            }
            if (Explorer != null)
                Explorer.Close();
        }

        public void ListButton()
        {

            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
                timer.Stop();

            }
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
                timer.Stop();
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
                while (p.MainWindowHandle == (IntPtr) 0)
                {
                    Thread.Sleep(10);
                }

                Player.SetFullScreen();
                timer.Start();
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
                timer.Stop();

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
                while (p.MainWindowHandle == (IntPtr)0)
                {
                    Thread.Sleep(10);
                }
                Player.SetFullScreen();
                timer.Start();

            }
        }

        public void VolUpButton()
        {
            Player.VolUp();
        }

        public void VolDownButton()
        {
            Player.VolDown();
        }

        public void OptionsButton()
        {
            //SendKey("{DOWN}");
        }

    }

    public interface IActions
    {
        void Power();
        void OkButton();
        void UpButton();
        void DownButton();
        void LeftButton();
        void RightButton();
        void ExitButton();
        void ListButton();
        void NextButton();
        void PreviousButton();
        void VolUpButton();
        void VolDownButton();
        void OptionsButton();
        IPlayer Player { get; }
        IExplorer Explorer { get; set; }
    }
}
