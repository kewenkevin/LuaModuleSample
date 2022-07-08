using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ND.UI.NDUI
{

    [AddComponentMenu("NDUI/NDHitArea", 33)]
    public class NDHitArea : MaskableGraphic
    {
        protected NDHitArea()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}