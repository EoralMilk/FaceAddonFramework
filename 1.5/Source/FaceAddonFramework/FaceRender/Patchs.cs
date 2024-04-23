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
    //[HarmonyPatch(typeof(PawnRenderTree))]
    //[HarmonyPatch("SetupDynamicNodes")]
    //public static class ResolveAllGraphics
    //{
    //    [HarmonyPostfix]
    //    public static void SetUp(PawnRenderTree __instance)
    //    {
    //        Pawn pawn = __instance.pawn;
    //        var faceprops = pawn.def.comps.FirstOrDefault(comp => comp is CompProperties_FaceAddonComps) as CompProperties_FaceAddonComps;
    //        if (faceprops != null && pawn.story != null && pawn.story.headType != null)
    //        {
    //            var racomp = pawn.GetComp<FaceAddonComp>();
    //            if (racomp.raceAddonGraphicSet == null)
    //            {
    //                racomp.raceAddonGraphicSet = new RaceAddonGraphicSet(pawn, faceprops, racomp);
    //            }

    //            List<PawnRenderNode> pawnRenderNodes = new List<PawnRenderNode>();
    //            foreach (var fg in racomp.raceAddonGraphicSet.faceGraphics)
    //            {
    //                PawnRenderNode_FaceAddon renderNode = new PawnRenderNode_FaceAddon(pawn, new PawnRenderNodeProperties_FaceAddon(), __instance,
    //                    racomp.raceAddonGraphicSet, fg);

    //                pawnRenderNodes.Add(renderNode);
    //            }

    //            __instance.rootNode.AddChildren(pawnRenderNodes.ToArray());
    //        }
    //    }
    //}

    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("ParallelGetPreRenderResults")]
    public static class ForceDisableCachePatch
    {
        [HarmonyPrefix]
        public static void ForceDisableCache(ref bool disableCache)
        {
            disableCache = true;
        }
    }

    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("TakeDamage")]
    public static class OnTakeDamage
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, DamageInfo dinfo)
        {
            if (__instance is Pawn)
            {
                var pawn = __instance as Pawn;
                if (pawn.GetComp<FaceAddonComp>() is var faceAddonComp && faceAddonComp != null && faceAddonComp.raceAddonGraphicSet != null)
                {
                    faceAddonComp.raceAddonGraphicSet.LastDamageElapse = 0;
                }
            }
        }
    }


    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("AddHediff", new[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
    public static class AddHediff
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn ___pawn, Hediff hediff)
        {
            if (___pawn.def.IsFaceAddonRaceDef() && ___pawn.GetComp<FaceAddonComp>() is var racomp && racomp.raceAddonGraphicSet != null && ___pawn.health != null && ___pawn.health.hediffSet != null)
            {
                foreach (var fgs in racomp.raceAddonGraphicSet.faceGraphics)
                {
                    fgs.Update(___pawn.health.hediffSet);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnRenderNode), nameof(PawnRenderNode.RecacheRequested), MethodType.Getter)]
    public static class RecacheRequestedGetterPatch
    {
        [HarmonyPostfix]
        public static void Postfix(PawnRenderNode __instance, ref bool __result)
        {
            var fa = __instance as PawnRenderNode_FaceAddon;
            if (__instance is PawnRenderNode_FaceAddon)
            {
                __result = fa.CheckState(fa.Comp.Pawn);
            }
        }
    }

}
