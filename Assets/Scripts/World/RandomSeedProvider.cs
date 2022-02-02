using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public static class RandomSeedProvider
    {
        public static int CurrentSeed;
        
        public static int NextSeed()
        {
            // allow for overflows
            unchecked
            {
                CurrentSeed = Random.Range(int.MinValue, int.MaxValue) + 8192 * 2556335;
                Random.InitState(CurrentSeed);
                return CurrentSeed;
            }
        }
    }
}