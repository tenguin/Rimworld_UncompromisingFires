using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace UncompromisingFires
{
    //Want a much longer delay between updating FireDanger value, 
    //so fires have time to burn instead of immediately raining after one starts
    [HarmonyPatch(typeof(FireWatcher))]
    internal static class FireWatcher_FireWatcherTick
    {
        [HarmonyPatch("FireWatcherTick")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4)
                {
                    int ticksBetweenOperation = (int)codes[i].operand;
                    if (ticksBetweenOperation == 426)
                    {
                        codes[i].operand = Settings.FireWatcherTick;
                    }
                    else
                    {
                        Log.Message($"Uncompromising Fires: FireWatcherTick() Transpiler failed to apply. Operand did not match 426, operand is: {codes[i].operand}");
                    }
                }
            }
            return codes.AsEnumerable();
        }

        //Needed to prevent an immediate update of fire values, causing instant rain when a game is loaded, since this will be repeatedly called by WeatherDecider when FireDanger data is -1 due to missing data from a fresh game load.
        [HarmonyPrefix]
        [HarmonyPatch("LargeFireDangerPresent", MethodType.Getter)]
        private static bool LargeFireDangerPresent(FireWatcher __instance, ref bool __result)
        {
            __result = __instance.FireDanger > 90f;
            //Log.Message($"FireDanger:{__instance.FireDanger}, LargeFireDanger:{__result}, Ticks:{Find.TickManager.TicksGame}, FireWatcherTicks:{Settings.FireWatcherTick}, TickModulo:{Find.TickManager.TicksGame % Settings.FireWatcherTick == 0}");
            return false;
        }
    }
}