using D2MTranslator.ViewModels;
using Ninject;
using System;
using System.Windows;

namespace D2MTranslator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IKernel Kernel { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Kernel = new StandardKernel();
            ConfigureBindings();

            var mainWindow = Kernel.Get<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureBindings()
        {
            Kernel.Bind<FileSystemViewModel>().ToSelf().InSingletonScope();
            Kernel.Bind<JsonFileViewModel>().ToSelf().InSingletonScope();
        }
    }
}
