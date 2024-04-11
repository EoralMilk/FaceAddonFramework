using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
using static RimWorld.MechClusterSketch;
using System.Reflection.Emit;
using RimWorld.Planet;

namespace FaceAddon
{

    //[HarmonyPatch(typeof(PawnRenderTree))]
    //[HarmonyPatch("Draw")]
    //public class DrawPatch
    //{
    //    [HarmonyPrefix]
    //    public static void Draw(PawnRenderTree __instance, PawnDrawParms parms)
    //    {
    //        var pawn = __instance.pawn;
    //        var headGraphic = __instance.HeadGraphic;
    //        if (!pawn.def.IsFaceAddonRaceDef() || headGraphic == null || (parms.skipFlags & RenderSkipFlagDefOf.Eyes) != 0)
    //        {
    //            return;
    //        }

    //        headGraphic.MeshAt

    //         Vector3 loc = headOffset;
    //        loc.y += 0.0231660213f + 0.0012f;
    //        int negOffset = 1;
    //        if (bodyFacing == Rot4.North)
    //        {
    //            loc.y -= 0.001f;
    //            negOffset = -1;
    //        }
    //        else
    //        {
    //            loc.y += 0.001f;
    //        }

    //        var racomp = pawn.GetComp<FaceAddonComp>();

    //        if (racomp == null || racomp.raceAddonGraphicSet == null)
    //            return;

    //        Mesh headMesh = HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn).MeshAt(bodyFacing);

    //        Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
    //        Vector3 vector = rootLoc;

    //        foreach (var fgs in racomp.raceAddonGraphicSet.faceGraphics)
    //        {
    //            if ((bodyDrawType == RotDrawMode.Rotting && !fgs.faceDef.displayOnRot) || (bodyDrawType == RotDrawMode.Dessicated && !fgs.faceDef.displayOnDessicated))
    //                continue;
    //            Material faceMat = OverrideMaterialIfNeeded_NewTemp(__instance.graphics, fgs.MatAt(headFacing, racomp.raceAddonGraphicSet.WInkAndBlink.BlinkNow, racomp.raceAddonGraphicSet.WInkAndBlink.WinkNow, false), pawn);
    //            GenDraw.DrawMeshNowOrLater(headMesh, vector + loc.SetLayer((fgs.faceDef.layerOffset) * (fgs.faceDef.layerOffsetAlwaysPositive ? 1 : negOffset)), quaternion, faceMat, flags.FlagSet(PawnRenderFlags.DrawNow));
    //        }


    //        bool ShellFullyCoversHead()
    //        {
    //            if (!flags.FlagSet(PawnRenderFlags.Clothes))
    //            {
    //                return false;
    //            }

    //            var apparels = pawn.apparel.WornApparel;
    //            for (int ai = 0; ai < apparels.Count; ai++)
    //            {
    //                if (apparels[ai].def.apparel.LastLayer == ApparelLayerDefOf.Shell && apparels[ai].def.apparel.shellCoversHead)
    //                {
    //                    return true;
    //                }
    //            }

    //            return false;
    //        }
    //    }



    //    private static Material OverrideMaterialIfNeeded_NewTemp(PawnGraphicSet graphics, Material original, Pawn pawn, bool portrait = false)
    //    {
    //        Material baseMat = (!portrait && pawn.IsInvisible()) ? InvisibilityMatPool.GetInvisibleMat(original) : original;
    //        return graphics.flasher.GetDamagedMat(baseMat);
    //    }

    //}

}
