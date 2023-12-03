using D2MTranslator.ViewModels;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace D2MTranslator
{
    /// <summary>
    /// LanguageConfigWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LanguageConfigWindow : Window
    {
        LanguageConfigViewModel viewModel;
        public LanguageConfigWindow()
        {
            viewModel = App.Kernel.Get<LanguageConfigViewModel>();
            DataContext = viewModel;
            InitializeComponent();
            Closing += viewModel.OnClose;
        }

    }
}
