
namespace InterfurCreations.AdventureGames.Services
{
    public interface IGameBridge
    {
        void StartAsync();

        void HandleNewInputAsync(string input);

        string GetCurrentStateInfo();
    }
}
