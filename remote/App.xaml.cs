using System.Windows;
using IoC;

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

            base.OnStartup(e);
        }
    }
}
