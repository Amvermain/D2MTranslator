using D2MTranslator.ViewModels.Models;
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
            InitializeComponent();
        }

        private void OnTrOriginalSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Debug.WriteLine("OnTrOriginalSelectedItemChanged");
            if (e.NewValue is FileSystemItem selectedFile)
            {
                SelectItemInTreeView(trReference, selectedFile.Name);
            }
        }

        private void OnTrReferenceSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Debug.WriteLine("OnTrReferenceSelectedItemChanged");
            //LoadFileContent(e.NewValue as FileSystemItem, txtReference);
        }



        private void SelectItemInTreeView(TreeView treeView, string fileName)
        {
            foreach (FileSystemItem item in treeView.Items)
            {
                if (FindAndSelectItem(item, fileName))
                    break;
            }
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
