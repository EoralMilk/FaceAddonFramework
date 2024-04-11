using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FaceAddon
{
    public class CompProperties_FaceAddonComps : CompProperties
    {
        public List<FaceTypeDef> faceTypeDefs;
        public List<FaceTypeDef> additionalfaceTypeDefs;

        public List<FaceAddonDef> additionalFaceAddonDefs;

        public CompProperties_FaceAddonComps()
        {
            compClass = typeof(FaceAddonComp);
        }
    }

    [StaticConstructorOnStartup]
    public class FaceAddonComp : ThingComp
    {
        public string pawnGeneratedVersion = "0.0.0-000000";

        public Pawn Pawn { private set; get; }
        public FaceTypeDef facetype;
        public FaceTypeDef additionalfacetype;

        public RaceAddonGraphicSet raceAddonGraphicSet;

        public override void CompTick()
        {
            base.CompTick();
            raceAddonGraphicSet?.Update_Normal();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            raceAddonGraphicSet?.Update_Rare();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn = parent as Pawn;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref facetype, "facetype");
            Scribe_Defs.Look(ref additionalfacetype, "additionalfacetype");

            Scribe_Values.Look(ref pawnGeneratedVersion, "pawnGeneratedVersion");
        }
    }

    public class FaceTypeDef : Def
    {
        public FaceAddonDef Upper;
        public FaceAddonDef Lower;
        public FaceAddonDef Attach;

        public int tickBlinkMin = 120;
        public int tickBlinkMax = 240;
        public int blinkDurationMin = 15;
        public int blinkDurationMax = 35;
        public float winkChance = 0.3f;

        public int randomWeight = 1;
        public List<HeadTypeDef> requireHeadTypes;
    }

    public class CustomPath
    {
        public HediffDef hediffDef;
        [NoTranslate]
        public string path;
        public int priority = 0;
    }

    public enum ColorType : byte
    {
        White,
        Skin,
        Hair,
    }

    public class FaceAddonDef : Def
    {
        public ShaderTypeDef shaderType;
        public bool displayOnRot = true;
        public bool displayOnDessicated = false;
        public int layerOffset = 0;
        public bool layerOffsetAlwaysPositive = false;

        public bool fixedOnBlink = false;
        public ColorType colorBase = ColorType.White;
        public ColorType colorSub = ColorType.White;

        [NoTranslate]
        public string fixedPath; // if you use this path, the face addon will only draw fixed path texture and won't use other texture

        [NoTranslate]
        public string mentalBreakPath;
        [NoTranslate]
        public string aboutToBreakPath;
        [NoTranslate]
        public string onEdgePath;
        [NoTranslate]
        public string stressedPath;
        [NoTranslate]
        public string neutralPath;
        [NoTranslate]
        public string contentPath;
        [NoTranslate]
        public string happyPath;
        [NoTranslate]
        public string sleepingPath;
        [NoTranslate]
        public string painShockPath;
        [NoTranslate]
        public string deadPath;
        [NoTranslate]
        public string blinkPath;
        [NoTranslate]
        public string winkPath;
        [NoTranslate]
        public string draftedPath;
        [NoTranslate]
        public string damagedPath;
        [NoTranslate]
        public string attackingPath;

        public List<CustomPath> customs = new List<CustomPath>();

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (shaderType == null)
            {
                shaderType = ShaderTypeDefOf.Cutout;
            }
        }
    }
}