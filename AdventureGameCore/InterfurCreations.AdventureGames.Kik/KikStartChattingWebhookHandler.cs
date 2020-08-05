using Autofac;
using InterfurCreations.AdventureGames.Kik.KikObjects;
using InterfurCreations.AdventureGames.Kik.KikObjects.Send;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public class KikStartChattingWebhookHandler //: IWebhookHandler
    {
        //public KikStartChattingWebhookHandler()
        //{
        //}

        //public Type ObjectType => typeof(KikRecieveDataObject);

        //public async void Handle<T>(T obj)
        //{
        //    if (obj is KikRecieveDataObject)
        //    {
        //        try
        //        {
        //            var kikMessagesDataObject = obj as KikRecieveDataObject;
        //            {
        //                using (var scope = ContainerStore.Container.BeginLifetimeScope())
        //                {
        //                    var kikMessageExecutor = scope.Resolve<KikMessageExecutor>();
        //                    await kikMessageExecutor.ProcessMessage(kikMessagesDataObject);
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Log.LogMessage("Error handling incoing KIK message: " + e.Message, LogType.Error, e.StackTrace);
        //        }
        //    }
        //}

        //public bool IsValid(object obj)
        //{
        //    if(obj is KikRecieveDataObject a)
        //    {
        //        if(a.messages != null && a.messages.Count > 1 && a.messages[0].chatId != null)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
