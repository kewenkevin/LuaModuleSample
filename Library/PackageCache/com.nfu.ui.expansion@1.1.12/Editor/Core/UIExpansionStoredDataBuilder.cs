using System.Collections.Generic;
using ND.UI.Core;
using UnityEngine;

namespace ND.UI
{
    public class UIExpansionStoredDataBuilder 
    {
        public List<GameObject> gameObjectList = new List<GameObject>();
        public Dictionary<GameObject, ushort> gameObjectDic = new Dictionary<GameObject, ushort>();

        public List<Sprite> spriteList = new List<Sprite>();
        public Dictionary<Sprite, ushort> spriteDic = new Dictionary<Sprite, ushort>();

        public List<float> floatList = new List<float>();
        public Dictionary<float, ushort> floatDic = new Dictionary<float, ushort>();

        public List<int> intList = new List<int>();
        public Dictionary<int, ushort> intDic = new Dictionary<int, ushort>();

        public List<string> stringList = new List<string>();
        public Dictionary<string, ushort> stringDic = new Dictionary<string, ushort>();
        
        public List<AnimationCurve> animationCurveList = new List<AnimationCurve>();
        public Dictionary<AnimationCurve, ushort>  animationCurveDic = new Dictionary<AnimationCurve, ushort>();
        
        public  List<Material> materialList = new List<Material>();
        public  Dictionary<Material, ushort> materialDic = new Dictionary<Material, ushort>();
        
        public List<Core.TextStyleBase> textFontStylelList = new List<Core.TextStyleBase>();
        public Dictionary<Core.TextStyleBase,ushort> textFontStylelDic = new Dictionary<Core.TextStyleBase, ushort>();
        
        public List<ColorStyleBase> textColorStylelList = new List<ColorStyleBase>();
        public Dictionary<ColorStyleBase,ushort> textColorStylelDic = new Dictionary<ColorStyleBase, ushort>();

        public ushort GetGameObjectIndex(GameObject target)
        {
            if (target == null)
            {
                return UIExpansionUtility.FalseValue; //return 0
            }
            if (gameObjectDic.ContainsKey(target))
            {
                return gameObjectDic[target];
            }
            ushort index = (ushort)gameObjectList.Count;
            gameObjectList.Add(target);
            gameObjectDic[target] = index;
            return index;
        }

        private ushort[] GetVector3Index(Vector3 vec)
        {
            ushort[] datas = new ushort[3];
            for (int i = 0; i < 3; i++)
            {
                if (floatDic.ContainsKey(vec[i]))
                {
                    datas[i] = floatDic[vec[i]];
                    continue;
                }
                ushort index = (ushort)floatList.Count;
                floatList.Add(vec[i]);
                floatDic[vec[i]] = index;
                datas[i] = index;
            }
            return datas;
        }

        private ushort[] GetVector2Index(Vector2 vec)
        {
            ushort[] datas = new ushort[2];
            for (int i = 0; i < 2; i++)
            {
                if (floatDic.ContainsKey(vec[i]))
                {
                    datas[i] = floatDic[vec[i]];
                    continue;
                }
                ushort index = (ushort)floatList.Count;
                floatList.Add(vec[i]);
                floatDic[vec[i]] = index;
                datas[i] = index;
            }
            return datas;
        }

        private ushort GetFloatIndex(float value)
        {
            if (floatDic.ContainsKey(value))
            {
                return floatDic[value];
            }
            ushort index = (ushort)floatList.Count;
            floatList.Add(value);
            floatDic[value] = index;
            return index;
        }

        private ushort GetIntIndex(int value)
        {
            if (intDic.ContainsKey(value))
            {
                return intDic[value];
            }
            ushort index = (ushort)intList.Count;
            intList.Add(value);
            intDic[value] = index;
            return index;
        }

        private ushort[] GetColorIndex(Color col)
        {
            ushort[] datas = new ushort[4];
            for (int i = 0; i < 4; i++)
            {
                if (floatDic.ContainsKey(col[i]))
                {
                    datas[i] = floatDic[col[i]];
                    continue;
                }
                ushort index = (ushort)floatList.Count;
                floatList.Add(col[i]);
                floatDic[col[i]] = index;
                datas[i] = index;
            }
            return datas;
        }

