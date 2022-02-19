using System;
using System.Collections;

namespace Patterns
{
    public static class Utils
    {
        public static IEnumerator WaitForFrames(int frames)
        {
            if (frames <= 0)
            {
                throw new ArgumentOutOfRangeException("frames", "you can't wait for 0 or less frames, silly");
            }

            for (; frames > 0; frames--)
            {
                // pass the CPU time back to unity
                yield return null;
            }
        }
    }
}