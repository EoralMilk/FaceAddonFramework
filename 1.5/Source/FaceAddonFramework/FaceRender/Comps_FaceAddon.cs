using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using static HarmonyLib.Code;

namespace FaceAddon
{
    public class CompProperties_FaceAddonComps : CompProperties
    {
        public List<FaceTypeDef> faceTypeDefs;
        public List<FaceTypeDef> additionalfaceTypeDefs;

        public List<FaceAddonDef> additionalFaceAddonDefs;
        public List<PawnRenderNodeProperties> renderNodeProperties;

        public CompProperties_FaceAddonComps()
        {
            compClass = typeof(FaceAddonComp);
        }
    }

    [StaticConstructorOnStartup]
    public class FaceAddonComp : ThingComp
    {
        public Pawn Pawn { private set; get; }
        public CompProperties_FaceAddonComps Props => (CompProperties_FaceAddonComps)props;

        public FaceTypeDef facetype;
        public FaceTypeDef additionalfacetype;
        public Color customColorBaseA = Color.white;
        public Color customColorSubA = Color.white;
        public Color customColorBaseB = Color.white;
        public Color customColorSubB = Color.white;

        public RaceAddonGraphicSet raceAddonGraphicSet;

        
        public override List<PawnRenderNode> CompRenderNodes()
        {
            if (raceAddonGraphicSet != null)
            {
                List<PawnRenderNode> list = new List<PawnRenderNode>();
                {
                    foreach (var props in raceAddonGraphicSet.pawnRNProps)
                    {
                        PawnRenderNode_FaceAddon renderNode = (PawnRenderNode_FaceAddon)Activator.CreateInstance(typeof(PawnRenderNode_FaceAddon), Pawn, props, Pawn.Drawer.renderer.renderTree);
                        renderNode.Comp = this;
                        renderNode.CheckState(Pawn);
                        list.Add(renderNode);
                    }
                    return list;
                }
            }

            return base.CompRenderNodes();
        }

        public override void CompTick()
        {
            base.CompTick();
            raceAddonGraphicSet?.UpdateTick();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            raceAddonGraphicSet?.UpadteTickRare();
        }

        public void CreateOrUpdateGraphicSet() 
        {
            if (raceAddonGraphicSet == null)
            {
                raceAddonGraphicSet = new RaceAddonGraphicSet(Pawn, Props, this);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn = parent as Pawn;
            CreateOrUpdateGraphicSet();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref facetype, "facetype");
            Scribe_Defs.Look(ref additionalfacetype, "additionalfacetype");
            Scribe_Values.Look(ref customColorBaseA, "customColorBaseA");
            Scribe_Values.Look(ref customColorSubA, "customColorSubA");
            Scribe_Values.Look(ref customColorBaseB, "customColorBaseB");
            Scribe_Values.Look(ref customColorSubB, "customColorSubB");

            //CreateOrUpdateGraphicSet();
        }
    }



    public class PawnRenderNode_FaceAddon : PawnRenderNode
    {
        public FaceAddonComp Comp;
        public FaceStateType CurState;
        public BlinkStateType CurBlinkState;
        public new PawnRenderNodeProperties_FaceAddon Props => (PawnRenderNodeProperties_FaceAddon)props;
        public PawnRenderNode_FaceAddon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            CurState = FaceStateType.Neutral;
            CurBlinkState = BlinkStateType.None;
            graphic = GraphicFor(pawn);
        }

        protected override void EnsureMaterialsInitialized()
        {
            if (CheckState(Comp.Pawn))
            {
                graphic = null;
            }
            base.EnsureMaterialsInitialized();
        }

        public bool CheckState(Pawn pawn)
        {
            var cstate = Props.addonRecord.CheckState(pawn, Comp.raceAddonGraphicSet);
            if (cstate != CurState)
            {
                requestRecache = true;
                CurState = cstate;
            }
            var cbstate = Props.addonRecord.CheckBlink(pawn, Comp.raceAddonGraphicSet);
            if (cbstate != CurBlinkState)
            {
                requestRecache = true;
                CurBlinkState = cbstate;
            }
            return requestRecache;
        }

