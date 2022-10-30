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
    [HarmonyPatch(typeof(FireWatcher), "FireWatcherTick")]
    internal static class FireWatcher_FireWatcherTick
    {
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
                        codes[i].operand = 60000;
                    }
                    else
                    {
                        Log.Message($"[Fuu] Uncompromising Fires: FireWatcherTick() Transpiler failed to apply. Operand did not match 426, operand is: {codes[i].operand}");
                    }
                }
            }
            return codes.AsEnumerable();
        }
    }
}