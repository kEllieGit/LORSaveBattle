using System;

namespace SaveBattle
{
    public static class AssemblyHelper
    {
        public static Type GetTypeFromLoadedAssemblies(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
