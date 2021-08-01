using Furventure.OfflineApplications.ViewModels.Play;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Furventure.OfflineApplications.DataTemplateSelectors
{
    class MessageTypeDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate MessageTemplate { get; set; }
        public DataTemplate ImageMessage { get; set; }
        public DataTemplate PlayerListItemViewModel { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is MessageListItemViewModel)
                return MessageTemplate;
            if (item is PlayerListItemViewModel)
                return PlayerListItemViewModel;
            return ImageMessage;
        }
    }
}
