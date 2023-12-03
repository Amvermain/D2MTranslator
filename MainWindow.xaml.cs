using D2MTranslator.Models;
using D2MTranslator.ViewModels;
using Ninject;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace D2MTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            var mvm = App.Kernel.Get<MainViewModel>();
            DataContext = mvm;
            Closing += mvm.OnClose;
            InitializeComponent();
        }
    }
}