        public override Color ColorFor(Pawn pawn)
        {
            Color color;
            switch (Props.colorType)
            {
                case PawnRenderNodeProperties.AttachmentColorType.Hair:
                    if (pawn.story == null)
                    {
                        Log.ErrorOnce("Trying to set render node color to hair for " + pawn.LabelShort + " without pawn story. Defaulting to white.", Gen.HashCombine(pawn.thingIDNumber, 828310001));
                        color = Color.white;
                    }
                    else
                    {
                        color = pawn.story.HairColor;
                    }

                    break;
                case PawnRenderNodeProperties.AttachmentColorType.Skin:
                    if (pawn.story == null)
                    {
                        Log.ErrorOnce("Trying to set render node color to skin for " + pawn.LabelShort + " without pawn story. Defaulting to white.", Gen.HashCombine(pawn.thingIDNumber, 228340903));
                        color = Color.white;
                    }
                    else
                    {
                        color = pawn.story.SkinColor;
                    }

                    break;
                default:
                    color = Props.colorCustomBase;
                    break;
            }

            color *= props.colorRGBPostFactor;
            if (props.useRottenColor && pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting)
            {
                color = PawnRenderUtility.GetRottenColor(color);
            }

            return color;
        }


        public virtual Color ColorForB(Pawn pawn)
        {
            Color color;
            switch (Props.colorTypeB)
            {
                case PawnRenderNodeProperties.AttachmentColorType.Hair:
                    if (pawn.story == null)
                    {
                        Log.ErrorOnce("Trying to set render node color to hair for " + pawn.LabelShort + " without pawn story. Defaulting to white.", Gen.HashCombine(pawn.thingIDNumber, 828310001));
                        color = Color.white;
                    }
                    else
                    {
                        color = pawn.story.HairColor;
                    }

                    break;
                case PawnRenderNodeProperties.AttachmentColorType.Skin:
                    if (pawn.story == null)
                    {
                        Log.ErrorOnce("Trying to set render node color to skin for " + pawn.LabelShort + " without pawn story. Defaulting to white.", Gen.HashCombine(pawn.thingIDNumber, 228340903));
                        color = Color.white;
                    }
                    else
                    {
                        color = pawn.story.SkinColor;
                    }

                    break;
                default:
                    color = Props.colorCustomSub;
                    break;
            }

            color *= props.colorRGBPostFactor;
            if (props.useRottenColor && pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting)
            {
                color = PawnRenderUtility.GetRottenColor(color);
            }

            return color;
        }

        public override GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            if (props.overrideMeshSize.HasValue)
            {
                return MeshPool.GetMeshSetForSize(props.overrideMeshSize.Value.x, props.overrideMeshSize.Value.y);
            }

            return HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn);
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            //if (!pawn.health.hediffSet.HasHead)
            //{
            //    return null;
            //}

            //if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated && !Props.addonRecord.faceDef.displayOnDessicated)
            //{
            //    return null;
            //}

            //if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting && !Props.addonRecord.faceDef.displayOnRot)
            //{
            //    return null;
            //}
            return Props.addonRecord.GraphicAt(pawn, CurState, CurBlinkState, ColorFor(pawn), ColorForB(pawn));
        }
    }

    public class PawnRenderNodeProperties_FaceAddon : PawnRenderNodeProperties
    {
        public FaceGraphicRecord addonRecord;
        public AttachmentColorType colorTypeB;
        public Color colorCustomBase;
        public Color colorCustomSub;

        public PawnRenderNodeProperties_FaceAddon()
        {
            workerClass = typeof(PawnRenderNodeWorker_FaceAddon);
            nodeClass = typeof(PawnRenderNode_FaceAddon);
        }
    }


    public class PawnRenderNodeWorker_FaceAddon : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public static PawnRenderNodeProperties_FaceAddon PropsFromNode(PawnRenderNode node) =>
            ((PawnRenderNode_FaceAddon)node).Props;

        public static FaceGraphicRecord AddonFromNode(PawnRenderNode node) =>
            ((PawnRenderNode_FaceAddon)node).Props.addonRecord;

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            if (!base.CanDrawNow(node, parms))
                return false;

            var faceDef = AddonFromNode(node).faceDef;
            return faceDef.CanDrawAddon(parms) && (faceDef.useSkipFlags.NullOrEmpty() || !faceDef.useSkipFlags.Any(rsfd => parms.skipFlags.HasFlag(rsfd)));
        }

        protected override Material GetMaterial(PawnRenderNode node, PawnDrawParms parms)
        {
            return base.GetMaterial(node, parms);
        }
    }


}
