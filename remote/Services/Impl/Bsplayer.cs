using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IoC;

namespace remote.Services.Impl
{
    public class Bsplayer : IPlayer
    {

        public IDispatcher Dispatcher { get { return IocKernel.GetInstance<IDispatcher>(); } }
        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        private static string player;
        public Bsplayer()
        {
            player = ConfigurationManager.AppSettings["playerName"];
        }
        public PlayerStatus GetStatus()
        {
            PlayerStatus status = new PlayerStatus();
            Process p = Process.GetProcessesByName(player).FirstOrDefault();
            if (p != null)
            {
                return status;
            }
            return null;
        }

        public void PlayPause()
        {
            SendKey(" ");
        }

        public void Forward()
        {
            SendKey(".");
        }

        public void Backward()
        {
            SendKey(",");
        }

        public void SetFullScreen(Process p)
        {
            SendKey("f");
        }

        public void VolUp()
        {
            SendKey("{UP}");
        }

        public void VolDown()
        {
            SendKey("{DOWN}");
        }
        public void SendKey(string key)
        {
            Dispatcher.Invoke(() =>
            {
                var p = FindWindow(null,player);
                if (p != null)
                {

                    SetForegroundWindow(p);
                    SendKeys.SendWait(key);
                    Console.WriteLine("{0} {1}", DateTime.Now, key);
                }
                
            });

        }

        [DllImportAttribute("User32.dll")]
        private static extern int FindWindow(String ClassName, String WindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        public int Activate(int hWnd)
        {
            if (hWnd > 0)
            {
                SetForegroundWindow(hWnd);
                return hWnd;
            }
            else
            {
                return -1;
            }
        }

        public int GetWindowHwnd(string className, string windowName)
        {
            int hwnd = 0;
            string cls = className == string.Empty ? null : className;
            string win = windowName == string.Empty ? null : windowName;

            hwnd = FindWindow(cls, win);
            return hwnd;
        }

    }
}
