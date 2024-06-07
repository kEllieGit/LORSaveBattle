using System;
using System.Text.Json;
using HarmonyLib;

namespace SaveBattle
{
    public class Initializer : ModInitializer
    {
        public override void OnInitializeMod()
        {
            var harmony = new Harmony("spoon.savebattle");
            harmony.PatchAll();

            FileLog.logPath = "./LibraryOfRuina_Data/Mods/SaveBattle/ErrorLog.txt";
        }
    }

    public static class SerializationInfo
    {
        public const string Filepath = "./LibraryOfRuina_Data/Mods/SaveBattle/SaveData/Data.json";

        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    // Only supports Dictionary<SaveId, int> for now.
                    new SaveIdDictionaryConverter()
                }
            };

            return options;
        }
    }
}