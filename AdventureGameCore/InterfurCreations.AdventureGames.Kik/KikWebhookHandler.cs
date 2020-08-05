using Autofac;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Kik.KikObjects;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public class KikWebhookHandler : IWebhookHandler
    {
        public KikWebhookHandler()
        {
        }

        public Type ObjectType => typeof(KikMessagesDataObject<KikGenericRecieveMessageDataObject>);

        public async void Handle<T>(T obj, JObject jsonData)
        {
            if (obj is KikMessagesDataObject<KikGenericRecieveMessageDataObject> kikMessagesDataObject)
            {
                try
                {
                    foreach (var a in jsonData.First.First)
                    {
                        KikGenericRecieveMessageDataObject realisedDto = a.ToObject<KikGenericRecieveMessageDataObject>();
                        if (realisedDto.type == KikMessageType.TextType)
                            realisedDto = a.ToObject<KikMessageDataObject>();
                        if (realisedDto.type == KikMessageType.StartChattingType)
                            realisedDto = a.ToObject<KikStartChattingDataObject>();
                        using (var scope = ContainerStore.Container.BeginLifetimeScope())
                        {
                            var kikMessageExecutor = scope.Resolve<KikMessageExecutor>();
                            if (realisedDto != null)
                                await kikMessageExecutor.ProcessMessage(realisedDto);
                            else
                                await kikMessageExecutor.ProcessInvalidInput(realisedDto);

                        }
                    }
                }
                catch (Exception e)
                {
                    Log.LogMessage("Error handling incoing KIK message: " + e.Message, LogType.Error, e.StackTrace);
                }
            }
        }

        public bool IsValid(object obj)
        {
            if (obj is KikMessagesDataObject<KikGenericRecieveMessageDataObject> a)
            {
                if (a.messages != null && a.messages[0].chatId != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
