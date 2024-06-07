using System.Diagnostics;
using System.Reflection;
using HarmonyLib;

namespace SaveBattle
{
    public static class Debug
    {
        public static string LogPath => FileLog.logPath;

        public static void LogError(string msg)
        {
            FileLog.Log(msg);
        }

        public static void LogError(object msg)
        {
            FileLog.Log(msg.ToString());
        }

        public static string GetCurrentMethodName()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            return method.Name;
        }
    }
}