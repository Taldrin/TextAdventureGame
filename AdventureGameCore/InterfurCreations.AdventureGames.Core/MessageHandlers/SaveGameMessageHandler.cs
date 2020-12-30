using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class SaveGameMessageHandler : IMessageHandler
    {
        private readonly IGameSaveService _gameSaveService;

        public int Priorioty => 8;

        private const string CONT = "Continue";
        private const string RTN = "Return";

        public SaveGameMessageHandler(IGameSaveService gameSaveService)
        {
            _gameSaveService = gameSaveService;
        }

        public List<string> GetOptions(Player player)
        {
            return new List<string> { "Continue", "Return" };
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            if(player.PlayerFlag != PlayerFlag.SAVE_GAME.ToString())
            {
                player.PlayerFlag = PlayerFlag.SAVE_GAME.ToString();
                return ExecutionResultHelper.SingleMessage($"Enter a name for your save, or press Continue to save without a name.", GetOptions(player));
            }
            if (message == CONT)
            {
                _gameSaveService.SaveCurrentGame(player);
                return SetMainMenu(player, "Your game has been successfully saved!");
            }
            else if(message == RTN)
            {
                return SetMainMenu(player, "Your game has not been saved");
            } else
            {
                if (message.Length > 100)
                    return ExecutionResultHelper.SingleMessage($"Please keep save game names under 100 characters! You are {message.Length - 100} over the limit.", GetOptions(player));
                _gameSaveService.SaveCurrentGame(player, message);
                return SetMainMenu(player, "Your game has been successfully saved!");
            }
        }

        private ExecutionResult SetMainMenu(Player player, params string[] messages )
        {
            ExecutionResult executionResult = new ExecutionResult();
            List<MessageResult> resultMessages = new List<MessageResult>();
            resultMessages.AddRange(messages.Select(a => new MessageResult {Message = a}));
            player.PlayerFlag = PlayerFlag.GAME_MENU.ToString();
            var result = MessageHandlerHelpers.ReturnToGameMenu(player, "");
            resultMessages.AddRange(result.MessagesToShow);
            executionResult.MessagesToShow = resultMessages;
            executionResult.OptionsToShow = result.OptionsToShow;
            return executionResult;
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            return playerFlag == PlayerFlag.SAVE_GAME.ToString() || (playerFlag == PlayerFlag.GAME_MENU.ToString() && message == Messages.SaveGame);
        }
    }
}
