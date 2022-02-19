using System;
using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class GameData
    {
        public float PlayerCurrentHealth = 100;
        public float PlayerXp = 1;
        public int DemonsKilled = 0;
        public int CurrentWorldIndex = 0;
        public bool FirstTimeSeeingSuicide = true;
        public bool FirstTimeSeeingHarm = true;
        public bool FirstTimeSeeingAnxiety = true;
        public bool FirstTimePlaying = true;
        public int SaveSlot = 0;
        public string SaveName = "default";
    }
}