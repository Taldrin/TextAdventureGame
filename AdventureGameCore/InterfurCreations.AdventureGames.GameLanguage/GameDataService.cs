using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameLanguage
{
    public class GameDataService : IGameDataService
    {
        public bool DataExists(PlayerGameSave data, string v)
        {
            return data.GameSaveData.Any(a => a.Name == v);
        }

        public void DecrementData(PlayerGameSave data, string dataName)
        {
            if (!data.GameSaveData.Any(a => a.Name == dataName))
            {
                throw new AdventureGameException("Attempted to increment: " + dataName + " But that data does not exist!");
            }
            if (!int.TryParse(data.GameSaveData.First(a => a.Name == dataName).Value, out int result))
                throw new Exception("Error - data with name: " + dataName + " Is not a number, so the -- operator cannot be applied to it");
            else
            {
                var newVal = result - 1;
                SaveData(data, dataName, newVal.ToString());
            }
        }

        public void DeleteData(PlayerGameSave data, string dataName)
        {
            data.GameSaveData = data.GameSaveData.Where(a => a.Name != dataName).ToList();
        }

        public string GetData(PlayerGameSave data, string name)
        {
            return data.GameSaveData.FirstOrDefault(a => a.Name == name)?.Value;
        }

        public void IncrementData(PlayerGameSave data, string dataName)
        {
            if (!data.GameSaveData.Any(a => a.Name == dataName))
            {
                throw new AdventureGameException("Attempted to increment: " + dataName + " But that data does not exist!");
            }
            if (!int.TryParse(data.GameSaveData.First(a => a.Name == dataName).Value, out int result))
                throw new Exception("Error - data with name: " + dataName + " Is not a number, so the -- operator cannot be applied to it");
            else
            {
                var newVal = result + 1;
                SaveData(data, dataName, newVal.ToString());
            }
        }

        public void SaveData(PlayerGameSave data, string dataName)
        {
            DeleteData(data, dataName);
            data.GameSaveData.Add(new PlayerGameSaveData
            {
                Name = dataName,
                Value = "N/A",
                PlayerGameSaveSaveId = data.SaveId
            });
        }

        public void SaveData(PlayerGameSave data, string dataName, string valueSet)
        {
            DeleteData(data, dataName);
            data.GameSaveData.Add(new PlayerGameSaveData
            {
                Name = dataName,
                Value = valueSet,
                PlayerGameSaveSaveId = data.SaveId
            });
        }

        public void SavePermanentData(Player player, string dataName, string dataValue, PlayerSaveDataType dataType)
        {
            if (!player.PermanentData.Any(a => a.DataName == dataName && a.DataValue == dataValue))
            {
                player.PermanentData.Add(new PlayerSavedData
                {
                    Player = player,
                    PlayerId = player.PlayerId,
                    DataName = dataName,
                    DataValue = dataValue,
                    DataType = dataType.ToString()
                });
            }
        }
    }
}
