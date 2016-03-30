using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using IoC;

namespace remote.Services.Impl
{
    public class MPCplayer : IPlayer
    {

        public IDispatcher Dispatcher { get { return IocKernel.GetInstance<IDispatcher>(); } }
        //        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        //        static extern int SetForegroundWindow(IntPtr point);
        private static string player;
        public MPCplayer()
        {
            player = "mpc-hc64";
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
            SendKey("wm_command=889&submit=Go%21");
        }

        public void Forward()
        {
            SendKey("wm_command=904&submit=Go%21");
        }

        public void Backward()
        {
            SendKey("wm_command=903&submit=Go%21");
        }

        public void SetFullScreen(Process p)
        {
            SendKey("wm_command=830&submit=Go%21");
        }

        public void VolUp()
        {
            SendKey("wm_command=907&submit=Go%21");
        }

        public void VolDown()
        {
            SendKey("wm_command=908&&submit=Go%21");
        }
        public void SendKey(string param)
        {
            var url = "http://localhost:13579/command.html?";
            try
            {
                WebRequest request = WebRequest.Create(url + param);
                Console.WriteLine(url + param);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var stream = new StreamReader(response.GetResponseStream());
                    var responceStr = stream.ReadToEnd();
                    stream.Close();


                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


    }
}
