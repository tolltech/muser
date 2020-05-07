using System;

namespace Tolltech.MuserUI.Sync
{
    public interface ICaptcha
    {
        void IncrementForbid(Guid userId, string key);
        bool IsForbid(Guid userId, string key);
    }
}