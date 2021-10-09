using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System;

namespace UncompromisingFires
{
    [HarmonyPatch(typeof(WeatherManager), "DoWeatherGUI")]
    internal static class WeatherManager_DoWeatherGUI
    {
        private const int FramesBetweenGuiLabelCalculations = 27; //~0.5s at 60fps

        private static String guiDrynessLabel;
        private static String daysSinceLastRainLabel;
        private static String averageDaysUntilRainfallLabel;
        private static String currentDrynessLabel;
        private static String drynessIncreaseLabel;

        //Translate is really expensive if updated every frame mid-game, so do it only once
        private static readonly String guiIndicatorTooltip = "UncompromisingFires_GuiIndicatorTooltip".Translate();
        private static readonly String currentDrynessTooltip = "UncompromisingFires_CurrentDrynessTooltip".Translate();
        private static readonly String increasePerDayTooltip = "UncompromisingFires_IncreasePerDayTooltip".Translate();
        private static readonly String daysSinceLastRainTooltip = "UncompromisingFires_DaysSinceLastRainTooltip".Translate();
        private static readonly String averageDaysUntilRainfallTooltip = "UncompromisingFires_AverageDaysUntilRainfallTooltip".Translate();
        private static readonly String noDryness = "UncompromisingFires_NoDryness".Translate();
        private static readonly String lowDryness = "UncompromisingFires_LowDryness".Translate();
        private static readonly String moderateDryness = "UncompromisingFires_ModerateDryness".Translate();
        private static readonly String highDryness = "UncompromisingFires_HighDryness".Translate();
        private static readonly String veryHighDryness = "UncompromisingFires_VeryHighDryness".Translate();
        private static readonly String severeDryness = "UncompromisingFires_SevereDryness".Translate();

        //Add a new indicator in GUI to show the current fire risk on the map
        [HarmonyPrefix]
        private static void Prefix_DoWeatherGUI(WeatherManager __instance, ref Rect rect)
        {
            //Move the vanilla Weather indicator to make room for our new fire risk indicator
            rect.x -= 10f; //20f; //Move to the left
            rect.width /= 1.4f; //Width smaller
        }

        [HarmonyPostfix]
        private static void Postfix_DoWeatherGUI(WeatherManager __instance, ref Rect rect)
        {
            Rect rect2 = new Rect(rect);
            rect2.x += rect2.width; //Draw the fire risk indicator where the vanilla Weather indicator used to be
            rect2.width /= 2.6f; //2.3f; //original width 230
            Text.Anchor = TextAnchor.MiddleRight;
            Text.Font = GameFont.Small;
            UpdateGuiDrynessLabel(__instance);
            Widgets.Label(rect2, guiDrynessLabel);
            TooltipHandler.TipRegion(rect2, guiIndicatorTooltip + currentDrynessLabel + drynessIncreaseLabel + daysSinceLastRainLabel + averageDaysUntilRainfallLabel);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void UpdateGuiDrynessLabel(WeatherManager instance)
        {
            if (RealTime.frameCount % FramesBetweenGuiLabelCalculations == 0) //Performance: Don't need to run calculation every tick
            {
                float mapDryness = MapDataManager.GetMapDryness(instance.map);
                if (mapDryness < Settings.MaxDryness * 0.17f)
                    guiDrynessLabel = noDryness; 
                else if (mapDryness < Settings.MaxDryness * 0.33f)
                    guiDrynessLabel = lowDryness; 
                else if (mapDryness < Settings.MaxDryness * 0.5f)
                    guiDrynessLabel = moderateDryness; 
                else if (mapDryness < Settings.MaxDryness * 0.67f)
                    guiDrynessLabel = highDryness; 
                else if (mapDryness < Settings.MaxDryness * 0.83f)
                    guiDrynessLabel = veryHighDryness; 
                else
                    guiDrynessLabel = severeDryness; 
            }
        }

        internal static void UpdateDynamicLabels(WeatherManager instance, float drynessPerDayAtTemp)
        {
            currentDrynessLabel = "\n\n" + currentDrynessTooltip + ": " + Settings.MakeHumanReadable(MapDataManager.GetMapDryness(instance.map)) + "%\n\n";
            drynessIncreaseLabel = increasePerDayTooltip + ": " + Settings.MakeHumanReadable(drynessPerDayAtTemp) + "%\n\n";
            daysSinceLastRainLabel = MapDataManager.GetDaysSinceLastRain(instance.map) + " " + daysSinceLastRainTooltip;
            int aveDays = MapDataManager.GetDaysOnAverageBetweenEachRainfall(instance.map);
            if (aveDays > 0)
            {
                averageDaysUntilRainfallLabel = "\n\n" + aveDays + " " + averageDaysUntilRainfallTooltip;
            }
            else
            {
                averageDaysUntilRainfallLabel = null;
            }
        }
    }
}