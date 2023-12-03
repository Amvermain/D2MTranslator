using D2MTranslator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace D2MTranslator.DataTemplateSelectors
{
    public class InteractiveTranslatorTreeSelector : DataTemplateSelector
    {
        public DataTemplate ValidTemplate { get; set; }
        public DataTemplate InvalidTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TranslationItem translationItem)
            {
                return translationItem.IsValid ? ValidTemplate : InvalidTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }

}
