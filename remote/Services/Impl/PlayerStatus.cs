using System.Collections.Generic;
using System.Xml.Serialization;

namespace remote.Services
{
    [XmlRoot("root")]
    public class PlayerStatus
    {
        public enum States { paused, running, stopped }
        public bool fullscreen { get; set; }
        public int volume { get; set; }
        public States state { get; set; }
        public double position { get; set; }
        public IList<category> information { get; set; }
    }
}