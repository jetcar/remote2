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

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);


        public static IDirectory Directory { get; set; }
        private static IExplorer explorer;
        static string player;
        static int screenIndex;
        public static string CurrentPath;
        static Actions()
        {
            Directory = new MyDirectory();
            player = ConfigurationManager.AppSettings["playerName"];
            screenIndex = Convert.ToInt32(ConfigurationManager.AppSettings["defaultScreenIndex"]);
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

            var files = new List<string>(Directory.GetFiles(Explorer.CurrentDirectory));
            var index = files.IndexOf(Explorer.Currentfile);
            if (index < files.Count - 1)
            {
                index++;
            }
            Explorer.Currentfile = files[index];
            var extension = Path.GetExtension(Explorer.Currentfile);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                Process.Start(Explorer.Currentfile);
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

            var files = new List<string>(Directory.GetFiles(Explorer.CurrentDirectory));
            var index = files.IndexOf(Explorer.Currentfile);
            if (index > 0)
            {
                index--;
            }
            Explorer.Currentfile = files[index];
            var extension = Path.GetExtension(Explorer.Currentfile);
            if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
            {
                Process.Start(Explorer.Currentfile);
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

        public static void OptionsButton()
        {
            //SendKey("{DOWN}");
        }

       

        public static void SendKey(string key)
        {
            Process currProcess = Process.GetCurrentProcess();
            var currHandle = GetForegroundWindow();
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (currHandle == process.MainWindowHandle)
                    currProcess = process;
            }
            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                p.WaitForInputIdle();
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                SendKeys.SendWait(key);
            }
            try
            {
                currProcess.WaitForInputIdle();
                SetForegroundWindow(currProcess.MainWindowHandle);
            }
            catch (Exception)
            {
            }

        }

    }
}
