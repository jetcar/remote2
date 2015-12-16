using System.Windows;
using IoC;
using remote.Services;
using remote.Services.Impl;

namespace remote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IocKernel.registeredServices[typeof(IProcess)] = new MyProcess();
            IocKernel.registeredServices[typeof(IDirectory)] = new MyDirectory();
            IocKernel.registeredServices[typeof(IPlayer)] = new MyPlayer();

            base.OnStartup(e);
        }
    }
}
