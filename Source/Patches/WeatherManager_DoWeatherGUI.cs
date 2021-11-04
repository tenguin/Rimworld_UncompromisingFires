using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UncompromisingFires
{
    [HarmonyPatch(typeof(WeatherManager), "DoWeatherGUI")]
    internal static class WeatherManager_DoWeatherGUI
    {
        private const int FramesBetweenGuiLabelCalculations = 57; //~1s at 60fps
        public static int currentMapID = -1;
        public static string guiDrynessLabel;
        public static string tooltipLabel;

        //Append dryness info to vanilla's Weather GUI
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                //Append guiDrynessLabel to the weather label
                if (codes[i].opcode == OpCodes.Callvirt &&
                    (MethodInfo)codes[i].operand == AccessTools.Method(typeof(Def), "get_LabelCap"))
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(WeatherManager_DoWeatherGUI), nameof(guiDrynessLabel))));
                    codes.Insert(i + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TaggedString), "op_Addition", new Type[] { typeof(TaggedString), typeof(string) })));
                }

                //Append description info to the weather tooltip
                if (codes[i].opcode == OpCodes.Call &&
                    codes[i].operand is MethodInfo operand && operand == AccessTools.Method(typeof(TipSignal), "op_Implicit", new Type[] { typeof(string) }))
                {
                    codes.Insert(i, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(WeatherManager_DoWeatherGUI), nameof(tooltipLabel))));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), "Concat", new Type[] { typeof(string), typeof(string) })));
                    break;
                }
            }
            return codes.AsEnumerable();
        }

        [HarmonyPrefix]
        private static void Prefix_DoWeatherGUI(WeatherManager __instance, ref Rect rect)
        {
            if (RealTime.frameCount % FramesBetweenGuiLabelCalculations == 0) //Performance: Don't need to run every tick
            {
                //Force a label update if map changes
                UpdateGuiDrynessLabel(__instance);
                if (__instance.map.uniqueID != currentMapID)
                {
                    UpdateTooltipLabel(__instance);
                    currentMapID = __instance.map.uniqueID;
                }
            }
        }

        internal static void UpdateGuiDrynessLabel(WeatherManager instance)
        {
            if (Settings.uiDisplayOption == Settings.UIDisplayOptions.Standard)
            {
                float mapDryness = MapDataManager.GetMapDryness(instance.map);
                if (mapDryness < Settings.MaxDryness * 0.17f)
                    guiDrynessLabel = Translations.noDryness;
                else if (mapDryness < Settings.MaxDryness * 0.33f)
                    guiDrynessLabel = Translations.lowDryness;
                else if (mapDryness < Settings.MaxDryness * 0.5f)
                    guiDrynessLabel = Translations.moderateDryness;
                else if (mapDryness < Settings.MaxDryness * 0.67f)
                    guiDrynessLabel = Translations.highDryness;
                else if (mapDryness < Settings.MaxDryness * 0.83f)
                    guiDrynessLabel = Translations.veryHighDryness;
                else
                    guiDrynessLabel = Translations.severeDryness;
            }
            else if (Settings.uiDisplayOption == Settings.UIDisplayOptions.Percentage)
            {
                guiDrynessLabel = ", " + Settings.MakeHumanReadable(MapDataManager.GetMapDryness(instance.map)) + "%";
            }
            else
            {
                guiDrynessLabel = null;
            }
        }

        internal static void UpdateTooltipLabel(WeatherManager instance)
        {
            if (Settings.uiDisplayOption != Settings.UIDisplayOptions.Off)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("\n\n");
                builder.Append(Translations.currentDrynessTooltip);
                builder.Append(": ");
                builder.Append(Settings.MakeHumanReadable(MapDataManager.GetMapDryness(instance.map)));
                builder.Append("%\n");
                builder.Append(Translations.increasePerDayTooltip);
                builder.Append(": ");
                builder.Append(Settings.MakeHumanReadable(MapDataManager.GetDrynessPerDayAtTemp(instance.map)));
                builder.Append("%\n");
                builder.Append(MapDataManager.GetDaysSinceLastRain(instance.map));
                builder.Append(" ");
                builder.Append(Translations.daysSinceLastRainTooltip);
                int aveDays = MapDataManager.GetDaysOnAverageBetweenEachRainfall(instance.map);
                if (aveDays > 0)
                {
                    builder.Append("\n");
                    builder.Append(aveDays);
                    builder.Append(" ");
                    builder.Append(Translations.averageDaysUntilRainfallTooltip);
                }
                tooltipLabel = builder.ToString();
            }
            else
            {
                tooltipLabel = null;
            }
        }
    }
}