using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MKStudio.UI.MessageBox
{
    [AddComponentMenu("UI/Effects/SimpleGradient")]
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
    public class SimpleGradient : BaseMeshEffect
#else
    public class SimpleGradient : BaseVertexEffect
#endif
    {
        [SerializeField]
        private Color32 topColor = Color.white;
        [SerializeField]
        private Color32 bottomColor = Color.black;

#if UNITY_5_2_3 || UNITY_5_3_OR_NEWER
        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive() || helper.currentVertCount == 0)
                return;

            List<UIVertex> vertices = new List<UIVertex>();
            helper.GetUIVertexStream(vertices);

            float bottomY = vertices[0].position.y;
            float topY = vertices[0].position.y;

            for (int i = 1; i < vertices.Count; i++)
            {
                float y = vertices[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }

            float uiElementHeight = topY - bottomY;

            UIVertex v = new UIVertex();

            for (int i = 0; i < helper.currentVertCount; i++)
            {
                helper.PopulateUIVertex(ref v, i);
                v.color = Color32.Lerp(bottomColor, topColor, (v.position.y - bottomY) / uiElementHeight);
                helper.SetUIVertex(v, i);
            }
        }

#elif UNITY_5_2
    public override void ModifyMesh(Mesh mesh)
    {
        if (!this.IsActive())
            return;

        List<UIVertex> list = new List<UIVertex>();
        using (VertexHelper vertexHelper = new VertexHelper(mesh))
        {
            vertexHelper.GetUIVertexStream(list);
        }

        ActualModify(list); // calls the old ModifyVertices which was used on pre 5.2

        using (VertexHelper vertexHelper2 = new VertexHelper())
        {
            vertexHelper2.AddUIVertexTriangleStream(list);
            vertexHelper2.FillMesh(mesh);
        }
    }
    public override void ModifyMesh(VertexHelper vh)
    {
        throw new System.NotImplementedException();
    }

#elif !UNITY_5_3_OR_NEWER
    
    public override void ModifyVertices(List<UIVertex> vertexList)
    {
        ActualModify(vertexList);
    }
#endif

#if !UNITY_5_2_3
        void ActualModify(List<UIVertex> vertexList)
        {
            if (!IsActive())
            {
                return;
            }

            int count = vertexList.Count;
            float bottomY = vertexList[0].position.y;
            float topY = vertexList[0].position.y;

            for (int i = 1; i < count; i++)
            {
                float y = vertexList[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }

            float uiElementHeight = topY - bottomY;

            for (int i = 0; i < count; i++)
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                vertexList[i] = uiVertex;
            }
        }
#endif
    }
}