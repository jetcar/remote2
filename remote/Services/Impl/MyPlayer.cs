using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        object volLocker = new object();
        object positionLocker = new object();

        PlayerStatus SendRequestGetStatus(string param)
        {
            try
            {
                WebRequest request = WebRequest.Create(url + param);
                request.Headers.Add("Authorization", auth);
                WebResponse response = request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream());
                var responceStr = stream.ReadToEnd();
                stream.Close();
                var xsSubmit = new XmlSerializer(typeof(PlayerStatus));
                using (var sww = new StringReader(responceStr))
                using (var xrr = XmlReader.Create(sww))
                {
                    var status = (PlayerStatus)xsSubmit.Deserialize(xrr);
                    lock (positionLocker)
                    {
                        position = status.position;
                    }
                    lock (volLocker)
                    {
                        volume = status.volume;
                    }
                    return status;
                }

            }
            catch (Exception e)
            {

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
            lock (positionLocker)
            {
                position += 0.01;
                SendRequestGetStatus(String.Format("?command=seek&val={0}%25", (int)position));
            }
        }

        public void Backward()
        {
            lock (positionLocker)
            {
                position -= 0.01;
                SendRequestGetStatus(String.Format("?command=seek&val={0}%25", (int)position));
            }
        }

        public void SetFullScreen()
        {
            SendRequestGetStatus("?command=fullscreen");
        }

        public void VolUp()
        {
            lock (volLocker)
            {
                volume += 5;
                SendRequestGetStatus("?command=volume&val=" + volume);
            }
        }

        public void VolDown()
        {
            lock (volLocker)
            {
                volume -= 5;
                SendRequestGetStatus("?command=volume&val=" + volume);
            }
        }
    }
}
