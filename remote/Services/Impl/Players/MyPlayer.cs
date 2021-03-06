﻿using System;
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

namespace remote.Services.Impl
{
    public class MyPlayer : IPlayer
    {
        private string url = "http://localhost:8080/requests/status.xml";
        private string auth = "Basic OnFxcXFxcQ==";
        private double position = 0;
        private int volume = 0;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


        PlayerStatus SendRequestGetStatus(string param)
        {
            try
            {
                WebRequest request = WebRequest.Create(url + param);
                Console.WriteLine(url + param);
                request.Headers.Add("Authorization", auth);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var stream = new StreamReader(response.GetResponseStream());
                    var responceStr = stream.ReadToEnd();
                    stream.Close();
                    var xsSubmit = new XmlSerializer(typeof(PlayerStatus));
                    using (var sww = new StringReader(responceStr))
                    using (var xrr = XmlReader.Create(sww))
                    {

                        var status = (PlayerStatus)xsSubmit.Deserialize(xrr);
                        if (string.IsNullOrEmpty(param))
                        {
                            position = (int)(status.position * 100) / 100.0;
                            volume = status.volume;
                        }
                        Console.Write(status.position + " ");
                        Console.Write(position + " ");
                        Console.Write(status.state + " ");
                        Console.Write(volume + " ");
                        Console.WriteLine(status.fullscreen);

                        return status;
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public PlayerStatus GetStatus()
        {
            return SendRequestGetStatus(null);
        }

        public void PlayPause()
        {
            SendRequestGetStatus("?command=pl_pause");
        }

        public void Forward()
        {
            position += 0.01;
            SendRequestGetStatus(String.Format("?command=seek&val={0}%25", (int)(position * 100)));
        }

        public void Backward()
        {
            position -= 0.01;
            SendRequestGetStatus(String.Format("?command=seek&val={0}%25", (int)(position * 100)));
        }

        public void SetFullScreen(Process process)
        {
            const short SWP_NOSIZE = 1;
            const short SWP_ASYNCWINDOWPOS = 0x4000;
            const int SWP_SHOWWINDOW = 0x0040;
            string playerName = ConfigurationManager.AppSettings["playerName"];
            var screenIndex = Convert.ToInt32(ConfigurationManager.AppSettings["defaultScreenIndex"]);
            Process p = Process.GetProcessesByName(playerName).FirstOrDefault();
            if (p != null)
            {
                IntPtr handle = p.MainWindowHandle;
                if (handle != IntPtr.Zero)
                {
                    Screen[] screens = Screen.AllScreens;
                    var x = screens[screenIndex].WorkingArea.X;
                    var y = screens[screenIndex].WorkingArea.Y;

                    SetWindowPos(handle, 0, 0, 0, 100, 100, SWP_SHOWWINDOW | SWP_ASYNCWINDOWPOS);
                    SetWindowPos(handle, 0, x, y, 0, 0, SWP_NOSIZE | SWP_SHOWWINDOW | SWP_ASYNCWINDOWPOS);
                }
            }

            var status = SendRequestGetStatus("?command=fullscreen");
            while (!status.fullscreen)
            {
                status = SendRequestGetStatus("?command=fullscreen");
                Thread.Sleep(100);
            }

        }

        public void VolUp()
        {
            volume += 5;
            SendRequestGetStatus("?command=volume&val=" + volume);
        }

        public void VolDown()
        {
            volume -= 5;
            SendRequestGetStatus("?command=volume&val=" + volume);
        }
    }
}
