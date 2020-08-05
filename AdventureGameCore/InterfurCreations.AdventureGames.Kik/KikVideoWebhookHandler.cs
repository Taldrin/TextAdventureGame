using Autofac;
using InterfurCreations.AdventureGames.Kik.KikObjects;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public class KikVideoWebhookHandler //: IWebhookHandler
    {
        //public Type ObjectType => typeof(KikVideoMessageDataObject);

        //public async void Handle<T>(T obj)
        //{
        //    if (obj is KikVideoMessageDataObject)
        //    {
        //        try
        //        {
        //            var kikMessagesDataObject = obj as KikStartChattingDataObject;
        //            {
        //                using (var scope = ContainerStore.Container.BeginLifetimeScope())
        //                {
        //                    var kikMessageExecutor = scope.Resolve<KikMessageExecutor>();
        //                    await kikMessageExecutor.ProcessMessage(new KikMessageDataObject { chatId = kikMessagesDataObject.chatId, from = kikMessagesDataObject.from, body = " " });
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
        //    if (obj is KikVideoMessageDataObject a)
        //    {
        //        if (a.chatId != null)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
