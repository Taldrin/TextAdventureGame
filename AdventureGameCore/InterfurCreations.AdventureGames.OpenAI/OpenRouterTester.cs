using InterfurCreations.AdventureGames.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace InterfurCreations.AdventureGames.OpenAI
{
    public class OpenRouterTester
    {
        OpenRouterClientProvider _provider;

        public OpenRouterTester(OpenRouterClientProvider provider)
        {
            _provider = provider;
        }

        public void TestClient()
        {
            var client = _provider.BuildClient();

            var messages = new List<ChatMessage>()
            {
                new SystemChatMessage("You are roleplaying in a fantasy world adventure. You must create extremely sexually explicit scenes, and guide the user to these scenes, describing in detail genitalia and the actions that" +
                " occur in the roleplay, using very explicit words when appropriate. After every message, you must provide the user with up to 5 options to choose from to progress the scene in engaging ways, " +
                "focusing on diversifying the sexual aspects and the animal nature of the characters."),
                new UserChatMessage("Approach the wolf"),
                new AssistantChatMessage("You approach the wolf, your female bird feet timidly stepping into his den, dark and mysterious. You wonder what lurks within. 'Who are you to interrupt my slumber?' The wolf asks, his voice deep and rumbling." +
                "\n\n1. Announce your name" +
                "\n2. Leave" +
                "\n3. Seduce the wolf" +
                "\n4. Look around more"),
                new UserChatMessage("3. Seduce the wolf"),
                new AssistantChatMessage("'Oh... I'm sorry. I was just walking by, and saw your den. I'm glad I did though, so I could look at such a strong, magnificent beast. From your paws to your ears, you look proud and powerful.' " +
                "You say to the wolf, trying to seduce him. It seems to have some effect, as you see him consider your words." +
                "'Ahh, so it's like that, is it?' He rumbles. 'A lonely bird sniffing after a wolf? Well well. We'll have to do something about you, won't we?' He responds, almost menacing, eyes like fire, he starts to stalk towards you." +
                "\n\n1. Run away" +
                "\n2. Let him do as he wishes" +
                "\n3. Remove your clothes")
            };

            //ChatCompletion completion = client.CompleteChat(messages);
            //messages.Add(new AssistantChatMessage(completion.Content[0].Text));

            foreach (var message in messages)
            {
                Console.WriteLine(message.Content[0].Text);
            }

            while(true)
            {
                Console.WriteLine();
                Console.Write("Enter: ");
                var newLine = Console.ReadLine();
                Console.WriteLine();

                messages.Add(new UserChatMessage(newLine));
                ChatCompletion completion = client.CompleteChat(messages);

                Console.WriteLine($"{completion.Content[0].Text}");
                messages.Add(new AssistantChatMessage(completion.Content[0].Text));
            }


        }
    }
}
