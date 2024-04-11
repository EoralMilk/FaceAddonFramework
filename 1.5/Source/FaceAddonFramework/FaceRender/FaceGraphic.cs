using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;
using Verse.Noise;

namespace FaceAddon
{
    public class RaceAddonGraphicSet
    {
        public List<PawnRenderNodeProperties> pawnRNProps = new List<PawnRenderNodeProperties>();
        public List<FaceGraphicRecord> faceGraphics = new List<FaceGraphicRecord>();
        public readonly WinkAndBlinkHandler WInkAndBlink;
        public int LastDamageElapse;
        public readonly Pawn Pawn;
        public RaceAddonGraphicSet(Pawn pawn, CompProperties_FaceAddonComps faceprops, FaceAddonComp comp)
        {
            Pawn = pawn;
            Exts.FaceAddonRaceDefs.Add(pawn.def);

            if (comp.facetype == null && faceprops.faceTypeDefs != null && faceprops.faceTypeDefs.Any())
            {
                int sumweight = 0;
                List<(int,FaceTypeDef)> idweights = new List<(int, FaceTypeDef)>();
                
                foreach (var ftype in faceprops.faceTypeDefs)
                {
                    if (ftype.requireHeadTypes == null || !ftype.requireHeadTypes.Any() || ftype.requireHeadTypes.Contains(pawn.story.headType))
                    {
                        idweights.Add((sumweight + ftype.randomWeight, ftype));
                        sumweight += ftype.randomWeight;
                    }
                }

                var rand = Verse.Rand.Range(0, sumweight);
                for (var i = 0; i < idweights.Count; i++)
                {
                    if (idweights[i].Item1 > rand)
                    {
                        comp.facetype = idweights[i].Item2;
                        break;
                    }
                }
            }
            if (comp.facetype != null)
            {
                if (comp.facetype.Lower != null)
                {
                    var ftA = new FaceGraphicRecord(comp.facetype.Lower, comp.facetype.Lower.fixedOnBlink);
                    faceGraphics.Add(ftA);
                }
                if (comp.facetype.Upper != null)
                {
                    var ftB = new FaceGraphicRecord(comp.facetype.Upper, comp.facetype.Upper.fixedOnBlink);
                    faceGraphics.Add(ftB);
                }
                if (comp.facetype.Attach != null)
                {
                    var ftC = new FaceGraphicRecord(comp.facetype.Attach, comp.facetype.Attach.fixedOnBlink);
                    faceGraphics.Add(ftC);
                }
            }


            if (comp.additionalfacetype == null && faceprops.additionalfaceTypeDefs != null && faceprops.additionalfaceTypeDefs.Any())
            {
                int sumweight = 0;
                List<(int, FaceTypeDef)> idweights = new List<(int, FaceTypeDef)>();

                foreach (var ftype in faceprops.additionalfaceTypeDefs)
                {
                    if (ftype.requireHeadTypes == null || !ftype.requireHeadTypes.Any() || ftype.requireHeadTypes.Contains(pawn.story.headType))
                    {
                        idweights.Add((sumweight + ftype.randomWeight, ftype));
                        sumweight += ftype.randomWeight;
                    }
                }

                var rand = Verse.Rand.Range(0, sumweight);
                for (var i = 0; i < idweights.Count; i++)
                {
                    if (idweights[i].Item1 > rand)
                    {
                        comp.additionalfacetype = idweights[i].Item2;
                        break;
                    }
                }
            }
            if (comp.additionalfacetype != null)
            {
                if (comp.additionalfacetype.Lower != null)
                {
                    var ftA = new FaceGraphicRecord(comp.additionalfacetype.Lower, comp.additionalfacetype.Lower.fixedOnBlink);
                    faceGraphics.Add(ftA);
                }
                if (comp.additionalfacetype.Upper != null)
                {
                    var ftB = new FaceGraphicRecord(comp.additionalfacetype.Upper, comp.additionalfacetype.Upper.fixedOnBlink);
                    faceGraphics.Add(ftB);
                }
                if (comp.additionalfacetype.Attach != null)
                {
                    var ftC = new FaceGraphicRecord(comp.additionalfacetype.Attach, comp.additionalfacetype.Attach.fixedOnBlink);
                    faceGraphics.Add(ftC);
                }
            }

            if (faceprops.additionalFaceAddonDefs != null)
            {
                foreach (var fdef in faceprops.additionalFaceAddonDefs)
                {
                    if (fdef != null)
                    {
                        var frecord = new FaceGraphicRecord(fdef, fdef.fixedOnBlink);
                        faceGraphics.Add(frecord);
                    }
                }
            }

            WInkAndBlink = new WinkAndBlinkHandler(comp.facetype.tickBlinkMin, comp.facetype.tickBlinkMax, comp.facetype.winkChance, comp.facetype.blinkDurationMin, comp.facetype.blinkDurationMax);

            pawnRNProps.Clear();
            foreach (var fg in faceGraphics)
            {
                var nodeProps = new PawnRenderNodeProperties_FaceAddon()
                {
                    baseLayer = 50 + fg.faceDef.layerOffset,
                    visibleFacing = fg.faceDef.visibleFacing,
                    //tagDef = PawnRenderNodeTagDefOf.Head,
                    //skipFlag = fg.faceDef.useSkipFlags,
                    addonRecord = fg,
                    colorType = fg.faceDef.colorBase,
                    colorTypeB = fg.faceDef.colorSub,
                    useRottenColor = fg.faceDef.useRottenColor,
                    shaderTypeDef = fg.faceDef.shaderType,
                    parentTagDef = PawnRenderNodeTagDefOf.Head,
                };
                pawnRNProps.Add(nodeProps);
            }
        }

