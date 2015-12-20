using remote.Services;

namespace remote
{
    public interface IActions
    {
        void Power();
        void OkButton();
        void UpButton();
        void DownButton();
        void LeftButton();
        void RightButton();
        void ExitButton();
        void ListButton();
        void NextButton();
        void PreviousButton();
        void VolUpButton();
        void VolDownButton();
        IPlayer Player { get; }
        IExplorer Explorer { get; set; }
    }
}