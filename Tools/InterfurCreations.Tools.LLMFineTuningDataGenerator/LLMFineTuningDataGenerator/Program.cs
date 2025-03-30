using Spectre.Console;
using System;
using Autofac;
using InterfurCreations.AdventureGames.Graph.Store;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using TiktokenSharp;

namespace LLMFineTuningDataGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.Markup("[underline red]Furventure Games LLM Data Generator[/]!\n");

            var contianer = new ConsoleAppBootstrap().BuildContainer();

            var games = contianer.Resolve<IGameRetrieverService>().ListGames();

            var formattedData = new List<List<FormattedGameTestResultData>>();

            int contextMaxLength = 1500;

            while (true)
            {
                var selectedGame = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select a game[/]")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more games)[/]")
                        .AddChoices(games.Select(a => a.GameName).ToArray()));

                var timesToRun = AnsiConsole.Ask<int>("How many times should we run this game for?: ");

                using (var scope = contianer.BeginLifetimeScope())
                {
                    var executor = scope.Resolve<GameTestExecutor>();
                    var allData = executor.Execute(selectedGame, timesToRun);

                    TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
                    foreach (var data in allData)
                    {
                        var formattedDataConvo = new List<FormattedGameTestResultData>();
                        int tokenCount = 0;
                        foreach (var item in data)
                        {
                            if (!string.IsNullOrEmpty(item.OptionTaken))
                                formattedDataConvo.Add(new FormattedGameTestResultData() { Message = item.OptionTaken, Role = "user" });
                            int i = 1;
                            var optionsString = string.Join(' ', item.OptionsPresented.Select(a => $"[{i++}] - {a}"));

                            var scrubbedMessage = item.GameMessage.Replace("\r", "").Replace("\n", "").Replace(@"\", "").Trim();

                            var fullGameMessage = scrubbedMessage + " " + optionsString;

                            var tokensInThisMessage = tikToken.Encode(fullGameMessage).Count;
                            if (tokenCount + tokensInThisMessage > contextMaxLength)
                            {
                                formattedData.Add(formattedDataConvo);
                                formattedDataConvo = new List<FormattedGameTestResultData>();
                                formattedDataConvo.Add(new FormattedGameTestResultData() { Message = fullGameMessage, Role = "assistant" });
                                tokenCount = tokensInThisMessage;
                            } else
                            {
                                formattedDataConvo.Add(new FormattedGameTestResultData() { Message = fullGameMessage, Role = "assistant" });
                                tokenCount += tokensInThisMessage;
                            }
                        }
                        formattedData.Add(formattedDataConvo);
                    }

                    var openAiData = formattedData.Select(a => new OpenAIFormatGameTestResult { messages = a.Select(a => new OpenAIFormatGameTestResultData { content = a.Message, role = a.Role }).ToList()});
                    List<SimpleFormatGameTestResultData> simpleDatallama2 = new List<SimpleFormatGameTestResultData>();

                    string startToken = "<s>";
                    string endToken = @" </s>";
                    string startPrompt = "[INST] ";
                    string endPrompt = " [/INST] ";
                    foreach(var convo in formattedData)
                    {
                        string currentStr = "";
                        foreach (var convoItem in convo)
                        {
                            if (currentStr == "")
                            {
                                if (convoItem.Role == "user")
                                {
                                    currentStr = $"{startToken}{startPrompt}{convoItem.Message}{endPrompt}";
                                } else
                                    currentStr = $"{startToken}{convoItem.Message}{endToken}";

                            }
                            else if (convoItem.Role == "assistant")
                                currentStr = $"{currentStr}{convoItem.Message}{endToken}";
                            else
                                currentStr = $"{currentStr}{startToken}{startPrompt}{convoItem.Message}{endPrompt}";
                        }
                        simpleDatallama2.Add(new SimpleFormatGameTestResultData() { text = currentStr });
                    }
                    //var simpleData = formattedData.Select(a => new SimpleFormatGameTestResultData { text = string.Join("\n<|endoftext|>\n", a.Select(b => $"[{b.Role}]: {b.Message}")) });

                    var openAiJsonLines = openAiData.Select(a => JsonConvert.SerializeObject(a));
                    //var simpleDataJsonLines = simpleData.Select(a => JsonConvert.SerializeObject(a));
                    var simpleDataJsonLines = simpleDatallama2.Select(a => JsonConvert.SerializeObject(a));
                    var jsonString = JsonConvert.SerializeObject(formattedData);

                    AnsiConsole.Markup($"We're done! We have [green]{formattedData.Count}[/] conversations to save.!\n");
                    if (!AnsiConsole.Confirm("Add more runs?"))
                    {
                        var path = AnsiConsole.Ask<string>("Give us a file path with name to save too (no ext.): ");
                        File.WriteAllText(path + ".json", jsonString);
                        File.WriteAllLines(path + ".jsonl", openAiJsonLines);
                        File.WriteAllLines(path + "-simple" + ".jsonl", simpleDataJsonLines);
                        AnsiConsole.Markup($"\n[underline green]Done[/]! :)");
                        return;
                    }
                }
            }
        }
    }
}