        public void UpdateTick()
        {
            WInkAndBlink?.Check(Pawn.needs?.mood?.CurLevel ?? 0.5f);
            LastDamageElapse++;
        }
        public void UpadteTickRare()
        {
            if (Pawn.health != null && Pawn.health.hediffSet != null)
            {
                foreach (var fgs in faceGraphics)
                {
                    fgs.Check(Pawn.health.hediffSet);
                }
            }
        }
    }
    public enum FaceStateType
    {
        None,
        Dead,
        Damaged,
        PainShocked,
        Sleeping,
        Happy,
        Content,
        Neutral,
        Stressed,
        OnEdge,
        AboutToBreak,
        MentalBreak,
        Drafted,
        Attacking,

        Custom,
    }

    public enum BlinkStateType
    {
        None,
        Blink,
        Wink,
    }

    public class FaceGraphicRecord
    {
        private readonly bool fixedBlinkWinkFace;
        public readonly FaceAddonDef faceDef;
        public int curState = 0;

        Color main;
        Color sub;
        Shader shader => faceDef.shaderType.Shader;
        public Graphic fixedGraphic => faceDef.fixedPath.GetGraphic(shader, main, sub, Vector2.one);

        public Graphic mentalBreak => faceDef.mentalBreakPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic aboutToBreak => faceDef.aboutToBreakPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic onEdge => faceDef.onEdgePath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic stressed => faceDef.stressedPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic neutral => faceDef.neutralPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic content => faceDef.contentPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic happy => faceDef.happyPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic sleeping => faceDef.sleepingPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic painShock => faceDef.painShockPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic dead => faceDef.deadPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic blink => faceDef.blinkPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic wink => faceDef.winkPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic damaged => faceDef.damagedPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic drafted => faceDef.draftedPath.GetGraphic(shader, main, sub, Vector2.one);
        public Graphic attacking => faceDef.attackingPath.GetGraphic(shader, main, sub, Vector2.one);
        public Pair<CustomPath, Graphic> custom;

        public FaceGraphicRecord(FaceAddonDef faceDef, bool fixedBlinkWinkFace)
        {
            this.fixedBlinkWinkFace = fixedBlinkWinkFace;
            this.faceDef = faceDef;
        }
        public void Update(HediffSet hediffSet)
        {
            foreach (var custom in faceDef.customs)
            {
                if (hediffSet.HasHediff(custom.hediffDef) && custom.priority > this.custom.First.priority)
                {
                    this.custom = new Pair<CustomPath, Graphic>(custom, custom.path.GetGraphic(neutral.Shader, neutral.Color, neutral.ColorTwo, Vector2.one));
                    return;
                }
            }
        }

        public void Check(HediffSet hediffSet)
        {
            if (!hediffSet.HasHediff(custom.First?.hediffDef))
            {
                custom = new Pair<CustomPath, Graphic>();
            }
        }

