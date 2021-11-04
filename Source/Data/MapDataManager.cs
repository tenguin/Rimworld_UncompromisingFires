using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace UncompromisingFires
{
    internal class MapDataManager
    {
        private static WorldStorageComponent worldStorageComponent;

        public static void OnWorldLoaded(World world)
        {
            worldStorageComponent = world.GetComponent<WorldStorageComponent>();
        }

        public static float GetMapDryness(Map map)
        {
            return GetMapDrynessData(map).MapDryness;
        }

        public static float GetDrynessPerDayAtTemp(Map map)
        {
            return GetMapDrynessData(map).DrynessPerDayAtTemp;
        }

        public static void SetMapDryness(Map map, float dryness, float drynessPerDayAtTemp)
        {
            MapDrynessData mapDrynessData = GetMapDrynessData(map);
            if (dryness == Settings.MinDryness && mapDrynessData.MapDryness != Settings.MinDryness)
            {
                //Log.Message($"SetMapDryness: It's raining! Old Dryness: {Settings.MakeHumanReadable(mapDrynessData.MapDryness)}% RainfallHistory: {mapDrynessData.DaysBetweenEachRainfall.ToStringSafeEnumerable()}");
                mapDrynessData.DaysBetweenEachRainfall.Add((int)Math.Round(GenDate.TicksToDays(Find.TickManager.TicksGame - mapDrynessData.TickOfLastRain), MidpointRounding.AwayFromZero));
                if (mapDrynessData.DaysBetweenEachRainfall.Count > 10)
                {
                    mapDrynessData.DaysBetweenEachRainfall.RemoveAt(0);
                }
            }
            mapDrynessData.MapDryness = dryness;
            mapDrynessData.DrynessPerDayAtTemp = drynessPerDayAtTemp;
        }

        public static void SetDaysSinceLastRain(Map map)
        {
            GetMapDrynessData(map).TickOfLastRain = Find.TickManager.TicksGame;
            //Log.Message($"SetDaysSinceLastRain: {Find.TickManager.TicksGame}");
        }

        public static int GetDaysSinceLastRain(Map map)
        {
            //Log.Message($"GetDaysSinceLastRain: CurrTick: {Find.TickManager.TicksGame} - LastRain: {GetMapDrynessData(map).TickOfLastRain} = {Find.TickManager.TicksGame - GetMapDrynessData(map).TickOfLastRain} toDays: {GenDate.TicksToDays(Find.TickManager.TicksGame - GetMapDrynessData(map).TickOfLastRain)}");
            return (int)Math.Round(GenDate.TicksToDays(Find.TickManager.TicksGame - GetMapDrynessData(map).TickOfLastRain), MidpointRounding.AwayFromZero);
        }

        public static int GetDaysOnAverageBetweenEachRainfall(Map map)
        {
            float averageDays = 0f;
            List<int> history = GetMapDrynessData(map).DaysBetweenEachRainfall;
            if (history != null)
            {
                foreach (int days in history)
                {
                    averageDays += days;
                }
                if (history.Count > 0)
                {
                    //Log.Message($"GetAveDaysUntilLastRainfall: history: {history.ToStringSafeEnumerable()} average: {(int)Math.Round(averageDays / history.Count, MidpointRounding.AwayFromZero)} dryness: {Settings.MakeHumanReadable(GetMapDryness(map))}%");
                    return (int)Math.Round(averageDays / history.Count, MidpointRounding.AwayFromZero);
                }
            }
            return 0;
        }

        private static MapDrynessData GetMapDrynessData(Map map)
        {
            worldStorageComponent.MapDrynessDataDictionary.TryGetValue(map.uniqueID, out MapDrynessData mapDrynessData);
            if (mapDrynessData == null)
            {
                mapDrynessData = new MapDrynessData(map);
                worldStorageComponent.MapDrynessDataDictionary.Add(map.uniqueID, mapDrynessData);
            }
            return mapDrynessData;
        }
    }
}
