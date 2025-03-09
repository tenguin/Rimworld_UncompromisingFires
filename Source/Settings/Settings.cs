using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace UncompromisingFires
{
    internal class Settings : ModSettings
    {
        public const float MinDryness = 0f;
        public static float MaxDryness;
        public static float IndoorDryness;
        public static int FireWatcherTick;
        public static int SeasonalShift;
        public static UIDisplayOptions uiDisplayOption;

        public static float EvaporationRateMultiplier;
        private static void Initialize()
        {
            EvaporationRateMultiplier = 1f;
            MaxDryness = 2f;
            IndoorDryness = 2f;
            FireWatcherTick = 30000;
            SeasonalShift = 0;
            uiDisplayOption = UIDisplayOptions.Standard;
        }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref EvaporationRateMultiplier, "EvaporationRateMultiplier", 1f);
            Scribe_Values.Look(ref MaxDryness, "MaxDryness", 2f);
            Scribe_Values.Look(ref IndoorDryness, "IndoorDryness", 2f);
            Scribe_Values.Look(ref FireWatcherTick, "FireWatcherTick", 30000);
            Scribe_Values.Look(ref SeasonalShift, "SeasonalShift", 0);
            Scribe_Values.Look(ref uiDisplayOption, "uiDisplayOptions", UIDisplayOptions.Standard);
        }
        public enum UIDisplayOptions : byte
        {
            Standard,
            Percentage,
            TooltipOnly,
            Off
        }
        private static string TranslateUIDisplayOptions(UIDisplayOptions mode)
        {
            switch (mode)
            {
                case UIDisplayOptions.Standard:
                    return "UncompromisingFires_UIStandard".Translate();
                case UIDisplayOptions.Percentage:
                    return "UncompromisingFires_UIPercentage".Translate();
                case UIDisplayOptions.TooltipOnly:
                    return "UncompromisingFires_UITooltipOnly".Translate();
                case UIDisplayOptions.Off:
                    return "UncompromisingFires_UIOff".Translate();
                default:
                    throw new NotImplementedException();
            }
        }
        public Settings()
        {
            Initialize();
        }
        public static void DoWindowContents(Rect inRect)
        {
            //30f for top page description and bottom close button
            Rect viewRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.ColumnWidth = viewRect.width;
            listingStandard.Begin(viewRect);
            listingStandard.Gap(5f);

            //Indoor dryness
            listingStandard.Label("UncompromisingFires_IndoorDryness".Translate() + ": " + MakeHumanReadable(IndoorDryness) + "%");
            IndoorDryness = (float)Math.Round(listingStandard.Slider(IndoorDryness, 0.5f, 6f), 1, MidpointRounding.AwayFromZero);

            //Max dryness
            listingStandard.Label("UncompromisingFires_MaxDryness".Translate() + ": " + MakeHumanReadable(MaxDryness) + "%");
            MaxDryness = (float)Math.Round(listingStandard.Slider(MaxDryness, 0.5f, 6f), 1, MidpointRounding.AwayFromZero);

            //Firewatcher ticks
            listingStandard.Label("UncompromisingFires_Firewatcher".Translate() + ": " + (float)Math.Round(FireWatcherTick / 60000f, 1, MidpointRounding.AwayFromZero) + " days", -1f, "UncompromisingFires_FirewatcherDesc".Translate());
            FireWatcherTick = (int)listingStandard.Slider(FireWatcherTick, 2500, 600000);

            //Seasonal Shift
            listingStandard.Label("UncompromisingFires_SeasonalShift".Translate() + ": " + SeasonalShift + "°C");
            SeasonalShift = (int)listingStandard.Slider(SeasonalShift, 0, 50);

            //Evaporation Multiplier
           // listingStandard.Gap(5f);
            listingStandard.Label("UncompromisingFires_SettingsDescription".Translate());
            listingStandard.Label("UncompromisingFires_EvaporationRateMultiplier".Translate() + ": " + Mathf.RoundToInt(EvaporationRateMultiplier * 100f) + "%", -1f, "UncompromisingFires_EvaporationRateMultiplierDesc".Translate());
            EvaporationRateMultiplier = listingStandard.Slider(EvaporationRateMultiplier, 0.1f, 5f);
            listingStandard.Gap(5f);
            listingStandard.Label("UncompromisingFires_EvaporationRateMultiplierDesc".Translate());
            listingStandard.Gap(5f);
            foreach (CurvePoint curvePoint in DrynessConstants.daysUntilMaxDrynessForTemperature)
            {
                listingStandard.Label("     " + "UncompromisingFires_EvaporationRatePredictions".Translate(
                    curvePoint.x,
                    Mathf.RoundToInt(GenTemperature.CelsiusTo(curvePoint.x, TemperatureDisplayMode.Fahrenheit)),
                    Math.Round(curvePoint.y / EvaporationRateMultiplier, 1)));
            }

            //UI Options Dropdown
            listingStandard.Gap(5f);
            if (listingStandard.ButtonTextLabeled("UncompromisingFires_UIOptionsDropdown".Translate() + ":", TranslateUIDisplayOptions(uiDisplayOption)))
            {
                List<FloatMenuOption> menuOptions = new List<FloatMenuOption>();
                foreach (UIDisplayOptions currentOption in Enum.GetValues(typeof(UIDisplayOptions)))
                {
                    menuOptions.Add(new FloatMenuOption(TranslateUIDisplayOptions(currentOption), delegate
                    {
                        if (uiDisplayOption != currentOption)
                        {
                            uiDisplayOption = currentOption;
                        }
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(menuOptions));
            }

            listingStandard.Gap(5f);
            if (listingStandard.ButtonText("UncompromisingFires_ResetAll".Translate()))
            {
                Initialize();
            }
            listingStandard.End();
        }

        public static float MakeHumanReadable(float drynessFloat)
        {
            return (float)Math.Round(drynessFloat * 100f, MidpointRounding.AwayFromZero);
        }

    }
}
