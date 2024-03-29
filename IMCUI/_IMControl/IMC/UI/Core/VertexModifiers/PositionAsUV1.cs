using System.Linq;
using UnityEngine;

namespace IMCUI.UI
{
    [AddComponentMenu("IMCUI/Effects/Position As UV1", 16)]
    public class PositionAsUV1 : BaseMeshEffect
    {
        protected PositionAsUV1()
        {}

        public override void ModifyMesh(VertexHelper vh)
        {
            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.uv1 =  new Vector2(vert.position.x, vert.position.y);
                vh.SetUIVertex(vert, i);
            }
        }
    }
}
