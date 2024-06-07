using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

namespace SaveBattle
{
    public static class UIAlarmPopupExtensions
    {
        public static void AddAlarm(this UIAlarmPopup popup, string text, UIAlarmButtonType buttonType, ConfirmEvent confirmEvent = null)
        {
            popup.SetAlarmText("");

            var textField = AccessTools.Field(typeof(UIAlarmPopup), "txt_alarm");
            TMP_Text alarmText = (TMP_Text)textField.GetValue(popup);
            alarmText.SetText(text);

            var btField = AccessTools.Field(typeof(UIAlarmPopup), "buttonNumberType");
            btField.SetValue(popup, buttonType);

            var btnsField = AccessTools.Field(typeof(UIAlarmPopup), "ButtonRoots");
            List<GameObject> buttons = (List<GameObject>)btnsField.GetValue(popup);

            buttons[0].gameObject.SetActive(false);
            buttons[(int)btField.GetValue(popup)].gameObject.SetActive(true);
            
            var confirmField = AccessTools.Field(typeof(UIAlarmPopup), "_confirmEvent");
            confirmField.SetValue(popup, confirmEvent);
        }
    }
}
