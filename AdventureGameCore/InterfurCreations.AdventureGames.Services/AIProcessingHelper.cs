using InterfurCreations.AdventureGames.OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public static class AIProcessingHelper
    {
        public static (string message, List<string> options) GetOrRequestNewOptions(string message, ChatClient client, List<(string entity, string message)> currentChat, string instruction)
        {
            var options = ParseOptions(message);

            int i = 0;
            int limit = 5;
            while ( (options == null || !options.Any() || options.All(a => String.IsNullOrWhiteSpace(a))) && i != limit)
            {
                var newChat = currentChat.ToList();
                newChat.Add(("assistant", message));
                newChat.Add(("user", "Please generate 4 options for me to choose from to continue the current scene, numbered from 1 to 4."));

                ChatCompletion result = client.CompleteChat(newChat.ToChat(instruction));
                var completedText = result.Content[0].Text;

                options = ParseOptions(completedText);
                i++;
            }

            message = message.Split("1.")[0];

            return (message, options);
        }

        public static string IsComplete(ChatClient client, List<(string entity, string message)> currentChat, string instruction)
        {
            var completion = false; //CheckOverseer(client, currentChat);

            if(!completion)
            {
                return null;
            }
            currentChat.RemoveAt(currentChat.Count - 1);
            currentChat.Add(("user", "Please wrap the scene up naturally, and do not provide me any further options"));

            ChatCompletion result = client.CompleteChat(currentChat.ToChat(instruction));
            var completedText = result.Content[0].Text;

            return completedText;
        }

        private static bool CheckOverseer(ChatClient client, List<(string entity, string message)> currentChat)
        {
            bool isYes = false;
            bool isNo = false;

            while(!isYes && !isNo)
            {
                if (currentChat.Count > 45)
                    isYes = true;
                else
                {
                    var messages = new List<ChatMessage>();
                    messages.Add(new SystemChatMessage("Your role is to evaluate when a scene from a roleplay is complete. A scene typically reaches a natural end point, " +
                        "either when one role player decides to leave the scene, or all characters involved have climaxed and finished their fun. " +
                        "You must respond with only a 'yes' if the scene is complete, or 'no' if it is not, and nothing else. Simply use your best judgement to decide, allowing a scene to run its course but not overstay its welcome."));
                    var fullConvo = string.Join("\n\n", currentChat.Select(a => a.entity == "user" ? $"Player 1: {a.message}" : $"Player 2: {a.message}").ToList());
                    messages.Add(new UserChatMessage(fullConvo));
                    ChatCompletion result = client.CompleteChat(messages);
                    var completedText = result.Content[0].Text;

                    isYes = completedText.Trim().ToLower().StartsWith("yes");
                    isNo = completedText.Trim().ToLower().StartsWith("no");
                }

            }

            return isYes;
        }

        private static List<string> ParseOptions(string input)
        {
            var options = new List<string>();
            int currentNumber = 1;

            while (true)
            {
                string searchPattern = currentNumber + ". ";
                int startIndex = input.IndexOf(searchPattern);

                if (startIndex == -1)
                    break;

                startIndex += searchPattern.Length;

                string nextPattern = (currentNumber + 1) + ". ";
                int endIndex = input.IndexOf(nextPattern, startIndex);

                string option;
                if (endIndex == -1)
                {
                    option = input.Substring(startIndex).Trim();
                }
                else
                {
                    option = input.Substring(startIndex, endIndex - startIndex).Trim();
                }

                if (option.Length > 128)
                {
                    option = option.Substring(0, 120) + "...";
                }

                options.Add(option);
                currentNumber++;
            }

            return options;
        }

        public static List<ChatMessage> ToChat(this List<(string entity, string message)> existingChat, string instruction)
        {
            var messages = new List<ChatMessage>();
            messages.Add(new SystemChatMessage(instruction));
            foreach (var msg in existingChat)
            {
                if (msg.entity == "user")
                    messages.Add(new UserChatMessage(msg.message));
                else
                    messages.Add(new AssistantChatMessage(msg.message));
            }

            return messages;
        }

    }
}
