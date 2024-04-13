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
    public static class Exts
    {
        public static Dictionary<PawnKindDef, List<Pair<PawnKindDef, float>>> PawnKindDefReplaceSettings { get; set; } = new Dictionary<PawnKindDef, List<Pair<PawnKindDef, float>>>();

        public static PawnKindDef GetReplacedPawnKindDef(PawnKindDef key)
        {
            if (PawnKindDefReplaceSettings.TryGetValue(key, out var list) && list.Count > 0)
            {
                return list.RandomElementByWeight(x => x.Second).First;
            }
            return key;
        }


        public static HashSet<ThingDef> FaceAddonRaceDefs = new HashSet<ThingDef>();
        public static bool IsFaceAddonRaceDef(this ThingDef thingDef)
        {
            if (FaceAddonRaceDefs.Contains(thingDef))
                return true;
            return false;
        }

        public static Graphic GetGraphic(this string path, Shader shader, Color main, Color sub, Vector2 scale)
        {
            return path.NullOrEmpty() ? null : GraphicDatabase.Get<Graphic_Multi>(path, shader, scale, main, sub);
        }
        public static Vector3 SetLayer(this Vector3 loc, int priority)
        {
            loc.y += 0.003f * (priority / 10f);
            return loc;
        }
        public static Color RandomColorInList(this List<Color> colors)
        {
            if (colors == null || colors.Count == 0)
            {
                return Color.white;
            }

            return colors[Verse.Rand.Range(0, colors.Count)];
        }
    }
}
