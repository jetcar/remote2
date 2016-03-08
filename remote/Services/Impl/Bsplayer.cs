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
        //        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        //        static extern int SetForegroundWindow(IntPtr point);
        const int WM_KEYDOWN = 0x100;
        private static string player;
        Dictionary<string,int> codes = new Dictionary<string, int>()
        {
            {" ", 0x20 },
            {".", 0xBE },
            {",", 0xBC },
            {"f", 0x46 },
            {"{UP}", 0x26 },
            {"{DOWN}", 0x28 },
        };
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
                var p = FindWindowByCaption(0, player);
                if (p != null)
                {

                    //                    SetForegroundWindow(p);
                    //                    SendKeys.SendWait(key);
                    var keycode = codes[key];
                    SendMessage(p, WM_KEYDOWN, keycode, IntPtr.Zero);
                    Console.WriteLine("{0} {1}", DateTime.Now, key);
                }
                
            });

        }

//        [DllImportAttribute("User32.dll")]
//        private static extern int FindWindow(String ClassName, String WindowName);
//
//        [DllImport("user32.dll")]
//        private static extern IntPtr SetForegroundWindow(int hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(int ZeroOnly, string lpWindowName);


    }
}
