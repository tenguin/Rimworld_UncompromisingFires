﻿using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace UncompromisingFires
{
    //Performance Warning: WeatherManagerTick is called every tick (60 times per second)
    [HarmonyPatch(typeof(WeatherManager), "WeatherManagerTick")]
    internal static class WeatherManager_WeatherManagerTick
    {
        private const int TicksBetweenDrynessCalculations = 2593; //~42s, 1 ingame hour

        [HarmonyPostfix]
        private static void WeatherManagerTick(WeatherManager __instance)
        {
            if (Find.TickManager.TicksGame % TicksBetweenDrynessCalculations == 0) //Performance: Don't need to run calculation every tick
            {
                Map map = __instance.map;
                if (__instance.RainRate > 0.01f) //Raining
                {
                    MapDataManager.SetMapDryness(map, Settings.MinDryness, 0f);
                    MapDataManager.SetDaysSinceLastRain(map);
                    //Log.Message($"{__instance.map}: isRaining: Dryness: {MapDataManager.GetMapDryness(__instance.map)}");
                }
                else //Increase map dryness due to evaporation
                {
                    //Calculate the amount of dryness to increase based on map temperature
                    float currentTemperature = GenTemperature.GetTemperatureAtTile(map.Tile);
                    float totalDryness = Settings.MaxDryness - Settings.MinDryness;
                    float daysToReachMaxDrynessAtTemp = DrynessConstants.daysUntilMaxDrynessForTemperature.Evaluate(currentTemperature) / Settings.EvaporationRateMultiplier;
                    float drynessPerDayAtTemp = totalDryness / daysToReachMaxDrynessAtTemp;
                    float totalCalculationsPerDay = GenDate.TicksPerDay / TicksBetweenDrynessCalculations;
                    float drynessIncrease = drynessPerDayAtTemp / totalCalculationsPerDay;

                    //Increasingly lower the drynessIncrease as map dryness reaches the cap for the average rainfall of the map (To prevent humid regions from getting unrealistically dry)
                    float currentDryness = MapDataManager.GetMapDryness(map);
                    float drynessCap = DrynessConstants.drynessCapFromRainfall.Evaluate(map.TileInfo.rainfall);
                    float overcapMultiplier = Mathf.Clamp(drynessCap - currentDryness, 0f, 1f - DrynessConstants.overcapMultiplier) + DrynessConstants.overcapMultiplier;

                    drynessIncrease *= overcapMultiplier;
                    MapDataManager.SetMapDryness(map, currentDryness + drynessIncrease, drynessPerDayAtTemp * overcapMultiplier);
                    //Log.Message($"{__instance.map}: currDry: {currentDryness}, drynessCap: {drynessCap}, capMulti: {overcapMultiplier} dailyDrynessIncrease: {drynessIncrease} daysToReachMaxDrynessAtTemp: {daysToReachMaxDrynessAtTemp}");
                }
                WeatherManager_DoWeatherGUI.UpdateTooltipLabel(__instance);
            }
        }
    }
}