using UnityEngine;

namespace IMCUI.UI
{
    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}
