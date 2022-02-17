using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public static class RandomSeedProvider
    {
        public static int CurrentSeed;
        private static int _currentWorldIndex = 0;
        public static int NextSeed()
        {
            // allow for overflows
            unchecked
            {
                ++_currentWorldIndex;
                return GenerateSeed();

            }
        }
        public static int PreviousSeed()
        {
            // allow for overflows
            unchecked
            {
                --_currentWorldIndex;
                return GenerateSeed();
            }
        }
        private static int GenerateSeed()
        {
            Debug.Log($"random {_currentWorldIndex}");
            unchecked
            {
                CurrentSeed = Random.Range(int.MinValue, int.MaxValue) * (_currentWorldIndex * 256);
                Random.InitState(CurrentSeed);
                return CurrentSeed;
            }
        }
    }
}