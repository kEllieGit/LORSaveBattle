using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LOR_DiceSystem;

namespace SaveBattle
{
    //[HarmonyPatch(typeof(DiceCardXmlInfo))]
    //[HarmonyPatch(nameof(DiceCardXmlInfo.IsPersonal))]
    public static class RemoveEGOLock
    {
        //[HarmonyPrefix]
        public static bool IsAvailable(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
