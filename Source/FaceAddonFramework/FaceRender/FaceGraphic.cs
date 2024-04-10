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
    public class RaceAddonGraphicSet
    {
        private readonly Pawn pawn;

        public List<FaceGraphicRecord> faceGraphics = new List<FaceGraphicRecord>();
        public readonly WinkAndBlinkHandler WInkAndBlink;

        public RaceAddonGraphicSet(Pawn pawn, CompProperties_FaceAddonComps faceprops, FaceAddonComp comp)
        {
            this.pawn = pawn;

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
                    var ftA = new FaceGraphicRecord(pawn, comp.facetype.Lower, comp.facetype.Lower.fixedOnBlink);
                    faceGraphics.Add(ftA);
                }
                if (comp.facetype.Upper != null)
                {
                    var ftB = new FaceGraphicRecord(pawn, comp.facetype.Upper, comp.facetype.Upper.fixedOnBlink);
                    faceGraphics.Add(ftB);
                }
                if (comp.facetype.Attach != null)
                {
                    var ftC = new FaceGraphicRecord(pawn, comp.facetype.Attach, comp.facetype.Attach.fixedOnBlink);
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
                    var ftA = new FaceGraphicRecord(pawn, comp.additionalfacetype.Lower, comp.additionalfacetype.Lower.fixedOnBlink);
                    faceGraphics.Add(ftA);
                }
                if (comp.additionalfacetype.Upper != null)
                {
                    var ftB = new FaceGraphicRecord(pawn, comp.additionalfacetype.Upper, comp.additionalfacetype.Upper.fixedOnBlink);
                    faceGraphics.Add(ftB);
                }
                if (comp.additionalfacetype.Attach != null)
                {
                    var ftC = new FaceGraphicRecord(pawn, comp.additionalfacetype.Attach, comp.additionalfacetype.Attach.fixedOnBlink);
                    faceGraphics.Add(ftC);
                }
            }

            if (faceprops.additionalFaceAddonDefs != null)
            {
                foreach (var fdef in faceprops.additionalFaceAddonDefs)
                {
                    if (fdef != null)
                    {
                        var frecord = new FaceGraphicRecord(pawn, fdef, fdef.fixedOnBlink);
                        faceGraphics.Add(frecord);
                    }
                }
            }

            WInkAndBlink = new WinkAndBlinkHandler(comp.facetype.tickBlinkMin, comp.facetype.tickBlinkMax, comp.facetype.winkChance, comp.facetype.blinkDurationMin, comp.facetype.blinkDurationMax);

            pawnSkinColor = pawn.story?.SkinColor ?? Color.white;
            pawnHairColor = pawn.story?.HairColor ?? Color.white;

        }
        Color pawnSkinColor;
        Color pawnHairColor;
        public void Update_Normal()
        {
            WInkAndBlink?.Check(pawn.needs?.mood?.CurLevel ?? 0.5f);

            if (pawnSkinColor != (pawn.story?.SkinColor ?? Color.white) || pawnHairColor != (pawn.story?.HairColor ?? Color.white))
            {
                foreach (var fgs in faceGraphics)
                {
                    fgs.UpdateAllGraphics();
                }
                pawnSkinColor = pawn.story?.SkinColor ?? Color.white;
                pawnHairColor = pawn.story?.HairColor ?? Color.white;
            }
        }

        public void Update_Rare()
        {
            //CheckAgeSetting();
            if (pawn.health != null && pawn.health.hediffSet != null)
            {
                foreach (var fgs in faceGraphics)
                {
                    fgs.Check(pawn.health.hediffSet);
                }
            }
        }

    }

    public class FaceGraphicRecord
    {
        private readonly bool fixedBlinkWinkFace;
        private readonly Pawn pawn;
        public readonly FaceAddonDef faceDef;

        public Graphic fixedGraphic;

        public Graphic mentalBreak;
        public Graphic aboutToBreak;
        public Graphic onEdge;
        public Graphic stressed;
        public Graphic neutral;
        public Graphic content;
        public Graphic happy;
        public Graphic sleeping;
        public Graphic painShock;
        public Graphic dead;
        public Graphic blink;
        public Graphic wink;
        public Graphic damaged;
        public Graphic drafted;
        public Graphic attacking;
        public Pair<CustomPath, Graphic> custom;

        public FaceGraphicRecord(Pawn pawn, FaceAddonDef faceDef, bool fixedBlinkWinkFace)
        {
            this.fixedBlinkWinkFace = fixedBlinkWinkFace;
            this.pawn = pawn;
            this.faceDef = faceDef;

            UpdateAllGraphics();
        }

        public void UpdateAllGraphics()
        {
            var shader = faceDef.shaderType.Shader;
            var main = SwitchColor(pawn, faceDef.colorBase);
            var sub = SwitchColor(pawn, faceDef.colorSub);

            fixedGraphic = faceDef.fixedPath.GetGraphic(shader, main, sub, Vector2.one);
            mentalBreak = faceDef.mentalBreakPath.GetGraphic(shader, main, sub, Vector2.one);
            aboutToBreak = faceDef.aboutToBreakPath.GetGraphic(shader, main, sub, Vector2.one);
            onEdge = faceDef.onEdgePath.GetGraphic(shader, main, sub, Vector2.one);
            stressed = faceDef.stressedPath.GetGraphic(shader, main, sub, Vector2.one);
            neutral = faceDef.neutralPath.GetGraphic(shader, main, sub, Vector2.one);
            content = faceDef.contentPath.GetGraphic(shader, main, sub, Vector2.one);
            happy = faceDef.happyPath.GetGraphic(shader, main, sub, Vector2.one);
            sleeping = faceDef.sleepingPath.GetGraphic(shader, main, sub, Vector2.one);
            painShock = faceDef.painShockPath.GetGraphic(shader, main, sub, Vector2.one);
            dead = faceDef.deadPath.GetGraphic(shader, main, sub, Vector2.one);
            blink = faceDef.blinkPath.GetGraphic(shader, main, sub, Vector2.one);
            wink = faceDef.winkPath.GetGraphic(shader, main, sub, Vector2.one);
            damaged = faceDef.damagedPath.GetGraphic(shader, main, sub, Vector2.one);
            drafted = faceDef.draftedPath.GetGraphic(shader, main, sub, Vector2.one);
            attacking = faceDef.attackingPath.GetGraphic(shader, main, sub, Vector2.one);
        }

        public Color SwitchColor(Pawn pawn, ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.White: return Color.white;
                case ColorType.Skin: return pawn?.story?.SkinColor ?? Color.white;
                case ColorType.Hair: return pawn?.story?.HairColor ?? Color.white;
            }

            return Color.white;
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

        public Material MatAt(Rot4 rot, bool blinkNow, bool winkNow, bool keepNeutral)
        {
            if (fixedGraphic != null)
                return fixedGraphic.MatAt(rot);

            if (pawn.Dead)
            {
                return dead.MatAt(rot);
            }
            if (keepNeutral)
            {
                return neutral.MatAt(rot);
            }
            if (pawn.health.InPainShock)
            {
                return painShock.MatAt(rot);
            }
            if (!pawn.Awake())
            {
                return sleeping.MatAt(rot);
            }
            if (attacking != null && pawn.CurJobDef != JobDefOf.Wait_Combat && pawn.IsFighting())
            {
                return attacking.MatAt(rot);
            }
            if (damaged != null && pawn.Drawer.renderer.graphics.flasher.FlashingNowOrRecently)
            {
                return damaged.MatAt(rot);
            }
            if (blinkNow && !fixedBlinkWinkFace)
            {
                return blink.MatAt(rot);
            }
            if (winkNow && !fixedBlinkWinkFace)
            {
                return wink.MatAt(rot);
            }
            if (drafted != null && pawn.Drafted)
            {
                return drafted.MatAt(rot);
            }
            if (pawn.MentalStateDef != null)
            {
                return mentalBreak.MatAt(rot);
            }
            if (custom.Second != null)
            {
                return custom.Second.MatAt(rot);
            }
            if (pawn.needs != null && pawn.needs.mood != null)
            {
                if (pawn.needs.mood.CurLevel < pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true))
                {
                    return aboutToBreak.MatAt(rot);
                }
                if (pawn.needs.mood.CurLevel < pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true) + 0.05f)
                {
                    return onEdge.MatAt(rot);
                }
                if (pawn.needs.mood.CurLevel < pawn.mindState.mentalBreaker.BreakThresholdMinor)
                {
                    return stressed.MatAt(rot);
                }
                if (pawn.needs.mood.CurLevel < 0.65f)
                {
                    return neutral.MatAt(rot);
                }
                if (pawn.needs.mood.CurLevel < 0.9f)
                {
                    return content.MatAt(rot);
                }
                return happy.MatAt(rot);
            }

            return neutral.MatAt(rot);
        }
    }

}
