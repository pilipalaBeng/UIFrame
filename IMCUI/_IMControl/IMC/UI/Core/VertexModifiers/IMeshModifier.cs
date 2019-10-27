using System;
using System.Collections.Generic;
using UnityEngine;

namespace IMCUI.UI
{
    [Obsolete("Use IMeshModifier instead", true)]
    public interface IVertexModifier
    {
        [Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts)  instead", true)]
        void ModifyVertices(List<UIVertex> verts);
    }

    public interface IMeshModifier
    {
        [Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts) instead", false)]
        void ModifyMesh(Mesh mesh);
        void ModifyMesh(VertexHelper verts);
    }
}
