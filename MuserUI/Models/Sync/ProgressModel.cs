using System;

namespace Tolltech.MuserUI.Models.Sync
{
    public class ProgressModel
    {
        public Guid Id { get; set; }
        public int Total { get; set; }
        public int Proecssed { get; set; }
    }
}