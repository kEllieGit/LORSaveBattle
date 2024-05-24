using System;
using System.Collections.Generic;

namespace SaveBattle.Data
{
    [Serializable]
    public class StageSaveData
    {
        public SaveId StageID { get; set; }

        public int Wave { get; set; }

        public int Turn { get; set; }

        public int EmotionLevel { get; set; }

        public StageManagerType StageType { get; set; }

        public SephirahType Sephirah { get; set; }

        public List<SephirahType> UsedFloors { get; set; } = new List<SephirahType>();

        public List<SaveId> EGOCards { get; set; } = new List<SaveId>();

        public List<UnitSaveData> UnitData { get; set; } = new List<UnitSaveData>();

        public StageClassInfo GetStage()
        {
            return Singleton<StageClassInfoList>.Instance.GetData(StageID.GetLorId());
        }
    }

    public enum StageManagerType
    {
        Map,
        Creature,
        Enemy,
        KetherFinal
    }
}