﻿using Humanizer;
using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Exceptions;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class LoadGameMessageHandler : IMessageHandler
    {
        private readonly IGameRetrieverService _gameStore;
        private readonly IGameProcessor _gameProcessor;
        private readonly ITextParsing _textParsing;
        private readonly IGameSaveService _gameSaveService;

        private const int PageSize = 8;

        public LoadGameMessageHandler(IGameRetrieverService gameStore, IGameProcessor gameProcessor, ITextParsing textParsing, IGameSaveService gameSaveService)
        {
            _gameStore = gameStore;
            _gameProcessor = gameProcessor;
            _textParsing = textParsing;
            _gameSaveService = gameSaveService;
        }

        public int Priorioty => 5;

        public List<string> GetOptions(Player player)
        {
            int pageNumber = 0;
            if (!string.IsNullOrEmpty(player.PlayerMenuContext2))
            {
                int.TryParse(player.PlayerMenuContext2, out pageNumber);
            }
            if (pageNumber < 0) pageNumber = 0;
            var gameSaves = _gameSaveService.ListGameSaves(PageSize, pageNumber, player.PlayerId);

            var options = gameSaves.Select(a => 
                string.IsNullOrEmpty(a.Name) ? $"{a.PlayerGameSaveId} - {a.PlayerGameSave.GameName} - {a.CreatedDate.Humanize()}" 
                                             : $"{a.PlayerGameSaveId} - {a.Name} - {a.PlayerGameSave.GameName} - {a.CreatedDate.Humanize()}").ToList();
            if (pageNumber > 0)
                options.Add($"< Page {pageNumber}");
            if (gameSaves.Count != 0)
                options.Add($"> Page {pageNumber + 2}");
            options.Add(Messages.Return);
            return options;
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            if(player.PlayerFlag != PlayerFlag.LOAD_GAME.ToString())
            {
                player.PlayerMenuContext = player.PlayerFlag.ToString();
                player.PlayerFlag = PlayerFlag.LOAD_GAME.ToString();
                return ExecutionResultHelper.SingleMessage("Select a game save to load", GetOptions(player));
            } else if(message == Messages.Return)
            {
                player.PlayerMenuContext2 = "0";
                if(player.PlayerMenuContext == PlayerFlag.GAME_MENU.ToString())
                {
                    return MessageHandlerHelpers.ReturnToGameMenu(player, "");
                }
                return MessageHandlerHelpers.ReturnToMainMenu(player);
            } else if(message.StartsWith("<"))
            {
                if (int.TryParse(player.PlayerMenuContext2, out int pageNumber))
                {
                    if (pageNumber <= 0)
                        pageNumber = 1;
                    player.PlayerMenuContext2 = "" + (pageNumber - 1);
                }
                else
                    player.PlayerMenuContext2 = "0";
                return ExecutionResultHelper.SingleMessage("Select a game save to load", GetOptions(player));
            }else if(message.StartsWith(">"))
            {
                if (int.TryParse(player.PlayerMenuContext2, out int pageNumber))
                {
                    if (pageNumber < 0)
                        pageNumber = 0;
                    player.PlayerMenuContext2 = "" + (pageNumber + 1);
                }
                else
                    player.PlayerMenuContext2 = "" + player.GameSaves.Count / PageSize;
                return ExecutionResultHelper.SingleMessage("Select a game save to load", GetOptions(player));
            }
            else
            {
                var saveIdString = message.Split(' ')[0];
                if(!int.TryParse(saveIdString, out var saveId))
                {
                    return ExecutionResultHelper.SingleMessage("Invalid save specified", GetOptions(player));
                }
                var gameSave = _gameSaveService.GetGameSaveById(saveId, player.PlayerId)?.PlayerGameSave;
                if(gameSave == null)
                {
                    return ExecutionResultHelper.SingleMessage($"Save with ID {saveId} is invalid. Either it doesn't exist, or it doesn't belong to you!", GetOptions(player));
                }

                player.PlayerMenuContext2 = "0";

                player.ActiveGameSave.GameName = gameSave.GameName;
                player.ActiveGameSave.GameSaveData = gameSave.GameSaveData.Select(a => new PlayerGameSaveData {Name = a.Name, Value = a.Value }).ToList();
                player.ActiveGameSave.StateId = gameSave.StateId;
                player.ActiveGameSave.FrameStack = gameSave.FrameStack.Select(a => new PlayerFrameStack
                {
                    CreatedDate = a.CreatedDate,
                    FunctionName = a.FunctionName,
                    ReturnStateId = a.ReturnStateId,
                    Save = player.ActiveGameSave,
                    SaveId = player.ActiveGameSave.SaveId
                }).ToList();

                player.PlayerFlag = PlayerFlag.IN_GAME.ToString();
                var games = _gameStore.ListGames();
                var playerState = player.ActiveGameSave;
                var gameFound = games.Where(a => a.GameName == playerState.GameName).FirstOrDefault();
                if (gameFound == null)
                {
                    throw new AdventureGameException($"No game found with name {playerState.GameName}", true);
                }
                var state = gameFound.FindStateById(playerState.StateId);
                var execResult = new ExecutionResult
                {
                    MessagesToShow = new List<MessageResult> { new MessageResult { Message = _textParsing.ParseText(playerState, state.StateText) } },
                    OptionsToShow = _gameProcessor.GetCurrentOptions(playerState, gameFound, state)
                };

                execResult.OptionsToShow.Add("-Menu-");
                return execResult;

            }
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            if (playerFlag == PlayerFlag.LOAD_GAME.ToString() || (playerFlag == PlayerFlag.MAIN_MENU.ToString() && message == Messages.LoadGame) || (playerFlag == PlayerFlag.GAME_MENU.ToString() && message == Messages.LoadGame))
                return true;
            return false;
        }
    }
}
