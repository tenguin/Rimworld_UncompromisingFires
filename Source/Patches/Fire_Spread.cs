using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace UncompromisingFires
{
    [HarmonyPatch(typeof(Fire))]
    internal static class Fire_Spread
    {
        private const int TicksBetweenSpreadCalculations = 10; //Can be a small performance hit when thousands of fire objects are all calling per tick

        //Scale fire spread rate based on dryness
        [HarmonyPostfix]
        [HarmonyPatch("Tick")]
        private static void Tick(Fire __instance, ref int ___ticksSinceSpread)
        {
            if (__instance.IsHashIntervalTick(TicksBetweenSpreadCalculations) && __instance.fireSize > 1f)
            {
                //Need to manually set rounding mode or it defaults to ToEven rounding which skews the results
                ___ticksSinceSpread += (int)Math.Round(GetFireDryness(__instance) * TicksBetweenSpreadCalculations, MidpointRounding.AwayFromZero);
            }
        }

        //Scale fire growth rate based on dryness
        [HarmonyPostfix]
        [HarmonyPatch("DoComplexCalcs")]
        private static void DoComplexCalcs(Fire __instance, ref float ___flammabilityMax)
        {
            if (__instance.Spawned)
            {
                __instance.fireSize += 0.00055f * ___flammabilityMax * 150f * GetFireDryness(__instance);
                if (__instance.fireSize > 1.75f)
                {
                    __instance.fireSize = 1.75f;
                }
            }
        }

        //Reverse patch to access private method
        [HarmonyReversePatch]
        [HarmonyPatch("VulnerableToRain")]
        private static bool VulnerableToRain(Fire instance)
        {
            throw new NotImplementedException("It's a stub");
        }

        //Subtract 1 because up to -1 is used to go under the 100% fire growth and spread rate from vanilla
        private static float GetFireDryness(Fire instance)
        {
            if (!VulnerableToRain(instance)) //Max flammability for anything indoors/under a roof (completely dry)
                return Settings.IndoorDryness - 1f;
            else
                return MapDataManager.GetMapDryness(instance.Map) - 1f;
        }
    }
}