using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using remote.Annotations;

namespace remote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SerialPort port;
        private bool run = true;
        private Thread thread;
        private IList<string> _ports;
        private string _portTxt;
        private Visibility _onlineVisibility = Visibility.Collapsed;
        private Visibility _offlineVisibility = Visibility.Visible;
        private ObservableCollection<string> _lines = new ObservableCollection<string>();
        private Command<ButtonCommands> _addCommand;
        private string _selectedCodes;
        private IList<ButtonCommands> _commands = new List<ButtonCommands>();
        public ButtonCommands Power { get; set; }
        IActions Actions { get { return IoC.IocKernel.GetInstance<IActions>(); } }
        public ButtonCommands OkButton { get; set; }
        public ButtonCommands UpButton { get; set; }

        public ButtonCommands DownButton { get; set; }
        public ButtonCommands LeftButton { get; set; }
        public ButtonCommands RightButton { get; set; }

        public ButtonCommands ExitButton { get; set; }
        public ButtonCommands ListButton { get; set; }
        public ButtonCommands NextButton { get; set; }

        public ButtonCommands PreviousButton { get; set; }
        public ButtonCommands VolUpButton { get; set; }
        public ButtonCommands VolDownButton { get; set; }
        public ButtonCommands Options { get; set; }
        public ButtonCommands Green { get; set; }
        public ButtonCommands Yellow { get; set; }
        public ButtonCommands Blue { get; set; }
        private IList<ButtonCommands> buttons = new List<ButtonCommands>();
        IDictionary<string, ButtonCommands> actions = new Dictionary<string, ButtonCommands>();
        private void Add_click(ButtonCommands command)
        {
            command.Commands.Add(SelectedCode);
            foreach (var buttonCommand in buttons)
            {
                foreach (var remotecommand in buttonCommand.Commands)
                {
                    actions[remotecommand] = buttonCommand;
                }
            }

            Save();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        public MainWindow()
        {
            TrayClick = new Command<string>(Tray_click);
            Power = new ButtonCommands("power", "power.png", Actions.Power);
            OkButton = new ButtonCommands("ok", "ok.png", Actions.OkButton,500);
            UpButton = new ButtonCommands("up", "up.png", Actions.UpButton);

            DownButton = new ButtonCommands("down", "down.png", Actions.DownButton);
            LeftButton = new ButtonCommands("left", "left.png", Actions.LeftButton);
            RightButton = new ButtonCommands("right", "right.png", Actions.RightButton);

            ExitButton = new ButtonCommands("exit", "exit.png", Actions.ExitButton);
            ListButton = new ButtonCommands("list", "list.png", Actions.ListButton);
            NextButton = new ButtonCommands("next", "next.png", Actions.NextButton,500);

            PreviousButton = new ButtonCommands("previous", "previous.png", Actions.PreviousButton,500);
            VolUpButton = new ButtonCommands("volUp", "volUp.png", Actions.VolUpButton);
            VolDownButton = new ButtonCommands("volDown", "volDown.png", Actions.VolDownButton);
            Options = new ButtonCommands("options", "options.png", Actions.OptionsButton);

            buttons.Add(Power);
            buttons.Add(OkButton);
            buttons.Add(UpButton);

            buttons.Add(DownButton);
            buttons.Add(LeftButton);
            buttons.Add(RightButton);

            buttons.Add(ExitButton);
            buttons.Add(ListButton);
            buttons.Add(NextButton);

            buttons.Add(PreviousButton);
            buttons.Add(VolUpButton);
            buttons.Add(VolDownButton);
            var tempButtons = Load();
            foreach (var buttonCommand in tempButtons)
            {
                var button = buttons.First(x => x.Name == buttonCommand.Name);
                button.Commands = buttonCommand.Commands;
            }
            foreach (var buttonCommand in buttons)
            {
                foreach (var command in buttonCommand.Commands)
                {
                    actions[command] = buttonCommand;
                }
            }

            InitializeComponent();
            AddCommand = new Command<ButtonCommands>(Add_click);
            Ports = new List<string>(SerialPort.GetPortNames());
            if (Ports.Count == 1)
            {
                PortTxt = Ports.First();
            }
            this.Hide();
        }

        private void Tray_click(string obj)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Show();
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;
        }

        public void Save()
        {
            var xsSubmit = new XmlSerializer(typeof(List<ButtonCommands>));
            using (var sww = new StringWriter())
            using (var writer = XmlWriter.Create(sww))
            using (var swriter = new StreamWriter("config.xml"))
            {
                xsSubmit.Serialize(writer, buttons);
                var xml = sww.ToString(); // Your XML
                swriter.Write(xml);
            }
        }
        public List<ButtonCommands> Load()
        {
            var xsSubmit = new XmlSerializer(typeof(List<ButtonCommands>));
            using (var sww = new StreamReader("config.xml"))
            using (var xrr = XmlReader.Create(sww))
            {
                return (List<ButtonCommands>)xsSubmit.Deserialize(xrr);
            }
        }

        public IList<ButtonCommands> Commands
        {
            get { return _commands; }
            set
            {
                if (Equals(value, _commands)) return;
                _commands = value;
                OnPropertyChanged();
            }
        }

        public IList<string> Ports
        {
            get { return _ports; }
            set
            {
                if (Equals(value, _ports)) return;
                _ports = value;
                OnPropertyChanged();
            }
        }

        public string PortTxt
        {
            get { return _portTxt; }
            set
            {
                _portTxt = value;
                ConnectToPort(_portTxt);
            }
        }

        public string SelectedCode
        {
            get { return _selectedCodes; }
            set { _selectedCodes = value; }
        }

        public Command<ButtonCommands> AddCommand
        {
            get { return _addCommand; }
            set
            {
                if (Equals(value, _addCommand)) return;
                _addCommand = value;
                OnPropertyChanged();
            }
        }

        public Visibility OfflineVisibility
        {
            get { return _offlineVisibility; }
            set
            {
                if (value == _offlineVisibility) return;
                _offlineVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility OnlineVisibility
        {
            get { return _onlineVisibility; }
            set
            {
                if (value == _onlineVisibility) return;
                _onlineVisibility = value;
                OnPropertyChanged();
            }
        }

        private void ConnectToPort(string portTxt)
        {
            run = false;
            if (port != null && port.IsOpen)
            {
                OfflineVisibility = Visibility.Visible;
                OnlineVisibility = Visibility.Collapsed;
                port.Close();
            }
            while (thread != null && thread.IsAlive)
            {
                thread.Interrupt();
            }

            port = new SerialPort(portTxt);
            run = true;
            thread = new Thread(() =>
            {

                while (run)
                {
                    if (!port.IsOpen)
                    {
                        port.Open();
                        OnlineVisibility = Visibility.Visible;
                        OfflineVisibility = Visibility.Collapsed;
                    }
                    try
                    {
                        string speedReading = port.ReadLine();
                        speedReading = speedReading.Replace("\r", "");
                        if (!string.IsNullOrEmpty(speedReading.Trim()))
                            HandleRemoteCode(speedReading);
                        Dispatcher.Invoke(() =>
                        {
                            if (!string.IsNullOrEmpty(speedReading.Trim()))
                                Lines.Insert(0, speedReading);

                        });
                    }
                    catch (Exception)
                    {

                    }

                }
            });
            thread.Start();
        }

        private void HandleRemoteCode(string speedReading)
        {
            if (actions.ContainsKey(speedReading))
                Dispatcher.Invoke(() =>
                {
                    actions[speedReading].Run();
                });
        }


        public ObservableCollection<String> Lines
        {
            get { return _lines; }
            set { _lines = value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            run = false;
            if (port.IsOpen)
            {
                port.Close();
            }
            while (thread.IsAlive)
            {
                thread.Interrupt();
            }

            base.OnClosing(e);
        }

        public Command<string> TrayClick { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
