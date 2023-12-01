using D2MTranslator.Models;
using D2MTranslator.ViewModels;
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

        public MainWindow(MainViewModel mvm)
        {

            DataContext = mvm;
            InitializeComponent();
        }

        private bool FindAndSelectItem(FileSystemItem item, string fileName)
        {
            if (item.Name == fileName)
            {
                // 해당 항목을 TreeView에서 선택하는 로직을 여기에 구현

                return true;
            }

            foreach (var child in item.Items)
            {
                if (FindAndSelectItem(child, fileName))
                    return true;
            }

            return false;
        }
    }
}
