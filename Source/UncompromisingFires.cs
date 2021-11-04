using HarmonyLib;
using UnityEngine;
using Verse;

namespace UncompromisingFires
{
    public class UncompromisingFires : Mod
    {
        public UncompromisingFires(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony(content.PackageId);
            harmony.PatchAll();
            GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "UncompromisingFires_Title".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }

        /// <summary>For UI modders who want to add support for this mod.</summary>
        /// <returns>
        /// Returns a string containing the map's "dryness" for display in the Weather GUI.
        /// e.g. "Humid", "Fair", "Dry", "100%"
        /// </returns>
        public static string GetGUIDrynessLabel()
        {
            if (WeatherManager_DoWeatherGUI.guiDrynessLabel != null)
                return WeatherManager_DoWeatherGUI.guiDrynessLabel.Remove(0, 2);
            else
                return null;
        }

        /// <summary>For UI modders who want to add support for this mod.</summary>
        /// <returns>
        /// Returns a string containing extended tooltip information, for display in the Weather GUI's tooltip.
        /// e.g. "Current dryness, Increase per day, Days since last rain, Days average until rain"
        /// </returns>
        public static string GetGUITooltipLabel()
        {
            if (WeatherManager_DoWeatherGUI.tooltipLabel != null)
                return WeatherManager_DoWeatherGUI.tooltipLabel.Remove(0, 2);
            else
                return null;
        }
    }
}