using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FaceAddon
{
    [HarmonyPatch(typeof(PawnGraphicSet))]
    [HarmonyPatch("ResolveAllGraphics")]
    public static class ResolveAllGraphics
    {
        [HarmonyPrefix]
        public static void Prefix(PawnGraphicSet __instance)
        {
            //Log.Message("Check FaceAddonComps");

            var faceprops = __instance.pawn.def.comps.FirstOrDefault(comp => comp is CompProperties_FaceAddonComps) as CompProperties_FaceAddonComps;
            if (faceprops != null)
            {
                Pawn pawn = __instance.pawn;
                if (pawn.story != null && pawn.story.headType != null)
                {
                    var racomp = pawn.GetComp<FaceAddonComp>();
                    if (racomp.raceAddonGraphicSet == null)
                    {
                        racomp.raceAddonGraphicSet = new RaceAddonGraphicSet(pawn, faceprops, racomp);
                    }
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

}
