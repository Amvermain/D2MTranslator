using D2MTranslator.ViewModels;
using ICSharpCode.AvalonEdit.Search;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace D2MTranslator
{
    /// <summary>
    /// TextView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextView : UserControl
    {

        public static readonly DependencyProperty ModTextProperty = DependencyProperty.Register(
    "ModText", typeof(string), typeof(TextView), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RefTextProperty = DependencyProperty.Register(
    "RefText", typeof(string), typeof(TextView), new PropertyMetadata(default(string)));

        public string ModText
        {
            get { return (string)GetValue(ModTextProperty); }
            set { SetValue(ModTextProperty, value); }
        }

        public string RefText
        {
            get { return (string)GetValue(RefTextProperty); }
            set { SetValue(RefTextProperty, value); }
        }

        public TextView()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
            SearchPanel.Install(modEditor.TextArea);

        }

        string refOriginalText;
        string modOriginalText;

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as MainViewModel;
            if (e.PropertyName == nameof(MainViewModel.RefText))
            {
                if (refOriginalText == null || refOriginalText == string.Empty || refOriginalText == refEditor.Text)
                {       
                    refEditor.Text = viewModel.RefText;
                    refOriginalText = viewModel.RefText;
                } else
                {
                    /** create new dialog and ask user if they want to overwrite
                     */
                    MessageBoxResult result = MessageBox.Show("Do you want to overwrite?", "Overwrite", MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            refEditor.Text = viewModel.RefText;
                            refOriginalText = viewModel.RefText;
                            break;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
            }
            if (e.PropertyName == nameof(MainViewModel.ModText))
            {
                if (modOriginalText == null || modOriginalText == string.Empty || modOriginalText == modEditor.Text)
                {
                    modEditor.Text = viewModel.ModText;
                    modOriginalText = viewModel.ModText;
                } else
                {
                    /** create new dialog and ask user if they want to overwrite
                     *                     */
                    MessageBoxResult result = MessageBox.Show("Do you want to overwrite?", "Overwrite", MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            modEditor.Text = viewModel.ModText;
                            modOriginalText = viewModel.ModText;
                            break;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
            }
        }

        //public string ModText
        //{
        //    get { return txtOriginal.Text; }
        //    set { txtOriginal.Text = value; }
        //}
    }
}
