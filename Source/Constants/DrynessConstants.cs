using Verse;

namespace UncompromisingFires
{
	internal class DrynessConstants
	{
		//Used for initial map generation dryness calculation only:
		//First value: Tile temperature
		//Second value: Flammability multiplier applied to fire growth and spread
		public static readonly SimpleCurve InitialDrynessFromTemperature = new SimpleCurve
		{
			new CurvePoint(-20f, Settings.MaxDryness * (1f / 12f)),
			new CurvePoint(0f, Settings.MaxDryness * (1f / 6f)),
			new CurvePoint(20f, Settings.MaxDryness * (1f / 3f)),
			new CurvePoint(40f, Settings.MaxDryness * (2f / 3f)),
			new CurvePoint(60f, Settings.MaxDryness)
		};

		//To prevent humid regions like jungles from becoming unrealistically dry, by decreasing dryness gain as the map dryness reaches the following cap
		//First value: Tile average rainfall
		//Second value: Threshold at which map dryness gain reaches slowest level 
		public static readonly SimpleCurve drynessCapFromRainfall = new SimpleCurve
		{
			new CurvePoint(0f, Settings.MaxDryness * (3f / 2f)), //400%
			new CurvePoint(1000f, Settings.MaxDryness), //300%
			new CurvePoint(2000f, Settings.MaxDryness * (2f / 3f)), //200%
			new CurvePoint(3000f, Settings.MaxDryness * (1f / 3f)) //100%
		};

		//Used to determine the rate to increase map dryness. 
		//First value: Average map temperature
		//Second value: How many days after a rain until the map reaches max dryness
		public static readonly SimpleCurve daysUntilMaxDrynessForTemperature = new SimpleCurve
		{
			new CurvePoint(-20f, 80f),
			new CurvePoint(0f, 40f),
			new CurvePoint(20f, 20f),
			new CurvePoint(40f, 10f),
			new CurvePoint(60f, 5f)
		};

		//When map dryness approaches or goes over the drynessCapFromRainfall, slow the dryness increase by this value
		public const float overcapMultiplier = 0.33f;
	}
}
