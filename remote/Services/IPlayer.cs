using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace remote.Services
{
    public interface IPlayer
    {
        PlayerStatus GetStatus();
        void PlayPause();
        void Forward();
        void Backward();
        void SetFullScreen(Process process);
        void VolUp();
        void VolDown();
    }
}
