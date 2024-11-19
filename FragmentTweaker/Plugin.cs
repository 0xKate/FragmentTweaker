using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json.ExtensionMethods;

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
                if (scanTech.isFragment && scanTech.totalFragments < 20)
                {
                    fragmentDict.Add(scanTech.key, scanTech.totalFragments);

                    if (Plugin.TweakedTechs.TryGetValue(scanTech.key, out int newFragmentCount))
                    {
                        if (scanTech.totalFragments != newFragmentCount)
                        {
                            Plugin.Logger.LogInfo($"Increased fragment count for {scanTech.key} from {scanTech.totalFragments} to {newFragmentCount}!");
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

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static string Directory { get; } = Path.GetDirectoryName(Assembly.Location);
        public static string TechDB { get; } = Directory + "/ScanFragments.json";
        public static Dictionary<TechType, int> TweakedTechs { get; } = new Dictionary<TechType, int>();

        public void Awake()
        {
            Logger = base.Logger;

            Initialize();

            if (!File.Exists(TechDB))
            {
                Logger.LogWarning("Missing ScanFragments.json! Will be generated on next save load.");
                Logger.LogInfo($"ScanFragments.json expected @ {TechDB}");
            }

            OptionsPanelHandler.RegisterModOptions(new Settings());

            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Initialize()
        {
            if (File.Exists(Plugin.TechDB))
            {
                TweakedTechs.LoadJson(Plugin.TechDB);
            }

            Logger.LogInfo($"Handling {TweakedTechs.Count} fragments!");
        }
    }
}