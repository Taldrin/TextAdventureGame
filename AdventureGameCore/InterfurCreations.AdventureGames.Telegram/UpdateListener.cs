using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterfurCreations.AdventureGames.Telegram.DataObjects;
using Autofac;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.Telegram
{
    public class UpdateListener
    {
        private TelegramService _service;
        private int update_id = 0;

        public UpdateListener(TelegramService service)
        {
            _service = service;
        }

        public void Start()
        {
            new Thread(async () =>
            {
                while (true)
                {
                    try
                    {
                        var results = await Methods.getUpdates(_service, update_id);

                        results.ForEach(a =>
                        {
                            using (var scope = ContainerStore.Container.BeginLifetimeScope())
                            {
                                update_id = a.update_id + 1;
                                var teleBotController = scope.Resolve<TelegramBotOverseer>();
                                teleBotController.RecieveNewMessage(a.message, _service);
                            }
                        });
                    } catch (Exception e) { Log.LogMessage("Error with HTML query: " + e.Message, LogType.Error, e.StackTrace); }
                }
            }).Start();
        }
    }
}
