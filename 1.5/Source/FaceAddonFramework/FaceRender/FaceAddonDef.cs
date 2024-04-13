using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static Verse.PawnRenderNodeProperties;

namespace FaceAddon
{
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

        public List<Color> customColorsBase;
        public List<Color> customColorsSub;

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

    public class FaceAddonDef : Def
    {
        public ShaderTypeDef shaderType;
        public List<RenderSkipFlagDef> useSkipFlags = null;
        public List<Rot4> visibleFacing;

        public bool displayOnRot = true;
        public bool useRottenColor = false;
        public bool displayOnDessicated = false;

        public float layerOffset = 0;

        public AttachmentColorType colorBase = AttachmentColorType.Custom;
        public AttachmentColorType colorSub = AttachmentColorType.Custom;
        public int damageAnimDuration = 30;
        public List<MentalStateDef> handleMentalBreakState = new List<MentalStateDef>()
        {
            MentalStateDefOf.Berserk,
            MentalStateDefOf.Manhunter,
            MentalStateDefOf.ManhunterPermanent,
        };

        HashSet<MentalStateDef> handleMentalBreak;
        public HashSet<MentalStateDef> HandleMentalBreak => handleMentalBreak;

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
            handleMentalBreak = handleMentalBreakState.ToHashSet();
        }

        public bool CanDrawAddon(PawnDrawParms parms)
        {
            if (parms.flags.FlagSet(PawnRenderFlags.HeadStump))
                return false;
            if (!displayOnDessicated && parms.pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated)
                return false;
            if (!displayOnRot && parms.pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting)
                return false;

            return true;
        }
    }
}