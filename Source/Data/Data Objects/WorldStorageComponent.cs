using System.Collections.Generic;
using Verse;
using RimWorld.Planet;

namespace UncompromisingFires
{
	//A data storage object tied to the currently generated world
	//Used to track and save map dryness data for all the active maps in the world
    public class WorldStorageComponent : WorldComponent
	{
		internal Dictionary<int, MapDrynessData> MapDrynessDataDictionary = new Dictionary<int, MapDrynessData>(); 

		public WorldStorageComponent(World world) : base(world)
		{
		}

		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				MapDrynessDataDictionary.RemoveAll(keyValuePair => keyValuePair.Value == null || !keyValuePair.Value.HasValidMap);
			}
			base.ExposeData();
			Scribe_Collections.Look(ref MapDrynessDataDictionary, "MapDrynessDataDictionary", LookMode.Value, LookMode.Deep);
		}
	}
}
