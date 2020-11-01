using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Exceptions;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Services.ImageStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.Core
{
    public class GameProcessor : IGameProcessor
    {
        private readonly ITextParsing _textParsing;
        private readonly IImagingService _imageService;
        private readonly ImageBuildDataTracker _imageBuildDataTracker;

        public GameProcessor(ITextParsing textParsing, IImagingService imageService, ImageBuildDataTracker imageBuildDataTracker)
        {
            _textParsing = textParsing;
            _imageService = imageService;
            _imageBuildDataTracker = imageBuildDataTracker;
        }

        private (DrawState resultState, string optionText, StateOption optionObject) HandlePermanentButtons(DrawGame game, DrawState state, PlayerGameSave save, Player player, string message)
        {
            foreach (var button in game.Metadata.PermanentButtons)
            {
                if (message.ToLower() == button.ButtonText.ToLower())
                {
                    save.FrameStack.Add(new PlayerFrameStack
                    {
                        CreatedDate = DateTime.UtcNow,
                        ReturnStateId = state.Id,
                        FunctionName = button.Function.FunctionName,
                        Save = save
                    });
                    save.StateId = button.Function.StartState.Id;
                    return (button.Function.StartState, button.ButtonText, new StateOption {Id = $"PERMANENT#{button.ButtonText}", ResultState = button.Function.StartState });
                }
            }

            return (null, null, null);
        }

        private (List<MessageResult> Messages, DrawState EndingState, List<string> StatesVisited) HandleFunction(string message, DrawState currentState, PlayerGameSave gameSave, Player player, DrawGame game, bool withDataChanges = true)
        {
            var functionName = message.Trim().ToLower().Split("#function")[1].Trim();
            var foundFunctions = game.GameFunctions.Where(a => a.FunctionName.ToLower() == functionName);
            if (foundFunctions.Count() > 1)
                throw new AdventureGameException($"Found more than 1 function matching name {functionName}!", true);
            var function = foundFunctions.FirstOrDefault();
            if(foundFunctions == null)
                throw new AdventureGameException($"Found no function matching name {functionName}!", true);

            gameSave.FrameStack.Add(new PlayerFrameStack
            {
                CreatedDate = DateTime.UtcNow,
                ReturnStateId = currentState.Id,
                FunctionName = function.FunctionName,
                Save = gameSave
            });
            gameSave.StateId = function.StartState.Id;

            return RecursivelyHandleStates(function.StartState, gameSave, player, game, withDataChanges);
        }

        private (List<MessageResult> Messages, DrawState EndingState, List<string> StatesVisited) HandleFunctionReturn(string message, DrawState currentState, PlayerGameSave gameSave, Player player, DrawGame game, bool withDataChanges = true)
        {
            var topStackItem = gameSave.FrameStack.OrderByDescending(a => a.Id).FirstOrDefault();
            if(topStackItem == null)
                throw new AdventureGameException($"No items on frame stack, but encountered a 'Return' statement", true);
            var returnState = game.FindStateById(topStackItem.ReturnStateId);

            gameSave.FrameStack.Remove(topStackItem);

            gameSave.StateId = returnState.Id;
            return RecursivelyHandleStates(returnState, gameSave, player, game, false, true);
        }


        public (List<MessageResult> Messages, DrawState EndingState, List<string> StatesVisited) RecursivelyHandleStates(DrawState currentState, PlayerGameSave gameSave, Player player, DrawGame game, bool withDataChanges = true, bool ignoreFrameShift = false)
        {
            if (withDataChanges)
                HandleAnyAttachments(currentState, gameSave, player, false);

            List<MessageResult> messages = new List<MessageResult>();
            var message = _textParsing.ParseText(gameSave, currentState.StateText);

            if(message != null && message.Trim().ToLower().StartsWith("#showbuiltimage"))
            {
                var imageUrl =  _imageService.CreateImageAsync(_imageBuildDataTracker.GetParams()).Result;
                messages.Add(new MessageResult
                {
                    ImageUrl = imageUrl
                });
                message = "";
            }
            if (message != null &&  message.Trim().ToLower().StartsWith("#function"))
            {
                if (ignoreFrameShift)
                    message = "";
                else
                    return HandleFunction(message, currentState, gameSave, player, game, withDataChanges);
            }
            if (message != null && message.Trim().ToLower().Equals("#return"))
                return HandleFunctionReturn(message, currentState, gameSave, player, game, withDataChanges);

            if (!string.IsNullOrEmpty(message))
                messages.Add(new MessageResult
                {
                    Message = message,
                    ImageUrl = currentState.IsImage ? currentState.StateText : null,
                });

            DrawState newState = null;
            var directTransitions = currentState.StateOptions.Where(a => a.IsDirectTransition).ToList();
            var nondirectTransitions = currentState.StateOptions.Where(a => !a.IsDirectTransition).ToList();
            var resolvedTransitions = nondirectTransitions.Select(a => (a.ResultState, _textParsing.ResolveOption(gameSave, a.StateText)));

            var validTransitions = resolvedTransitions.Where(a => a.Item2.IsDirectTransition && a.Item2.DirectTransitionCommandResult && a.Item2.OptionType != OptionType.Fallback).ToList();
            var fallbackTransitions = resolvedTransitions.Where(a => a.Item2.IsDirectTransition && a.Item2.DirectTransitionCommandResult && a.Item2.OptionType == OptionType.Fallback).ToList();
            if (validTransitions.Count > 1)
                throw new AdventureGameException("Found more than 1 applicable conditional direct transition");
            else if (validTransitions.Count == 1)
            {
                newState = validTransitions.First().ResultState;
            } else if(directTransitions.Count > 1)
            {
                throw new AdventureGameException("Found more than 1 applicable direct transition");
            } else if(directTransitions.Count == 1)
            {
                newState = directTransitions.First().ResultState;
            } else if(fallbackTransitions.Count > 1)
            {
                throw new AdventureGameException("Found more than 1 applicable conditional direct transition");
            } else if(fallbackTransitions.Count == 1)
            {
                newState = fallbackTransitions.First().ResultState;
            }

            if (withDataChanges)
                HandleAnyAttachments(currentState, gameSave, player, true);

            if (newState == null)
                return (messages, currentState, new List<string> {currentState.Id});

            var r = RecursivelyHandleStates(newState, gameSave, player, game, withDataChanges);
            r.Messages.AddRange(messages);
            r.StatesVisited.Add(currentState.Id);
            return r;
        }

        public ExecutionResult ProcessMessage(string message, PlayerGameSave playerGameData, DrawGame game, Player player)
        {
            var currentDrawGameState = game.FindStateById(playerGameData.StateId);

            List<MessageResult> Messages = new List<MessageResult>();

           // In general, any direct transition states ~should~ be processed already in the previous execution.
           // We'll check again here just in case.
            for (int i = 0; i < 100; i++)
            {
                var dResult = HandleAnyDirectTransitions(currentDrawGameState, playerGameData, player);
                if (dResult.Item1 == null) break;
                currentDrawGameState = dResult.Item1;
                Messages.Add(new MessageResult { Message = dResult.Item2 });
            }

            var resultOption = CalculateResultingOption(playerGameData, game, currentDrawGameState, message);

            // If it's null, it's invalid. Send the current state.
            if (resultOption.resultState == null)
            {
                var funcReturn = HandlePermanentButtons(game, currentDrawGameState, playerGameData, player, message);
                if (funcReturn.resultState == null)
                {
                    var execResult = ExecutionResultHelper.SingleMessage(_textParsing.ParseText(playerGameData, currentDrawGameState.StateText), GetCurrentOptions(playerGameData, game, currentDrawGameState));
                    execResult.IsInvalidInput = true;
                    return execResult;
                } else
                {
                    resultOption = funcReturn;
                }
            }

            player.Actions.Add(new PlayerAction { ActionName = resultOption.optionObject.Id, GameName = game.GameName, Player = player, Time = DateTime.UtcNow });

            var result = RecursivelyHandleStates(resultOption.resultState, playerGameData, player, game);
            result.Messages.Reverse();

            var newOptions = GetCurrentOptions(playerGameData, game, result.EndingState);

            playerGameData.StateId = result.EndingState.Id;

            return new ExecutionResult {
                MessagesToShow = result.Messages,
                OptionsToShow = newOptions,
                StatesVisited = result.StatesVisited
            };
        }

        private (DrawState resultState, string optionText, StateOption optionObject) CalculateResultingOption(PlayerGameSave playerGameData, DrawGame game, DrawState state, string message)
        {
            var currentOptions = GetCurrentOptionsWithResults(playerGameData, game, state);

            foreach(var a in currentOptions)
            {
                if (a.option.ToLower() == message.ToLower())
                {
                    return (a.resultState, a.option, a.stateOption);
                }
            };
            foreach(var button in game.Metadata.PermanentButtons)
            {
                if(button.ButtonText.ToLower() == message.ToLower())
                {

                }
            }
            return (null, null, null);
        }

        public (DrawState, string messages) HandleAnyDirectTransitions(DrawState currentState, PlayerGameSave playerGameData, Player player)
        {
            var transitions = currentState.StateOptions.Where(a => a.IsDirectTransition).ToList();
            if (transitions.Count > 1)
                throw new AdventureGameException("Found more than 1 applicable direct transition");
            if (transitions.Count == 1)
            {
                HandleAnyAttachments(currentState, playerGameData, player, true);
                currentState = transitions.First().ResultState;
                return (currentState, currentState.StateText);
            }
            return (null, null);
        }

        private void HandleAnyAttachments(DrawState state, PlayerGameSave playerGameData, Player player, bool afterMessage)
        {
            state.StateAttachements.ForEach(a =>
            {
                StateAttachment attachment = new StateAttachment
                {
                    Id = a.Id,
                    StateConditional = a.StateConditional,
                    StateText = a.StateText,
                    XmlElement = a.XmlElement
                };
                if (!_textParsing.ShouldRun(attachment.StateText, afterMessage, out attachment.StateText)) return;
                if (!string.IsNullOrEmpty(attachment.StateConditional))
                {
                    var condition = _textParsing.ResolveCommand(playerGameData, attachment.StateConditional);
                    if (condition == true)
                    {
                        _textParsing.ParseAttachment(player, playerGameData, attachment);
                    }
                }
                else
                {
                    _textParsing.ParseAttachment(player, playerGameData, attachment);
                }
            });
        }

        public List<string> GetCurrentOptions(PlayerGameSave playerGameData, DrawGame game, DrawState currentDrawGameState = null)
        {
            return GetCurrentOptionsWithResults(playerGameData, game, currentDrawGameState).Select(a => a.option).ToList();
        }

        public List<(string option, StateOption optionData)> GetCurrentOptionsFullDrawData(PlayerGameSave playerGameData, DrawGame game, DrawState currentDrawGameState = null)
        {
            return GetCurrentOptionsWithResults(playerGameData, game, currentDrawGameState).Select(a => (a.option, a.stateOption)).ToList();
        }

        public List<(string option, DrawState resultState, StateOption stateOption)> GetCurrentOptionsWithResults(PlayerGameSave playerGameData, DrawGame game, DrawState currentDrawGameState = null)
        {
            if(currentDrawGameState == null)
                currentDrawGameState = game.FindStateById(playerGameData.StateId);
            var parsedOptions = currentDrawGameState.StateOptions.Select(a => (resultState: a, messageResult: _textParsing.ResolveOption(playerGameData, a.StateText))).Where(a => a.messageResult.text != null);
            if (!parsedOptions.Any(a => a.messageResult.OptionType == OptionType.Normal)) {
                var fallbackOption = parsedOptions.SingleOrDefault(a => a.messageResult.OptionType == OptionType.Fallback);
                if (fallbackOption.resultState != null)
                    return new List<(string option, DrawState resultState, StateOption stateOption)> { (fallbackOption.messageResult.text, fallbackOption.resultState.ResultState, fallbackOption.resultState) };
            } else
            {
                parsedOptions = parsedOptions.Where(a => a.messageResult.OptionType != OptionType.Fallback);
            }

            var returnList = parsedOptions.Select(a => (a.messageResult.text, a.resultState.ResultState, a.resultState)).ToList();

            // Return before adding permanent buttons, otherwise the GameMessageHandler won't add the Restart and MainMenu buttons
            if (returnList.Count == 0) return new List<(string option, DrawState resultState, StateOption stateOption)>();

            var currentFunction = playerGameData.FrameStack?.OrderByDescending(a => a.Id).LastOrDefault()?.FunctionName;
            if (game.Metadata.PermanentButtons != null)
                returnList.AddRange(game.Metadata.PermanentButtons?.Where(a => a.Function.FunctionName != currentFunction).Select(
                    a => (a.ButtonText, (DrawState)null, new StateOption {Id = a.ButtonText }) ));

            return returnList;
        }
    }
}