        private ushort GetStringIndex(string str)
        {
            if (stringDic.ContainsKey(str))
            {
                return stringDic[str];
            }
            ushort index = (ushort)stringDic.Count;
            stringList.Add(str);
            stringDic[str] = index;
            return index;
        }

        private ushort GetSpriteIndex(Sprite sprite)
        {
            //FIXME 当sprite为空时候处理
            if (sprite == null)
            {
                sprite = Sprite.Create(null,new Rect(),new Vector2());
            }
            if (spriteDic.ContainsKey(sprite))
            {
                return spriteDic[sprite];
            }
            ushort index = (ushort)spriteList.Count;
            spriteList.Add(sprite);
            spriteDic[sprite] = index;
            return index;
        }

        private ushort GetMaterialIndex(Material material)
        {
            if (materialDic.ContainsKey(material))
            {
                return materialDic[material];
            }

            ushort index = (ushort) materialList.Count;
            materialList.Add(material);
            materialDic[material] = index;
            return index;
        }
        
        private ushort GetTextFontStyleIndex(TextStyleBase style)
        {
            if (textFontStylelDic.ContainsKey(style))
            {
                return textFontStylelDic[style];
            }

            ushort index = (ushort) textFontStylelList.Count;
            textFontStylelList.Add(style);
            textFontStylelDic[style] = index;
            return index;
        }
        
        private ushort GetTextColorStyleIndex(ColorStyleBase style)
        {
            if (textColorStylelDic.ContainsKey(style))
            {
                return textColorStylelDic[style];
            }

            ushort index = (ushort) textColorStylelList.Count;
            textColorStylelList.Add(style);
            textColorStylelDic[style] = index;
            return index;
        }
        
        private ushort GetAnimationCurveIndex(AnimationCurve curve)
        {
            if (animationCurveDic.ContainsKey(curve))
            {
                return animationCurveDic[curve];
            }
            ushort index = (ushort)animationCurveList.Count;
            animationCurveList.Add(curve);
            animationCurveDic[curve] = index;
            return index;
        }

