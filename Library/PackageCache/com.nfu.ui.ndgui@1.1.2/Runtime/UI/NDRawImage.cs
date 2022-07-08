using System;
using ND.UI.NDUI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDRawImage", 12)]
    public class NDRawImage : RawImage
    {
        
        [SerializeField][Tooltip("垂直翻转")]
        private bool m_flipVertical;
        public bool flipVertical { get => m_flipVertical; set => m_flipVertical = value; }
        
        [SerializeField][Tooltip("水平翻转")]
        private bool m_flipHorizontal;
        public bool flipHorizontal { get => m_flipHorizontal; set => m_flipHorizontal = value; }
        
        //[SerializeField]
        //private string m_textureName;

        //public string textureName
        //{
        //    get => m_textureName;
        //    set => m_textureName = value;
        //}
        [SerializeField]
        private bool m_CullNoneSprite = true;

        public bool cullNoneSprite { get => m_CullNoneSprite; set => m_CullNoneSprite = value; }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            //if (mainTexture == null && texture == null)
            if (texture == null)
            {
                if (m_CullNoneSprite)
                {
                    toFill.Clear();
                    return;
                }
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
        
        
    }
}
 