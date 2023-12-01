using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Utility;
using D2MTranslator.ViewModels;
using ICSharpCode.AvalonEdit.Search;
using Ninject;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace D2MTranslator.UserControls
{
    /// <summary>
    /// TextView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TranslationEditor : UserControl
    {

        public TranslationEditor()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new JsonFileViewModel();
                InitializeComponent();
                Loaded += OnLoaded;
            }
            else
            {
                DataContext = App.Kernel.Get<JsonFileViewModel>();
                InitializeComponent();
                Loaded += OnLoaded;
            }
        }




        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as JsonFileViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
            SearchPanel.Install(modEditor.TextArea);

        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as JsonFileViewModel;
            if (e.PropertyName == nameof(JsonFileViewModel.RefContentText))
            {

                refEditor.Text = viewModel.RefContentText;
            } else if (e.PropertyName == nameof(JsonFileViewModel.ModContentText))
            {
                modEditor.Text = viewModel.ModContentText;
            }
            
        }

        private Debouncer debouncer = new Debouncer();

        private void ModTextChanged(object sender, System.EventArgs e)
        {
            debouncer.Debounce(1000, () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WeakReferenceMessenger.Default.Send(new ModTextChangedMessage(modEditor.Text));
                });
            });
        }

        private void RefTextChanged(object sender, System.EventArgs e)
        {
        }

        //public string ModText
        //{
        //    get { return txtOriginal.Text; }
        //    set { txtOriginal.Text = value; }
        //}
    }
}
