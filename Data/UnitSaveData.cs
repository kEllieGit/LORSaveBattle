using System;
using System.Collections.Generic;

namespace SaveBattle.Data
{
    [Serializable]
    public class UnitSaveData
    {
        public string Name { get; set; }

        public SaveId ID { get; set; }

        public int Index { get; set; }

        public float HP { get; set; }

        public float SR { get; set; }

        public bool IsStaggered { get; set; }

        public int PlayPoints { get; set; }

        public Faction Faction { get; set; }

        public int EmotionLevel { get; set; }

        public List<EmotionCoinData> EmotionCoins { get; set; } = new List<EmotionCoinData>();

        public List<SaveId> Hand { get; set; } = new List<SaveId>();

        public List<SaveId> EGOHand { get; set; } = new List<SaveId>();

        public List<int> EmotionCards { get; set; } = new List<int>();

        public List<SaveId> Passives { get; set; } = new List<SaveId>();

        public Dictionary<KeywordBuf, int> Bufs { get; set; } = new Dictionary<KeywordBuf, int>();
    }

    /// <summary>
    /// A JSON serializable version of a <see cref="EmotionCoin"/>
    /// </summary>
    public class EmotionCoinData
    {
        public EmotionCoinType CoinType { get; set; }
    }
}