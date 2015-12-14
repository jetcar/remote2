using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using IoC;
using remote.Services;

namespace remote
{
    public class Actions
    {
        public static IDirectory Directory { get { return IocKernel.GetInstance<IDirectory>(); } }
        public static IProcess Process { get { return IocKernel.GetInstance<IProcess>(); } }
        public static IExplorer Explorer { get; set; }
        public static IPlayer Player { get { return IocKernel.GetInstance<IPlayer>(); } }
        private static Timer timer;
        static string playerName;
        static int screenIndex;
        public static string CurrentPath;
        static Actions()
        {
            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += timer_Tick;
            timer.Start();
            playerName = ConfigurationManager.AppSettings["playerName"];
            screenIndex = Convert.ToInt32(ConfigurationManager.AppSettings["defaultScreenIndex"]);
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            PlayerStatus status = Player.GetStatus();
            if(status == null)
                timer.Stop();
        }


        public static void OkButton()
        {
            PlayerStatus status = Player.GetStatus();
            if(status.state != PlayerStatus.States.stopped)
                Player.PlayPause();

            else if(Explorer != null)
                Explorer.OpenSelected();
        }

        public static void Power()
        {
            //throw new NotImplementedException();
        }

        public static void UpButton()
        {
            if (Explorer != null) Explorer.MoveUp();
        }

        public static void DownButton()
        {
            if (Explorer != null) Explorer.MoveDown();
        }

        public static void LeftButton()
        {
            Player.Backward();
        }

        public static void RightButton()
        {
            Player.Forward();
        }

        public static void ExitButton()
        {
            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
            }
            if(Explorer != null)
                Explorer.Close();
        }

        public static void ListButton()
        {

            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                Process.Kill(p);
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

        static void explorer_Closed(object sender, EventArgs e)
        {
            Explorer = null;
        }

        public static void NextButton()
        {
            if(Explorer != null)
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
                Player.SetFullScreen();
            }
        }

        public static void PreviousButton()
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
                Player.SetFullScreen();
            }
        }

        public static void VolUpButton()
        {
            Player.VolUp();
        }

        public static void VolDownButton()
        {
            Player.VolDown();
        }

        public static void OptionsButton()
        {
            //SendKey("{DOWN}");
        }

    }
}
