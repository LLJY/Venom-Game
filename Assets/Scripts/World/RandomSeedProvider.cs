using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public static class RandomSeedProvider
    {
        public static int CurrentSeed;
        public static int CurrentWorldIndex = 0;
        public static int NextSeed()
        {
            // allow for overflows
            unchecked
            {
                ++CurrentWorldIndex;
                GameCache.GameData.CurrentWorldIndex = CurrentWorldIndex;
                return GenerateSeed();

            }
        }
        public static int PreviousSeed()
        {
            // allow for overflows
            unchecked
            {
                --CurrentWorldIndex;
                GameCache.GameData.CurrentWorldIndex = CurrentWorldIndex;
                return GenerateSeed();
            }
        }
        private static int GenerateSeed()
        {
            Debug.Log($"random {CurrentWorldIndex}");
            unchecked
            {
                CurrentSeed = Random.Range(int.MinValue, int.MaxValue) * (CurrentWorldIndex * 256);
                Random.InitState(CurrentSeed);
                return CurrentSeed;
            }
        }
    }
}