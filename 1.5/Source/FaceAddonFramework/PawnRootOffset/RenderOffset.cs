using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FaceAddon
{
    public class RenderOffset
    {
        [HarmonyPatch(typeof(PawnRenderer))]
        [HarmonyPatch("ParallelGetPreRenderResults")]
        public class RenderPawnOffsetPatch
        {
            [HarmonyPrefix]
            public static void ChangeOffset(PawnRenderer __instance, ref Vector3 drawLoc, Rot4? rotOverride = null)
            {
                var pr = Traverse.Create(__instance);
                var pawn = (Pawn)(pr.Field("pawn").GetValue());

                PawnRenderFlags pawnRenderFlags = PawnRenderFlags.None;
                if (!pawn.health.hediffSet.HasHead)
                {
                    pawnRenderFlags |= PawnRenderFlags.HeadStump;
                }
                if (pawnRenderFlags.FlagSet(PawnRenderFlags.Portrait))
                {
                    return;
                }

                var props = pawn.def.comps.FirstOrDefault(p => p is CompProperties_DrawRootOffset) as CompProperties_DrawRootOffset;
                if (props != null && (pawn.GetPosture() == PawnPosture.Standing || !props.offsetOnlyStand))
                {
                    Rot4 rot = rotOverride ?? pawn.Rotation;
                    var offset = Vector3.zero;
                    switch (rot.AsInt)
                    {
                        case 0: offset = props.northOffset; break; // North
                        case 1: offset = props.eastOffset; break; // East
                        case 2: offset = props.southOffset; break; // South
                        case 3: offset = props.westOffset; break; // West
                    }
                    offset *= props.offsetScale;
                    drawLoc += offset;
                }
            }

        }

        //[HarmonyPatch(typeof(PawnRenderer))]
        //[HarmonyPatch("RenderCache")]
        //public class RenderCacheOffsetPatch
        //{
        //    [HarmonyPrefix]
        //    public static void ChangeOffset(PawnRenderer __instance, Rot4 rotation, ref Vector3 positionOffset, bool portrait)
        //    {
        //        //if (flags.FlagSet(PawnRenderFlags.Portrait))
        //        //{
        //        //    return;
        //        //}
        //        //bool flag = __instance.GetDefaultRenderFlags(pawn);

        //        var pr = Traverse.Create(__instance);
        //        var pawn = (Pawn)(pr.Field("pawn").GetValue());
        //        var props = pawn.def.comps.FirstOrDefault(p => p is CompProperties_DrawRootOffset) as CompProperties_DrawRootOffset;
        //        if (props != null && (pawn.GetPosture() == PawnPosture.Standing || !props.offsetOnlyStand))
        //        {
        //            var offset = Vector3.zero;
        //            switch (rotation.AsInt)
        //            {
        //                case 0: offset = props.northOffset; break; // North
        //                case 1: offset = props.eastOffset; break; // East
        //                case 2: offset = props.southOffset; break; // South
        //                case 3: offset = props.westOffset; break; // West
        //            }
        //            offset *= props.offsetScale;
        //            positionOffset += offset;
        //        }
        //    }

        //}

    }


    public class CompProperties_DrawRootOffset : CompProperties
    {
        public bool offsetOnlyStand = true;
        public float offsetScale = 1;
        public Vector3 eastOffset;
        public Vector3 westOffset;
        public Vector3 southOffset;
        public Vector3 northOffset;

        public CompProperties_DrawRootOffset()
        {
            compClass = typeof(DrawRootOffsetComp);
        }
    }

    [StaticConstructorOnStartup]
    public class DrawRootOffsetComp : ThingComp
    {
        
    }

}
