using D2MTranslator.ViewModels;
using Ninject;
using System.ComponentModel;
using System.Windows.Controls;

namespace D2MTranslator.UserControls
{
    /// <summary>
    /// InteractiveTranslator.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InteractiveTranslator : UserControl
    {
        public InteractiveTranslator()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new JsonFileViewModel();
                InitializeComponent();
            }
            else
            {
                DataContext = App.Kernel.Get<JsonFileViewModel>();
                InitializeComponent();
            }
        }
    }
}
