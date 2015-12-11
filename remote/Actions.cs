using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace remote
{
    public class Actions
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        private static Explorer explorer = null;
        static string player;
        static int screenIndex;
        static string defaultPath;
        public static string CurrentPath;
        static Actions()
        {
            player = ConfigurationManager.AppSettings["playerName"];
            screenIndex = Convert.ToInt32(ConfigurationManager.AppSettings["defaultScreenIndex"]);
            defaultPath = ConfigurationManager.AppSettings["defaultPath"];
        }


        public static void OkButton()
        {
            SendKey(" ");
            if(explorer != null)
                explorer.OpenSelected();
        }

        public static void Power()
        {
            //throw new NotImplementedException();
        }

        public static void UpButton()
        {
            if (explorer != null) explorer.MoveUp();
        }

        public static void DownButton()
        {
            if (explorer != null) explorer.MoveDown();
        }

        public static void LeftButton()
        {
            SendKey(",");
        }

        public static void RightButton()
        {
            SendKey(".");
        }

        public static void ExitButton()
        {
            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                p.Kill();
            }
            if(explorer != null)
                explorer.Close();
        }

        public static void ListButton()
        {

            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                p.Kill();
            }
            if (explorer != null)
                explorer.Close();
            explorer = new Explorer();
            explorer.Show();
            explorer.Closed += explorer_Closed;
            Screen[] screens = Screen.AllScreens;
            var x = screens[screenIndex].WorkingArea.X;
            var y = screens[screenIndex].WorkingArea.Y;
            explorer.Left = x;
            explorer.Top = y;
            explorer.WindowState = WindowState.Minimized;
            explorer.WindowState = WindowState.Maximized;

        }

        static void explorer_Closed(object sender, EventArgs e)
        {
            explorer = null;
        }

        public static void NextButton()
        {
            if(explorer != null)
                return;
            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                p.Kill();
            }

            var files = new List<string>(Directory.GetFiles(Explorer.currentDirectory));
            var index = files.IndexOf(Explorer.currentfile);
            if (index < files.Count - 1)
            {
                index++;
            }
            Explorer.currentfile = files[index];
            var extension = Path.GetExtension(Explorer.currentfile);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                Process.Start(Explorer.currentfile);
                SendKey("f");

            }

        }

        public static void PreviousButton()
        {
            if (explorer != null)
                return;

            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                p.Kill();
            }

            var files = new List<string>(Directory.GetFiles(Explorer.currentDirectory));
            var index = files.IndexOf(Explorer.currentfile);
            if (index > 0)
            {
                index--;
            }
            Explorer.currentfile = files[index];
            var extension = Path.GetExtension(Explorer.currentfile);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                Process.Start(Explorer.currentfile);
                SendKey("f");

            }


        }

        public static void VolUpButton()
        {
            SendKey("{UP}");
        }

        public static void VolDownButton()
        {
            SendKey("{DOWN}");
        }

        public static void SendKey(string key)
        {
            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                p.WaitForInputIdle();
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                SendKeys.SendWait(key);
            }

        }
    }
}
