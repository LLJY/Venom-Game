using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [Serializable]
    public class GameData
    {
        public float PlayerCurrentHealth = 100;
        public float PlayerXP = 1;
        public int DemonsKilled = 0;
        public int CurrentWorldIndex = 0;
        public bool FirstTimeSeeingSuicide = true;
        public bool FirstTimeSeeingHarm = true;
        public bool FirstTimeSeeingAnxiety = true;
        public bool FirstTimePlaying = true;
        public int SaveSlot = 0;
        public string SaveName = "default";

        public GameData()
        {
            CurrentWorldIndex = Random.Range(int.MinValue, int.MaxValue);
        }

        public int PlayerLevel => Mathf.FloorToInt(Mathf.Sqrt(PlayerXP));

        public static void SaveGame()
        {
            var gameData = GameCache.GameData;
            var path = $"{Application.persistentDataPath}/save-{gameData.SaveSlot}.dat";
            Debug.Log($"Save file saved to {path}");
            using var stream = new FileStream(path, FileMode.OpenOrCreate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, gameData);
        }
        
        public static GameData LoadGameData(int saveSlot)
        {
            var path = $"{Application.persistentDataPath}/save-{saveSlot}.dat";
            var formatter = new BinaryFormatter();
            using var file = File.Open(path, FileMode.Open);
            return (GameData) formatter.Deserialize(file);
        }
    }
}