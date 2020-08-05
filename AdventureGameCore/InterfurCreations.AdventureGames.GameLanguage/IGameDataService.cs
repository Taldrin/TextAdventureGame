using InterfurCreations.AdventureGames.Database;

namespace InterfurCreations.AdventureGames.GameLanguage
{
    public interface IGameDataService
    {
        void SaveData(PlayerGameSave data, string dataName);
        void SaveData(PlayerGameSave data, string dataName, string valueSet);
        void DecrementData(PlayerGameSave data, string dataName);
        void IncrementData(PlayerGameSave data, string dataName);
        void DeleteData(PlayerGameSave data, string dataName);
        string GetData(PlayerGameSave data, string word);
        bool DataExists(PlayerGameSave data, string v);
        void SavePermanentData(Player player, string dataName, string dataValue, PlayerSaveDataType dataType);
    }
}