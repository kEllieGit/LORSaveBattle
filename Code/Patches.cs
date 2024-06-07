using System.IO;
using System.Reflection;
using UI;
using HarmonyLib;

namespace SaveBattle
{
    [HarmonyPatch(typeof(MapManager))]
    [HarmonyPatch(nameof(MapManager.OnRoundStart))]
    public static class MapManagerPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            SavedBattle.Save(Data.StageManagerType.Map);
        }
    }

    /*
    [HarmonyPatch]
    public static class CreatureMapManagerPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(CreatureMapManager), nameof(MapManager.OnRoundStart));
        }

        [HarmonyPrefix]
        public static void Prefix()
        {
            SavedBattle.Save(Data.StageManagerType.Creature);
        }
    }*/

    /*
    [HarmonyPatch(typeof(EnemyTeamStageManager))]
    [HarmonyPatch(nameof(EnemyTeamStageManager.OnRoundStart))]
    public static class EnemyTeamStageManagerPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            SavedBattle.Save(Data.StageManagerType.Enemy);
        }
    }*/

    [HarmonyPatch]
    public static class EnemyTeamStageManager_KetherFinalPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(EnemyTeamStageManager_KeterFinal), nameof(EnemyTeamStageManager.OnRoundStart));
        }

        [HarmonyPrefix]
        public static void Prefix()
        {
            SavedBattle.Save(Data.StageManagerType.KetherFinal);
        }
    }

    [HarmonyPatch(typeof(GlobalGameManager))]
    [HarmonyPatch(nameof(GlobalGameManager.ContinueGame))]
    public static class ContinuePatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (File.Exists(SerializationInfo.Filepath))
            {
                UIAlarmPopup.instance.AddAlarm("Found previously saved battle data. Load it?", UIAlarmButtonType.YesNo, new ConfirmEvent(Load));
            }
        }

        private static void Load(bool yes)
        {
            if (yes)
            {
                SavedBattle.Load();
            }
        }
    }
}
