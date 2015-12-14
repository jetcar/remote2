using System;
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
        void SetFullScreen();
        void VolUp();
        void VolDown();
    }
}
