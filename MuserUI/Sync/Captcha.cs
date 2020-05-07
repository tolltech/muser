using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tolltech.MuserUI.Sync
{
    public class Captcha : ICaptcha
    {
        private static readonly ConcurrentDictionary<(Guid, string), (DateTime TryUtcDate, int TryCount)> tries =
            new ConcurrentDictionary<(Guid, string), (DateTime, int)>();

        private static readonly TimeSpan ForbidTime = TimeSpan.FromMinutes(10);

        public void IncrementForbid(Guid userId, string key)
        {
            tries.AddOrUpdate((userId, key), tuple => (DateTime.UtcNow, 1), (tuple, tr) => (DateTime.UtcNow, tr.TryCount + 1));
        }

        public bool IsForbid(Guid userId, string key)
        {
            if (!tries.TryGetValue((userId, key), out var lastTry))
            {
                return false;
            }

            if (lastTry.TryCount >= 5)
            {
                if (lastTry.TryUtcDate + ForbidTime < DateTime.UtcNow)
                {
                    tries.Remove((userId, key), out _);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}