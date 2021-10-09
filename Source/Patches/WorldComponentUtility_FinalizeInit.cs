using HarmonyLib;
using RimWorld.Planet;

namespace UncompromisingFires
{
    [HarmonyPatch(typeof(WorldComponentUtility), "FinalizeInit")]
    internal static class WorldComponentUtility_FinalizeInit
    {
        [HarmonyPrefix]
        private static void FinalizeInit(World world)
        {
            MapDataManager.OnWorldLoaded(world);
        }
    }
}