using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FaceAddon
{
    public class WinkAndBlinkHandler
    {
        private int nextDelay;

        private int currentTick = 0;
        private int tickForOverallAnimation = 0;

        public readonly int minWaitTick = 120;
        public readonly int maxWaitTick = 240;

        public readonly int minTickForAnimation = 30;
        public readonly int maxTickForAnimation = 60;

        public readonly float winkChance = 0.2f;

        public WinkAndBlinkHandler(int minBlinkTick, int maxBlinkTick, float winkChance, int minTickForAnimation, int maxTickForAnimation)
        {
            this.minWaitTick = minBlinkTick;
            this.maxWaitTick = maxBlinkTick;
            this.winkChance = winkChance;
            this.minTickForAnimation = minTickForAnimation;
            this.maxTickForAnimation = maxTickForAnimation;
            nextDelay = Rand.RangeInclusive(minWaitTick, maxWaitTick);
        }

        public void Check(float mood)
        {
            if (!NowPlaying)
            {
                nextDelay--;
                if (nextDelay == 0)
                {
                    // start animation
                    tickForOverallAnimation = Rand.RangeInclusive(minTickForAnimation, maxTickForAnimation);
                    if (Rand.Chance(winkChance * mood))
                    {
                        WinkNow = true;
                    }
                    else
                    {
                        BlinkNow = true;
                    }
                }
            }
            else
            {
                currentTick++;
                if (currentTick > tickForOverallAnimation)
                {
                    // waiting tick
                    WinkNow = false;
                    BlinkNow = false;
                    currentTick = 0;
                    nextDelay = Rand.RangeInclusive(minWaitTick, maxWaitTick);
                }
            }
        }

        public bool NowPlaying
        {
            get
            {
                return nextDelay <= 0;
            }
        }

        public bool WinkNow { get; private set; } = false;

        public bool BlinkNow { get; private set; } = false;

        public void ForcedExecution()
        {
            nextDelay = 1;
        }
    }
}
