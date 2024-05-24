using UI;
using TMPro;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

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
                var uiAlarmPopup = UIAlarmPopup.instance;
                uiAlarmPopup.SetAlarmText("");

                var textField = AccessTools.Field(typeof(UIAlarmPopup), "txt_alarm");
                TMP_Text text = (TMP_Text)textField.GetValue(uiAlarmPopup);
                text.SetText("Found previously saved battle data. Load it?");

                var btField = AccessTools.Field(typeof(UIAlarmPopup), "buttonNumberType");
                btField.SetValue(uiAlarmPopup, UIAlarmButtonType.YesNo);

                var btnsField = AccessTools.Field(typeof(UIAlarmPopup), "ButtonRoots");
                List<GameObject> buttons = (List<GameObject>)btnsField.GetValue(uiAlarmPopup);

                buttons[0].gameObject.SetActive(false);
                buttons[(int)btField.GetValue(uiAlarmPopup)].gameObject.SetActive(true);

                var confirmField = AccessTools.Field(typeof(UIAlarmPopup), "_confirmEvent");
                ConfirmEvent confirmEvent = (ConfirmEvent)confirmField.GetValue(uiAlarmPopup);

                var del = new ConfirmEvent(Load);
                confirmField.SetValue(uiAlarmPopup, del);
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
