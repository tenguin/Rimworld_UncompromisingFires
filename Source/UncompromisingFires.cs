using HarmonyLib;
using UnityEngine;
using Verse;

namespace UncompromisingFires
{
    public class UncompromisingFires : Mod
    {       
        public UncompromisingFires(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony(content.PackageId);
            harmony.PatchAll();
            GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "UncompromisingFires_Title".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }
    }
}