using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace UncompromisingFires
{
    //Stores the dryness data for a map
    internal class MapDrynessData : IExposable
    {
        private Map Map;
        private float mapDryness = 0f;
        private List<int> daysBetweenEachRainfall = new List<int> { };
        public int TickOfLastRain = 0;
        public float DrynessPerDayAtTemp = 0f;
        public float MapDryness
        {
            get => mapDryness;
            set => mapDryness = Mathf.Clamp(value, Settings.MinDryness, Settings.MaxDryness);
        }
        public List<int> DaysBetweenEachRainfall
        {
            get => daysBetweenEachRainfall ?? (daysBetweenEachRainfall = new List<int> { });
        }

        public MapDrynessData() { }

        //When a map is first generated, pick an initial dryness level based on the map's temperature and average rainfall, then add some randomness.
        public MapDrynessData(Map map)
        {
            Map = map;
            TickOfLastRain = Find.TickManager.TicksGame;

            float averageTemperature = GenTemperature.AverageTemperatureAtTileForTwelfth(map.Tile, GenLocalDate.Twelfth(map));
            float flammability = DrynessConstants.InitialDrynessFromTemperature.Evaluate(averageTemperature);
            float cappedFlammability = Mathf.Min(flammability, DrynessConstants.drynessCapFromRainfall.Evaluate(map.TileInfo.rainfall));
            float overCap = (flammability - cappedFlammability) * DrynessConstants.overcapMultiplier;
            MapDryness = cappedFlammability + overCap;
            MapDryness += Rand.Range(-0.5f, 0.5f);

            float totalDryness = Settings.MaxDryness - Settings.MinDryness;
            float daysToReachMaxDrynessAtTemp = DrynessConstants.daysUntilMaxDrynessForTemperature.Evaluate(averageTemperature) / Settings.EvaporationRateMultiplier;
            float drynessPerDayAtTemp = totalDryness / daysToReachMaxDrynessAtTemp;
            DrynessPerDayAtTemp = drynessPerDayAtTemp;
            //Log.Message($"CreatingNewMapDrynessData: {map} initialDryness: {cappedFlammability + overCap} afterRandDryness: {mapDryness} tickOfLastRain: {TickOfLastRain}");
        }

        public void ExposeData()
        {
            Scribe_References.Look<Map>(ref Map, "Map");
            Scribe_Values.Look(ref mapDryness, "MapDryness", 0f);
            Scribe_Values.Look(ref TickOfLastRain, "TickOfLastRain", 0);
            Scribe_Values.Look(ref DrynessPerDayAtTemp, "DrynessPerDayAtTemp", 0f);
            Scribe_Collections.Look(ref daysBetweenEachRainfall, "DaysBetweenEachRainfall", LookMode.Value);
        }

        internal bool HasValidMap
        {
            get
            {
                List<Map> mapList = Find.Maps; //Find all active maps in the game
                if (mapList.Find(m => m == Map) == null) //Check if our MapDrynessData is referring to an abandoned map
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
