using System;
using System.Collections.Generic;
using ND.UI.NDUI.Utility;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDImage", 11)]
    public class NDImage : Image
    {
        public static Action<string, Action<Sprite>> localizationProvider;
        
        public bool isStaticFont=false;
        
        [SerializeField]
        private string m_spriteName;
        
        [SerializeField][Tooltip("垂直翻转")]
        private bool m_flipVertical;
        public bool flipVertical { get => m_flipVertical; set => m_flipVertical = value; }
        
        [SerializeField][Tooltip("水平翻转")]
        private bool m_flipHorizontal;
        public bool flipHorizontal { get => m_flipHorizontal; set => m_flipHorizontal = value; }
        
        [SerializeField][Tooltip("运行时优化掉为精灵为空的图片")]
        private bool m_CullNoneSprite;

        public bool cullNoneSprite { get => m_CullNoneSprite; set => m_CullNoneSprite = value; }

        private Material m_clonedMaterial;

        public Material originalMaterial;

        public string spriteName
        {
            get => m_spriteName;
            set => m_spriteName = value;
        }

        public Material clonedMaterial
        {
            get
            {
                if (m_clonedMaterial == null || m_clonedMaterial != this.material)
                {
                    m_clonedMaterial = new Material(this.material);
                    material = m_clonedMaterial;
                }
                return material;
            }
        }

        public void ResetMaterial()
        {
            this.material = originalMaterial;
            UnityEngine.Object.Destroy(m_clonedMaterial);
            m_clonedMaterial = null;
        }

        protected override void Awake()
        {
            base.Awake();
            originalMaterial = this.material;
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(m_clonedMaterial);
#else
            UnityEngine.Object.Destroy(m_clonedMaterial);
#endif
            m_clonedMaterial = null;
            originalMaterial = null;
            base.OnDestroy();
        }

        static NDImage()
        {
            SpriteAtlasManager.atlasRegistered += RebuildImage;
        }

        
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if(CullNoneSprite() || !spriteIsReady(overrideSprite))
            {
                toFill.Clear();
                return;
            }
            base.OnPopulateMesh(toFill);
        }

        protected override void UpdateGeometry()
        {
            if (m_flipHorizontal || m_flipVertical)
            {
                if (useLegacyMeshGeneration)
                {
                    this.DoLegacyMeshGeneration();
                }
                else
                {
                    this.DoMeshGeneration();
                }
            }
            else
            {
                base.UpdateGeometry();
            }
        }
        
        [NonSerialized] private static readonly VertexHelper s_VertexHelper = new VertexHelper();
        private void DoMeshGeneration()
        {
            if (rectTransform != null && rectTransform.rect.width >= 0 && rectTransform.rect.height >= 0)
                OnPopulateMesh(s_VertexHelper);
            else
                s_VertexHelper.Clear(); // clear the vertex helper so invalid graphics dont draw.

            var components = ListPool2<Component>.Get();
            GetComponents(typeof(IMeshModifier), components);

            for (var i = 0; i < components.Count; i++)
                ((IMeshModifier)components[i]).ModifyMesh(s_VertexHelper);

            ListPool2<Component>.Release(components);
            
            s_VertexHelper.FillMesh(workerMesh);
            
            ImageFlipUVUtility.CorrectMeshFlippedUV(workerMesh, m_flipVertical, m_flipHorizontal);
            
            canvasRenderer.SetMesh(workerMesh);
        }

        
        private void DoLegacyMeshGeneration()
        {
            if (rectTransform != null && rectTransform.rect.width >= 0 && rectTransform.rect.height >= 0)
            {
#pragma warning disable 618
                OnPopulateMesh(workerMesh);
#pragma warning restore 618
            }
            else
            {
                workerMesh.Clear();
            }

            var components = ListPool2<Component>.Get();
            GetComponents(typeof(IMeshModifier), components);

            for (var i = 0; i < components.Count; i++)
            {
#pragma warning disable 618
                ((IMeshModifier)components[i]).ModifyMesh(workerMesh);
#pragma warning restore 618
            }

            ListPool2<Component>.Release(components);
            
            ImageFlipUVUtility.CorrectMeshFlippedUV(workerMesh, m_flipVertical, m_flipHorizontal);
            
            canvasRenderer.SetMesh(workerMesh);
        }

        
        
        private bool CullNoneSprite()
        {
            return overrideSprite == null && m_CullNoneSprite;
        }

        static readonly List<SpriteAtlas> s_LoadedSpriteAtlas = new List<SpriteAtlas>();
        
        static void RebuildImage(SpriteAtlas spriteAtlas)
        {
            if (!s_LoadedSpriteAtlas.Contains(spriteAtlas))
            {
                s_LoadedSpriteAtlas.Add(spriteAtlas);
            }
        }

        
        private static bool spriteIsReady(Sprite sprite)
        {
            if (sprite == null) return true;
            if (sprite.texture != null) return true;
            for (var i = s_LoadedSpriteAtlas.Count - 1; i > 0; --i)
            {
                var atlas = s_LoadedSpriteAtlas[i];
                if (atlas != null)
                {
                    if (atlas.CanBindTo(sprite))
                    {
                        return true;
                    }
                }
                else
                {
                    s_LoadedSpriteAtlas.RemoveAt(i);
                }
            }
            return false;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateLocalization();
        }
        
        public void UpdateLocalization()
        {
            if (sprite==null)
            {
                return;
            }
            if (isStaticFont)
            {
                if (localizationProvider != null)
                {
                    localizationProvider.Invoke(sprite.name, (s) =>
                    {
                        if (s!=null)
                        {
                            sprite = s;
                        }
                    });
                }
            }
        }

    }
}
