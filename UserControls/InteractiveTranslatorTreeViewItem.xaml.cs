using D2MTranslator.Models;
using D2MTranslator.Services;
using Ninject;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace D2MTranslator.UserControls
{
    /// <summary>
    /// InteractiveTreeViewItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InteractiveTranslatorTreeViewItem : UserControl
    {
        ReferenceJsonDataService _referenceJsonDataService;

        public InteractiveTranslatorTreeViewItem()
        {            
            _referenceJsonDataService = App.Kernel.Get<ReferenceJsonDataService>();
            InitializeComponent();
        }
    }
}