        public BlinkStateType CheckBlink(Pawn pawn, RaceAddonGraphicSet raceAddonGraphicSet)
        {
            if (fixedGraphic != null || pawn.Dead || pawn.health.InPainShock || !pawn.Awake() ||
                (attacking != null && pawn.CurJobDef != JobDefOf.Wait_Combat && pawn.IsFighting()) ||
                (damaged != null && faceDef.damageAnimDuration > raceAddonGraphicSet.LastDamageElapse)
                )
                return BlinkStateType.None;

            if (raceAddonGraphicSet.WInkAndBlink.BlinkNow && !fixedBlinkWinkFace)
            {
                return BlinkStateType.Blink;
            }
            if (raceAddonGraphicSet.WInkAndBlink.WinkNow && !fixedBlinkWinkFace)
            {
                return BlinkStateType.Wink;
            }

            return BlinkStateType.None;
        }

        public FaceStateType CheckState(Pawn pawn, RaceAddonGraphicSet raceAddonGraphicSet)
        {
            if (fixedGraphic != null)
                return FaceStateType.None;

            if (pawn.Dead)
            {
                return FaceStateType.Dead;
            }
            if (pawn.health.InPainShock)
            {
                return FaceStateType.PainShocked;
            }
            if (!pawn.Awake())
            {
                return FaceStateType.Sleeping;
            }
            if (attacking != null && pawn.CurJobDef != JobDefOf.Wait_Combat && pawn.IsFighting())
            {
                return FaceStateType.Attacking;
            }
            if (damaged != null && faceDef.damageAnimDuration > raceAddonGraphicSet.LastDamageElapse)
            {
                return FaceStateType.Damaged;
            }
            if (drafted != null && pawn.Drafted)
            {
                return FaceStateType.Drafted;
            }
            if (pawn.MentalStateDef != null && faceDef.HandleMentalBreak != null && faceDef.HandleMentalBreak.Contains(pawn.MentalStateDef))
            {
                return FaceStateType.MentalBreak;
            }
            if (custom.Second != null)
            {
                return FaceStateType.Custom;
            }
            if (pawn.needs != null && pawn.needs.mood != null)
            {
                if (pawn.needs.mood.CurLevel < pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true))
                {
                    return FaceStateType.AboutToBreak;
                }
                if (pawn.needs.mood.CurLevel < pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true) + 0.05f)
                {
                    return FaceStateType.OnEdge;
                }
                if (pawn.needs.mood.CurLevel < pawn.mindState.mentalBreaker.BreakThresholdMinor)
                {
                    return FaceStateType.Stressed;
                }
                if (pawn.needs.mood.CurLevel < 0.65f)
                {
                    return FaceStateType.Neutral;
                }
                if (pawn.needs.mood.CurLevel < 0.9f)
                {
                    return FaceStateType.Content;
                }
                return FaceStateType.Happy;
            }
            return FaceStateType.None;
        }

        public Graphic GraphicAt(Pawn pawn, FaceStateType faceStateType, BlinkStateType blinkStateType, Color c1, Color c2)
        {
            this.main = c1;
            this.sub = c2;

            switch (faceStateType)
            {
                case FaceStateType.None: return fixedGraphic;
                case FaceStateType.Dead: return dead;
                case FaceStateType.PainShocked: return painShock;
                case FaceStateType.Sleeping: return sleeping;
                case FaceStateType.Damaged: return damaged;
                case FaceStateType.Attacking: return attacking;
            }

            switch (blinkStateType)
            {
                case BlinkStateType.Blink: return blink;
                case BlinkStateType.Wink: return wink;
            }

            switch (faceStateType)
            {
                case FaceStateType.Drafted: return drafted;
                case FaceStateType.MentalBreak: return mentalBreak;
                case FaceStateType.Custom: return custom.Second;
                case FaceStateType.AboutToBreak: return aboutToBreak;
                case FaceStateType.OnEdge: return onEdge;
                case FaceStateType.Stressed: return stressed;
                case FaceStateType.Neutral: return neutral;
                case FaceStateType.Content: return content;
                case FaceStateType.Happy: return happy;
                default:
                    return neutral;
            }
        }

    }

}
