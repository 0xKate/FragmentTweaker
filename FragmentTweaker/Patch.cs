using HarmonyLib;
using Nautilus.Json.ExtensionMethods;
using System.Collections.Generic;
using System.IO;

namespace FragmentTweaker
{
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    public static class KnownTechInitializePatch
    {
        [HarmonyPrefix]
        public static void Prefix(PDAData data)
        {
            Dictionary<TechType, int> fragmentDict = new Dictionary<TechType, int>();
            data.scanner.ForEach(scanTech =>
            {
                if (scanTech.totalFragments < 20)
                {
                    fragmentDict.Add(scanTech.key, scanTech.totalFragments);

                    if (Plugin.TweakedTechs.TryGetValue(scanTech.key, out int newFragmentCount))
                    {
                        if (scanTech.totalFragments != newFragmentCount)
                        {
                            Plugin.Logger.LogInfo($"Changed fragment count for {scanTech.key} from {scanTech.totalFragments} to {newFragmentCount}!");
                            scanTech.totalFragments = newFragmentCount;
                        }
                    }
                }
            });
            if (!File.Exists(Plugin.TechDB))
            {
                fragmentDict.SaveJson(Plugin.TechDB);
            }
        }
    }
}
