using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System.Collections.Generic;

namespace InterfurCreations.AdventureGames.Telegram
{
    public class TelegramCommunicator : ICommunicator
    {
        public static TelegramService _service;
        TelegramBotOverseer _overseer;

        private readonly IReporter _reporter;

        List<string> messages = new List<string>();

        public TelegramCommunicator(IConfigurationService configService, IPlayerDatabaseController playerDatabaseController,
            IReporter reporter, IAccountController accountController, IGoogleDriveService gdriveService, IGameExecutor gameExecutor)
        {
            _service = new TelegramService(configService.GetConfig("TelegramUrl"), configService.GetConfig("TelegramApiKey"));
            _reporter = reporter;
        }

        public async void SetupAsync()
        {
            UpdateListener updateListener = new UpdateListener(_service);

            updateListener.Start();

            _reporter.Initialise();

            Log.LogMessage("Testing API with getMe: ");
            var user = await Methods.getMe(_service);
            if(user.is_bot)
            {
                Log.LogMessage("API Succesfully connected, bot found and available");
            }

            Log.LogMessage("Beginning bot...");
        }
    }
}
