using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [Serializable]
    public class GameData
    {
        public float PlayerCurrentHealth = 100;
        private float PlayerXP = 0;
        [NonSerialized] public ReactiveProperty<float> PlayerXpReactiveProperty;
        public int DemonsKilled = 0;
        public int CurrentWorldIndex = 0;
        public bool FirstTimeSeeingSuicide = true;
        public bool FirstTimeSeeingHarm = true;
        public bool FirstTimeSeeingAnxiety = true;
        public bool FirstTimePlaying = true;
        public int SaveSlot = 0;
        public bool FreshRespawn = true;
        public string SaveName = "default";

        public GameData()
        {
            CurrentWorldIndex = Random.Range(int.MinValue, int.MaxValue);
            PlayerXpReactiveProperty = new ReactiveProperty<float>(PlayerXP);
            PlayerXpReactiveProperty.Subscribe(x =>
            {
                PlayerXP = x;
                Debug.Log(PlayerLevelRaw + "level");
            });
        }

        public float PlayerLevelRaw => Mathf.Sqrt(PlayerXP);

        public int PlayerLevel => Mathf.FloorToInt(PlayerLevelRaw);
        public float PlayerLevelProgress => Mathf.Ceil(PlayerLevelRaw) - PlayerLevelRaw; 

        /// <summary>
        /// Saves the current GameData into the selected slot
        /// </summary>
        public static void SaveGame()
        {
            var gameData = GameCache.GameData;
            var path = $"{Application.persistentDataPath}/save-{gameData.SaveSlot}.dat";
            Debug.Log($"Save file saved to {path}");
            using var stream = new FileStream(path, FileMode.OpenOrCreate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, gameData);
        }
        
        /// <summary>
        /// Loads the game data from the selected slot
        /// </summary>
        /// <param name="saveSlot">game save slot</param>
        /// <returns></returns>
        public static GameData LoadGameData(int saveSlot)
        {
            var path = $"{Application.persistentDataPath}/save-{saveSlot}.dat";
            var formatter = new BinaryFormatter();
            using var file = File.Open(path, FileMode.Open);
            return (GameData) formatter.Deserialize(file);
        }
    }
}