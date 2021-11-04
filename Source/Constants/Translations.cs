using Verse;

namespace UncompromisingFires
{
    internal class Translations
    {
        //Do UI related translations here because translate is expensive if called per frame 
        public static string noDryness = ", " + "UncompromisingFires_NoDryness".Translate();
        public static string lowDryness = ", " + "UncompromisingFires_LowDryness".Translate();
        public static string moderateDryness = ", " + "UncompromisingFires_ModerateDryness".Translate();
        public static string highDryness = ", " + "UncompromisingFires_HighDryness".Translate();
        public static string veryHighDryness = ", " + "UncompromisingFires_VeryHighDryness".Translate();
        public static string severeDryness = ", " + "UncompromisingFires_SevereDryness".Translate();

        public static string daysSinceLastRainTooltip = "UncompromisingFires_DaysSinceLastRainTooltip".Translate();
        public static string currentDrynessTooltip = "UncompromisingFires_CurrentDrynessTooltip".Translate();
        public static string increasePerDayTooltip = "UncompromisingFires_IncreasePerDayTooltip".Translate();
        public static string averageDaysUntilRainfallTooltip = "UncompromisingFires_AverageDaysUntilRainfallTooltip".Translate();
    }
}
