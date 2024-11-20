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