using D2MTranslator.ViewModels;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace D2MTranslator.UserControls
{
    /// <summary>
    /// FileSystemItemTree.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileSystemItemTree : UserControl
    {
        public FileSystemItemTree()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new FileSystemViewModel();
                InitializeComponent();
            }
            else
            {
                DataContext = App.Kernel.Get<FileSystemViewModel>();
                InitializeComponent();
            }
        }
    }
}
