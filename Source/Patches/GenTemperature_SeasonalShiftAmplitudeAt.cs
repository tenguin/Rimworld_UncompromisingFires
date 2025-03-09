using HarmonyLib;
using Verse;

namespace UncompromisingFires
{
    [HarmonyPatch(typeof(GenTemperature), "SeasonalShiftAmplitudeAt")]
    internal static class GenTemperature_SeasonalShiftAmplitudeAt
    {
        //ORIGINAL CURVE:
        //new CurvePoint(0f, 3f),
        //new CurvePoint(0.1f, 4f),
        //new CurvePoint(1f, 28f)
        [HarmonyPostfix]
        private static void SeasonalShiftAmplitudeAt(int tile, ref float __result)
        {
            if (Settings.SeasonalShift > 0 && __result != 0f)
            {
               // Log.Message($"TempShift:{__result}, newTemp:{__result + Settings.SeasonalShift}, YCoord:{Find.WorldGrid.LongLatOf(tile).y}, curveEval>=0:{TemperatureTuning.SeasonalTempVariationCurve.Evaluate(Find.WorldGrid.DistanceFromEquatorNormalized(tile))}, curveEval<0:{0f - TemperatureTuning.SeasonalTempVariationCurve.Evaluate(Find.WorldGrid.DistanceFromEquatorNormalized(tile))}");
                __result = __result + Settings.SeasonalShift;
            }
        }
    }
}