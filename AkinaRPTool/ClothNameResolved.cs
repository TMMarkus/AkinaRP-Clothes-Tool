using System;
using System.IO;

namespace AkinaRPTool
{
    public class ClothNameResolver
    {
        public enum Type
        {
            Component,
            PedProp
        }

        public enum DrawableType
        {
            Head,
            Mask,
            Hair,
            Body,
            Legs,
            Bag,
            Shoes,
            Accessories,
            Undershirt,
            Armor,
            Decal,
            Top,
            PropHead,
            PropEyes,
            PropEars,
            PropMouth,
            PropLHand,
            PropRHand,
            PropLWrist,
            PropRWrist,
            PropHip,
            PropLFoot,
            PropRFoot,
            PropPHLHand,
            PropPHRHand,
            COUNT
        }

        public enum PropRenderFlag
        {
            NONE,
            PRF_ALPHA,
            PRF_DECAL,
            PRF_CUTOUT
        }

        public Type clothType;
        public DrawableType drawableType;
        public PropRenderFlag propRenderFlag;
        public string bindedNumber = "";
        public string postfix = "";
        public bool isVariation = false;
        public ClothNameResolver(string filename)
        {
            string[] parts = Path.GetFileNameWithoutExtension(filename).Split('_');
            if (parts.Length < 3)
                throw new Exception("Wrong drawable name on " + filename);

            if (parts[0].ToLower() == "p")
            {
                clothType = Type.PedProp;

                string drName = parts[1].ToLower();
                switch (drName)
                {
                    case "head": drawableType = DrawableType.PropHead; break;
                    case "eyes": drawableType = DrawableType.PropEyes; break;
                    case "ears": drawableType = DrawableType.PropEars; break;
                    case "mouth": drawableType = DrawableType.PropMouth; break;
                    case "lhand": drawableType = DrawableType.PropLHand; break;
                    case "rhand": drawableType = DrawableType.PropRHand; break;
                    case "lwrist": drawableType = DrawableType.PropLWrist; break;
                    case "rwrist": drawableType = DrawableType.PropRWrist; break;
                    case "hip": drawableType = DrawableType.PropHip; break;
                    case "lfoot": drawableType = DrawableType.PropLFoot; break;
                    case "rfoot": drawableType = DrawableType.PropRFoot; break;
                    case "phlhand": drawableType = DrawableType.PropPHLHand; break;
                    case "phrhand": drawableType = DrawableType.PropPHRHand; break;
                    default: break;
                }

                bindedNumber = parts[2];
            }
            else
            {
                clothType = Type.Component;

                string drName = parts[0].ToLower();
                switch (drName)
                {
                    case "head": drawableType = DrawableType.Head; break;
                    case "berd": drawableType = DrawableType.Mask; break;
                    case "hair": drawableType = DrawableType.Hair; break;
                    case "uppr": drawableType = DrawableType.Body; break;
                    case "lowr": drawableType = DrawableType.Legs; break;
                    case "hand": drawableType = DrawableType.Bag; break;
                    case "feet": drawableType = DrawableType.Shoes; break;
                    case "teef": drawableType = DrawableType.Accessories; break;
                    case "accs": drawableType = DrawableType.Undershirt; break;
                    case "task": drawableType = DrawableType.Armor; break;
                    case "decl": drawableType = DrawableType.Decal; break;
                    case "jbib": drawableType = DrawableType.Top; break;
                    default: break;
                }

                bindedNumber = parts[1];
                postfix = parts[2].ToLower();
                if (parts.Length > 3)
                    isVariation = true;
            }
        }

        public override string ToString()
        {
            return clothType.ToString() + " " + drawableType.ToString() + " " + bindedNumber;
        }

        public static string DrawableTypeToString(DrawableType type)
        {
            switch (type)
            {
                case DrawableType.Head: return "head";
                case DrawableType.Mask: return "berd";
                case DrawableType.Hair: return "hair";
                case DrawableType.Body: return "uppr";
                case DrawableType.Legs: return "lowr";
                case DrawableType.Bag: return "hand";
                case DrawableType.Shoes: return "feet";
                case DrawableType.Accessories: return "teef";
                case DrawableType.Undershirt: return "accs";
                case DrawableType.Armor: return "task";
                case DrawableType.Decal: return "decl";
                case DrawableType.Top: return "jbib";
                case DrawableType.PropHead: return "p_head";
                case DrawableType.PropEyes: return "p_eyes";
                case DrawableType.PropEars: return "p_ears";
                case DrawableType.PropMouth: return "p_mouth";
                case DrawableType.PropLHand: return "p_lhand";
                case DrawableType.PropRHand: return "p_rhand";
                case DrawableType.PropLWrist: return "p_lwrist";
                case DrawableType.PropRWrist: return "p_rwrist";
                case DrawableType.PropHip: return "p_hip";
                case DrawableType.PropLFoot: return "p_lfoot";
                case DrawableType.PropRFoot: return "p_rfoot";
                case DrawableType.PropPHLHand: return "p_phlhand";
                case DrawableType.PropPHRHand: return "p_phrhand";
                default: return "";
            }
        }

        public static string PropTypeToString(DrawableType type)
        {
            switch (type)
            {
                case DrawableType.PropHead: return "head";
                case DrawableType.PropEyes: return "eyes";
                case DrawableType.PropEars: return "ears";
                case DrawableType.PropMouth: return "mouth";
                case DrawableType.PropLHand: return "left_hand";
                case DrawableType.PropRHand: return "right_hand";
                case DrawableType.PropLWrist: return "left_wrist";
                case DrawableType.PropRWrist: return "right_wrist";
                case DrawableType.PropHip: return "hip";
                case DrawableType.PropLFoot: return "left_foot";
                case DrawableType.PropRFoot: return "right_foot";
                case DrawableType.PropPHLHand: return "ph_left_hand";
                case DrawableType.PropPHRHand: return "ph_right_hand";
                default: return "";
            }
        }
    }
}
