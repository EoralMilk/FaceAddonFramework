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

namespace FaceAddon
{
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("DrawHeadHair")]
    public class DrawHeadHairPatch
    {
        [HarmonyPrefix]
        public static void Draw(PawnRenderer __instance, Vector3 rootLoc, Vector3 headOffset, float angle, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags, bool bodyDrawn)
        {
            var pr = Traverse.Create(__instance);
            var pawn = (Pawn)(pr.Field("pawn").GetValue());
            if (!pawn.def.IsFaceAddonRaceDef() || (ShellFullyCoversHead() && bodyDrawn))
            {
                return;
            }

            Vector3 loc = headOffset;
            loc.y += 0.0231660213f + 0.0012f;
            int negOffset = 1;
            if (bodyFacing == Rot4.North)
            {
                loc.y -= 0.001f;
                negOffset = -1;
            }
            else
            {
                loc.y += 0.001f;
            }

            var racomp = pawn.GetComp<FaceAddonComp>();

            if (racomp == null || racomp.raceAddonGraphicSet == null)
                return;

            Mesh headMesh = HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn).MeshAt(bodyFacing);

            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 vector = rootLoc;

            foreach (var fgs in racomp.raceAddonGraphicSet.faceGraphics)
            {
                if ((bodyDrawType == RotDrawMode.Rotting && !fgs.faceDef.displayOnRot) || (bodyDrawType == RotDrawMode.Dessicated && !fgs.faceDef.displayOnDessicated))
                    continue;
                Material faceMat = OverrideMaterialIfNeeded_NewTemp(__instance.graphics, fgs.MatAt(headFacing, racomp.raceAddonGraphicSet.WInkAndBlink.BlinkNow, racomp.raceAddonGraphicSet.WInkAndBlink.WinkNow, false), pawn);
                GenDraw.DrawMeshNowOrLater(headMesh, vector + loc.SetLayer((fgs.faceDef.layerOffset) * (fgs.faceDef.layerOffsetAlwaysPositive ? 1 : negOffset)), quaternion, faceMat, flags.FlagSet(PawnRenderFlags.DrawNow));
            }


            bool ShellFullyCoversHead()
            {
                if (!flags.FlagSet(PawnRenderFlags.Clothes))
                {
                    return false;
                }

                var apparels = pawn.apparel.WornApparel;
                for (int ai = 0; ai < apparels.Count; ai++)
                {
                    if (apparels[ai].def.apparel.LastLayer == ApparelLayerDefOf.Shell && apparels[ai].def.apparel.shellCoversHead)
                    {
                        return true;
                    }
                }

                return false;
            }
        }



        private static Material OverrideMaterialIfNeeded_NewTemp(PawnGraphicSet graphics, Material original, Pawn pawn, bool portrait = false)
        {
            Material baseMat = (!portrait && pawn.IsInvisible()) ? InvisibilityMatPool.GetInvisibleMat(original) : original;
            return graphics.flasher.GetDamagedMat(baseMat);
        }

    }

    //[HarmonyPatch(typeof(PawnRenderer))]
    //[HarmonyPatch("TryDrawGenes")]
    //public class TryDrawGenesPatch
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(PawnRenderer __instance, GeneDrawLayer layer)
    //    {
    //        Vector3 loc = rootLoc + headOffset;
    //        loc.y += 0.0231660213f;
    //        if (bodyFacing == Rot4.North)
    //        {
    //            loc.y -= 0.001f;
    //        }
    //        else
    //        {
    //            loc.y += 0.001f;
    //        }


    //        if (layer == GeneDrawLayer.PostTattoo && DrawHeadHairPatch.NeedDrawFace)
    //        {
    //            var pawn = DrawHeadHairPatch.CurrentPawn;
    //            var racomp = pawn.GetComp<FaceAddonComp>();
    //            Mesh headMesh = HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn).MeshAt(DrawHeadHairPatch.bodyFacing);

    //            Quaternion quaternion = Quaternion.AngleAxis(DrawHeadHairPatch.angle, Vector3.up);
    //            Vector3 vector = DrawHeadHairPatch.rootLoc;
    //            if (pawn.ageTracker.CurLifeStage.bodyDrawOffset.HasValue)
    //            {
    //                vector += pawn.ageTracker.CurLifeStage.bodyDrawOffset.Value;
    //            }

    //            Vector3 vector2 = vector;
    //            Vector3 vector3 = vector;
    //            if (DrawHeadHairPatch.bodyFacing != Rot4.North)
    //            {
    //                vector3.y += 0.0231660213f;
    //                vector2.y += 3f / 148f;
    //            }
    //            else
    //            {
    //                vector3.y += 3f / 148f;
    //                vector2.y += 0.0231660213f;
    //            }
    //            var vector4 = quaternion * __instance.BaseHeadOffsetAt(DrawHeadHairPatch.bodyFacing);

    //            foreach (var fgs in racomp.raceAddonGraphicSet.faceGraphics)
    //            {
    //                Material faceMat = OverrideMaterialIfNeeded_NewTemp(__instance.graphics, fgs.MatAt(DrawHeadHairPatch.headFacing, false, false, false), pawn);
    //                GenDraw.DrawMeshNowOrLater(headMesh, vector3 + vector4, quaternion, faceMat, DrawHeadHairPatch.flags.FlagSet(PawnRenderFlags.DrawNow));
    //            }
    //        }
    //    }


    //    private static Material OverrideMaterialIfNeeded_NewTemp(PawnGraphicSet graphics, Material original, Pawn pawn, bool portrait = false)
    //    {
    //        Material baseMat = (!portrait && pawn.IsInvisible()) ? InvisibilityMatPool.GetInvisibleMat(original) : original;
    //        return graphics.flasher.GetDamagedMat(baseMat);
    //    }
    //}
}
