using System;
using Hypercastle.Web;
using UnityEngine;

namespace Hypercastle.Render
{

    public static class Animation
    {

        public static void Update(
            in AnimationInput inputs,
            in int[] mainSet,
            in int[] charSet,
            ContextInfo[] glyphContexts, // glyphContexts may be mutable for classes that use 'j'
            ref uint airShip,
            Action<int, int> textUpdate,
            Action<int, uint> fontSizeAction)
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    var index = y + 32 * x;
                    var ctx = glyphContexts[index];

                    if (inputs.Mode == 0)
                    {
                        if (ctx.h > 6 - inputs.Resource)
                        {
                            var unicode = mainSet[
                                Mathf.FloorToInt(
                                        0.25f * airShip +
                                        (ctx.h + 0.5f * x + 0.1f * inputs.Direction * y))
                                    % mainSet.Length];

                            textUpdate.Invoke(index, unicode);
                        }
                    } else if (inputs.IsDayDream | inputs.IsTerraformed)
                    {
                        if (ctx.h == 0)
                        {
                            var unicode = inputs.Seed < 8e3
                                ? mainSet[Mathf.FloorToInt(airShip / 1e3f + 0.05f * x + 0.005f * y)
                                    % mainSet.Length]
                                : mainSet[Mathf.FloorToInt(airShip / 2f + 0.05f * x)
                                    % mainSet.Length];
                            textUpdate.Invoke(index, unicode);
                        } else
                        {
                            var unicode = charSet[
                                Mathf.FloorToInt(airShip / inputs.SpeedFactor + x + ctx.h)
                                    % charSet.Length];
                            textUpdate(index, unicode);

                            if (UnityEngine.Random.value < 0.005f && inputs.Seed > 5e3)
                            {
                                fontSizeAction.Invoke(index, 3 + airShip % 34);
                            }

                            if (!('j' != ctx.OriginalClass && 'j' != ctx.ActiveClass))
                            {
                                textUpdate.Invoke(index, ' ');
                            }
                        }
                    }
                }
            }
            airShip++;
        }
    }
}