        public List<ushort> BuildDataList(GameObject target, List<AnimationCurve> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetAnimationCurveIndex(values[i]));
            }

            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<Sprite> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetSpriteIndex(values[i]));
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<Material> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetMaterialIndex(values[i]));
            }

            return dataList;
        }
        
        public List<ushort> BuildDataList(GameObject target, List<TextStyleBase> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add((GetTextFontStyleIndex(values[i])));
            }

            return dataList;
        }
        
        public List<ushort> BuildDataList(GameObject target, List<ColorStyleBase> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add((GetTextColorStyleIndex(values[i])));
            }

            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<bool> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(values[i] ? UIExpansionUtility.TrueValue : UIExpansionUtility.FalseValue);
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<Vector3> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                ushort[] vec3Datas = GetVector3Index(values[i]);
                for (int j = 0; j < 3; j++)
                {
                    dataList.Add(vec3Datas[j]);
                }
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<Vector2> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                ushort[] vec3Datas = GetVector3Index(values[i]);
                for (int j = 0; j < 2; j++)
                {
                    dataList.Add(vec3Datas[j]);
                }
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<Color> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                ushort[] colorDatas = GetColorIndex(values[i]);
                for (int j = 0; j < 4; j++)
                {
                    dataList.Add(colorDatas[j]);
                }
            }
            return dataList;
        }
        public List<ushort> BuildDataList(GameObject target, GearTypeState state ,List<string> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add((ushort)state);
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetStringIndex(values[i]));
            }
            return dataList;
        }
        public List<ushort> BuildDataList(GameObject target, List<string> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetStringIndex(values[i]));
            }
            return dataList;
        }
        
        public List<ushort> BuildDataList(GameObject target, List<float> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetFloatIndex(values[i]));
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, List<int> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetIntIndex(values[i]));
            }
            return dataList;
        }
        
        public List<ushort> BuildDataList(GameObject target, string strValue, List<int> values)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetStringIndex(strValue));
            for (int i = 0; i < values.Count; i++)
            {
                dataList.Add(GetIntIndex(values[i]));
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target,string binderType,string linkerType,string label,int valueType)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetStringIndex(binderType));
            dataList.Add(GetStringIndex(linkerType));
            dataList.Add(GetStringIndex(label));
            dataList.Add(GetIntIndex(valueType));
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, string binderType, string linkerType, string label)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetStringIndex(binderType));
            dataList.Add(GetStringIndex(linkerType));
            dataList.Add(GetStringIndex(label));
            return dataList;
        }
        
        public List<ushort> BuildDataList(GameObject target, 
                                            int frameIndex, 
                                            int valueSize, 
                                            List<float> values, 
                                            List<bool> actives, 
                                            bool hasTween = false, 
                                            EaseType easeType = EaseType.Linear, 
                                            AnimationCurve curve = null)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetIntIndex(frameIndex));
            for (int i = 0; i < valueSize; i++)
            {
                dataList.Add(GetFloatIndex(values[i]));
            }
            for (int i = 0; i < valueSize; i++)
            {
                dataList.Add(actives[i] ? UIExpansionUtility.TrueValue : UIExpansionUtility.FalseValue);
            }
            //倒数第二项为easeType
            if (hasTween)
            {
                dataList.Add(GetIntIndex((int)easeType));
            }
            else
            {
                dataList.Add(GetIntIndex((int)EaseType.None));
            }
            //dataList最后一项为curve
            if (curve == null)
            {
                curve = AnimationCurve.Linear(0, 0, 1, 1);
            }
            dataList.Add(GetAnimationCurveIndex(curve));
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, int frameIndex, int valueSize, List<float> values, List<bool> actives, bool hasTween = false, EaseType easeType = EaseType.Linear)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetIntIndex(frameIndex));
            for (int i = 0; i < valueSize; i++)
            {
                dataList.Add(GetFloatIndex(values[i]));
            }
            for (int i = 0; i < valueSize; i++)
            {
                dataList.Add(actives[i] ? UIExpansionUtility.TrueValue : UIExpansionUtility.FalseValue);
            }
            if (hasTween)
            {
                dataList.Add(GetIntIndex((int)easeType));
            }
            else
            {
                dataList.Add(GetIntIndex((int)EaseType.None));
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, int frameIndex, Sprite value, bool hasTween = false, EaseType easeType = EaseType.Linear)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetIntIndex(frameIndex));
            dataList.Add(GetSpriteIndex(value));
            if (hasTween)
            {
                dataList.Add(GetIntIndex((int)easeType));
            }
            else
            {
                dataList.Add(GetIntIndex((int)EaseType.None));
            }
            return dataList;
        }

        public List<ushort> BuildDataList(GameObject target, int frameIndex, string value, bool hasTween = false, EaseType easeType = EaseType.Linear)
        {
            List<ushort> dataList = new List<ushort>();
            dataList.Add(GetGameObjectIndex(target));
            dataList.Add(GetIntIndex(frameIndex));
            dataList.Add(GetStringIndex(value));
            if (hasTween)
            {
                dataList.Add(GetIntIndex((int)easeType));
            }
            else
            {
                dataList.Add(GetIntIndex((int)EaseType.None));
            }
            return dataList;
        }

        public void Store(UIExpansion uiexpansion)
        {
            uiexpansion.StoredGameObjects = gameObjectList.ToArray();
            uiexpansion.StoredSprites = spriteList.ToArray();
            uiexpansion.StoredFloats = floatList.ToArray();
            uiexpansion.StoredInts = intList.ToArray();
            uiexpansion.StoredStrings = stringList.ToArray();
            uiexpansion.StoredAnimationCurves = animationCurveList.ToArray();
            uiexpansion.StoredMaterials = materialList.ToArray();
            uiexpansion.StoredTextColorStyles = textColorStylelList.ToArray();
            uiexpansion.StoredTextFontStyles = textFontStylelList.ToArray();
        }
    }
}