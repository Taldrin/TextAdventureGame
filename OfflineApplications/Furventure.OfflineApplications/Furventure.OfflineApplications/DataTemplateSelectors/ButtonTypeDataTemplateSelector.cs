using Furventure.OfflineApplications.ViewModels.Play;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Furventure.OfflineApplications.DataTemplateSelectors
{
    class ButtonTypeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ButtonTemplate { get; set; }
        public DataTemplate InputTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is GameButtonViewModel)
                return ButtonTemplate;
            if (item is InputButtonViewModel)
                return InputTemplate;
            return null;
        }
    }
}
